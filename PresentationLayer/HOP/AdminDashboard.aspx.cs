using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Student_Information_Management_System.HOP
{
    public partial class AdminDashboard : System.Web.UI.Page
    {
        string cs = ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "HOP_ADMIN")
            {
                Response.Redirect("~/PresentationLayer/HOP/LoginPage.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        private void LoadDashboard()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                lblUsers.Text = GetValue(con, "SELECT COUNT(*) FROM [USER]");
                lblStudents.Text = GetValue(con, "SELECT COUNT(*) FROM STUDENT WHERE status='Active'");
                lblLecturers.Text = GetValue(con, "SELECT COUNT(*) FROM LECTURER");
                lblCourses.Text = GetValue(con, "SELECT COUNT(*) FROM COURSE WHERE is_active=1");

                lblEnrolment.Text = GetValue(con, "SELECT COUNT(*) FROM ENROLMENT");
                lblApproved.Text = GetValue(con, "SELECT COUNT(*) FROM ENROLMENT WHERE status='Approved'");
                lblPending.Text = GetValue(con, "SELECT COUNT(*) FROM ENROLMENT WHERE status='Pending'");
                lblRejected.Text = GetValue(con, "SELECT COUNT(*) FROM ENROLMENT WHERE status='Rejected'");

                lblAssignments.Text = GetValue(con, "SELECT COUNT(*) FROM LECTURER_COURSE");
                lblAttendance.Text = GetValue(con, "SELECT COUNT(*) FROM ATTENDANCE");
                lblMarks.Text = GetValue(con, "SELECT COUNT(*) FROM COURSE_MARKS");
                lblNotifications.Text = GetValue(con, "SELECT COUNT(*) FROM NOTIFICATION WHERE is_read = 0");

                lblAverageGPA.Text = GetValue(con,
                    "SELECT ISNULL(CAST(AVG(cgpa) AS DECIMAL(4,2)),0) FROM STUDENT WHERE status='Active'");

                lblRiskStudents.Text = GetValue(con,
                    "SELECT COUNT(*) FROM STUDENT WHERE cgpa < 2.00 AND status='Active'");

                lblPassRate.Text = "0%";
                lblAttendanceRate.Text = "0%";
            }
        }

        private string GetValue(SqlConnection con, string query)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(query, con);
                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return "0";

                return result.ToString();
            }
            catch
            {
                return "0";
            }
        }

        // ==========================
        // NOTIFICATION BUTTON
        // ==========================
        protected void btnNotification_Click(object sender, EventArgs e)
        {
            Response.Redirect("NotificationManagement.aspx");
        }

        // ==========================
        // MESSAGE BUTTON
        // ==========================
        protected void btnMessages_Click(object sender, EventArgs e)
        {
            Response.Redirect("MessageManagement.aspx");
        }

        // ==========================
        // LOGOUT
        // ==========================
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/PresentationLayer/HOP/LoginPage.aspx");
        }
    }
}