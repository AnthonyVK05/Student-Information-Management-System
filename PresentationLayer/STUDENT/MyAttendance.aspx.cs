using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.STUDENT
{
    public partial class MyAttendance : System.Web.UI.Page
    {

    private readonly string cs =
    ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCourses();
                LoadAttendance();
                LoadAttendanceStatistics();
            }
        }

        private void LoadCourses()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string studentId = Session["StudentID"]?.ToString() ?? "1";

                string q = @"
                    SELECT DISTINCT 
                        c.course_id,
                        c.course_code + ' - ' + c.course_name AS course_display
                    FROM ENROLMENT e
                    INNER JOIN COURSE c ON e.course_id = c.course_id
                    WHERE e.student_id = @student_id
                    AND e.status = 'Active'
                    ORDER BY c.course_code";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@student_id", studentId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                ddlCourseFilter.DataSource = dr;
                ddlCourseFilter.DataTextField = "course_display";
                ddlCourseFilter.DataValueField = "course_id";
                ddlCourseFilter.DataBind();

                dr.Close();

                // Add "All Courses" option if not already present
                if (ddlCourseFilter.Items.FindByValue("All") == null)
                {
                    ddlCourseFilter.Items.Insert(0, new ListItem("All Courses", "All"));
                }
            }
        }
        private void LoadAttendance()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string search = txtSearch == null ? "" : txtSearch.Text.Trim();
                string status = ddlSearchStatus == null ? "All" : ddlSearchStatus.SelectedValue;
                string searchDate = txtSearchDate == null ? "" : txtSearchDate.Text.Trim();

                string q = @"
                    SELECT
                        a.attendance_id,
                        a.enrolment_id,
                        s.matric_number,
                        s.nric_passport,
                        s.full_name AS student_name,
                        p.programme_name,
                        c.course_code,
                        c.course_name,
                        e.status AS enrolment_status,
                        a.lecturer_id,
                        l.full_name AS lecturer_name,
                        a.class_date,
                        a.status,
                        a.remarks
                    FROM ATTENDANCE a
                    INNER JOIN ENROLMENT e
                        ON a.enrolment_id = e.enrolment_id
                    INNER JOIN STUDENT s
                        ON e.student_id = s.student_id
                    INNER JOIN PROGRAMME p
                        ON s.programme_id = p.programme_id
                    INNER JOIN COURSE c
                        ON e.course_id = c.course_id
                    INNER JOIN LECTURER l
                        ON a.lecturer_id = l.lecturer_id
                    WHERE
                    (
                        @search = ''
                        OR s.matric_number LIKE @searchLike
                        OR s.nric_passport LIKE @searchLike
                        OR s.full_name LIKE @searchLike
                        OR p.programme_name LIKE @searchLike
                        OR c.course_code LIKE @searchLike
                        OR c.course_name LIKE @searchLike
                        OR l.full_name LIKE @searchLike
                    )
                    AND
                    (
                        @status = 'All'
                        OR a.status = @status
                    )
                    AND
                    (
                        @class_date = ''
                        OR CONVERT(VARCHAR(10), a.class_date, 120) = @class_date
                    )
                    ORDER BY a.attendance_id DESC";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@searchLike", "%" + search + "%");
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@class_date", searchDate);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvAttendance.DataSource = dt;
                gvAttendance.DataBind();
            }
        }

        private void LoadAttendanceStatistics()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT 
                        COUNT(*) AS total_classes,
                        SUM(CASE WHEN status = 'Present' THEN 1 ELSE 0 END) AS present_count,
                        SUM(CASE WHEN status = 'Late' THEN 1 ELSE 0 END) AS late_count,
                        SUM(CASE WHEN status = 'Absent' THEN 1 ELSE 0 END) AS absent_count,
                        SUM(CASE WHEN status = 'Excused' THEN 1 ELSE 0 END) AS excused_count
                    FROM ATTENDANCE a
                    INNER JOIN ENROLMENT e ON a.enrolment_id = e.enrolment_id
                    INNER JOIN STUDENT s ON e.student_id = s.student_id
                    WHERE s.student_id = @student_id";

                // Get the current student's ID from session
                string studentId = Session["StudentID"]?.ToString() ?? "1";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@student_id", studentId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    int totalClasses = Convert.ToInt32(dr["total_classes"]);
                    int present = Convert.ToInt32(dr["present_count"]);
                    int late = Convert.ToInt32(dr["late_count"]);
                    int absent = Convert.ToInt32(dr["absent_count"]);
                    int excused = Convert.ToInt32(dr["excused_count"]);

                    // Calculate attendance percentage (Present + Late are considered as attended)
                    int attended = present + late;
                    double percentage = totalClasses > 0 ? (double)attended / totalClasses * 100 : 0;

                    // Update labels
                    lblTotalClasses.Text = totalClasses.ToString();
                    lblPresent.Text = present.ToString();
                    lblLate.Text = late.ToString();
                    lblAbsent.Text = absent.ToString();
                    lblExcused.Text = excused.ToString();

                    // Update percentage display
                    string percentageText = percentage.ToString("F1") + "%";
                    lblAttendancePercentage.Text = percentageText;

                    // Update progress bar
                    lblProgressBar.Text = percentageText;
                    lblProgressBar.Style["width"] = percentage.ToString("F1") + "%";

                    // Set progress bar color based on percentage
                    if (percentage >= 80)
                    {
                        lblProgressBar.Style["background-color"] = "#28a745"; // Green
                    }
                    else if (percentage >= 60)
                    {
                        lblProgressBar.Style["background-color"] = "#ffc107"; // Yellow
                    }
                    else if (percentage >= 40)
                    {
                        lblProgressBar.Style["background-color"] = "#fd7e14"; // Orange
                    }
                    else
                    {
                        lblProgressBar.Style["background-color"] = "#dc3545"; // Red
                    }

                    // Show warning if below 80%
                    if (percentage < 80 && totalClasses > 0)
                    {
                        lblProgressBar.Text += " ⚠️";
                    }
                }

                dr.Close();
            }
        }


        protected void gvAttendance_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvAttendance.SelectedRow;

            hfAttendanceID.Value =
                gvAttendance.DataKeys[row.RowIndex].Value.ToString();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        enrolment_id,
                        lecturer_id,
                        class_date,
                        status,
                        remarks
                    FROM ATTENDANCE
                    WHERE attendance_id = @attendance_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@attendance_id", hfAttendanceID.Value);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();


                }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadAttendance();
            LoadAttendanceStatistics();
        }

        protected void btnResetSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            txtSearchDate.Text = "";

            if (ddlSearchStatus.Items.Count > 0)
            {
                ddlSearchStatus.SelectedIndex = 0;
            }

            LoadAttendance();
            LoadAttendanceStatistics();
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

        private bool AttendanceExists(
         string enrolmentId,
         string lecturerId,
         string classDate,
         string currentAttendanceId
        )

        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT COUNT(*)
                    FROM ATTENDANCE
                    WHERE enrolment_id = @enrolment_id
                    AND lecturer_id = @lecturer_id
                    AND class_date = @class_date
                    AND (@attendance_id = '' OR attendance_id <> @attendance_id)";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@enrolment_id", enrolmentId);
                cmd.Parameters.AddWithValue("@lecturer_id", lecturerId);
                cmd.Parameters.AddWithValue("@class_date", classDate);
                cmd.Parameters.AddWithValue("@attendance_id", currentAttendanceId ?? "");

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
    }
}
