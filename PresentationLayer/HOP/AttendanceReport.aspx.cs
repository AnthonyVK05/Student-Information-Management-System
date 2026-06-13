using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Student_Information_Management_System.HOP
{
    public partial class AttendanceReport : System.Web.UI.Page
    {
        string cs = ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAttendanceReport();
            }
        }

        private void LoadAttendanceReport()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        s.matric_number,
                        s.full_name,
                        c.course_name,
                        COUNT(a.attendance_id) AS total_classes,
                        SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) AS present_classes,
                        CAST(
                            (SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) * 100.0)
                            / NULLIF(COUNT(a.attendance_id), 0)
                            AS DECIMAL(5,2)
                        ) AS attendance_percentage,
                        CASE
                            WHEN CAST(
                                (SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) * 100.0)
                                / NULLIF(COUNT(a.attendance_id), 0)
                                AS DECIMAL(5,2)
                            ) < 80 THEN 'Poor Attendance'
                            ELSE 'Good'
                        END AS status
                    FROM ATTENDANCE a
                    INNER JOIN ENROLMENT e ON a.enrolment_id = e.enrolment_id
                    INNER JOIN STUDENT s ON e.student_id = s.student_id
                    INNER JOIN COURSE c ON e.course_id = c.course_id
                    GROUP BY s.matric_number, s.full_name, c.course_name
                    ORDER BY attendance_percentage ASC";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvAttendanceReport.DataSource = dt;
                gvAttendanceReport.DataBind();
            }
        }
    }
}