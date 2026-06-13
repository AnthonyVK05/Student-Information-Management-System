using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net.Mail;

namespace Student_Information_Management_System.Student
{
    public partial class ResendVerification : System.Web.UI.Page
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
                    SELECT user_id, username, is_active, email_verified
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
                bool emailVerified = Convert.ToBoolean(dr["email_verified"]);

                dr.Close();

                if (!isActive)
                {
                    ShowError("This account is inactive. Please contact HOP.");
                    return;
                }

                if (emailVerified)
                {
                    ShowSuccess("This email is already verified. You can login now.");
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

                SendVerificationEmail(email, username, token);

                ShowSuccess("Verification link sent. Please check your email.");
            }
        }

        protected void btnBackLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }

        private void SendVerificationEmail(string email, string username, string token)
        {
            string verifyUrl =
                Request.Url.GetLeftPart(UriPartial.Authority) +
                ResolveUrl("~/PresentationLayer/STUDENT/VerifyEmail.aspx?token=" + token);

            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.Subject = "SIMS Student Email Verification";

            mail.Body =
                "Hi " + username + ",<br/><br/>" +
                "Please click the link below to verify your SIMS student account:<br/><br/>" +
                "<a href='" + verifyUrl + "'>Verify Email</a><br/><br/>" +
                "This link will expire in 24 hours.<br/><br/>" +
                "SIMS College Portal";

            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();
            smtp.Send(mail);
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass =
                "d-block text-danger text-center mt-3 fw-bold";
        }

        private void ShowSuccess(string message)
        {
            lblMessage.Text = message;
            lblMessage.CssClass =
                "d-block text-success text-center mt-3 fw-bold";
        }
    }
}