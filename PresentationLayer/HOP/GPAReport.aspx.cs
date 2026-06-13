using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class GPAReport : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadStudents();

                lblGeneratedDate.Text =
                    DateTime.Now.ToString("dd MMMM yyyy");

                string studentId =
                    Request.QueryString["studentid"];

                if (!string.IsNullOrEmpty(studentId))
                {
                    ListItem item =
                        ddlStudent.Items.FindByValue(studentId);

                    if (item != null)
                    {
                        ddlStudent.SelectedValue = studentId;

                        LoadStudentInfo();
                        GenerateTranscript();

                        lblMessage.Text =
                            "Transcript loaded successfully.";

                        lblMessage.CssClass =
                            "text-success fw-bold";
                    }
                    else
                    {
                        lblMessage.Text =
                            "Selected student was not found.";

                        lblMessage.CssClass =
                            "text-danger fw-bold";
                    }
                }
            }
        }

        private void LoadStudents()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT
                        student_id,
                        matric_number + ' - ' + full_name AS student_display
                    FROM STUDENT
                    ORDER BY full_name";

                SqlDataAdapter da =
                    new SqlDataAdapter(query, con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlStudent.DataSource = dt;
                ddlStudent.DataTextField = "student_display";
                ddlStudent.DataValueField = "student_id";
                ddlStudent.DataBind();

                ddlStudent.Items.Insert(
                    0,
                    new ListItem("-- Select Student --", "")
                );
            }
        }

        protected void btnGenerate_Click(
            object sender,
            EventArgs e
        )
        {
            if (string.IsNullOrEmpty(ddlStudent.SelectedValue))
            {
                lblMessage.Text =
                    "Please select a student.";

                lblMessage.CssClass =
                    "text-danger fw-bold";

                return;
            }

            LoadStudentInfo();
            GenerateTranscript();

            lblMessage.Text =
                "Transcript generated successfully.";

            lblMessage.CssClass =
                "text-success fw-bold";
        }

        private void LoadStudentInfo()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT
                        s.full_name,
                        s.matric_number,
                        s.status,
                        p.programme_name,
                        p.department
                    FROM STUDENT s
                    INNER JOIN PROGRAMME p
                        ON s.programme_id = p.programme_id
                    WHERE s.student_id = @student_id";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@student_id",
                    ddlStudent.SelectedValue
                );

                con.Open();

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblStudentName.Text =
                        dr["full_name"].ToString();

                    lblMatric.Text =
                        dr["matric_number"].ToString();

                    lblProgramme.Text =
                        dr["programme_name"].ToString();

                    lblDepartment.Text =
                        dr["department"].ToString();

                    lblStatus.Text =
                        dr["status"].ToString();

                    lblGeneratedDate.Text =
                        DateTime.Now.ToString("dd MMMM yyyy");
                }
            }
        }

        private void GenerateTranscript()
        {
            DataTable dt =
                GetTranscriptData();

            if (dt.Rows.Count == 0)
            {
                phTranscript.Controls.Clear();

                phTranscript.Controls.Add(
                    new Literal
                    {
                        Text =
                            "<div class='alert alert-warning'>No course marks found for this student.</div>"
                    }
                );

                lblCGPA.Text = "0.00";
                lblFinalCGPA.Text = "0.00";
                lblCreditsEarned.Text = "0";
                lblStanding.Text = "No Results";

                return;
            }

            StringBuilder html =
                new StringBuilder();

            decimal totalGradePoints = 0;
            int totalCredits = 0;

            string currentYear = "";
            string currentSemester = "";

            decimal semesterGradePoints = 0;
            int semesterCredits = 0;

            foreach (DataRow row in dt.Rows)
            {
                string academicYear =
                    row["academic_year"].ToString();

                string semester =
                    row["semester"].ToString();

                if (
                    currentYear != academicYear ||
                    currentSemester != semester
                )
                {
                    if (currentYear != "")
                    {
                        decimal semesterGPA =
                            semesterCredits == 0
                            ? 0
                            : semesterGradePoints / semesterCredits;

                        html.Append("</tbody></table>");

                        html.Append(
                            "<div class='text-end fw-bold mb-3'>Semester GPA: "
                            + semesterGPA.ToString("0.00")
                            + "</div>"
                        );

                        semesterGradePoints = 0;
                        semesterCredits = 0;
                    }

                    currentYear = academicYear;
                    currentSemester = semester;

                    html.Append("<div class='semester-title'>");
                    html.Append(
                        Server.HtmlEncode(
                            currentYear + " - " + currentSemester
                        )
                    );
                    html.Append("</div>");

                    html.Append(
                        "<table class='table table-bordered table-sm'>"
                    );

                    html.Append("<thead>");
                    html.Append("<tr>");
                    html.Append("<th>Course Code</th>");
                    html.Append("<th>Course Name</th>");
                    html.Append("<th>Credit Hours</th>");
                    html.Append("<th>Total Marks</th>");
                    html.Append("<th>Grade</th>");
                    html.Append("<th>Grade Point</th>");
                    html.Append("</tr>");
                    html.Append("</thead>");
                    html.Append("<tbody>");
                }

                int creditHours =
                    Convert.ToInt32(row["credit_hours"]);

                decimal gradePoint =
                    Convert.ToDecimal(row["grade_point"]);

                decimal weightedPoint =
                    creditHours * gradePoint;

                totalCredits += creditHours;
                totalGradePoints += weightedPoint;

                semesterCredits += creditHours;
                semesterGradePoints += weightedPoint;

                html.Append("<tr>");

                html.Append(
                    "<td>" +
                    Server.HtmlEncode(row["course_code"].ToString()) +
                    "</td>"
                );

                html.Append(
                    "<td>" +
                    Server.HtmlEncode(row["course_name"].ToString()) +
                    "</td>"
                );

                html.Append(
                    "<td>" +
                    creditHours +
                    "</td>"
                );

                html.Append(
                    "<td>" +
                    row["total_marks"].ToString() +
                    "</td>"
                );

                html.Append(
                    "<td>" +
                    Server.HtmlEncode(row["grade"].ToString()) +
                    "</td>"
                );

                html.Append(
                    "<td>" +
                    gradePoint.ToString("0.00") +
                    "</td>"
                );

                html.Append("</tr>");
            }

            if (currentYear != "")
            {
                decimal semesterGPA =
                    semesterCredits == 0
                    ? 0
                    : semesterGradePoints / semesterCredits;

                html.Append("</tbody></table>");

                html.Append(
                    "<div class='text-end fw-bold mb-3'>Semester GPA: "
                    + semesterGPA.ToString("0.00")
                    + "</div>"
                );
            }

            decimal cgpa =
                totalCredits == 0
                ? 0
                : totalGradePoints / totalCredits;

            phTranscript.Controls.Clear();

            phTranscript.Controls.Add(
                new Literal
                {
                    Text = html.ToString()
                }
            );

            lblCreditsEarned.Text =
                totalCredits.ToString();

            lblCGPA.Text =
                cgpa.ToString("0.00");

            lblFinalCGPA.Text =
                cgpa.ToString("0.00");

            lblStanding.Text =
                GetAcademicStanding(cgpa);

            UpdateStudentCGPA(cgpa);
        }

        private DataTable GetTranscriptData()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT
                        e.academic_year,
                        e.semester,
                        c.course_code,
                        c.course_name,
                        c.credit_hours,
                        cm.total_marks,
                        cm.grade,
                        cm.grade_point
                    FROM COURSE_MARKS cm
                    INNER JOIN ENROLMENT e
                        ON cm.enrolment_id = e.enrolment_id
                    INNER JOIN COURSE c
                        ON e.course_id = c.course_id
                    WHERE e.student_id = @student_id
                    ORDER BY
                        e.academic_year,
                        e.semester,
                        c.course_code";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@student_id",
                    ddlStudent.SelectedValue
                );

                SqlDataAdapter da =
                    new SqlDataAdapter(cmd);

                DataTable dt =
                    new DataTable();

                da.Fill(dt);

                return dt;
            }
        }

        private string GetAcademicStanding(decimal cgpa)
        {
            if (cgpa >= 3.75m)
            {
                return "Dean's List";
            }

            if (cgpa >= 3.00m)
            {
                return "Good Standing";
            }

            if (cgpa >= 2.00m)
            {
                return "Academic Warning";
            }

            return "At Risk";
        }

        private void UpdateStudentCGPA(decimal cgpa)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    UPDATE STUDENT
                    SET cgpa = @cgpa
                    WHERE student_id = @student_id";

                SqlCommand cmd =
                    new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                    "@cgpa",
                    cgpa
                );

                cmd.Parameters.AddWithValue(
                    "@student_id",
                    ddlStudent.SelectedValue
                );

                con.Open();

                cmd.ExecuteNonQuery();
            }
        }
    }
}