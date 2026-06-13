using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Student_Information_Management_System.Lecturer
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void btnReset_Click(object sender, EventArgs e)
        {
            string token = Request.QueryString["token"];

            if (string.IsNullOrWhiteSpace(token))
            {
                ShowError("Invalid reset link.");
                return;
            }

            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (!ValidatePassword(password, confirmPassword))
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT user_id
                    FROM [USER]
                    WHERE verification_token = @token
                    AND verification_expiry > GETDATE()
                    AND role = 'LECTURER'";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@token", token);

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    ShowError("Reset link expired or invalid.");
                    return;
                }

                int userId = Convert.ToInt32(result);

                string hashedPassword =
                    BCrypt.Net.BCrypt.HashPassword(password);

                string updateQuery = @"
                    UPDATE [USER]
                    SET password_hash = @password_hash,
                        verification_token = NULL,
                        verification_expiry = NULL
                    WHERE user_id = @user_id";

                SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                updateCmd.Parameters.AddWithValue("@password_hash", hashedPassword);
                updateCmd.Parameters.AddWithValue("@user_id", userId);

                updateCmd.ExecuteNonQuery();

                ShowSuccess("Password reset successfully. You can login now.");

                btnReset.Enabled = false;
                txtPassword.Enabled = false;
                txtConfirmPassword.Enabled = false;
            }
        }

        protected void btnLoginPage_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }

        private bool ValidatePassword(string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter new password.");
                return false;
            }

            if (password != confirmPassword)
            {
                ShowError("Passwords do not match.");
                return false;
            }

            if (password.Length < 8)
            {
                ShowError("Password must be at least 8 characters.");
                return false;
            }

            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                ShowError("Password must contain at least one uppercase letter.");
                return false;
            }

            if (!Regex.IsMatch(password, "[a-z]"))
            {
                ShowError("Password must contain at least one lowercase letter.");
                return false;
            }

            if (!Regex.IsMatch(password, "[0-9]"))
            {
                ShowError("Password must contain at least one number.");
                return false;
            }

            if (!Regex.IsMatch(password, @"[\W_]"))
            {
                ShowError("Password must contain at least one special character.");
                return false;
            }

            return true;
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