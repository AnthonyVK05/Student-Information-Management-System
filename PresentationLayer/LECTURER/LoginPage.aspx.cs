using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Student_Information_Management_System.Lecturer
{
    public partial class LoginPage : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {
                lblMessage.Text = "Please enter username.";
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Please enter password.";
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT
                        u.user_id,
                        u.username,
                        u.password_hash,
                        u.is_active,
                        u.email_verified,
                        l.lecturer_id,
                        l.full_name,
                        l.status
                    FROM [USER] u
                    INNER JOIN LECTURER l
                        ON u.user_id = l.user_id
                    WHERE u.username = @username
                    AND u.role = 'LECTURER'";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@username", username);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (!dr.Read())
                {
                    lblMessage.Text = "Invalid username or password.";
                    return;
                }

                string storedHash = dr["password_hash"].ToString();

                bool passwordValid = false;

                try
                {
                    passwordValid = BCrypt.Net.BCrypt.Verify(password, storedHash);
                }
                catch
                {
                    passwordValid = false;
                }

                if (!passwordValid)
                {
                    lblMessage.Text = "Invalid username or password.";
                    return;
                }

                if (!Convert.ToBoolean(dr["is_active"]))
                {
                    lblMessage.Text = "Your account is inactive. Please contact HOP.";
                    return;
                }

                if (!Convert.ToBoolean(dr["email_verified"]))
                {
                    lblMessage.Text = "Please verify your email before login.";
                    return;
                }

                if (dr["status"].ToString() != "Active")
                {
                    lblMessage.Text = "Your lecturer status is not active.";
                    return;
                }

                Session["UserID"] = dr["user_id"];
                Session["LecturerID"] = dr["lecturer_id"];
                Session["User"] = dr["username"];
                Session["FullName"] = dr["full_name"];
                Session["Role"] = "LECTURER";

                Response.Redirect("LecturerDashboard.aspx");
            }
        }
    }
}