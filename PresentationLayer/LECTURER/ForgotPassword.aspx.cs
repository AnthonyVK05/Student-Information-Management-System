using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;

namespace Student_Information_Management_System.Lecturer
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                ShowError("Please enter your registered email.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        user_id,
                        username,
                        is_active
                    FROM [USER]
                    WHERE email=@email
                    AND role='LECTURER'";

                SqlCommand cmd =
                    new SqlCommand(q, con);

                cmd.Parameters.AddWithValue("@email", email);

                con.Open();

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (!dr.Read())
                {
                    ShowError("No lecturer account found.");
                    return;
                }

                int userId =
                    Convert.ToInt32(dr["user_id"]);

                string username =
                    dr["username"].ToString();

                bool isActive =
                    Convert.ToBoolean(dr["is_active"]);

                dr.Close();

                if (!isActive)
                {
                    ShowError("Account is inactive.");
                    return;
                }

                string token =
                    Guid.NewGuid().ToString();

                DateTime expiry =
                    DateTime.Now.AddHours(24);

                string updateQuery = @"
                    UPDATE [USER]
                    SET verification_token=@token,
                        verification_expiry=@expiry
                    WHERE user_id=@user_id";

                SqlCommand updateCmd =
                    new SqlCommand(updateQuery, con);

                updateCmd.Parameters.AddWithValue(
                    "@token", token);

                updateCmd.Parameters.AddWithValue(
                    "@expiry", expiry);

                updateCmd.Parameters.AddWithValue(
                    "@user_id", userId);

                updateCmd.ExecuteNonQuery();

                SendEmail(email, username, token);

                ShowSuccess(
                    "Password reset email sent successfully.");
            }
        }

        private void SendEmail(
            string email,
            string username,
            string token)
        {
            string resetUrl =
                Request.Url.GetLeftPart(UriPartial.Authority)
                + ResolveUrl(
                "~/PresentationLayer/Lecturer/ResetPassword.aspx?token="
                + token);

            MailMessage mail =
                new MailMessage();

            mail.To.Add(email);

            mail.Subject =
                "SIMS Lecturer Password Reset";

            mail.IsBodyHtml = true;

            mail.Body =
                "Dear " + username +
                "<br/><br/>" +
                "Please click the link below to reset your password." +
                "<br/><br/>" +
                "<a href='" + resetUrl + "'>Reset Password</a>" +
                "<br/><br/>" +
                "This link expires in 24 hours.";

            SmtpClient smtp =
                new SmtpClient();

            smtp.Send(mail);
        }

        protected void btnBack_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect(
                "LoginPage.aspx");
        }

        private void ShowError(string msg)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass =
                "d-block text-danger text-center mt-3 fw-bold";
        }

        private void ShowSuccess(string msg)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass =
                "d-block text-success text-center mt-3 fw-bold";
        }
    }
}