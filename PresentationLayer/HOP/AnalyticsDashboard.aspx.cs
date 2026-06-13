using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class AnalyticsDashboard : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadAcademicYears();
                LoadProgrammes();
                LoadAllAnalytics();
            }
        }

        private void LoadAllAnalytics()
        {
            LoadAnalyticsCards();
            LoadProgrammeStats();
            LoadCourseStats();
            LoadGradeDistribution();
            LoadAttendanceStats();
            LoadRiskOverview();
        }

        private void LoadAcademicYears()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT DISTINCT academic_year
                    FROM ENROLMENT
                    WHERE academic_year IS NOT NULL
                    ORDER BY academic_year DESC";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlAcademicYear.DataSource = dt;
                ddlAcademicYear.DataTextField = "academic_year";
                ddlAcademicYear.DataValueField = "academic_year";
                ddlAcademicYear.DataBind();

                ddlAcademicYear.Items.Insert(0, new ListItem("All Academic Years", "All"));
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

                ddlProgramme.Items.Insert(0, new ListItem("All Programmes", "All"));
            }
        }

        private void LoadAnalyticsCards()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                lblAverageCGPA.Text = GetSingleValue(con, @"
                    SELECT CAST(ISNULL(AVG(student_cgpa), 0) AS DECIMAL(4,2))
                    FROM
                    (
                        SELECT
                            e.student_id,
                            SUM(cm.grade_point * c.credit_hours) / NULLIF(SUM(c.credit_hours), 0) AS student_cgpa
                        FROM COURSE_MARKS cm
                        INNER JOIN ENROLMENT e ON cm.enrolment_id = e.enrolment_id
                        INNER JOIN COURSE c ON e.course_id = c.course_id
                        INNER JOIN STUDENT s ON e.student_id = s.student_id
                        WHERE " + FilterCondition("e", "s") + @"
                        GROUP BY e.student_id
                    ) x");

                lblRiskStudents.Text = GetSingleValue(con, @"
                    SELECT COUNT(*)
                    FROM
                    (
                        SELECT
                            e.student_id,
                            SUM(cm.grade_point * c.credit_hours) / NULLIF(SUM(c.credit_hours), 0) AS student_cgpa
                        FROM COURSE_MARKS cm
                        INNER JOIN ENROLMENT e ON cm.enrolment_id = e.enrolment_id
                        INNER JOIN COURSE c ON e.course_id = c.course_id
                        INNER JOIN STUDENT s ON e.student_id = s.student_id
                        WHERE " + FilterCondition("e", "s") + @"
                        GROUP BY e.student_id
                        HAVING SUM(cm.grade_point * c.credit_hours) / NULLIF(SUM(c.credit_hours), 0) < 2.00
                    ) x");

                lblAverageAttendance.Text = GetSingleValue(con, @"
                    SELECT
                        CAST(
                            ISNULL(
                                (SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) * 100.0)
                                / NULLIF(COUNT(a.attendance_id), 0),
                            0)
                        AS DECIMAL(5,2))
                    FROM ATTENDANCE a
                    INNER JOIN ENROLMENT e ON a.enrolment_id = e.enrolment_id
                    INNER JOIN STUDENT s ON e.student_id = s.student_id
                    WHERE " + FilterCondition("e", "s")) + "%";

                lblApprovedEnrolments.Text = GetSingleValue(con, @"
                    SELECT COUNT(*)
                    FROM ENROLMENT e
                    INNER JOIN STUDENT s ON e.student_id = s.student_id
                    WHERE e.status = 'Approved'
                    AND " + FilterCondition("e", "s"));

                lblActiveStudents.Text = GetSingleValue(con, @"
                    SELECT COUNT(*)
                    FROM STUDENT s
                    WHERE s.status = 'Active'
                    AND " + StudentOnlyProgrammeFilter("s"));

                lblDroppedStudents.Text = GetSingleValue(con, @"
                    SELECT COUNT(*)
                    FROM STUDENT s
                    WHERE s.status = 'Dropped'
                    AND " + StudentOnlyProgrammeFilter("s"));

                lblVerifiedAccounts.Text = GetSingleValue(con, @"
                    SELECT COUNT(*)
                    FROM [USER]
                    WHERE email_verified = 1");

                lblPendingVerification.Text = GetSingleValue(con, @"
                    SELECT COUNT(*)
                    FROM [USER]
                    WHERE email_verified = 0");
            }
        }

        private string GetSingleValue(SqlConnection con, string query)
        {
            SqlCommand cmd = new SqlCommand(query, con);
            AddFilterParameters(cmd);

            object result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
                return "0";

            return result.ToString();
        }

        private void LoadProgrammeStats()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        p.programme_name,
                        SUM(CASE WHEN s.status = 'Active' THEN 1 ELSE 0 END) AS active_students,
                        SUM(CASE WHEN s.status = 'Dropped' THEN 1 ELSE 0 END) AS dropped_students,
                        COUNT(s.student_id) AS total_students,
                        CAST(ISNULL(AVG(ISNULL(s.cgpa, 0)), 0) AS DECIMAL(4,2)) AS average_cgpa
                    FROM PROGRAMME p
                    LEFT JOIN STUDENT s
                        ON p.programme_id = s.programme_id
                    WHERE
                    (
                        @programme_id IS NULL
                        OR p.programme_id = @programme_id
                    )
                    GROUP BY p.programme_name
                    ORDER BY total_students DESC";

                SqlCommand cmd = new SqlCommand(q, con);
                AddFilterParameters(cmd);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvProgrammeStats.DataSource = dt;
                gvProgrammeStats.DataBind();
            }
        }

        private void LoadCourseStats()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        c.course_code,
                        c.course_name,
                        c.credit_hours,
                        SUM(CASE WHEN e.status = 'Approved' THEN 1 ELSE 0 END) AS approved_count,
                        SUM(CASE WHEN e.status = 'Pending' THEN 1 ELSE 0 END) AS pending_count,
                        SUM(CASE WHEN e.status = 'Rejected' THEN 1 ELSE 0 END) AS rejected_count,
                        SUM(CASE WHEN e.status = 'Dropped' THEN 1 ELSE 0 END) AS dropped_count,
                        COUNT(e.enrolment_id) AS total_enrolments
                    FROM COURSE c
                    LEFT JOIN ENROLMENT e
                        ON c.course_id = e.course_id
                    LEFT JOIN STUDENT s
                        ON e.student_id = s.student_id
                    WHERE
                    (
                        @academic_year = 'All'
                        OR e.academic_year = @academic_year
                        OR e.academic_year IS NULL
                    )
                    AND
                    (
                        @semester = 'All'
                        OR e.semester = @semester
                        OR e.semester IS NULL
                    )
                    AND
                    (
                        @programme_id IS NULL
                        OR c.programme_id = @programme_id
                    )
                    GROUP BY
                        c.course_code,
                        c.course_name,
                        c.credit_hours
                    ORDER BY total_enrolments DESC";

                SqlCommand cmd = new SqlCommand(q, con);
                AddFilterParameters(cmd);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvCourseStats.DataSource = dt;
                gvCourseStats.DataBind();
            }
        }

        private void LoadGradeDistribution()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        cm.grade,
                        COUNT(*) AS student_count,
                        CAST(
                            COUNT(*) * 100.0 /
                            NULLIF(
                                (
                                    SELECT COUNT(*)
                                    FROM COURSE_MARKS cm2
                                    INNER JOIN ENROLMENT e2
                                        ON cm2.enrolment_id = e2.enrolment_id
                                    INNER JOIN STUDENT s2
                                        ON e2.student_id = s2.student_id
                                    WHERE
                                    (
                                        @academic_year = 'All'
                                        OR e2.academic_year = @academic_year
                                    )
                                    AND
                                    (
                                        @semester = 'All'
                                        OR e2.semester = @semester
                                    )
                                    AND
                                    (
                                        @programme_id IS NULL
                                        OR s2.programme_id = @programme_id
                                    )
                                ),
                                0
                            )
                        AS DECIMAL(5,2)) AS percentage
                    FROM COURSE_MARKS cm
                    INNER JOIN ENROLMENT e
                        ON cm.enrolment_id = e.enrolment_id
                    INNER JOIN STUDENT s
                        ON e.student_id = s.student_id
                    WHERE " + FilterCondition("e", "s") + @"
                    GROUP BY cm.grade
                    ORDER BY
                        CASE cm.grade
                            WHEN 'A' THEN 1
                            WHEN 'A-' THEN 2
                            WHEN 'B+' THEN 3
                            WHEN 'B' THEN 4
                            WHEN 'B-' THEN 5
                            WHEN 'C+' THEN 6
                            WHEN 'C' THEN 7
                            WHEN 'D' THEN 8
                            WHEN 'F' THEN 9
                            ELSE 10
                        END";

                SqlCommand cmd = new SqlCommand(q, con);
                AddFilterParameters(cmd);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvGradeDistribution.DataSource = dt;
                gvGradeDistribution.DataBind();
            }
        }

        private void LoadAttendanceStats()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        a.status AS attendance_status,
                        COUNT(*) AS total_records,
                        CAST(
                            COUNT(*) * 100.0 /
                            NULLIF(
                                (
                                    SELECT COUNT(*)
                                    FROM ATTENDANCE a2
                                    INNER JOIN ENROLMENT e2
                                        ON a2.enrolment_id = e2.enrolment_id
                                    INNER JOIN STUDENT s2
                                        ON e2.student_id = s2.student_id
                                    WHERE
                                    (
                                        @academic_year = 'All'
                                        OR e2.academic_year = @academic_year
                                    )
                                    AND
                                    (
                                        @semester = 'All'
                                        OR e2.semester = @semester
                                    )
                                    AND
                                    (
                                        @programme_id IS NULL
                                        OR s2.programme_id = @programme_id
                                    )
                                ),
                                0
                            )
                        AS DECIMAL(5,2)) AS percentage
                    FROM ATTENDANCE a
                    INNER JOIN ENROLMENT e
                        ON a.enrolment_id = e.enrolment_id
                    INNER JOIN STUDENT s
                        ON e.student_id = s.student_id
                    WHERE " + FilterCondition("e", "s") + @"
                    GROUP BY a.status
                    ORDER BY total_records DESC";

                SqlCommand cmd = new SqlCommand(q, con);
                AddFilterParameters(cmd);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvAttendanceStats.DataSource = dt;
                gvAttendanceStats.DataBind();
            }
        }

        private void LoadRiskOverview()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT TOP 10
                        s.matric_number,
                        s.full_name AS student_name,
                        p.programme_name,
                        CAST(
                            ISNULL(
                                SUM(cm.grade_point * c.credit_hours) /
                                NULLIF(SUM(c.credit_hours), 0),
                            0)
                        AS DECIMAL(4,2)) AS cgpa,
                        CAST(
                            ISNULL(
                                (
                                    SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) * 100.0
                                )
                                / NULLIF(COUNT(a.attendance_id), 0),
                            0)
                        AS DECIMAL(5,2)) AS attendance_percentage,
                        CASE
                            WHEN
                                ISNULL(
                                    SUM(cm.grade_point * c.credit_hours) /
                                    NULLIF(SUM(c.credit_hours), 0),
                                0) < 2.00
                                THEN 'High Risk'
                            WHEN
                                ISNULL(
                                    (
                                        SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) * 100.0
                                    )
                                    / NULLIF(COUNT(a.attendance_id), 0),
                                0) < 75
                                THEN 'Attendance Risk'
                            ELSE 'Monitor'
                        END AS risk_level
                    FROM STUDENT s
                    INNER JOIN PROGRAMME p
                        ON s.programme_id = p.programme_id
                    LEFT JOIN ENROLMENT e
                        ON s.student_id = e.student_id
                    LEFT JOIN COURSE c
                        ON e.course_id = c.course_id
                    LEFT JOIN COURSE_MARKS cm
                        ON e.enrolment_id = cm.enrolment_id
                    LEFT JOIN ATTENDANCE a
                        ON e.enrolment_id = a.enrolment_id
                    WHERE s.status = 'Active'
                    AND " + FilterCondition("e", "s") + @"
                    GROUP BY
                        s.matric_number,
                        s.full_name,
                        p.programme_name
                    HAVING
                        ISNULL(
                            SUM(cm.grade_point * c.credit_hours) /
                            NULLIF(SUM(c.credit_hours), 0),
                        0) < 2.00
                        OR
                        ISNULL(
                            (
                                SUM(CASE WHEN a.status = 'Present' THEN 1 ELSE 0 END) * 100.0
                            )
                            / NULLIF(COUNT(a.attendance_id), 0),
                        0) < 75
                    ORDER BY cgpa ASC, attendance_percentage ASC";

                SqlCommand cmd = new SqlCommand(q, con);
                AddFilterParameters(cmd);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvRiskOverview.DataSource = dt;
                gvRiskOverview.DataBind();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadAllAnalytics();
            lblMessage.Text = "Analytics filter applied.";
            lblMessage.CssClass = "text-success fw-bold";
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlAcademicYear.SelectedIndex = 0;
            ddlSemester.SelectedIndex = 0;
            ddlProgramme.SelectedIndex = 0;

            LoadAllAnalytics();

            lblMessage.Text = "";
        }

        private string FilterCondition(string enrolmentAlias, string studentAlias)
        {
            return @"
                (
                    @academic_year = 'All'
                    OR " + enrolmentAlias + @".academic_year = @academic_year
                    OR " + enrolmentAlias + @".academic_year IS NULL
                )
                AND
                (
                    @semester = 'All'
                    OR " + enrolmentAlias + @".semester = @semester
                    OR " + enrolmentAlias + @".semester IS NULL
                )
                AND
                (
                    @programme_id IS NULL
                    OR " + studentAlias + @".programme_id = @programme_id
                )";
        }

        private string StudentOnlyProgrammeFilter(string studentAlias)
        {
            return @"
                (
                    @programme_id IS NULL
                    OR " + studentAlias + @".programme_id = @programme_id
                )";
        }

        private void AddFilterParameters(SqlCommand cmd)
        {
            string academicYear = "All";
            string semester = "All";
            object programmeId = DBNull.Value;

            if (ddlAcademicYear != null)
                academicYear = ddlAcademicYear.SelectedValue;

            if (ddlSemester != null)
                semester = ddlSemester.SelectedValue;

            if (ddlProgramme != null && ddlProgramme.SelectedValue != "All")
                programmeId = Convert.ToInt32(ddlProgramme.SelectedValue);

            cmd.Parameters.AddWithValue("@academic_year", academicYear);
            cmd.Parameters.AddWithValue("@semester", semester);
            cmd.Parameters.AddWithValue("@programme_id", programmeId);
        }
    }
}