using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Student_Information_Management_System.HOP
{
    public partial class MessageManagement : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/PresentationLayer/HOP/LoginPage.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadReceivers();
                LoadMessages();
            }
        }

        private void LoadReceivers()
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

                ddlReceiver.DataSource = dt;
                ddlReceiver.DataTextField = "user_display";
                ddlReceiver.DataValueField = "user_id";
                ddlReceiver.DataBind();

                ddlReceiver.Items.Insert(0, new ListItem("-- Select Receiver --", ""));
            }
        }

        private void LoadMessages()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                int currentUserId = GetCurrentUserId();

                string search = txtSearch == null ? "" : txtSearch.Text.Trim();
                string readStatus = ddlReadStatus == null ? "All" : ddlReadStatus.SelectedValue;
                string box = ddlBox == null ? "Inbox" : ddlBox.SelectedValue;

                string q = @"
                    SELECT
                        m.message_id,
                        sender.username AS sender_name,
                        receiver.username AS receiver_name,
                        m.subject,
                        m.message_body,
                        CASE
                            WHEN m.is_read = 1 THEN 'Read'
                            ELSE 'Unread'
                        END AS read_status,
                        m.sent_at
                    FROM MESSAGE m
                    INNER JOIN [USER] sender
                        ON m.sender_user_id = sender.user_id
                    INNER JOIN [USER] receiver
                        ON m.receiver_user_id = receiver.user_id
                    WHERE
                    (
                        @box = 'All'
                        OR (@box = 'Inbox' AND m.receiver_user_id = @current_user_id)
                        OR (@box = 'Sent' AND m.sender_user_id = @current_user_id)
                    )
                    AND
                    (
                        @search = ''
                        OR sender.username LIKE @searchLike
                        OR receiver.username LIKE @searchLike
                        OR m.subject LIKE @searchLike
                        OR m.message_body LIKE @searchLike
                    )
                    AND
                    (
                        @read_status = 'All'
                        OR (@read_status = 'Read' AND m.is_read = 1)
                        OR (@read_status = 'Unread' AND m.is_read = 0)
                    )
                    ORDER BY m.sent_at DESC";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@current_user_id", currentUserId);
                cmd.Parameters.AddWithValue("@box", box);
                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@searchLike", "%" + search + "%");
                cmd.Parameters.AddWithValue("@read_status", readStatus);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);

                gvMessages.DataSource = dt;
                gvMessages.DataBind();
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                return;
            }

            InsertMessage(
                GetCurrentUserId(),
                Convert.ToInt32(ddlReceiver.SelectedValue),
                txtSubject.Text.Trim(),
                txtMessage.Text.Trim(),
                DBNull.Value
            );

            lblMessage.Text = "Message sent successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadMessages();
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfMessageID.Value))
            {
                ShowError("Please select a message to reply.");
                return;
            }

            if (string.IsNullOrEmpty(hfReplyToUserID.Value))
            {
                ShowError("Reply user not found.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                ShowError("Please write your reply message.");
                return;
            }

            string subject = txtSubject.Text.Trim();

            if (!subject.StartsWith("RE:", StringComparison.OrdinalIgnoreCase))
            {
                subject = "RE: " + subject;
            }

            InsertMessage(
                GetCurrentUserId(),
                Convert.ToInt32(hfReplyToUserID.Value),
                subject,
                txtMessage.Text.Trim(),
                Convert.ToInt32(hfMessageID.Value)
            );

            lblMessage.Text = "Reply sent successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadMessages();
        }

        private void InsertMessage(
            int senderUserId,
            int receiverUserId,
            string subject,
            string messageBody,
            object parentMessageId
        )
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    INSERT INTO MESSAGE
                    (
                        sender_user_id,
                        receiver_user_id,
                        subject,
                        message_body,
                        parent_message_id,
                        is_read,
                        sent_at
                    )
                    VALUES
                    (
                        @sender_user_id,
                        @receiver_user_id,
                        @subject,
                        @message_body,
                        @parent_message_id,
                        0,
                        GETDATE()
                    )";

                SqlCommand cmd = new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@sender_user_id", senderUserId);
                cmd.Parameters.AddWithValue("@receiver_user_id", receiverUserId);
                cmd.Parameters.AddWithValue("@subject", subject);
                cmd.Parameters.AddWithValue("@message_body", messageBody);
                cmd.Parameters.AddWithValue("@parent_message_id", parentMessageId);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected void btnMarkRead_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfMessageID.Value))
            {
                ShowError("Please select a message first.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    UPDATE MESSAGE
                    SET is_read = 1
                    WHERE message_id = @message_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@message_id", hfMessageID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Message marked as read.";
            lblMessage.CssClass = "text-success fw-bold";

            LoadMessages();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hfMessageID.Value))
            {
                ShowError("Please select a message first.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = "DELETE FROM MESSAGE WHERE message_id = @message_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@message_id", hfMessageID.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Message deleted successfully.";
            lblMessage.CssClass = "text-success fw-bold";

            Clear();
            LoadMessages();
        }

        protected void gvMessages_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = gvMessages.SelectedRow;

            hfMessageID.Value =
                gvMessages.DataKeys[row.RowIndex].Value.ToString();

            LoadSelectedMessage();

            MarkSelectedMessageAsRead();

            lblMessage.Text = "Message selected. You can reply now.";
            lblMessage.CssClass = "text-primary fw-bold";

            LoadMessages();
        }

        private void LoadSelectedMessage()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        sender_user_id,
                        receiver_user_id,
                        subject,
                        message_body
                    FROM MESSAGE
                    WHERE message_id = @message_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@message_id", hfMessageID.Value);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    int currentUserId = GetCurrentUserId();
                    int senderId = Convert.ToInt32(dr["sender_user_id"]);
                    int receiverId = Convert.ToInt32(dr["receiver_user_id"]);

                    int replyToUserId = senderId == currentUserId
                        ? receiverId
                        : senderId;

                    hfReplyToUserID.Value = replyToUserId.ToString();

                    if (ddlReceiver.Items.FindByValue(replyToUserId.ToString()) != null)
                    {
                        ddlReceiver.SelectedValue = replyToUserId.ToString();
                    }

                    txtSubject.Text = dr["subject"].ToString();
                    txtMessage.Text = "";
                }
            }
        }

        private void MarkSelectedMessageAsRead()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    UPDATE MESSAGE
                    SET is_read = 1
                    WHERE message_id = @message_id
                    AND receiver_user_id = @current_user_id";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@message_id", hfMessageID.Value);
                cmd.Parameters.AddWithValue("@current_user_id", GetCurrentUserId());

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadMessages();
        }

        protected void btnResetSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";

            if (ddlReadStatus.Items.Count > 0)
            {
                ddlReadStatus.SelectedIndex = 0;
            }

            if (ddlBox.Items.Count > 0)
            {
                ddlBox.SelectedIndex = 0;
            }

            LoadMessages();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
            lblMessage.Text = "";
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(ddlReceiver.SelectedValue))
            {
                ShowError("Please select receiver.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                ShowError("Please enter subject.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                ShowError("Please enter message.");
                return false;
            }

            return true;
        }

        private int GetCurrentUserId()
        {
            return Convert.ToInt32(Session["UserID"]);
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "text-danger fw-bold";
        }

        private void Clear()
        {
            hfMessageID.Value = "";
            hfReplyToUserID.Value = "";

            if (ddlReceiver.Items.Count > 0)
            {
                ddlReceiver.SelectedIndex = 0;
            }

            txtSubject.Text = "";
            txtMessage.Text = "";
        }
    }
}