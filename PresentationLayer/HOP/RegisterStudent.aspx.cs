using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class RegisterStudent : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProgrammes();
                LoadStudents();
            }
        }

        private void LoadProgrammes()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT programme_id, programme_name
                    FROM PROGRAMME
                    ORDER BY programme_name";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlProgramme.DataSource = dt;
                ddlProgramme.DataTextField = "programme_name";
                ddlProgramme.DataValueField = "programme_id";
                ddlProgramme.DataBind();

                ddlProgramme.Items.Insert(0, new ListItem("-- Select Programme --", ""));
            }
        }

        private void LoadStudents()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT
                        s.student_id,
                        s.user_id,
                        s.programme_id,
                        s.matric_number,
                        s.nric_passport,
                        s.full_name,
                        p.programme_name,
                        u.username,
                        u.email,
                        s.date_of_birth,
                        s.admission_year,
                        s.admission_month,
                        ISNULL(s.status, 'Active') AS status,

                        CASE
                            WHEN u.email_verified = 1 THEN 'Verified'
                            WHEN u.verification_expiry IS NOT NULL
                                 AND u.verification_expiry < GETDATE() THEN 'Expired'
                            ELSE 'Pending Verification'
                        END AS verification_status,

                        CASE
                            WHEN u.is_active = 1 THEN 'Active'
                            ELSE 'Inactive'
                        END AS account_status

                    FROM STUDENT s
                    INNER JOIN [USER] u
                        ON s.user_id = u.user_id
                    INNER JOIN PROGRAMME p
                        ON s.programme_id = p.programme_id
                    WHERE
                        (
                            @search = ''
                            OR
                            (
                                @searchBy = 'all'
                                AND
                                (
                                    CAST(s.student_id AS VARCHAR) LIKE @searchLike
                                    OR s.matric_number LIKE @searchLike
                                    OR s.nric_passport LIKE @searchLike
                                    OR s.full_name LIKE @searchLike
                                    OR u.username LIKE @searchLike
                                    OR u.email LIKE @searchLike
                                )
                            )
                            OR (@searchBy = 'student_id' AND CAST(s.student_id AS VARCHAR) LIKE @searchLike)
                            OR (@searchBy = 'matric_number' AND s.matric_number LIKE @searchLike)
                            OR (@searchBy = 'nric_passport' AND s.nric_passport LIKE @searchLike)
                            OR (@searchBy = 'full_name' AND s.full_name LIKE @searchLike)
                            OR (@searchBy = 'email' AND u.email LIKE @searchLike)
                            OR (@searchBy = 'username' AND u.username LIKE @searchLike)
                        )
                    ORDER BY s.student_id DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                string search = txtSearch == null ? "" : txtSearch.Text.Trim();
                string searchBy = ddlSearchBy == null ? "all" : ddlSearchBy.SelectedValue;

                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@searchLike", "%" + search + "%");
                cmd.Parameters.AddWithValue("@searchBy", searchBy);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                if (ddlVerificationFilter != null &&
                    ddlVerificationFilter.SelectedValue != "All")
                {
                    DataView dv = dt.DefaultView;
                    dv.RowFilter =
                        "verification_status = '" +
                        ddlVerificationFilter.SelectedValue.Replace("'", "''") +
                        "'";

                    gvStudents.DataSource = dv;
                }
                else
                {
                    gvStudents.DataSource = dt;
                }

                gvStudents.DataBind();
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!ValidateForm(true))
                return;

            string matricNumber = "";
            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.Now.AddDays(7);

            bool databaseSaved = false;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        matricNumber = GenerateMatricNumber(
                            con,
                            tran,
                            txtAdmissionYear.Text.Trim(),
                            ddlAdmissionMonth.SelectedValue
                        );

                        string userQuery = @"
                            INSERT INTO [USER]
                            (
                                username,
                                password_hash,
                                email,
                                role,
                                is_active,
                                email_verified,
                                verification_token,
                                verification_expiry
                            )
                            VALUES
                            (
                                @username,
                                @password_hash,
                                @email,
                                'STUDENT',
                                0,
                                0,
                                @verification_token,
                                @verification_expiry
                            );

                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        SqlCommand userCmd = new SqlCommand(userQuery, con, tran);

                        userCmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        userCmd.Parameters.AddWithValue("@password_hash", HashPassword(txtPassword.Text.Trim()));
                        userCmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        userCmd.Parameters.AddWithValue("@verification_token", token);
                        userCmd.Parameters.AddWithValue("@verification_expiry", expiry);

                        int userId = Convert.ToInt32(userCmd.ExecuteScalar());

                        string studentQuery = @"
                            INSERT INTO STUDENT
                            (
                                user_id,
                                programme_id,
                                matric_number,
                                nric_passport,
                                full_name,
                                date_of_birth,
                                admission_year,
                                admission_month,
                                status
                            )
                            VALUES
                            (
                                @user_id,
                                @programme_id,
                                @matric_number,
                                @nric_passport,
                                @full_name,
                                @date_of_birth,
                                @admission_year,
                                @admission_month,
                                @status
                            )";

                        SqlCommand studentCmd = new SqlCommand(studentQuery, con, tran);

                        studentCmd.Parameters.AddWithValue("@user_id", userId);
                        studentCmd.Parameters.AddWithValue("@programme_id", ddlProgramme.SelectedValue);
                        studentCmd.Parameters.AddWithValue("@matric_number", matricNumber);
                        studentCmd.Parameters.AddWithValue("@nric_passport", txtNRIC.Text.Trim());
                        studentCmd.Parameters.AddWithValue("@full_name", txtFullName.Text.Trim());
                        studentCmd.Parameters.AddWithValue("@date_of_birth", GetDateOrDBNull(txtDOB.Text.Trim()));
                        studentCmd.Parameters.AddWithValue("@admission_year", txtAdmissionYear.Text.Trim());
                        studentCmd.Parameters.AddWithValue("@admission_month", ddlAdmissionMonth.SelectedValue);
                        studentCmd.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);

                        studentCmd.ExecuteNonQuery();

                        tran.Commit();
                        databaseSaved = true;
                    }
                    catch (Exception ex)
                    {
                        if (!databaseSaved)
                        {
                            try
                            {
                                tran.Rollback();
                            }
                            catch
                            {
                            }
                        }

                        ShowError("Database Error: " + ex.Message);
                        return;
                    }
                }
            }

            try
            {
                SendVerificationEmail(
                    txtEmail.Text.Trim(),
                    txtFullName.Text.Trim(),
                    matricNumber,
                    token,
                    expiry
                );

                lblMessage.Text =
                    "Student registered successfully. Matric Number: " +
                    matricNumber +
                    ". Verification email sent. Link expires in 7 days.";

                lblMessage.CssClass = "text-success fw-bold";
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Student registered successfully. Matric Number: " +
                    matricNumber +
                    ", but verification email failed to send. Error: " +
                    ex.Message;

                lblMessage.CssClass = "text-warning fw-bold";
            }

            txtMatricNumber.Text = matricNumber;

            ClearExceptMatric();
            LoadStudents();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfStudentID.Value) ||
                string.IsNullOrEmpty(hfUserID.Value))
            {
                ShowError("Please select a student first.");
                return;
            }

            if (!ValidateForm(false))
                return;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        string currentEmail = GetCurrentEmail(con, tran, hfUserID.Value);

                        bool emailChanged =
                            !currentEmail.Equals(
                                txtEmail.Text.Trim(),
                                StringComparison.OrdinalIgnoreCase
                            );

                        string token = "";
                        DateTime expiry = DateTime.Now.AddDays(7);

                        string userQuery;

                        if (emailChanged)
                        {
                            token = Guid.NewGuid().ToString();

                            userQuery = @"
                                UPDATE [USER]
                                SET
                                    username = @username,
                                    email = @email,
                                    is_active = 0,
                                    email_verified = 0,
                                    verification_token = @token,
                                    verification_expiry = @expiry
                                WHERE user_id = @user_id";
                        }
                        else
                        {
                            userQuery = @"
                                UPDATE [USER]
                                SET
                                    username = @username,
                                    email = @email
                                WHERE user_id = @user_id";
                        }

                        SqlCommand userCmd = new SqlCommand(userQuery, con, tran);

                        userCmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        userCmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        userCmd.Parameters.AddWithValue("@user_id", hfUserID.Value);

                        if (emailChanged)
                        {
                            userCmd.Parameters.AddWithValue("@token", token);
                            userCmd.Parameters.AddWithValue("@expiry", expiry);
                        }

                        userCmd.ExecuteNonQuery();

                        string studentQuery = @"
                            UPDATE STUDENT
                            SET
                                programme_id = @programme_id,
                                nric_passport = @nric_passport,
                                full_name = @full_name,
                                date_of_birth = @date_of_birth,
                                admission_year = @admission_year,
                                admission_month = @admission_month,
                                status = @status
                            WHERE student_id = @student_id";

                        SqlCommand studentCmd = new SqlCommand(studentQuery, con, tran);

                        studentCmd.Parameters.AddWithValue("@programme_id", ddlProgramme.SelectedValue);
                        studentCmd.Parameters.AddWithValue("@nric_passport", txtNRIC.Text.Trim());
                        studentCmd.Parameters.AddWithValue("@full_name", txtFullName.Text.Trim());
                        studentCmd.Parameters.AddWithValue("@date_of_birth", GetDateOrDBNull(txtDOB.Text.Trim()));
                        studentCmd.Parameters.AddWithValue("@admission_year", txtAdmissionYear.Text.Trim());
                        studentCmd.Parameters.AddWithValue("@admission_month", ddlAdmissionMonth.SelectedValue);
                        studentCmd.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                        studentCmd.Parameters.AddWithValue("@student_id", hfStudentID.Value);

                        studentCmd.ExecuteNonQuery();

                        tran.Commit();

                        if (emailChanged)
                        {
                            try
                            {
                                SendVerificationEmail(
                                    txtEmail.Text.Trim(),
                                    txtFullName.Text.Trim(),
                                    txtMatricNumber.Text.Trim(),
                                    token,
                                    expiry
                                );

                                lblMessage.Text =
                                    "Student updated. Email changed, new verification email sent.";

                                lblMessage.CssClass = "text-success fw-bold";
                            }
                            catch (Exception ex)
                            {
                                lblMessage.Text =
                                    "Student updated, but verification email failed: " +
                                    ex.Message;

                                lblMessage.CssClass = "text-warning fw-bold";
                            }
                        }
                        else
                        {
                            lblMessage.Text = "Student updated successfully.";
                            lblMessage.CssClass = "text-success fw-bold";
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            tran.Rollback();
                        }
                        catch
                        {
                        }

                        ShowError("Update Error: " + ex.Message);
                        return;
                    }
                }
            }

            Clear();
            LoadStudents();
        }

        protected void btnDrop_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfStudentID.Value) ||
                string.IsNullOrEmpty(hfUserID.Value))
            {
                ShowError("Please select a student first.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    UPDATE STUDENT
                    SET status = 'Dropped'
                    WHERE student_id = @student_id;

                    UPDATE [USER]
                    SET is_active = 0
                    WHERE user_id = @user_id;";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@student_id", hfStudentID.Value);
                cmd.Parameters.AddWithValue("@user_id", hfUserID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text =
                "Student dropped successfully. Account has been deactivated.";

            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadStudents();
        }

        protected void btnReactivate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfStudentID.Value) ||
                string.IsNullOrEmpty(hfUserID.Value))
            {
                ShowError("Please select a student first.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    UPDATE STUDENT
                    SET status = 'Active'
                    WHERE student_id = @student_id;

                    UPDATE [USER]
                    SET is_active = 1
                    WHERE user_id = @user_id
                    AND email_verified = 1;";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@student_id", hfStudentID.Value);
                cmd.Parameters.AddWithValue("@user_id", hfUserID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text =
                "Student reactivated. Account will be active only if email is verified.";

            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadStudents();
        }

        protected void gvStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvStudents.SelectedRow;

            hfStudentID.Value =
                gvStudents.DataKeys[row.RowIndex]["student_id"].ToString();

            hfUserID.Value =
                gvStudents.DataKeys[row.RowIndex]["user_id"].ToString();

            LoadSelectedStudent();

            lblMessage.Text =
                "Student selected. You can update, drop, reactivate or resend verification.";

            lblMessage.CssClass = "text-primary fw-bold";
        }

        private void LoadSelectedStudent()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT
                        s.programme_id,
                        s.matric_number,
                        s.nric_passport,
                        s.full_name,
                        s.date_of_birth,
                        s.admission_year,
                        s.admission_month,
                        ISNULL(s.status, 'Active') AS status,
                        u.username,
                        u.email
                    FROM STUDENT s
                    INNER JOIN [USER] u
                        ON s.user_id = u.user_id
                    WHERE s.student_id = @student_id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@student_id", hfStudentID.Value);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    ddlProgramme.SelectedValue = dr["programme_id"].ToString();
                    txtMatricNumber.Text = dr["matric_number"].ToString();
                    txtNRIC.Text = dr["nric_passport"].ToString();
                    txtFullName.Text = dr["full_name"].ToString();
                    txtUsername.Text = dr["username"].ToString();
                    txtEmail.Text = dr["email"].ToString();

                    if (dr["date_of_birth"] != DBNull.Value)
                    {
                        txtDOB.Text =
                            Convert.ToDateTime(dr["date_of_birth"])
                            .ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        txtDOB.Text = "";
                    }

                    txtAdmissionYear.Text = dr["admission_year"].ToString();

                    string month = dr["admission_month"].ToString();

                    if (month.Length == 1)
                    {
                        month = "0" + month;
                    }

                    if (ddlAdmissionMonth.Items.FindByValue(month) != null)
                    {
                        ddlAdmissionMonth.SelectedValue = month;
                    }

                    ddlStatus.SelectedValue = dr["status"].ToString();

                    txtPassword.Text = "";
                }
            }
        }

        protected void gvStudents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ResendVerification")
            {
                string userId = e.CommandArgument.ToString();
                ResendVerification(userId);
            }
        }

        private void ResendVerification(string userId)
        {
            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.Now.AddDays(7);

            string email = "";
            string studentName = "";
            string matricNumber = "";

            using (SqlConnection con = new SqlConnection(cs))
            {
                string selectQuery = @"
                    SELECT
                        u.email,
                        u.email_verified,
                        s.full_name,
                        s.matric_number
                    FROM [USER] u
                    INNER JOIN STUDENT s
                        ON u.user_id = s.user_id
                    WHERE u.user_id = @user_id";

                SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                selectCmd.Parameters.AddWithValue("@user_id", userId);

                con.Open();

                SqlDataReader dr = selectCmd.ExecuteReader();

                if (!dr.Read())
                {
                    ShowError("Student account not found.");
                    return;
                }

                if (Convert.ToBoolean(dr["email_verified"]))
                {
                    lblMessage.Text = "This student email is already verified.";
                    lblMessage.CssClass = "text-success fw-bold";
                    return;
                }

                email = dr["email"].ToString();
                studentName = dr["full_name"].ToString();
                matricNumber = dr["matric_number"].ToString();

                dr.Close();

                string updateQuery = @"
                    UPDATE [USER]
                    SET
                        verification_token = @token,
                        verification_expiry = @expiry,
                        email_verified = 0,
                        is_active = 0
                    WHERE user_id = @user_id";

                SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                updateCmd.Parameters.AddWithValue("@token", token);
                updateCmd.Parameters.AddWithValue("@expiry", expiry);
                updateCmd.Parameters.AddWithValue("@user_id", userId);

                updateCmd.ExecuteNonQuery();
            }

            try
            {
                SendVerificationEmail(
                    email,
                    studentName,
                    matricNumber,
                    token,
                    expiry
                );

                lblMessage.Text =
                    "Verification email resent successfully. Link expires in 7 days.";

                lblMessage.CssClass = "text-success fw-bold";
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Token was updated, but email failed to send: " +
                    ex.Message;

                lblMessage.CssClass = "text-warning fw-bold";
            }

            LoadStudents();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadStudents();
        }

        protected void btnResetSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";

            if (ddlSearchBy.Items.Count > 0)
            {
                ddlSearchBy.SelectedIndex = 0;
            }

            if (ddlVerificationFilter.Items.Count > 0)
            {
                ddlVerificationFilter.SelectedIndex = 0;
            }

            LoadStudents();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            lblMessage.Text = "";
        }

        private bool ValidateForm(bool isRegister)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string email = txtEmail.Text.Trim();
            string nric = txtNRIC.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                ShowError("Please enter username.");
                return false;
            }

            if (UsernameExists(username, hfUserID.Value))
            {
                ShowError("This username is already taken.");
                return false;
            }

            if (isRegister)
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    ShowError("Please enter password.");
                    return false;
                }

                if (!IsStrongPassword(password))
                {
                    ShowError("Password must be at least 8 characters and include uppercase, lowercase, number and special character. Example: Student@123");
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Please enter email.");
                return false;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ShowError("Please enter a valid email address.");
                return false;
            }

            if (EmailExists(email, hfUserID.Value))
            {
                ShowError("This email is already registered.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                ShowError("Please enter full name.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(nric))
            {
                ShowError("Please enter NRIC / Passport Number.");
                return false;
            }

            if (NRICExists(nric, hfStudentID.Value))
            {
                ShowError("NRIC / Passport Number already exists.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDOB.Text))
            {
                ShowError("Please enter date of birth.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAdmissionYear.Text))
            {
                ShowError("Please enter admission year.");
                return false;
            }

            if (string.IsNullOrEmpty(ddlProgramme.SelectedValue))
            {
                ShowError("Please select a programme.");
                return false;
            }

            return true;
        }

        private bool UsernameExists(string username, string currentUserId)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM [USER]
                    WHERE username = @username
                    AND (@user_id = '' OR user_id <> @user_id)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@user_id", currentUserId ?? "");

                con.Open();

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool EmailExists(string email, string currentUserId)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM [USER]
                    WHERE email = @email
                    AND (@user_id = '' OR user_id <> @user_id)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@user_id", currentUserId ?? "");

                con.Open();

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool NRICExists(string nric, string currentStudentId)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM STUDENT
                    WHERE nric_passport = @nric
                    AND (@student_id = '' OR student_id <> @student_id)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nric", nric);
                cmd.Parameters.AddWithValue("@student_id", currentStudentId ?? "");

                con.Open();

                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private string GetCurrentEmail(
            SqlConnection con,
            SqlTransaction tran,
            string userId
        )
        {
            string query = "SELECT email FROM [USER] WHERE user_id = @user_id";

            SqlCommand cmd = new SqlCommand(query, con, tran);
            cmd.Parameters.AddWithValue("@user_id", userId);

            object result = cmd.ExecuteScalar();

            return result == null ? "" : result.ToString();
        }

        private string GenerateMatricNumber(
            SqlConnection con,
            SqlTransaction tran,
            string year,
            string month
        )
        {
            string prefix = "P" + year + month;

            string query = @"
                SELECT ISNULL(MAX(CAST(RIGHT(matric_number, 5) AS INT)), 0) + 1
                FROM STUDENT
                WHERE matric_number LIKE @prefix";

            SqlCommand cmd = new SqlCommand(query, con, tran);
            cmd.Parameters.AddWithValue("@prefix", prefix + "%");

            int nextNumber = Convert.ToInt32(cmd.ExecuteScalar());

            return prefix + nextNumber.ToString("00000");
        }

        private object GetDateOrDBNull(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText))
            {
                return DBNull.Value;
            }

            return Convert.ToDateTime(dateText);
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "text-danger fw-bold";
        }

        private bool IsStrongPassword(string password)
        {
            if (password.Length < 8)
                return false;

            return Regex.IsMatch(password, "[A-Z]") &&
                   Regex.IsMatch(password, "[a-z]") &&
                   Regex.IsMatch(password, "[0-9]") &&
                   Regex.IsMatch(password, @"[\W_]");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes =
                    sha.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder =
                    new StringBuilder();

                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private void SendVerificationEmail(
            string toEmail,
            string studentName,
            string matricNumber,
            string token,
            DateTime expiry
        )
        {
            string verificationLink =
                Request.Url.GetLeftPart(UriPartial.Authority) +
                ResolveUrl("~/PresentationLayer/HOP/VerifyEmail.aspx?token=" + token);

            MailMessage mail = new MailMessage();

            mail.To.Add(toEmail);
            mail.Subject = "SIMS Student Email Verification";

            mail.Body =
                "Dear " + studentName + ",\n\n" +
                "Your SIMS student account has been created or updated.\n\n" +
                "Matric Number: " + matricNumber + "\n\n" +
                "Please verify your email using the link below:\n\n" +
                verificationLink + "\n\n" +
                "This verification link will expire on " +
                expiry.ToString("dd MMMM yyyy hh:mm tt") + ".\n\n" +
                "You cannot login until your email is verified.\n\n" +
                "Thank you,\nSIMS Admin";

            mail.IsBodyHtml = false;

            SmtpClient smtp = new SmtpClient();
            smtp.Send(mail);
        }

        private void Clear()
        {
            hfStudentID.Value = "";
            hfUserID.Value = "";

            txtUsername.Text = "";
            txtPassword.Text = "";
            txtEmail.Text = "";
            txtFullName.Text = "";
            txtNRIC.Text = "";
            txtMatricNumber.Text = "";
            txtDOB.Text = "";
            txtAdmissionYear.Text = "";

            if (ddlAdmissionMonth.Items.Count > 0)
            {
                ddlAdmissionMonth.SelectedIndex = 0;
            }

            if (ddlProgramme.Items.Count > 0)
            {
                ddlProgramme.SelectedIndex = 0;
            }

            if (ddlStatus.Items.Count > 0)
            {
                ddlStatus.SelectedIndex = 0;
            }
        }

        private void ClearExceptMatric()
        {
            hfStudentID.Value = "";
            hfUserID.Value = "";

            txtUsername.Text = "";
            txtPassword.Text = "";
            txtEmail.Text = "";
            txtFullName.Text = "";
            txtNRIC.Text = "";
            txtDOB.Text = "";
            txtAdmissionYear.Text = "";

            if (ddlAdmissionMonth.Items.Count > 0)
            {
                ddlAdmissionMonth.SelectedIndex = 0;
            }

            if (ddlProgramme.Items.Count > 0)
            {
                ddlProgramme.SelectedIndex = 0;
            }

            if (ddlStatus.Items.Count > 0)
            {
                ddlStatus.SelectedIndex = 0;
            }
        }
    }
}