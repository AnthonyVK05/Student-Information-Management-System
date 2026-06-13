using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class ManageCourses : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProgrammes();
                LoadCourses();
            }
        }

        private void LoadProgrammes()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT programme_id, programme_name
                    FROM PROGRAMME
                    ORDER BY programme_name";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlProgramme.DataSource = dt;
                ddlProgramme.DataTextField = "programme_name";
                ddlProgramme.DataValueField = "programme_id";
                ddlProgramme.DataBind();

                ddlProgramme.Items.Insert(0, new ListItem("-- Select Programme --", ""));
            }
        }

        private void LoadCourses()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        c.course_id,
                        c.programme_id,
                        p.programme_name,
                        c.course_name,
                        c.course_code,
                        c.credit_hours,
                        c.semester,
                        c.is_active,
                        CASE
                            WHEN c.is_active = 1 THEN 'Active'
                            ELSE 'Inactive'
                        END AS status_text
                    FROM COURSE c
                    INNER JOIN PROGRAMME p
                        ON c.programme_id = p.programme_id
                    ORDER BY c.course_id DESC";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvCourses.DataSource = dt;
                gvCourses.DataBind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            if (CourseCodeExists(txtCourseCode.Text.Trim(), ""))
            {
                ShowError("Course code already exists.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    INSERT INTO COURSE
                    (
                        programme_id,
                        course_name,
                        course_code,
                        credit_hours,
                        semester,
                        is_active
                    )
                    VALUES
                    (
                        @programme_id,
                        @course_name,
                        @course_code,
                        @credit_hours,
                        @semester,
                        @is_active
                    )";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@programme_id", ddlProgramme.SelectedValue);
                cmd.Parameters.AddWithValue("@course_name", txtCourseName.Text.Trim());
                cmd.Parameters.AddWithValue("@course_code", txtCourseCode.Text.Trim().ToUpper());
                cmd.Parameters.AddWithValue("@credit_hours", Convert.ToInt32(txtCreditHours.Text.Trim()));
                cmd.Parameters.AddWithValue("@semester", ddlSemester.SelectedValue);
                cmd.Parameters.AddWithValue("@is_active", ddlStatus.SelectedValue);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Course added successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadCourses();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfCourseID.Value))
            {
                ShowError("Please select a course first.");
                return;
            }

            if (!ValidateForm())
            {
                return;
            }

            if (CourseCodeExists(txtCourseCode.Text.Trim(), hfCourseID.Value))
            {
                ShowError("Another course already uses this course code.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    UPDATE COURSE
                    SET
                        programme_id = @programme_id,
                        course_name = @course_name,
                        course_code = @course_code,
                        credit_hours = @credit_hours,
                        semester = @semester,
                        is_active = @is_active
                    WHERE course_id = @course_id";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@programme_id", ddlProgramme.SelectedValue);
                cmd.Parameters.AddWithValue("@course_name", txtCourseName.Text.Trim());
                cmd.Parameters.AddWithValue("@course_code", txtCourseCode.Text.Trim().ToUpper());
                cmd.Parameters.AddWithValue("@credit_hours", Convert.ToInt32(txtCreditHours.Text.Trim()));
                cmd.Parameters.AddWithValue("@semester", ddlSemester.SelectedValue);
                cmd.Parameters.AddWithValue("@is_active", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@course_id", hfCourseID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Course updated successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadCourses();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfCourseID.Value))
            {
                ShowError("Please select a course first.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    UPDATE COURSE
                    SET is_active = 0
                    WHERE course_id = @course_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@course_id", hfCourseID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Course deactivated successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadCourses();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            lblMessage.Text = "";
        }

        protected void gvCourses_SelectedIndexChanged(object sender, EventArgs e)
        {
            string courseId = gvCourses.DataKeys[gvCourses.SelectedRow.RowIndex].Value.ToString();

            hfCourseID.Value = courseId;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        programme_id,
                        course_name,
                        course_code,
                        credit_hours,
                        semester,
                        is_active
                    FROM COURSE
                    WHERE course_id = @course_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@course_id", courseId);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    string programmeId = dr["programme_id"].ToString();

                    if (ddlProgramme.Items.FindByValue(programmeId) != null)
                    {
                        ddlProgramme.SelectedValue = programmeId;
                    }

                    txtCourseName.Text = dr["course_name"].ToString();
                    txtCourseCode.Text = dr["course_code"].ToString();
                    txtCreditHours.Text = dr["credit_hours"].ToString();

                    string semester = dr["semester"].ToString();

                    if (ddlSemester.Items.FindByValue(semester) != null)
                    {
                        ddlSemester.SelectedValue = semester;
                    }

                    bool isActive = Convert.ToBoolean(dr["is_active"]);
                    ddlStatus.SelectedValue = isActive ? "1" : "0";
                }
            }

            lblMessage.Text = "Course selected. You can update or deactivate it.";
            lblMessage.CssClass = "text-primary fw-bold";
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(ddlProgramme.SelectedValue))
            {
                ShowError("Please select programme.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCourseName.Text))
            {
                ShowError("Please enter course name.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCourseCode.Text))
            {
                ShowError("Please enter course code.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCreditHours.Text))
            {
                ShowError("Please enter credit hours.");
                return false;
            }

            int creditHours;

            if (!int.TryParse(txtCreditHours.Text.Trim(), out creditHours))
            {
                ShowError("Credit hours must be a valid number.");
                return false;
            }

            if (creditHours < 1 || creditHours > 6)
            {
                ShowError("Credit hours must be between 1 and 6.");
                return false;
            }

            return true;
        }

        private bool CourseCodeExists(string courseCode, string currentCourseId)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT COUNT(*)
                    FROM COURSE
                    WHERE course_code = @course_code
                    AND (@course_id = '' OR course_id <> @course_id)";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@course_code", courseCode.ToUpper());
                cmd.Parameters.AddWithValue("@course_id", currentCourseId ?? "");

                con.Open();

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count > 0;
            }
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "text-danger fw-bold";
        }

        private void Clear()
        {
            hfCourseID.Value = "";

            if (ddlProgramme.Items.Count > 0)
            {
                ddlProgramme.SelectedIndex = 0;
            }

            txtCourseName.Text = "";
            txtCourseCode.Text = "";
            txtCreditHours.Text = "";

            if (ddlSemester.Items.Count > 0)
            {
                ddlSemester.SelectedIndex = 0;
            }

            if (ddlStatus.Items.Count > 0)
            {
                ddlStatus.SelectedIndex = 0;
            }
        }
    }
}