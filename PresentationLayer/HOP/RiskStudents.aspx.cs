using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class RiskStudents : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProgrammes();
                LoadSummaryCards();
                LoadRiskStudents();
            }
        }

        private void LoadProgrammes()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT 
                        programme_id, 
                        programme_name
                    FROM PROGRAMME
                    ORDER BY programme_name";

                SqlDataAdapter da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlProgramme.DataSource = dt;
                ddlProgramme.DataTextField = "programme_name";
                ddlProgramme.DataValueField = "programme_id";
                ddlProgramme.DataBind();

                ddlProgramme.Items.Insert(
                    0,
                    new ListItem("All Programmes", "All")
                );
            }
        }

        private void LoadSummaryCards()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                lblTotalStudents.Text = GetValue(
                    con,
                    "SELECT COUNT(*) FROM STUDENT WHERE status = 'Active'"
                );

                lblHighRisk.Text = GetValue(
                    con,
                    @"SELECT COUNT(*) 
                      FROM STUDENT 
                      WHERE cgpa < 2.00 
                      AND status = 'Active'"
                );

                lblWarning.Text = GetValue(
                    con,
                    @"SELECT COUNT(*) 
                      FROM STUDENT 
                      WHERE cgpa >= 2.00 
                      AND cgpa < 3.00 
                      AND status = 'Active'"
                );

                lblDeansList.Text = GetValue(
                    con,
                    @"SELECT COUNT(*) 
                      FROM STUDENT 
                      WHERE cgpa >= 3.75 
                      AND status = 'Active'"
                );
            }
        }

        private string GetValue(SqlConnection con, string query)
        {
            SqlCommand cmd = new SqlCommand(query, con);
            object result = cmd.ExecuteScalar();

            if (result == null || result == DBNull.Value)
            {
                return "0";
            }

            return result.ToString();
        }

        private void LoadRiskStudents()
        {
            DataTable dt = GetRiskStudentData();

            gvRiskStudents.DataSource = dt;
            gvRiskStudents.DataBind();
        }

        private DataTable GetRiskStudentData()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"
                    SELECT
                        s.student_id,
                        s.matric_number,
                        s.full_name,
                        p.programme_name,
                        ISNULL(s.cgpa, 0) AS cgpa,

                        ISNULL
                        (
                            (
                                SELECT TOP 1 e2.semester
                                FROM ENROLMENT e2
                                WHERE e2.student_id = s.student_id
                                ORDER BY 
                                    e2.academic_year DESC, 
                                    e2.semester DESC
                            ),
                            '-'
                        ) AS current_semester,

                        CAST
                        (
                            ISNULL
                            (
                                (
                                    SELECT
                                        (
                                            SUM
                                            (
                                                CASE 
                                                    WHEN a.status = 'Present' 
                                                    THEN 1 
                                                    ELSE 0 
                                                END
                                            ) * 100.0
                                        )
                                        / NULLIF(COUNT(*), 0)
                                    FROM ATTENDANCE a
                                    INNER JOIN ENROLMENT e3
                                        ON a.enrolment_id = e3.enrolment_id
                                    WHERE e3.student_id = s.student_id
                                ),
                                0
                            ) AS DECIMAL(5,2)
                        ) AS attendance_percentage,

                        CASE
                            WHEN ISNULL(s.cgpa, 0) >= 3.75 
                                THEN 'Dean''s List'

                            WHEN ISNULL(s.cgpa, 0) >= 3.00 
                                THEN 'Good Standing'

                            WHEN ISNULL(s.cgpa, 0) >= 2.00 
                                THEN 'Academic Warning'

                            ELSE 'High Risk'
                        END AS risk_level,

                        CASE
                            WHEN ISNULL(s.cgpa, 0) < 2.00
                                 AND
                                 ISNULL
                                 (
                                    (
                                        SELECT
                                            (
                                                SUM
                                                (
                                                    CASE 
                                                        WHEN a.status = 'Present' 
                                                        THEN 1 
                                                        ELSE 0 
                                                    END
                                                ) * 100.0
                                            )
                                            / NULLIF(COUNT(*), 0)
                                        FROM ATTENDANCE a
                                        INNER JOIN ENROLMENT e4
                                            ON a.enrolment_id = e4.enrolment_id
                                        WHERE e4.student_id = s.student_id
                                    ),
                                    0
                                 ) < 75
                                THEN 'Immediate Intervention'

                            WHEN ISNULL(s.cgpa, 0) < 2.00
                                THEN 'Academic Counselling'

                            WHEN ISNULL(s.cgpa, 0) >= 2.00
                                 AND ISNULL(s.cgpa, 0) < 3.00
                                THEN 'Monitor Progress'

                            WHEN ISNULL(s.cgpa, 0) >= 3.75
                                THEN 'Recognition Candidate'

                            ELSE 'Continue Monitoring'
                        END AS recommended_action

                    FROM STUDENT s
                    INNER JOIN PROGRAMME p
                        ON s.programme_id = p.programme_id
                    WHERE s.status = 'Active'";

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    query += @"
                        AND
                        (
                            s.full_name LIKE @search
                            OR s.matric_number LIKE @search
                        )";
                }

                if (ddlProgramme.SelectedValue != "All")
                {
                    query += @"
                        AND s.programme_id = @programme_id";
                }

                if (ddlRiskLevel.SelectedValue != "All")
                {
                    query += @"
                        AND
                        (
                            CASE
                                WHEN ISNULL(s.cgpa, 0) >= 3.75 
                                    THEN 'Dean''s List'

                                WHEN ISNULL(s.cgpa, 0) >= 3.00 
                                    THEN 'Good Standing'

                                WHEN ISNULL(s.cgpa, 0) >= 2.00 
                                    THEN 'Academic Warning'

                                ELSE 'High Risk'
                            END
                        ) = @risk_level";
                }

                query += @"
                    ORDER BY 
                        s.cgpa ASC, 
                        s.full_name ASC";

                SqlCommand cmd = new SqlCommand(query, con);

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    cmd.Parameters.AddWithValue(
                        "@search",
                        "%" + txtSearch.Text.Trim() + "%"
                    );
                }

                if (ddlProgramme.SelectedValue != "All")
                {
                    cmd.Parameters.AddWithValue(
                        "@programme_id",
                        ddlProgramme.SelectedValue
                    );
                }

                if (ddlRiskLevel.SelectedValue != "All")
                {
                    cmd.Parameters.AddWithValue(
                        "@risk_level",
                        ddlRiskLevel.SelectedValue
                    );
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                return dt;
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadRiskStudents();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";

            if (ddlProgramme.Items.Count > 0)
            {
                ddlProgramme.SelectedIndex = 0;
            }

            if (ddlRiskLevel.Items.Count > 0)
            {
                ddlRiskLevel.SelectedIndex = 0;
            }

            LoadRiskStudents();
        }

        protected void gvRiskStudents_RowCommand(
            object sender,
            GridViewCommandEventArgs e
        )
        {
            if (e.CommandName == "ViewTranscript")
            {
                string studentId = e.CommandArgument.ToString();

                Response.Redirect(
                    "GPAReport.aspx?studentid=" + studentId
                );
            }
        }
    }
}