using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Student_Information_Management_System.Student
{
    public partial class StudentDashboard : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        private int StudentID
        {
            get
            {
                return Convert.ToInt32(Session["StudentID"]);
            }
        }

        private int UserID
        {
            get
            {
                return Convert.ToInt32(Session["UserID"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null ||
                Session["Role"].ToString() != "STUDENT")
            {
                Response.Redirect("LoginPage.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadProfile();
                LoadSummaryCards();
                LoadCurrentCourses();
                LoadLatestResults();
                LoadNotifications();
                LoadAnnouncements();
                CheckAcademicStanding();
            }
        }

        private void LoadProfile()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        s.full_name,
                        s.matric_number,
                        p.programme_name
                    FROM STUDENT s
                    INNER JOIN PROGRAMME p
                        ON s.programme_id = p.programme_id
                    WHERE s.student_id = @student_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@student_id", StudentID);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblStudentName.Text = dr["full_name"].ToString();
                    lblMatric.Text = dr["matric_number"].ToString();
                    lblProgramme.Text = dr["programme_name"].ToString();
                }
            }
        }

        private void LoadSummaryCards()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                SqlCommand cgpaCmd = new SqlCommand(
                    "SELECT cgpa FROM STUDENT WHERE student_id=@id", con);

                cgpaCmd.Parameters.AddWithValue("@id", StudentID);

                object cgpa = cgpaCmd.ExecuteScalar();

                lblCGPA.Text =
                    cgpa == DBNull.Value ? "0.00" :
                    Convert.ToDecimal(cgpa).ToString("0.00");

                SqlCommand courseCmd = new SqlCommand(@"
                    SELECT COUNT(*)
                    FROM ENROLMENT
                    WHERE student_id=@id
                    AND status='Approved'", con);

                courseCmd.Parameters.AddWithValue("@id", StudentID);

                lblCourses.Text =
                    courseCmd.ExecuteScalar().ToString();

                SqlCommand feeCmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(balance_amount),0)
                    FROM FEE
                    WHERE student_id=@id", con);

                feeCmd.Parameters.AddWithValue("@id", StudentID);

                lblFeeBalance.Text =
                    Convert.ToDecimal(
                    feeCmd.ExecuteScalar())
                    .ToString("0.00");

                SqlCommand attendanceCmd = new SqlCommand(@"
                    SELECT
                    ISNULL(
                    CAST(
                    SUM(CASE WHEN a.status='Present'
                    THEN 1 ELSE 0 END)
                    *100.0/
                    NULLIF(COUNT(*),0)
                    AS DECIMAL(5,2)),0)

                    FROM ATTENDANCE a
                    INNER JOIN ENROLMENT e
                    ON a.enrolment_id=e.enrolment_id

                    WHERE e.student_id=@id", con);

                attendanceCmd.Parameters.AddWithValue(
                    "@id", StudentID);

                lblAttendance.Text =
                    attendanceCmd.ExecuteScalar().ToString() + "%";
            }
        }

        private void CheckAcademicStanding()
        {
            decimal cgpa = 0;
            decimal attendance = 100;

            decimal.TryParse(lblCGPA.Text, out cgpa);

            string att =
                lblAttendance.Text.Replace("%", "");

            decimal.TryParse(att, out attendance);

            if (cgpa < 2.00m || attendance < 75)
            {
                pnlWarning.Visible = true;
                pnlGoodStanding.Visible = false;
            }
            else
            {
                pnlWarning.Visible = false;
                pnlGoodStanding.Visible = true;
            }
        }

        private void LoadCurrentCourses()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        c.course_code,
                        c.course_name,
                        c.credit_hours,
                        c.semester
                    FROM ENROLMENT e
                    INNER JOIN COURSE c
                        ON e.course_id=c.course_id
                    WHERE e.student_id=@student_id
                    AND e.status='Approved'
                    ORDER BY c.course_name";

                SqlDataAdapter da =
                    new SqlDataAdapter(q, con);

                da.SelectCommand.Parameters.AddWithValue(
                    "@student_id",
                    StudentID);

                DataTable dt = new DataTable();

                da.Fill(dt);

                gvCurrentCourses.DataSource = dt;
                gvCurrentCourses.DataBind();
            }
        }

        private void LoadLatestResults()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT TOP 10
                        c.course_code,
                        c.course_name,
                        cm.total_marks,
                        cm.grade,
                        cm.grade_point
                    FROM COURSE_MARKS cm
                    INNER JOIN ENROLMENT e
                        ON cm.enrolment_id=e.enrolment_id
                    INNER JOIN COURSE c
                        ON e.course_id=c.course_id
                    WHERE e.student_id=@student_id
                    ORDER BY cm.mark_id DESC";

                SqlDataAdapter da =
                    new SqlDataAdapter(q, con);

                da.SelectCommand.Parameters.AddWithValue(
                    "@student_id",
                    StudentID);

                DataTable dt = new DataTable();

                da.Fill(dt);

                gvLatestResults.DataSource = dt;
                gvLatestResults.DataBind();
            }
        }

        private void LoadNotifications()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT TOP 10
                        type,
                        message,
                        created_at
                    FROM NOTIFICATION
                    WHERE user_id=@user_id
                    ORDER BY created_at DESC";

                SqlDataAdapter da =
                    new SqlDataAdapter(q, con);

                da.SelectCommand.Parameters.AddWithValue(
                    "@user_id",
                    UserID);

                DataTable dt = new DataTable();

                da.Fill(dt);

                gvNotifications.DataSource = dt;
                gvNotifications.DataBind();
            }
        }

        private void LoadAnnouncements()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT TOP 10
                        a.title,
                        ISNULL(c.course_name,'General')
                        AS course_name,
                        a.posted_at
                    FROM ANNOUNCEMENT a
                    LEFT JOIN COURSE c
                        ON a.course_id=c.course_id
                    ORDER BY a.posted_at DESC";

                SqlDataAdapter da =
                    new SqlDataAdapter(q, con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                gvAnnouncements.DataSource = dt;
                gvAnnouncements.DataBind();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect("LoginPage.aspx");
        }
    }
}