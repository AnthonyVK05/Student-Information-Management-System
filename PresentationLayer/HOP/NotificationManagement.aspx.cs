using Student_Information_Management_System.STUDENT;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class NotificationManagement : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
                LoadNotifications();
            }
        }

        private void LoadUsers()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        user_id,
                        username + ' - ' + role + ' - ' + ISNULL(email, '') AS user_display
                    FROM [USER]
                    WHERE is_active = 1
                    ORDER BY role, username";

                SqlDataAdapter da = new SqlDataAdapter(q, con);
                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlUser.DataSource = dt;
                ddlUser.DataTextField = "user_display";
                ddlUser.DataValueField = "user_id";
                ddlUser.DataBind();

                ddlUser.Items.Insert(0, new ListItem("-- Select User --", ""));
            }
        }

        private void LoadNotifications()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string search = txtSearch == null ? "" : txtSearch.Text.Trim();
                string readStatus = ddlReadStatus == null ? "All" : ddlReadStatus.SelectedValue;

                string q = @"
                    SELECT
                        n.notification_id,
                        u.username,
                        u.role,
                        n.type,
                        n.message,
                        CASE
                            WHEN n.is_read = 1 THEN 'Read'
                            ELSE 'Unread'
                        END AS read_status,
                        n.created_at
                    FROM NOTIFICATION n
                    INNER JOIN [USER] u
                        ON n.user_id = u.user_id
                    WHERE
                    (
                        @search = ''
                        OR u.username LIKE @searchLike
                        OR u.role LIKE @searchLike
                        OR n.type LIKE @searchLike
                        OR n.message LIKE @searchLike
                    )
                    AND
                    (
                        @read_status = 'All'
                        OR (@read_status = 'Read' AND n.is_read = 1)
                        OR (@read_status = 'Unread' AND n.is_read = 0)
                    )
                    ORDER BY n.created_at DESC";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@searchLike", "%" + search + "%");
                cmd.Parameters.AddWithValue("@read_status", readStatus);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvNotifications.DataSource = dt;
                gvNotifications.DataBind();
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    INSERT INTO NOTIFICATION
                    (
                        user_id,
                        type,
                        message,
                        is_read,
                        created_at
                    )
                    VALUES
                    (
                        @user_id,
                        @type,
                        @message,
                        0,
                        GETDATE()
                    )";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@user_id", ddlUser.SelectedValue);
                cmd.Parameters.AddWithValue("@type", ddlType.SelectedValue);
                cmd.Parameters.AddWithValue("@message", txtMessage.Text.Trim());

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Notification sent successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadNotifications();
        }

        protected void btnMarkRead_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfNotificationID.Value))
            {
                ShowError("Please select a notification first.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    UPDATE NOTIFICATION
                    SET is_read = 1
                    WHERE notification_id = @notification_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@notification_id", hfNotificationID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Notification marked as read.";
            lblMessage.CssClass = "text-success fw-bold";

            LoadNotifications();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfNotificationID.Value))
            {
                ShowError("Please select a notification first.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    DELETE FROM NOTIFICATION
                    WHERE notification_id = @notification_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@notification_id", hfNotificationID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Notification deleted successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadNotifications();
        }

        protected void gvNotifications_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvNotifications.SelectedRow;

            hfNotificationID.Value =
                gvNotifications.DataKeys[row.RowIndex].Value.ToString();

            LoadSelectedNotification();

            lblMessage.Text = "Notification selected.";
            lblMessage.CssClass = "text-primary fw-bold";
        }

        private void LoadSelectedNotification()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        user_id,
                        type,
                        message
                    FROM NOTIFICATION
                    WHERE notification_id = @notification_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@notification_id", hfNotificationID.Value);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    string userId = dr["user_id"].ToString();

                    if (ddlUser.Items.FindByValue(userId) != null)
                    {
                        ddlUser.SelectedValue = userId;
                    }

                    if (ddlType.Items.FindByValue(dr["type"].ToString()) != null)
                    {
                        ddlType.SelectedValue = dr["type"].ToString();
                    }

                    txtMessage.Text = dr["message"].ToString();
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadNotifications();
        }

        protected void btnResetSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";

            if (ddlReadStatus.Items.Count > 0)
            {
                ddlReadStatus.SelectedIndex = 0;
            }

            LoadNotifications();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            lblMessage.Text = "";
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(ddlUser.SelectedValue))
            {
                ShowError("Please select user.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                ShowError("Please enter notification message.");
                return false;
            }

            return true;
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "text-danger fw-bold";
        }

        private void Clear()
        {
            hfNotificationID.Value = "";

            if (ddlUser.Items.Count > 0)
            {
                ddlUser.SelectedIndex = 0;
            }

            if (ddlType.Items.Count > 0)
            {
                ddlType.SelectedIndex = 0;
            }

            txtMessage.Text = "";
        }
    }
}