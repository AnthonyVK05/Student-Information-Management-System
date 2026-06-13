using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class Announcement_Admin_ : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCourses();
                LoadSearchCourses();
                LoadAnnouncements();
            }
        }

        private void LoadCourses()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        course_id,
                        course_code + ' - ' + course_name AS course_display
                    FROM COURSE
                    WHERE is_active = 1
                    ORDER BY course_code";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlCourse.DataSource = dt;
                ddlCourse.DataTextField = "course_display";
                ddlCourse.DataValueField = "course_id";
                ddlCourse.DataBind();

                ddlCourse.Items.Insert(0, new ListItem("General Announcement", ""));
            }
        }

        private void LoadSearchCourses()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        course_id,
                        course_code + ' - ' + course_name AS course_display
                    FROM COURSE
                    WHERE is_active = 1
                    ORDER BY course_code";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlSearchCourse.DataSource = dt;
                ddlSearchCourse.DataTextField = "course_display";
                ddlSearchCourse.DataValueField = "course_id";
                ddlSearchCourse.DataBind();

                ddlSearchCourse.Items.Insert(0, new ListItem("All Announcements", "All"));
                ddlSearchCourse.Items.Insert(1, new ListItem("General Only", "General"));
            }
        }

        private void LoadAnnouncements()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string search = txtSearch == null ? "" : txtSearch.Text.Trim();
                string courseFilter = ddlSearchCourse == null ? "All" : ddlSearchCourse.SelectedValue;

                string q = @"
                    SELECT
                        a.announcement_id,
                        a.title,
                        a.content,
                        a.course_id,
                        ISNULL(c.course_code, 'GENERAL') AS course_code,
                        ISNULL(c.course_name, 'General Announcement') AS course_name,
                        ISNULL(u.username, 'HOP Admin') AS posted_by_name,
                        a.posted_at
                    FROM ANNOUNCEMENT a
                    LEFT JOIN COURSE c
                        ON a.course_id = c.course_id
                    INNER JOIN [USER] u
                        ON a.posted_by = u.user_id
                    WHERE
                    (
                        @search = ''
                        OR a.title LIKE @searchLike
                        OR a.content LIKE @searchLike
                        OR c.course_code LIKE @searchLike
                        OR c.course_name LIKE @searchLike
                    )
                    AND
                    (
                        @course_filter = 'All'
                        OR (@course_filter = 'General' AND a.course_id IS NULL)
                        OR (
                            @course_filter NOT IN ('All', 'General')
                            AND a.course_id = @course_filter_id
                        )
                    )
                    ORDER BY a.posted_at DESC";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@searchLike", "%" + search + "%");
                cmd.Parameters.AddWithValue("@course_filter", courseFilter);

                if (courseFilter == "All" || courseFilter == "General")
                {
                    cmd.Parameters.AddWithValue("@course_filter_id", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@course_filter_id", Convert.ToInt32(courseFilter));
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvAnnouncements.DataSource = dt;
                gvAnnouncements.DataBind();
            }
        }

        protected void btnPost_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            int postedBy = GetCurrentUserId();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    INSERT INTO ANNOUNCEMENT
                    (
                        posted_by,
                        course_id,
                        title,
                        content,
                        posted_at
                    )
                    VALUES
                    (
                        @posted_by,
                        @course_id,
                        @title,
                        @content,
                        GETDATE()
                    )";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@posted_by", postedBy);
                cmd.Parameters.AddWithValue("@course_id", GetCourseValue());
                cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@content", txtContent.Text.Trim());

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Announcement posted successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadAnnouncements();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfAnnouncementID.Value))
            {
                lblMessage.Text = "Please select an announcement first.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            if (!ValidateForm())
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    UPDATE ANNOUNCEMENT
                    SET
                        course_id = @course_id,
                        title = @title,
                        content = @content
                    WHERE announcement_id = @announcement_id";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@course_id", GetCourseValue());
                cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@content", txtContent.Text.Trim());
                cmd.Parameters.AddWithValue("@announcement_id", hfAnnouncementID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Announcement updated successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadAnnouncements();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfAnnouncementID.Value))
            {
                lblMessage.Text = "Please select an announcement first.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    DELETE FROM ANNOUNCEMENT
                    WHERE announcement_id = @announcement_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@announcement_id", hfAnnouncementID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Announcement deleted successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadAnnouncements();
        }

        protected void gvAnnouncements_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvAnnouncements.SelectedRow;

            hfAnnouncementID.Value =
                gvAnnouncements.DataKeys[row.RowIndex].Value.ToString();

            LoadSelectedAnnouncement();

            lblMessage.Text = "Announcement selected. You can update or delete it.";
            lblMessage.CssClass = "text-primary fw-bold";
        }

        private void LoadSelectedAnnouncement()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        title,
                        content,
                        course_id
                    FROM ANNOUNCEMENT
                    WHERE announcement_id = @announcement_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@announcement_id", hfAnnouncementID.Value);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    txtTitle.Text = dr["title"].ToString();
                    txtContent.Text = dr["content"].ToString();

                    if (dr["course_id"] == DBNull.Value)
                    {
                        ddlCourse.SelectedIndex = 0;
                    }
                    else
                    {
                        string courseId = dr["course_id"].ToString();

                        if (ddlCourse.Items.FindByValue(courseId) != null)
                        {
                            ddlCourse.SelectedValue = courseId;
                        }
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadAnnouncements();
        }

        protected void btnResetSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";

            if (ddlSearchCourse.Items.Count > 0)
            {
                ddlSearchCourse.SelectedIndex = 0;
            }

            LoadAnnouncements();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            lblMessage.Text = "";
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                lblMessage.Text = "Please enter announcement title.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                lblMessage.Text = "Please enter announcement content.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            if (txtTitle.Text.Trim().Length > 200)
            {
                lblMessage.Text = "Announcement title cannot exceed 200 characters.";
                lblMessage.CssClass = "text-danger fw-bold";
                return false;
            }

            return true;
        }

        private object GetCourseValue()
        {
            if (string.IsNullOrEmpty(ddlCourse.SelectedValue))
            {
                return DBNull.Value;
            }

            return Convert.ToInt32(ddlCourse.SelectedValue);
        }

        private int GetCurrentUserId()
        {
            if (Session["UserID"] != null)
            {
                return Convert.ToInt32(Session["UserID"]);
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT TOP 1 user_id
                    FROM [USER]
                    WHERE role = 'HOP'
                    ORDER BY user_id";

                SqlCommand cmd = new SqlCommand(q, con);

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
            }

            return 1;
        }

        private void Clear()
        {
            hfAnnouncementID.Value = "";

            txtTitle.Text = "";
            txtContent.Text = "";

            if (ddlCourse.Items.Count > 0)
            {
                ddlCourse.SelectedIndex = 0;
            }
        }
    }
}