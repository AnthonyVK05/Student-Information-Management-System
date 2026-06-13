using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class EnrolmentManagement : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStudents();
                LoadCourses();
                LoadCalendars();
                LoadEnrolments();

                txtEnrolDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void LoadStudents()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        student_id,
                        matric_number + ' - ' + full_name AS student_display
                    FROM STUDENT
                    WHERE status = 'Active'
                    ORDER BY full_name";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlStudent.DataSource = dt;
                ddlStudent.DataTextField = "student_display";
                ddlStudent.DataValueField = "student_id";
                ddlStudent.DataBind();

                ddlStudent.Items.Insert(0, new ListItem("-- Select Student --", ""));
            }
        }

        private void LoadCourses()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        course_id,
                        course_code + ' - ' + course_name AS course_display
                    FROM COURSE
                    WHERE is_active = 1
                    ORDER BY course_code";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlCourse.DataSource = dt;
                ddlCourse.DataTextField = "course_display";
                ddlCourse.DataValueField = "course_id";
                ddlCourse.DataBind();

                ddlCourse.Items.Insert(0, new ListItem("-- Select Course --", ""));
            }
        }

        private void LoadCoursesByStudentProgramme()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        c.course_id,
                        c.course_code + ' - ' + c.course_name AS course_display
                    FROM COURSE c
                    INNER JOIN STUDENT s
                        ON c.programme_id = s.programme_id
                    WHERE s.student_id = @student_id
                    AND c.is_active = 1
                    ORDER BY c.course_code";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@student_id", ddlStudent.SelectedValue);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlCourse.DataSource = dt;
                ddlCourse.DataTextField = "course_display";
                ddlCourse.DataValueField = "course_id";
                ddlCourse.DataBind();

                ddlCourse.Items.Insert(0, new ListItem("-- Select Course --", ""));
            }
        }

        protected void ddlStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlStudent.SelectedValue))
            {
                LoadCourses();
                return;
            }

            LoadCoursesByStudentProgramme();
        }

        private void LoadCalendars()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        calendar_id,
                        academic_year + ' - ' + semester AS calendar_display
                    FROM ACADEMIC_CALENDAR
                    WHERE status IN ('Open', 'Upcoming')
                    ORDER BY calendar_id DESC";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlCalendar.DataSource = dt;
                ddlCalendar.DataTextField = "calendar_display";
                ddlCalendar.DataValueField = "calendar_id";
                ddlCalendar.DataBind();

                ddlCalendar.Items.Insert(0, new ListItem("-- Select Academic Calendar --", ""));
            }
        }

        private void LoadEnrolments()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string search = "";

                if (txtSearch != null)
                {
                    search = txtSearch.Text.Trim();
                }

                string statusFilter = "All";

                if (ddlSearchStatus != null)
                {
                    statusFilter = ddlSearchStatus.SelectedValue;
                }

                string q = @"
                    SELECT
                        e.enrolment_id,
                        e.student_id,
                        s.matric_number,
                        s.nric_passport,
                        s.full_name AS student_name,
                        p.programme_name,
                        s.status AS student_status,

                        CASE
                            WHEN u.email_verified = 1 THEN 'Verified'
                            WHEN u.verification_expiry IS NOT NULL
                                 AND u.verification_expiry < GETDATE() THEN 'Expired'
                            ELSE 'Pending Verification'
                        END AS verification_status,

                        e.course_id,
                        c.course_code,
                        c.course_name,
                        c.credit_hours,
                        e.calendar_id,
                        ac.academic_year + ' - ' + ac.semester AS calendar_name,
                        e.semester,
                        e.academic_year,
                        e.enrol_date,
                        e.status
                    FROM ENROLMENT e
                    INNER JOIN STUDENT s
                        ON e.student_id = s.student_id
                    INNER JOIN [USER] u
                        ON s.user_id = u.user_id
                    INNER JOIN PROGRAMME p
                        ON s.programme_id = p.programme_id
                    INNER JOIN COURSE c
                        ON e.course_id = c.course_id
                    LEFT JOIN ACADEMIC_CALENDAR ac
                        ON e.calendar_id = ac.calendar_id
                    WHERE
                    (
                        @search = ''
                        OR s.matric_number LIKE @searchLike
                        OR s.nric_passport LIKE @searchLike
                        OR s.full_name LIKE @searchLike
                        OR p.programme_name LIKE @searchLike
                        OR c.course_code LIKE @searchLike
                        OR c.course_name LIKE @searchLike
                    )
                    AND
                    (
                        @status = 'All'
                        OR e.status = @status
                    )
                    ORDER BY e.enrolment_id DESC";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@searchLike", "%" + search + "%");
                cmd.Parameters.AddWithValue("@status", statusFilter);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvEnrolments.DataSource = dt;
                gvEnrolments.DataBind();
            }
        }

        protected string MaskNRIC(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "";
            }

            string nric = value.ToString();

            if (nric.Length <= 6)
            {
                return nric;
            }

            return nric.Substring(0, nric.Length - 6) + "******";
        }

        protected void ddlCalendar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlCalendar.SelectedValue))
            {
                txtAcademicYear.Text = "";
                txtSemester.Text = "";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT academic_year, semester
                    FROM ACADEMIC_CALENDAR
                    WHERE calendar_id = @calendar_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@calendar_id", ddlCalendar.SelectedValue);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    txtAcademicYear.Text = dr["academic_year"].ToString();
                    txtSemester.Text = dr["semester"].ToString();
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            if (EnrolmentExists(
                ddlStudent.SelectedValue,
                ddlCourse.SelectedValue,
                txtAcademicYear.Text.Trim(),
                txtSemester.Text.Trim(),
                ""))
            {
                lblMessage.Text =
                    "This student is already enrolled in this course for the selected academic session.";

                lblMessage.CssClass =
                    "text-danger fw-bold";

                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    INSERT INTO ENROLMENT
                    (
                        student_id,
                        course_id,
                        calendar_id,
                        semester,
                        academic_year,
                        enrol_date,
                        status
                    )
                    VALUES
                    (
                        @student_id,
                        @course_id,
                        @calendar_id,
                        @semester,
                        @academic_year,
                        @enrol_date,
                        @status
                    )";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@student_id", ddlStudent.SelectedValue);
                cmd.Parameters.AddWithValue("@course_id", ddlCourse.SelectedValue);
                cmd.Parameters.AddWithValue("@calendar_id", ddlCalendar.SelectedValue);
                cmd.Parameters.AddWithValue("@semester", txtSemester.Text.Trim());
                cmd.Parameters.AddWithValue("@academic_year", txtAcademicYear.Text.Trim());
                cmd.Parameters.AddWithValue("@enrol_date", GetDateOrDBNull(txtEnrolDate.Text.Trim()));
                cmd.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Enrolment added successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadEnrolments();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfEnrolmentID.Value))
            {
                lblMessage.Text = "Please select an enrolment record first.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            if (!ValidateForm())
            {
                return;
            }

            if (EnrolmentExists(
                ddlStudent.SelectedValue,
                ddlCourse.SelectedValue,
                txtAcademicYear.Text.Trim(),
                txtSemester.Text.Trim(),
                hfEnrolmentID.Value))
            {
                lblMessage.Text =
                    "Another enrolment record already exists for this student, course and academic session.";

                lblMessage.CssClass =
                    "text-danger fw-bold";

                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    UPDATE ENROLMENT
                    SET
                        student_id = @student_id,
                        course_id = @course_id,
                        calendar_id = @calendar_id,
                        semester = @semester,
                        academic_year = @academic_year,
                        enrol_date = @enrol_date,
                        status = @status
                    WHERE enrolment_id = @enrolment_id";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@student_id", ddlStudent.SelectedValue);
                cmd.Parameters.AddWithValue("@course_id", ddlCourse.SelectedValue);
                cmd.Parameters.AddWithValue("@calendar_id", ddlCalendar.SelectedValue);
                cmd.Parameters.AddWithValue("@semester", txtSemester.Text.Trim());
                cmd.Parameters.AddWithValue("@academic_year", txtAcademicYear.Text.Trim());
                cmd.Parameters.AddWithValue("@enrol_date", GetDateOrDBNull(txtEnrolDate.Text.Trim()));
                cmd.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@enrolment_id", hfEnrolmentID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Enrolment updated successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadEnrolments();
        }

        protected void btnApprove_Click(object sender, EventArgs e)
        {
            UpdateStatus("Approved");
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            UpdateStatus("Rejected");
        }

        protected void btnDrop_Click(object sender, EventArgs e)
        {
            UpdateStatus("Dropped");
        }

        private void UpdateStatus(string status)
        {
            if (string.IsNullOrEmpty(hfEnrolmentID.Value))
            {
                lblMessage.Text = "Please select an enrolment record first.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    UPDATE ENROLMENT
                    SET status = @status
                    WHERE enrolment_id = @enrolment_id";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@enrolment_id", hfEnrolmentID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Enrolment status updated to " + status + ".";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadEnrolments();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfEnrolmentID.Value))
            {
                lblMessage.Text = "Please select an enrolment record first.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q =
                    "DELETE FROM ENROLMENT WHERE enrolment_id = @enrolment_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@enrolment_id", hfEnrolmentID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Enrolment deleted successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadEnrolments();
        }

        protected void gvEnrolments_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvEnrolments.SelectedRow;

            hfEnrolmentID.Value =
                gvEnrolments.DataKeys[row.RowIndex].Value.ToString();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        student_id,
                        course_id,
                        calendar_id,
                        semester,
                        academic_year,
                        enrol_date,
                        status
                    FROM ENROLMENT
                    WHERE enrolment_id = @enrolment_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@enrolment_id", hfEnrolmentID.Value);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    string studentId = dr["student_id"].ToString();

                    if (ddlStudent.Items.FindByValue(studentId) != null)
                    {
                        ddlStudent.SelectedValue = studentId;
                    }

                    LoadCoursesByStudentProgramme();

                    string courseId = dr["course_id"].ToString();

                    if (ddlCourse.Items.FindByValue(courseId) != null)
                    {
                        ddlCourse.SelectedValue = courseId;
                    }

                    string calendarId = dr["calendar_id"].ToString();

                    if (ddlCalendar.Items.FindByValue(calendarId) != null)
                    {
                        ddlCalendar.SelectedValue = calendarId;
                    }

                    txtSemester.Text = dr["semester"].ToString();
                    txtAcademicYear.Text = dr["academic_year"].ToString();

                    if (dr["enrol_date"] != DBNull.Value)
                    {
                        txtEnrolDate.Text =
                            Convert.ToDateTime(dr["enrol_date"]).ToString("yyyy-MM-dd");
                    }

                    ddlStatus.SelectedValue = dr["status"].ToString();
                }
            }

            lblMessage.Text =
                "Enrolment selected. You can update, approve, reject, drop or delete it.";

            lblMessage.CssClass =
                "text-primary fw-bold";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadEnrolments();
        }

        protected void btnResetSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";

            if (ddlSearchStatus.Items.Count > 0)
            {
                ddlSearchStatus.SelectedIndex = 0;
            }

            LoadEnrolments();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            lblMessage.Text = "";
        }

        private bool ValidateForm()
        {
            if (ddlStudent.SelectedValue == "")
            {
                lblMessage.Text = "Please select student.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (ddlCourse.SelectedValue == "")
            {
                lblMessage.Text = "Please select course.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (ddlCalendar.SelectedValue == "")
            {
                lblMessage.Text = "Please select academic calendar.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSemester.Text))
            {
                lblMessage.Text = "Please enter semester.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAcademicYear.Text))
            {
                lblMessage.Text = "Please enter academic year.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEnrolDate.Text))
            {
                lblMessage.Text = "Please select enrolment date.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            return true;
        }

        private bool EnrolmentExists(
            string studentId,
            string courseId,
            string academicYear,
            string semester,
            string currentEnrolmentId
        )
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT COUNT(*)
                    FROM ENROLMENT
                    WHERE student_id = @student_id
                    AND course_id = @course_id
                    AND academic_year = @academic_year
                    AND semester = @semester
                    AND status <> 'Dropped'
                    AND (@enrolment_id = '' OR enrolment_id <> @enrolment_id)";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@student_id", studentId);
                cmd.Parameters.AddWithValue("@course_id", courseId);
                cmd.Parameters.AddWithValue("@academic_year", academicYear);
                cmd.Parameters.AddWithValue("@semester", semester);
                cmd.Parameters.AddWithValue("@enrolment_id", currentEnrolmentId ?? "");

                con.Open();

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count > 0;
            }
        }

        private object GetDateOrDBNull(string dateText)
        {
            if (string.IsNullOrWhiteSpace(dateText))
            {
                return DBNull.Value;
            }

            return Convert.ToDateTime(dateText);
        }

        private void Clear()
        {
            hfEnrolmentID.Value = "";

            if (ddlStudent.Items.Count > 0)
            {
                ddlStudent.SelectedIndex = 0;
            }

            LoadCourses();

            if (ddlCourse.Items.Count > 0)
            {
                ddlCourse.SelectedIndex = 0;
            }

            if (ddlCalendar.Items.Count > 0)
            {
                ddlCalendar.SelectedIndex = 0;
            }

            txtSemester.Text = "";
            txtAcademicYear.Text = "";
            txtEnrolDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            if (ddlStatus.Items.Count > 0)
            {
                ddlStatus.SelectedIndex = 0;
            }
        }
    }
}