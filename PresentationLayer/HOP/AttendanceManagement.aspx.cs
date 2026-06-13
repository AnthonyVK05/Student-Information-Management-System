using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class AttendanceManagement : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadEnrolments();
                LoadLecturers();
                LoadAttendance();

                txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void LoadEnrolments()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        e.enrolment_id,
                        s.matric_number + ' - ' + s.full_name + ' - ' + c.course_code AS enrolment_name
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

                ddlEnrolment.Items.Insert(0, new ListItem("-- Select Enrolment --", ""));
            }
        }

        private void LoadLecturers()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        lecturer_id,
                        staff_id + ' - ' + full_name AS lecturer_name
                    FROM LECTURER
                    WHERE status = 'Active'
                    ORDER BY full_name";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlLecturer.DataSource = dt;
                ddlLecturer.DataTextField = "lecturer_name";
                ddlLecturer.DataValueField = "lecturer_id";
                ddlLecturer.DataBind();

                ddlLecturer.Items.Insert(0, new ListItem("-- Select Lecturer --", ""));
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

            if (AttendanceExists(
                ddlEnrolment.SelectedValue,
                ddlLecturer.SelectedValue,
                txtDate.Text.Trim(),
                ""))
            {
                lblMessage.Text = "Attendance already recorded for this enrolment, lecturer and date.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    INSERT INTO ATTENDANCE
                    (
                        enrolment_id,
                        lecturer_id,
                        class_date,
                        status,
                        remarks
                    )
                    VALUES
                    (
                        @enrolment_id,
                        @lecturer_id,
                        @class_date,
                        @status,
                        @remarks
                    )";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@enrolment_id", ddlEnrolment.SelectedValue);
                cmd.Parameters.AddWithValue("@lecturer_id", ddlLecturer.SelectedValue);
                cmd.Parameters.AddWithValue("@class_date", GetDateOrDBNull(txtDate.Text.Trim()));
                cmd.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text.Trim());

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Attendance added successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadAttendance();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfAttendanceID.Value))
            {
                lblMessage.Text = "Please select an attendance record first.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            if (!ValidateForm())
            {
                return;
            }

            if (AttendanceExists(
                ddlEnrolment.SelectedValue,
                ddlLecturer.SelectedValue,
                txtDate.Text.Trim(),
                hfAttendanceID.Value))
            {
                lblMessage.Text = "Another attendance record already exists for this enrolment, lecturer and date.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    UPDATE ATTENDANCE
                    SET
                        enrolment_id = @enrolment_id,
                        lecturer_id = @lecturer_id,
                        class_date = @class_date,
                        status = @status,
                        remarks = @remarks
                    WHERE attendance_id = @attendance_id";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@enrolment_id", ddlEnrolment.SelectedValue);
                cmd.Parameters.AddWithValue("@lecturer_id", ddlLecturer.SelectedValue);
                cmd.Parameters.AddWithValue("@class_date", GetDateOrDBNull(txtDate.Text.Trim()));
                cmd.Parameters.AddWithValue("@status", ddlStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@remarks", txtRemarks.Text.Trim());
                cmd.Parameters.AddWithValue("@attendance_id", hfAttendanceID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Attendance updated successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadAttendance();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfAttendanceID.Value))
            {
                lblMessage.Text = "Please select an attendance record first.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = "DELETE FROM ATTENDANCE WHERE attendance_id = @attendance_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@attendance_id", hfAttendanceID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Attendance deleted successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadAttendance();
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

                if (dr.Read())
                {
                    string enrolmentId = dr["enrolment_id"].ToString();
                    string lecturerId = dr["lecturer_id"].ToString();

                    if (ddlEnrolment.Items.FindByValue(enrolmentId) != null)
                    {
                        ddlEnrolment.SelectedValue = enrolmentId;
                    }

                    if (ddlLecturer.Items.FindByValue(lecturerId) != null)
                    {
                        ddlLecturer.SelectedValue = lecturerId;
                    }

                    if (dr["class_date"] != DBNull.Value)
                    {
                        txtDate.Text =
                            Convert.ToDateTime(dr["class_date"]).ToString("yyyy-MM-dd");
                    }

                    if (ddlStatus.Items.FindByValue(dr["status"].ToString()) != null)
                    {
                        ddlStatus.SelectedValue = dr["status"].ToString();
                    }

                    txtRemarks.Text = dr["remarks"].ToString();
                }
            }

            lblMessage.Text = "Attendance selected. You can update or delete it.";
            lblMessage.CssClass = "text-primary fw-bold";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadAttendance();
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
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            lblMessage.Text = "";
        }

        private bool ValidateForm()
        {
            if (ddlEnrolment.SelectedValue == "")
            {
                lblMessage.Text = "Please select enrolment.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (ddlLecturer.SelectedValue == "")
            {
                lblMessage.Text = "Please select lecturer.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDate.Text))
            {
                lblMessage.Text = "Please select class date.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            return true;
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

        private void Clear()
        {
            hfAttendanceID.Value = "";

            if (ddlEnrolment.Items.Count > 0)
            {
                ddlEnrolment.SelectedIndex = 0;
            }

            if (ddlLecturer.Items.Count > 0)
            {
                ddlLecturer.SelectedIndex = 0;
            }

            txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            if (ddlStatus.Items.Count > 0)
            {
                ddlStatus.SelectedIndex = 0;
            }

            txtRemarks.Text = "";
        }
    }
}