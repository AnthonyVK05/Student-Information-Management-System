using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;

namespace Student_Information_Management_System.Student
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
                    SELECT user_id, username, is_active
                    FROM [USER]
                    WHERE email = @email
                    AND role = 'STUDENT'";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@email", email);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                {
                    ShowError("No student account found with this email.");
                    return;
                }

                int userId = Convert.ToInt32(dr["user_id"]);
                string username = dr["username"].ToString();
                bool isActive = Convert.ToBoolean(dr["is_active"]);

                dr.Close();

                if (!isActive)
                {
                    ShowError("This account is inactive. Please contact HOP.");
                    return;
                }

                string token = Guid.NewGuid().ToString();
                DateTime expiry = DateTime.Now.AddHours(24);

                string updateQuery = @"
                    UPDATE [USER]
                    SET verification_token = @token,
                        verification_expiry = @expiry
                    WHERE user_id = @user_id";

                SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                updateCmd.Parameters.AddWithValue("@token", token);
                updateCmd.Parameters.AddWithValue("@expiry", expiry);
                updateCmd.Parameters.AddWithValue("@user_id", userId);

                updateCmd.ExecuteNonQuery();

                SendResetEmail(email, username, token);

                ShowSuccess("Password reset link sent. Please check your email.");
            }
        }

        protected void btnBackLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }

        private void SendResetEmail(string email, string username, string token)
        {
            string resetUrl =
                Request.Url.GetLeftPart(UriPartial.Authority) +
                ResolveUrl("~/PresentationLayer/STUDENT/ResetPassword.aspx?token=" + token);

            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.Subject = "SIMS Student Password Reset";

            mail.Body =
                "Hi " + username + ",<br/><br/>" +
                "Click the link below to reset your SIMS password:<br/><br/>" +
                "<a href='" + resetUrl + "'>Reset Password</a><br/><br/>" +
                "This link will expire in 24 hours.<br/><br/>" +
                "SIMS College Portal";

            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Send(mail);
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "d-block text-danger text-center mt-3 fw-bold";
        }

        private void ShowSuccess(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "d-block text-success text-center mt-3 fw-bold";
        }
    }
}