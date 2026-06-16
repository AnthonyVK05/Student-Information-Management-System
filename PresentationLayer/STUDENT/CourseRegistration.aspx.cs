using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.STUDENT
{
    public partial class CourseRegistration : System.Web.UI.Page
    {
        string cs = ConfigurationManager
            .ConnectionStrings["UniversityDB"]
            .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // CHECK LOGIN

            if (Session["StudentEmail"] == null)
            {
                Response.Redirect("LoginPage.aspx");
            }

            if (!IsPostBack)
            {
                LoadSession();
                LoadCourses();
                LoadEnrollment();
            }
        }

        // LOAD SESSION

        void LoadSession()
        {
            SqlConnection con = new SqlConnection(cs);

            string query =
                "SELECT DISTINCT SessionName FROM Courses";

            SqlDataAdapter sda =
                new SqlDataAdapter(query, con);

            DataTable dt = new DataTable();

            sda.Fill(dt);

            ddlSession.DataSource = dt;

            ddlSession.DataTextField = "SessionName";

            ddlSession.DataValueField = "SessionName";

            ddlSession.DataBind();
        }

        // LOAD COURSES

        void LoadCourses()
        {
            SqlConnection con = new SqlConnection(cs);

            string query = @"SELECT *
                             FROM Courses
                             WHERE SessionName=@SessionName
                             AND Semester=@Semester";

            SqlCommand cmd =
                new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@SessionName",
                ddlSession.SelectedValue);

            cmd.Parameters.AddWithValue("@Semester",
                ddlSemester.SelectedValue);

            SqlDataAdapter sda =
                new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            sda.Fill(dt);

            gvCourses.DataSource = dt;

            gvCourses.DataBind();
        }

        // LOAD ENROLLMENT

        void LoadEnrollment()
        {
            SqlConnection con = new SqlConnection(cs);

            string query = @"

            SELECT
            EM.EnrolmentID,
            EM.StudentName,
            C.CourseCode,
            C.CourseName,
            C.CreditHours,
            EM.Status

            FROM EnrollmentMaster EM

            INNER JOIN EnrollmentDetails ED
            ON EM.EnrolmentID = ED.EnrolmentID

            INNER JOIN Courses C
            ON ED.CourseID = C.CourseID

            WHERE EM.StudentEmail=@StudentEmail";

            SqlCommand cmd =
                new SqlCommand(query, con);

            cmd.Parameters.AddWithValue("@StudentEmail",
                Session["StudentEmail"].ToString());

            SqlDataAdapter sda =
                new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            sda.Fill(dt);

            gvEnrollment.DataSource = dt;

            gvEnrollment.DataBind();
        }

        // SESSION DROPDOWN

        protected void ddlSession_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCourses();
        }

        // SEMESTER DROPDOWN

        protected void ddlSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCourses();
        }

        // SUBMIT ENROLLMENT

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(cs);

            con.Open();

            // INSERT MASTER RECORD

            string masterQuery = @"INSERT INTO EnrollmentMaster
                                  (StudentName,
                                   StudentEmail,
                                   SessionName,
                                   Semester,
                                   Status)

                                   OUTPUT INSERTED.EnrolmentID

                                   VALUES
                                  (@StudentName,
                                   @StudentEmail,
                                   @SessionName,
                                   @Semester,
                                   @Status)";

            SqlCommand masterCmd =
                new SqlCommand(masterQuery, con);

            masterCmd.Parameters.AddWithValue("@StudentName",
                Session["StudentName"].ToString());

            masterCmd.Parameters.AddWithValue("@StudentEmail",
                Session["StudentEmail"].ToString());

            masterCmd.Parameters.AddWithValue("@SessionName",
                ddlSession.SelectedValue);

            masterCmd.Parameters.AddWithValue("@Semester",
                ddlSemester.SelectedValue);

            masterCmd.Parameters.AddWithValue("@Status",
                "Pending");

            int enrolmentID =
                Convert.ToInt32(masterCmd.ExecuteScalar());

            // INSERT COURSE DETAILS

            foreach (GridViewRow row in gvCourses.Rows)
            {
                CheckBox chk =
                    (CheckBox)row.FindControl("chkSelect");

                if (chk.Checked)
                {
                    int courseID =
                        Convert.ToInt32(row.Cells[1].Text);

                    string detailQuery = @"INSERT INTO EnrollmentDetails
                                          (EnrolmentID, CourseID)

                                           VALUES
                                          (@EnrolmentID, @CourseID)";

                    SqlCommand detailCmd =
                        new SqlCommand(detailQuery, con);

                    detailCmd.Parameters.AddWithValue("@EnrolmentID",
                        enrolmentID);

                    detailCmd.Parameters.AddWithValue("@CourseID",
                        courseID);

                    detailCmd.ExecuteNonQuery();
                }
            }

            con.Close();

            lblMessage.Text =
                "Enrollment Submitted Successfully";

            LoadEnrollment();
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();              // clear session
            Response.Redirect("StudentLogin.aspx"); // go back to login
        }
    }
}
