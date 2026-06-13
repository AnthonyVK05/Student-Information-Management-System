using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class AcademicCalendar : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCalendar();
            }
        }

        private void LoadCalendar()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        calendar_id,
                        academic_year,
                        semester,
                        start_date,
                        end_date,
                        status
                    FROM ACADEMIC_CALENDAR
                    ORDER BY calendar_id DESC";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvCalendar.DataSource = dt;
                gvCalendar.DataBind();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            if (CalendarExists("", ddlAcademicYear.SelectedValue, ddlSemester.SelectedValue))
            {
                lblMessage.Text = "This academic year and semester already exists.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    INSERT INTO ACADEMIC_CALENDAR
                    (
                        academic_year,
                        semester,
                        start_date,
                        end_date,
                        status
                    )
                    VALUES
                    (
                        @academic_year,
                        @semester,
                        @start_date,
                        @end_date,
                        @status
                    )";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@academic_year", ddlAcademicYear.SelectedValue);
                cmd.Parameters.AddWithValue("@semester", ddlSemester.SelectedValue);
                cmd.Parameters.AddWithValue("@start_date", txtStartDate.Text.Trim());
                cmd.Parameters.AddWithValue("@end_date", txtEndDate.Text.Trim());
                cmd.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Academic calendar created successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadCalendar();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfCalendarID.Value))
            {
                lblMessage.Text = "Please select a calendar record first.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            if (!ValidateForm())
                return;

            if (CalendarExists(hfCalendarID.Value, ddlAcademicYear.SelectedValue, ddlSemester.SelectedValue))
            {
                lblMessage.Text = "Another calendar record already exists for this academic year and semester.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    UPDATE ACADEMIC_CALENDAR
                    SET
                        academic_year = @academic_year,
                        semester = @semester,
                        start_date = @start_date,
                        end_date = @end_date,
                        status = @status
                    WHERE calendar_id = @calendar_id";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@academic_year", ddlAcademicYear.SelectedValue);
                cmd.Parameters.AddWithValue("@semester", ddlSemester.SelectedValue);
                cmd.Parameters.AddWithValue("@start_date", txtStartDate.Text.Trim());
                cmd.Parameters.AddWithValue("@end_date", txtEndDate.Text.Trim());
                cmd.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@calendar_id", hfCalendarID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Academic calendar updated successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadCalendar();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfCalendarID.Value))
            {
                lblMessage.Text = "Please select a calendar record first.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    DELETE FROM ACADEMIC_CALENDAR
                    WHERE calendar_id = @calendar_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@calendar_id", hfCalendarID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Academic calendar deleted successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadCalendar();
        }

        protected void gvCalendar_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvCalendar.SelectedRow;

            hfCalendarID.Value =
                gvCalendar.DataKeys[row.RowIndex].Value.ToString();

            string academicYear = row.Cells[2].Text;
            string semester = row.Cells[3].Text;
            string status = row.Cells[6].Text;

            if (ddlAcademicYear.Items.FindByValue(academicYear) != null)
                ddlAcademicYear.SelectedValue = academicYear;

            if (ddlSemester.Items.FindByValue(semester) != null)
                ddlSemester.SelectedValue = semester;

            txtStartDate.Text =
                Convert.ToDateTime(row.Cells[4].Text).ToString("yyyy-MM-dd");

            txtEndDate.Text =
                Convert.ToDateTime(row.Cells[5].Text).ToString("yyyy-MM-dd");

            if (ddlStatus.Items.FindByValue(status) != null)
                ddlStatus.SelectedValue = status;

            lblMessage.Text = "Record selected. You can update or delete it.";
            lblMessage.CssClass = "text-primary fw-bold";
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            lblMessage.Text = "";
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(ddlAcademicYear.SelectedValue))
            {
                lblMessage.Text = "Please select academic year.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrEmpty(ddlSemester.SelectedValue))
            {
                lblMessage.Text = "Please select semester.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtStartDate.Text))
            {
                lblMessage.Text = "Please select start date.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEndDate.Text))
            {
                lblMessage.Text = "Please select end date.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            DateTime startDate = Convert.ToDateTime(txtStartDate.Text);
            DateTime endDate = Convert.ToDateTime(txtEndDate.Text);

            if (endDate < startDate)
            {
                lblMessage.Text = "End date cannot be earlier than start date.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            return true;
        }

        private bool CalendarExists(
            string currentCalendarId,
            string academicYear,
            string semester
        )
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT COUNT(*)
                    FROM ACADEMIC_CALENDAR
                    WHERE academic_year = @academic_year
                    AND semester = @semester
                    AND (@calendar_id = '' OR calendar_id <> @calendar_id)";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@academic_year", academicYear);
                cmd.Parameters.AddWithValue("@semester", semester);
                cmd.Parameters.AddWithValue("@calendar_id", currentCalendarId ?? "");

                con.Open();

                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count > 0;
            }
        }

        private void Clear()
        {
            hfCalendarID.Value = "";

            if (ddlAcademicYear.Items.Count > 0)
                ddlAcademicYear.SelectedIndex = 0;

            if (ddlSemester.Items.Count > 0)
                ddlSemester.SelectedIndex = 0;

            txtStartDate.Text = "";
            txtEndDate.Text = "";

            if (ddlStatus.Items.Count > 0)
                ddlStatus.SelectedIndex = 0;
        }
    }
}