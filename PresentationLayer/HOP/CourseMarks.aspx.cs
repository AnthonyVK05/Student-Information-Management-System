using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class CourseMarks : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEnrolments();
                LoadMarks();
            }
        }

        private void LoadEnrolments()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        e.enrolment_id,
                        s.matric_number + ' - ' +
                        s.full_name + ' - ' +
                        c.course_code + ' - ' +
                        c.course_name AS enrolment_name
                    FROM ENROLMENT e
                    INNER JOIN STUDENT s
                        ON e.student_id = s.student_id
                    INNER JOIN COURSE c
                        ON e.course_id = c.course_id
                    WHERE e.status = 'Approved'
                    AND s.status = 'Active'
                    ORDER BY s.full_name, c.course_code";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlEnrolment.DataSource = dt;
                ddlEnrolment.DataTextField = "enrolment_name";
                ddlEnrolment.DataValueField = "enrolment_id";
                ddlEnrolment.DataBind();

                ddlEnrolment.Items.Insert(0, new ListItem("-- Select Approved Enrolment --", ""));
            }
        }

        private void LoadMarks()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string search = txtSearch == null ? "" : txtSearch.Text.Trim();
                string gradeFilter = ddlGradeFilter == null ? "All" : ddlGradeFilter.SelectedValue;

                string q = @"
                    SELECT
                        cm.mark_id,
                        cm.enrolment_id,
                        s.matric_number,
                        s.nric_passport,
                        s.full_name AS student_name,
                        p.programme_name,
                        c.course_code,
                        c.course_name,
                        c.credit_hours,
                        e.academic_year,
                        e.semester,
                        cm.coursework_marks,
                        cm.final_exam_marks,
                        cm.total_marks,
                        cm.grade,
                        cm.grade_point
                    FROM COURSE_MARKS cm
                    INNER JOIN ENROLMENT e
                        ON cm.enrolment_id = e.enrolment_id
                    INNER JOIN STUDENT s
                        ON e.student_id = s.student_id
                    INNER JOIN PROGRAMME p
                        ON s.programme_id = p.programme_id
                    INNER JOIN COURSE c
                        ON e.course_id = c.course_id
                    WHERE
                    (
                        @search = ''
                        OR s.matric_number LIKE @searchLike
                        OR s.nric_passport LIKE @searchLike
                        OR s.full_name LIKE @searchLike
                        OR p.programme_name LIKE @searchLike
                        OR c.course_code LIKE @searchLike
                        OR c.course_name LIKE @searchLike
                    )
                    AND
                    (
                        @grade = 'All'
                        OR cm.grade = @grade
                    )
                    ORDER BY cm.mark_id DESC";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@searchLike", "%" + search + "%");
                cmd.Parameters.AddWithValue("@grade", gradeFilter);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvMarks.DataSource = dt;
                gvMarks.DataBind();
            }
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

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            if (MarksExist(ddlEnrolment.SelectedValue, ""))
            {
                lblMessage.Text = "Marks already exist for this enrolment.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            decimal coursework = Convert.ToDecimal(txtCoursework.Text.Trim());
            decimal finalExam = Convert.ToDecimal(txtFinal.Text.Trim());
            decimal total = coursework + finalExam;

            string grade = GetGrade(total);
            decimal gradePoint = GetGradePoint(total);

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    INSERT INTO COURSE_MARKS
                    (
                        enrolment_id,
                        coursework_marks,
                        final_exam_marks,
                        total_marks,
                        grade,
                        grade_point
                    )
                    VALUES
                    (
                        @enrolment_id,
                        @coursework_marks,
                        @final_exam_marks,
                        @total_marks,
                        @grade,
                        @grade_point
                    )";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@enrolment_id", ddlEnrolment.SelectedValue);
                cmd.Parameters.AddWithValue("@coursework_marks", coursework);
                cmd.Parameters.AddWithValue("@final_exam_marks", finalExam);
                cmd.Parameters.AddWithValue("@total_marks", total);
                cmd.Parameters.AddWithValue("@grade", grade);
                cmd.Parameters.AddWithValue("@grade_point", gradePoint);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Marks added successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            ShowCalculatedResult(total, grade, gradePoint);
            Clear();
            LoadMarks();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfMarkID.Value))
            {
                lblMessage.Text = "Please select a marks record first.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            if (!ValidateForm())
            {
                return;
            }

            if (MarksExist(ddlEnrolment.SelectedValue, hfMarkID.Value))
            {
                lblMessage.Text = "Another marks record already exists for this enrolment.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            decimal coursework = Convert.ToDecimal(txtCoursework.Text.Trim());
            decimal finalExam = Convert.ToDecimal(txtFinal.Text.Trim());
            decimal total = coursework + finalExam;

            string grade = GetGrade(total);
            decimal gradePoint = GetGradePoint(total);

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    UPDATE COURSE_MARKS
                    SET
                        enrolment_id = @enrolment_id,
                        coursework_marks = @coursework_marks,
                        final_exam_marks = @final_exam_marks,
                        total_marks = @total_marks,
                        grade = @grade,
                        grade_point = @grade_point
                    WHERE mark_id = @mark_id";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@enrolment_id", ddlEnrolment.SelectedValue);
                cmd.Parameters.AddWithValue("@coursework_marks", coursework);
                cmd.Parameters.AddWithValue("@final_exam_marks", finalExam);
                cmd.Parameters.AddWithValue("@total_marks", total);
                cmd.Parameters.AddWithValue("@grade", grade);
                cmd.Parameters.AddWithValue("@grade_point", gradePoint);
                cmd.Parameters.AddWithValue("@mark_id", hfMarkID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Marks updated successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            ShowCalculatedResult(total, grade, gradePoint);
            Clear();
            LoadMarks();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfMarkID.Value))
            {
                lblMessage.Text = "Please select a marks record first.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = "DELETE FROM COURSE_MARKS WHERE mark_id = @mark_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@mark_id", hfMarkID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Marks deleted successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadMarks();
        }

        protected void gvMarks_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvMarks.SelectedRow;

            hfMarkID.Value =
                gvMarks.DataKeys[row.RowIndex].Value.ToString();

            LoadSelectedMark();

            lblMessage.Text = "Marks record selected. You can update or delete it.";
            lblMessage.CssClass = "text-primary fw-bold";
        }

        private void LoadSelectedMark()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        enrolment_id,
                        coursework_marks,
                        final_exam_marks,
                        total_marks,
                        grade,
                        grade_point
                    FROM COURSE_MARKS
                    WHERE mark_id = @mark_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@mark_id", hfMarkID.Value);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    string enrolmentId = dr["enrolment_id"].ToString();

                    if (ddlEnrolment.Items.FindByValue(enrolmentId) != null)
                    {
                        ddlEnrolment.SelectedValue = enrolmentId;
                    }

                    txtCoursework.Text =
                        Convert.ToDecimal(dr["coursework_marks"]).ToString("0.00");

                    txtFinal.Text =
                        Convert.ToDecimal(dr["final_exam_marks"]).ToString("0.00");

                    txtTotal.Text =
                        Convert.ToDecimal(dr["total_marks"]).ToString("0.00");

                    txtGrade.Text = dr["grade"].ToString();

                    txtGradePoint.Text =
                        Convert.ToDecimal(dr["grade_point"]).ToString("0.00");
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadMarks();
        }

        protected void btnResetSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";

            if (ddlGradeFilter.Items.Count > 0)
            {
                ddlGradeFilter.SelectedIndex = 0;
            }

            LoadMarks();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            lblMessage.Text = "";
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(ddlEnrolment.SelectedValue))
            {
                lblMessage.Text = "Please select approved enrolment.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCoursework.Text))
            {
                lblMessage.Text = "Please enter coursework marks.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtFinal.Text))
            {
                lblMessage.Text = "Please enter final exam marks.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            decimal coursework;
            decimal finalExam;

            if (!Decimal.TryParse(txtCoursework.Text.Trim(), out coursework))
            {
                lblMessage.Text = "Coursework marks must be a valid number.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (!Decimal.TryParse(txtFinal.Text.Trim(), out finalExam))
            {
                lblMessage.Text = "Final exam marks must be a valid number.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (coursework < 0 || coursework > 40)
            {
                lblMessage.Text = "Coursework marks must be between 0 and 40.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (finalExam < 0 || finalExam > 60)
            {
                lblMessage.Text = "Final exam marks must be between 0 and 60.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            return true;
        }

        private bool MarksExist(string enrolmentId, string currentMarkId)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT COUNT(*)
                    FROM COURSE_MARKS
                    WHERE enrolment_id = @enrolment_id
                    AND (@mark_id = '' OR mark_id <> @mark_id)";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@enrolment_id", enrolmentId);
                cmd.Parameters.AddWithValue("@mark_id", currentMarkId ?? "");

                con.Open();

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count > 0;
            }
        }

        private string GetGrade(decimal total)
        {
            if (total >= 80) return "A";
            if (total >= 75) return "A-";
            if (total >= 70) return "B+";
            if (total >= 65) return "B";
            if (total >= 60) return "B-";
            if (total >= 55) return "C+";
            if (total >= 50) return "C";
            if (total >= 45) return "D";
            return "F";
        }

        private decimal GetGradePoint(decimal total)
        {
            if (total >= 80) return 4.00m;
            if (total >= 75) return 3.67m;
            if (total >= 70) return 3.33m;
            if (total >= 65) return 3.00m;
            if (total >= 60) return 2.67m;
            if (total >= 55) return 2.33m;
            if (total >= 50) return 2.00m;
            if (total >= 45) return 1.00m;
            return 0.00m;
        }

        private void ShowCalculatedResult(
            decimal total,
            string grade,
            decimal gradePoint
        )
        {
            txtTotal.Text = total.ToString("0.00");
            txtGrade.Text = grade;
            txtGradePoint.Text = gradePoint.ToString("0.00");
        }

        private void Clear()
        {
            hfMarkID.Value = "";

            if (ddlEnrolment.Items.Count > 0)
            {
                ddlEnrolment.SelectedIndex = 0;
            }

            txtCoursework.Text = "";
            txtFinal.Text = "";
            txtTotal.Text = "";
            txtGrade.Text = "";
            txtGradePoint.Text = "";
        }
    }
}