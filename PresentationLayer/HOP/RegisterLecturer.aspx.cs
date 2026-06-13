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
    public partial class RegisterLecturer : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadLecturers();
                txtStaffID.Text = PreviewNextStaffID();
            }
        }

        private void LoadLecturers()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT
                        l.lecturer_id,
                        l.user_id,
                        l.staff_id,
                        l.full_name,
                        l.specialisation,
                        ISNULL(l.status, 'Active') AS status,
                        u.username,
                        u.email,
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
                    FROM LECTURER l
                    INNER JOIN [USER] u
                        ON l.user_id = u.user_id
                    WHERE
                        (
                            @search = ''
                            OR l.staff_id LIKE @searchLike
                            OR l.full_name LIKE @searchLike
                            OR l.specialisation LIKE @searchLike
                            OR u.username LIKE @searchLike
                            OR u.email LIKE @searchLike
                        )
                    ORDER BY l.lecturer_id DESC";

                SqlCommand cmd = new SqlCommand(query, con);

                string search = txtSearch == null ? "" : txtSearch.Text.Trim();

                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@searchLike", "%" + search + "%");

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

                    gvLecturers.DataSource = dv;
                }
                else
                {
                    gvLecturers.DataSource = dt;
                }

                gvLecturers.DataBind();
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!ValidateForm(true))
                return;

            string staffId = "";
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
                        staffId = GenerateStaffID(con, tran);

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
                                'LECTURER',
                                0,
                                0,
                                @verification_token,
                                @verification_expiry
                            );

                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        SqlCommand userCmd =
                            new SqlCommand(userQuery, con, tran);

                        userCmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        userCmd.Parameters.AddWithValue("@password_hash", HashPassword(txtPassword.Text.Trim()));
                        userCmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        userCmd.Parameters.AddWithValue("@verification_token", token);
                        userCmd.Parameters.AddWithValue("@verification_expiry", expiry);

                        int userId =
                            Convert.ToInt32(userCmd.ExecuteScalar());

                        string lecturerQuery = @"
                            INSERT INTO LECTURER
                            (
                                user_id,
                                staff_id,
                                full_name,
                                specialisation,
                                hire_date,
                                status
                            )
                            VALUES
                            (
                                @user_id,
                                @staff_id,
                                @full_name,
                                @specialisation,
                                @hire_date,
                                'Active'
                            )";

                        SqlCommand lecturerCmd =
                            new SqlCommand(lecturerQuery, con, tran);

                        lecturerCmd.Parameters.AddWithValue("@user_id", userId);
                        lecturerCmd.Parameters.AddWithValue("@staff_id", staffId);
                        lecturerCmd.Parameters.AddWithValue("@full_name", txtFullName.Text.Trim());
                        lecturerCmd.Parameters.AddWithValue("@specialisation", txtSpecialisation.Text.Trim());
                        lecturerCmd.Parameters.AddWithValue("@hire_date", DateTime.Now.Date);

                        lecturerCmd.ExecuteNonQuery();

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
                    staffId,
                    token,
                    expiry
                );

                lblMessage.Text =
                    "Lecturer registered successfully. Staff ID: " +
                    staffId +
                    ". Verification email sent. Link expires in 7 days.";

                lblMessage.CssClass = "text-success fw-bold";
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Lecturer registered successfully. Staff ID: " +
                    staffId +
                    ", but verification email failed to send. Error: " +
                    ex.Message;

                lblMessage.CssClass = "text-warning fw-bold";
            }

            Clear();
            LoadLecturers();
            txtStaffID.Text = PreviewNextStaffID();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfLecturerID.Value) ||
                string.IsNullOrEmpty(hfUserID.Value))
            {
                ShowError("Please select a lecturer first.");
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
                        string currentEmail =
                            GetCurrentEmail(con, tran, hfUserID.Value);

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

                        SqlCommand userCmd =
                            new SqlCommand(userQuery, con, tran);

                        userCmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        userCmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        userCmd.Parameters.AddWithValue("@user_id", hfUserID.Value);

                        if (emailChanged)
                        {
                            userCmd.Parameters.AddWithValue("@token", token);
                            userCmd.Parameters.AddWithValue("@expiry", expiry);
                        }

                        userCmd.ExecuteNonQuery();

                        string lecturerQuery = @"
                            UPDATE LECTURER
                            SET
                                full_name = @full_name,
                                specialisation = @specialisation,
                                status = @status
                            WHERE lecturer_id = @lecturer_id";

                        SqlCommand lecturerCmd =
                            new SqlCommand(lecturerQuery, con, tran);

                        lecturerCmd.Parameters.AddWithValue("@full_name", txtFullName.Text.Trim());
                        lecturerCmd.Parameters.AddWithValue("@specialisation", txtSpecialisation.Text.Trim());
                        lecturerCmd.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                        lecturerCmd.Parameters.AddWithValue("@lecturer_id", hfLecturerID.Value);

                        lecturerCmd.ExecuteNonQuery();

                        tran.Commit();

                        if (emailChanged)
                        {
                            try
                            {
                                SendVerificationEmail(
                                    txtEmail.Text.Trim(),
                                    txtFullName.Text.Trim(),
                                    txtStaffID.Text.Trim(),
                                    token,
                                    expiry
                                );

                                lblMessage.Text =
                                    "Lecturer updated. Email changed, new verification email sent.";

                                lblMessage.CssClass =
                                    "text-success fw-bold";
                            }
                            catch (Exception ex)
                            {
                                lblMessage.Text =
                                    "Lecturer updated, but verification email failed: " +
                                    ex.Message;

                                lblMessage.CssClass =
                                    "text-warning fw-bold";
                            }
                        }
                        else
                        {
                            lblMessage.Text = "Lecturer updated successfully.";
                            lblMessage.CssClass = "text-success fw-bold";
                        }
                    }
                    catch (Exception ex)
                    {
                        try { tran.Rollback(); } catch { }

                        ShowError("Update Error: " + ex.Message);
                        return;
                    }
                }
            }

            Clear();
            LoadLecturers();
            txtStaffID.Text = PreviewNextStaffID();
        }

        protected void btnDeactivate_Click(object sender, EventArgs e)
        {
            ChangeLecturerStatus("Inactive", 0, "Lecturer deactivated successfully.");
        }

        protected void btnReactivate_Click(object sender, EventArgs e)
        {
            ChangeLecturerStatus("Active", 1, "Lecturer reactivated successfully.");
        }

        private void ChangeLecturerStatus(
            string lecturerStatus,
            int userActive,
            string message
        )
        {
            if (string.IsNullOrEmpty(hfLecturerID.Value) ||
                string.IsNullOrEmpty(hfUserID.Value))
            {
                ShowError("Please select a lecturer first.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    UPDATE LECTURER
                    SET status = @status
                    WHERE lecturer_id = @lecturer_id;

                    UPDATE [USER]
                    SET is_active = @is_active
                    WHERE user_id = @user_id;";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@status", lecturerStatus);
                cmd.Parameters.AddWithValue("@lecturer_id", hfLecturerID.Value);
                cmd.Parameters.AddWithValue("@is_active", userActive);
                cmd.Parameters.AddWithValue("@user_id", hfUserID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = message;
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadLecturers();
            txtStaffID.Text = PreviewNextStaffID();
        }

        protected void gvLecturers_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvLecturers.SelectedRow;

            hfLecturerID.Value =
                gvLecturers.DataKeys[row.RowIndex]["lecturer_id"].ToString();

            hfUserID.Value =
                gvLecturers.DataKeys[row.RowIndex]["user_id"].ToString();

            LoadSelectedLecturer();

            lblMessage.Text = "Lecturer selected. You can update, deactivate or resend verification.";
            lblMessage.CssClass = "text-primary fw-bold";
        }

        private void LoadSelectedLecturer()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT
                        l.staff_id,
                        l.full_name,
                        l.specialisation,
                        ISNULL(l.status, 'Active') AS status,
                        u.username,
                        u.email
                    FROM LECTURER l
                    INNER JOIN [USER] u
                        ON l.user_id = u.user_id
                    WHERE l.lecturer_id = @lecturer_id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@lecturer_id", hfLecturerID.Value);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    txtStaffID.Text = dr["staff_id"].ToString();
                    txtFullName.Text = dr["full_name"].ToString();
                    txtSpecialisation.Text = dr["specialisation"].ToString();
                    txtUsername.Text = dr["username"].ToString();
                    txtEmail.Text = dr["email"].ToString();
                    ddlStatus.SelectedValue = dr["status"].ToString();

                    txtPassword.Text = "";
                }
            }
        }

        protected void gvLecturers_RowCommand(
            object sender,
            GridViewCommandEventArgs e
        )
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
            string lecturerName = "";
            string staffId = "";

            using (SqlConnection con = new SqlConnection(cs))
            {
                string selectQuery = @"
                    SELECT
                        u.email,
                        l.full_name,
                        l.staff_id,
                        u.email_verified
                    FROM [USER] u
                    INNER JOIN LECTURER l
                        ON u.user_id = l.user_id
                    WHERE u.user_id = @user_id";

                SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                selectCmd.Parameters.AddWithValue("@user_id", userId);

                con.Open();

                SqlDataReader dr = selectCmd.ExecuteReader();

                if (!dr.Read())
                {
                    ShowError("Lecturer account not found.");
                    return;
                }

                if (Convert.ToBoolean(dr["email_verified"]))
                {
                    lblMessage.Text = "This lecturer email is already verified.";
                    lblMessage.CssClass = "text-success fw-bold";
                    return;
                }

                email = dr["email"].ToString();
                lecturerName = dr["full_name"].ToString();
                staffId = dr["staff_id"].ToString();

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
                    lecturerName,
                    staffId,
                    token,
                    expiry
                );

                lblMessage.Text =
                    "Verification email resent successfully. Link expires in 7 days.";

                lblMessage.CssClass =
                    "text-success fw-bold";
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                    "Token was updated, but email failed to send: " +
                    ex.Message;

                lblMessage.CssClass =
                    "text-warning fw-bold";
            }

            LoadLecturers();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadLecturers();
        }

        protected void btnResetSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";

            if (ddlVerificationFilter.Items.Count > 0)
            {
                ddlVerificationFilter.SelectedIndex = 0;
            }

            LoadLecturers();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            txtStaffID.Text = PreviewNextStaffID();
            lblMessage.Text = "";
        }

        private bool ValidateForm(bool isRegister)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string email = txtEmail.Text.Trim();

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
                    ShowError("Password must be at least 8 characters and include uppercase, lowercase, number and special character. Example: Lect@1234");
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
                ShowError("Please enter lecturer full name.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSpecialisation.Text))
            {
                ShowError("Please enter specialisation.");
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

        private string GenerateStaffID(
            SqlConnection con,
            SqlTransaction tran
        )
        {
            string prefix = "L" + DateTime.Now.Year;

            string query = @"
                SELECT ISNULL(MAX(CAST(RIGHT(staff_id, 5) AS INT)), 0) + 1
                FROM LECTURER
                WHERE staff_id LIKE @prefix";

            SqlCommand cmd = new SqlCommand(query, con, tran);
            cmd.Parameters.AddWithValue("@prefix", prefix + "%");

            int nextNumber =
                Convert.ToInt32(cmd.ExecuteScalar());

            return prefix + nextNumber.ToString("00000");
        }

        private string PreviewNextStaffID()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string prefix = "L" + DateTime.Now.Year;

                string query = @"
                    SELECT ISNULL(MAX(CAST(RIGHT(staff_id, 5) AS INT)), 0) + 1
                    FROM LECTURER
                    WHERE staff_id LIKE @prefix";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@prefix", prefix + "%");

                con.Open();

                int nextNumber =
                    Convert.ToInt32(cmd.ExecuteScalar());

                return prefix + nextNumber.ToString("00000");
            }
        }

        private void SendVerificationEmail(
            string toEmail,
            string lecturerName,
            string staffId,
            string token,
            DateTime expiry
        )
        {
            string verificationLink =
                Request.Url.GetLeftPart(UriPartial.Authority) +
                ResolveUrl("~/PresentationLayer/HOP/VerifyEmail.aspx?token=" + token);

            MailMessage mail = new MailMessage();

            mail.To.Add(toEmail);
            mail.Subject = "SIMS Lecturer Email Verification";

            mail.Body =
                "Dear " + lecturerName + ",\n\n" +
                "Your SIMS lecturer account has been created or updated.\n\n" +
                "Staff ID: " + staffId + "\n\n" +
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
            hfLecturerID.Value = "";
            hfUserID.Value = "";

            txtUsername.Text = "";
            txtPassword.Text = "";
            txtEmail.Text = "";
            txtFullName.Text = "";
            txtSpecialisation.Text = "";
            txtStaffID.Text = "";

            if (ddlStatus.Items.Count > 0)
            {
                ddlStatus.SelectedIndex = 0;
            }
        }
    }
}