using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class Lecturer_CourseAssignment : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProgrammes();
                LoadLecturers();
                LoadCoursesByProgramme();
                LoadAssignments();
            }
        }

        private void LoadProgrammes()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT programme_id, programme_name
                    FROM PROGRAMME
                    ORDER BY programme_name";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlProgramme.DataSource = dt;
                ddlProgramme.DataTextField = "programme_name";
                ddlProgramme.DataValueField = "programme_id";
                ddlProgramme.DataBind();

                ddlProgramme.Items.Insert(0, new ListItem("-- Select Programme --", ""));
            }
        }

        private void LoadLecturers()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT 
                        lecturer_id,
                        ISNULL(staff_id, '') + ' - ' + full_name AS lecturer_display
                    FROM LECTURER
                    WHERE status = 'Active'
                    ORDER BY full_name";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlLecturer.DataSource = dt;
                ddlLecturer.DataTextField = "lecturer_display";
                ddlLecturer.DataValueField = "lecturer_id";
                ddlLecturer.DataBind();

                ddlLecturer.Items.Insert(0, new ListItem("-- Select Lecturer --", ""));
            }
        }

        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadCoursesByProgramme();
        }

        private void LoadCoursesByProgramme()
        {
            cblCourses.Items.Clear();

            if (string.IsNullOrEmpty(ddlProgramme.SelectedValue))
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT 
                        course_id,
                        course_code + ' - ' + course_name AS course_display
                    FROM COURSE
                    WHERE is_active = 1
                    AND programme_id = @programme_id
                    ORDER BY course_code";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@programme_id", ddlProgramme.SelectedValue);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                cblCourses.DataSource = dt;
                cblCourses.DataTextField = "course_display";
                cblCourses.DataValueField = "course_id";
                cblCourses.DataBind();
            }
        }

        private void LoadAssignments()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT
                        x.lecturer_id,
                        x.programme_id,
                        x.lecturer_name,
                        x.programme_name,
                        x.academic_year,
                        x.semester,
                        COUNT(*) AS course_count,

                        STUFF
                        (
                            (
                                SELECT ', ' + c2.course_code + ' - ' + c2.course_name
                                FROM LECTURER_COURSE lc2
                                INNER JOIN COURSE c2
                                    ON lc2.course_id = c2.course_id
                                WHERE lc2.lecturer_id = x.lecturer_id
                                AND c2.programme_id = x.programme_id
                                AND lc2.academic_year = x.academic_year
                                AND lc2.semester = x.semester
                                ORDER BY c2.course_code
                                FOR XML PATH(''), TYPE
                            ).value('.', 'NVARCHAR(MAX)'),
                            1,
                            2,
                            ''
                        ) AS assigned_courses

                    FROM
                    (
                        SELECT
                            lc.lecturer_id,
                            c.programme_id,
                            l.full_name AS lecturer_name,
                            p.programme_name,
                            lc.academic_year,
                            lc.semester,
                            lc.course_id
                        FROM LECTURER_COURSE lc
                        INNER JOIN LECTURER l
                            ON lc.lecturer_id = l.lecturer_id
                        INNER JOIN COURSE c
                            ON lc.course_id = c.course_id
                        INNER JOIN PROGRAMME p
                            ON c.programme_id = p.programme_id
                    ) x

                    GROUP BY
                        x.lecturer_id,
                        x.programme_id,
                        x.lecturer_name,
                        x.programme_name,
                        x.academic_year,
                        x.semester

                    ORDER BY
                        x.lecturer_name,
                        x.academic_year DESC,
                        x.semester";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvAssignments.DataSource = dt;
                gvAssignments.DataBind();
            }
        }

        protected void btnAssign_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            int insertedCount = 0;
            int skippedCount = 0;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                foreach (ListItem item in cblCourses.Items)
                {
                    if (item.Selected)
                    {
                        if (AssignmentExists(con, item.Value))
                        {
                            skippedCount++;
                        }
                        else
                        {
                            InsertAssignment(con, item.Value);
                            insertedCount++;
                        }
                    }
                }
            }

            lblMessage.Text =
                insertedCount + " course(s) assigned. " +
                skippedCount + " duplicate course(s) skipped.";

            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadAssignments();
        }

        protected void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            int removedCount = 0;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                foreach (ListItem item in cblCourses.Items)
                {
                    if (item.Selected)
                    {
                        string query = @"
                            DELETE FROM LECTURER_COURSE
                            WHERE lecturer_id = @lecturer_id
                            AND course_id = @course_id
                            AND academic_year = @academic_year
                            AND semester = @semester";

                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@lecturer_id", ddlLecturer.SelectedValue);
                        cmd.Parameters.AddWithValue("@course_id", item.Value);
                        cmd.Parameters.AddWithValue("@academic_year", txtAcademicYear.Text.Trim());
                        cmd.Parameters.AddWithValue("@semester", txtSemester.Text.Trim());

                        removedCount += cmd.ExecuteNonQuery();
                    }
                }
            }

            lblMessage.Text = removedCount + " course assignment(s) removed.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadAssignments();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(ddlProgramme.SelectedValue))
            {
                lblMessage.Text = "Please select a programme.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrEmpty(ddlLecturer.SelectedValue))
            {
                lblMessage.Text = "Please select a lecturer.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtAcademicYear.Text))
            {
                lblMessage.Text = "Please enter academic year.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSemester.Text))
            {
                lblMessage.Text = "Please enter semester.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            bool hasSelectedCourse = false;

            foreach (ListItem item in cblCourses.Items)
            {
                if (item.Selected)
                {
                    hasSelectedCourse = true;
                    break;
                }
            }

            if (!hasSelectedCourse)
            {
                lblMessage.Text = "Please select at least one course.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            return true;
        }

        private bool AssignmentExists(SqlConnection con, string courseId)
        {
            string query = @"
                SELECT COUNT(*)
                FROM LECTURER_COURSE
                WHERE lecturer_id = @lecturer_id
                AND course_id = @course_id
                AND academic_year = @academic_year
                AND semester = @semester";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@lecturer_id", ddlLecturer.SelectedValue);
            cmd.Parameters.AddWithValue("@course_id", courseId);
            cmd.Parameters.AddWithValue("@academic_year", txtAcademicYear.Text.Trim());
            cmd.Parameters.AddWithValue("@semester", txtSemester.Text.Trim());

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }

        private void InsertAssignment(SqlConnection con, string courseId)
        {
            string query = @"
                INSERT INTO LECTURER_COURSE
                (lecturer_id, course_id, academic_year, semester)
                VALUES
                (@lecturer_id, @course_id, @academic_year, @semester)";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@lecturer_id", ddlLecturer.SelectedValue);
            cmd.Parameters.AddWithValue("@course_id", courseId);
            cmd.Parameters.AddWithValue("@academic_year", txtAcademicYear.Text.Trim());
            cmd.Parameters.AddWithValue("@semester", txtSemester.Text.Trim());

            cmd.ExecuteNonQuery();
        }

        protected void gvAssignments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ManageGroup")
            {
                string[] parts = e.CommandArgument.ToString().Split('|');

                string lecturerId = parts[0];
                string programmeId = parts[1];
                string academicYear = parts[2];
                string semester = parts[3];

                ddlProgramme.SelectedValue = programmeId;
                ddlLecturer.SelectedValue = lecturerId;
                txtAcademicYear.Text = academicYear;
                txtSemester.Text = semester;

                LoadCoursesByProgramme();
                TickAssignedCourses(lecturerId, academicYear, semester);

                lblMessage.Text =
                    "Assignment group loaded. Tick courses to add or remove.";

                lblMessage.CssClass =
                    "text-primary fw-bold";
            }
        }

        private void TickAssignedCourses(
            string lecturerId,
            string academicYear,
            string semester
        )
        {
            foreach (ListItem item in cblCourses.Items)
            {
                item.Selected = false;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT course_id
                    FROM LECTURER_COURSE
                    WHERE lecturer_id = @lecturer_id
                    AND academic_year = @academic_year
                    AND semester = @semester";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@lecturer_id", lecturerId);
                cmd.Parameters.AddWithValue("@academic_year", academicYear);
                cmd.Parameters.AddWithValue("@semester", semester);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    ListItem item =
                        cblCourses.Items.FindByValue(
                            dr["course_id"].ToString()
                        );

                    if (item != null)
                    {
                        item.Selected = true;
                    }
                }
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            lblMessage.Text = "";
        }

        private void Clear()
        {
            hfLecturerID.Value = "";
            hfProgrammeID.Value = "";

            if (ddlProgramme.Items.Count > 0)
            {
                ddlProgramme.SelectedIndex = 0;
            }

            if (ddlLecturer.Items.Count > 0)
            {
                ddlLecturer.SelectedIndex = 0;
            }

            cblCourses.Items.Clear();

            txtAcademicYear.Text = "";
            txtSemester.Text = "";
        }
    }
}