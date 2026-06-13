using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Student_Information_Management_System.HOP
{
    public partial class VerifyEmail : System.Web.UI.Page
    {
        string cs = ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            string token = Request.QueryString["token"];

            if (string.IsNullOrEmpty(token))
            {
                lblMessage.Text = "Invalid verification link.";
                lblMessage.CssClass = "text-danger fw-bold";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT user_id, verification_expiry
                    FROM [USER]
                    WHERE verification_token = @token
                    AND email_verified = 0";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@token", token);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                {
                    lblMessage.Text = "Invalid or already used verification link.";
                    lblMessage.CssClass = "text-danger fw-bold";
                    return;
                }

                int userId = Convert.ToInt32(dr["user_id"]);
                DateTime expiry = Convert.ToDateTime(dr["verification_expiry"]);

                dr.Close();

                if (DateTime.Now > expiry)
                {
                    lblMessage.Text = "Verification link expired. Please contact HOP to resend verification email.";
                    lblMessage.CssClass = "text-danger fw-bold";
                    return;
                }

                string update = @"
                    UPDATE [USER]
                    SET
                        email_verified = 1,
                        is_active = 1,
                        verification_token = NULL,
                        verification_expiry = NULL
                    WHERE user_id = @user_id";

                SqlCommand updateCmd = new SqlCommand(update, con);
                updateCmd.Parameters.AddWithValue("@user_id", userId);
                updateCmd.ExecuteNonQuery();

                lblMessage.Text = "Email verified successfully. You can now login.";
                lblMessage.CssClass = "text-success fw-bold";
            }
        }
    }
}