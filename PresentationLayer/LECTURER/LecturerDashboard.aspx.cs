using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Student_Information_Management_System.Lecturer
{
    public partial class LecturerDashboard : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        private int LecturerID
        {
            get { return Convert.ToInt32(Session["LecturerID"]); }
        }

        private int UserID
        {
            get { return Convert.ToInt32(Session["UserID"]); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null ||
                Session["Role"].ToString() != "LECTURER")
            {
                Response.Redirect("LoginPage.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadLecturerProfile();
                LoadDashboardCards();
                LoadTodayClasses();
                LoadRiskStudents();
            }
        }

        private void LoadLecturerProfile()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT full_name, specialisation
                    FROM LECTURER
                    WHERE lecturer_id = @lecturer_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@lecturer_id", LecturerID);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblLecturerName.Text = dr["full_name"].ToString();
                    lblWelcome.Text = dr["full_name"].ToString();
                }
            }
        }

        private void LoadDashboardCards()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                lblCourses.Text = GetValue(con, @"
                    SELECT COUNT(*)
                    FROM LECTURER_COURSE
                    WHERE lecturer_id = @lecturer_id");

                lblStudents.Text = GetValue(con, @"
                    SELECT COUNT(DISTINCT e.student_id)
                    FROM ENROLMENT e
                    INNER JOIN LECTURER_COURSE lc
                        ON e.course_id = lc.course_id
                    WHERE lc.lecturer_id = @lecturer_id
                    AND e.status = 'Approved'");

                lblPendingGrades.Text = GetValue(con, @"
                    SELECT COUNT(*)
                    FROM ENROLMENT e
                    INNER JOIN LECTURER_COURSE lc
                        ON e.course_id = lc.course_id
                    LEFT JOIN COURSE_MARKS cm
                        ON e.enrolment_id = cm.enrolment_id
                    WHERE lc.lecturer_id = @lecturer_id
                    AND e.status = 'Approved'
                    AND cm.mark_id IS NULL");

                lblAlerts.Text = GetValue(con, @"
                    SELECT COUNT(*)
                    FROM ACADEMIC_WARNING aw
                    INNER JOIN STUDENT s
                        ON aw.student_id = s.student_id
                    INNER JOIN ENROLMENT e
                        ON s.student_id = e.student_id
                    INNER JOIN LECTURER_COURSE lc
                        ON e.course_id = lc.course_id
                    WHERE lc.lecturer_id = @lecturer_id
                    AND aw.status = 'Active'");
            }
        }

        private string GetValue(SqlConnection con, string query)
        {
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@lecturer_id", LecturerID);

            object result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                return "0";

            return result.ToString();
        }

        private void LoadTodayClasses()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        c.course_code AS [Course Code],
                        c.course_name AS [Course Name],
                        c.credit_hours AS [Credit Hours],
                        lc.academic_year AS [Academic Year],
                        lc.semester AS [Semester]
                    FROM LECTURER_COURSE lc
                    INNER JOIN COURSE c
                        ON lc.course_id = c.course_id
                    WHERE lc.lecturer_id = @lecturer_id
                    ORDER BY c.course_code";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                da.SelectCommand.Parameters.AddWithValue("@lecturer_id", LecturerID);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvClasses.DataSource = dt;
                gvClasses.DataBind();
            }
        }

        private void LoadRiskStudents()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT DISTINCT TOP 10
                        s.matric_number AS [Matric No],
                        s.full_name AS [Student Name],
                        c.course_code AS [Course Code],
                        c.course_name AS [Course Name],
                        aw.warning_type AS [Warning Type],
                        aw.cgpa AS [CGPA],
                        aw.attendance_percentage AS [Attendance %],
                        aw.created_at AS [Date]
                    FROM ACADEMIC_WARNING aw
                    INNER JOIN STUDENT s
                        ON aw.student_id = s.student_id
                    INNER JOIN ENROLMENT e
                        ON s.student_id = e.student_id
                    INNER JOIN COURSE c
                        ON e.course_id = c.course_id
                    INNER JOIN LECTURER_COURSE lc
                        ON c.course_id = lc.course_id
                    WHERE lc.lecturer_id = @lecturer_id
                    AND aw.status = 'Active'
                    ORDER BY aw.created_at DESC";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                da.SelectCommand.Parameters.AddWithValue("@lecturer_id", LecturerID);

                DataTable dt = new DataTable();
                da.Fill(dt);

                gvRiskStudents.DataSource = dt;
                gvRiskStudents.DataBind();
            }
        }

        protected void btnAttendance_Click(object sender, EventArgs e)
        {
            Response.Redirect("Attendance.aspx");
        }

        protected void btnGrades_Click(object sender, EventArgs e)
        {
            Response.Redirect("GradeManagement.aspx");
        }

        protected void btnAnnouncement_Click(object sender, EventArgs e)
        {
            Response.Redirect("Announcements.aspx");
        }

        protected void btnMessages_Click(object sender, EventArgs e)
        {
            Response.Redirect("Messages.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("LoginPage.aspx");
        }
    }
}