using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Student_Information_Management_System.Student
{
    public partial class VerifyEmail : System.Web.UI.Page
    {
        private readonly string cs =
            ConfigurationManager.ConnectionStrings["UniversityDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                VerifyStudentEmail();
            }
        }

        private void VerifyStudentEmail()
        {
            string token = Request.QueryString["token"];

            if (string.IsNullOrEmpty(token))
            {
                ShowError("Invalid verification link.");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                string q = @"
                    SELECT user_id
                    FROM [USER]
                    WHERE verification_token = @token
                    AND verification_expiry > GETDATE()";

                SqlCommand cmd = new SqlCommand(q, con);
                cmd.Parameters.AddWithValue("@token", token);

                con.Open();

                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    ShowError("Verification link expired or invalid.");
                    return;
                }

                int userId = Convert.ToInt32(result);

                string updateQuery = @"
                    UPDATE [USER]
                    SET email_verified = 1,
                        verification_token = NULL,
                        verification_expiry = NULL
                    WHERE user_id = @user_id";

                SqlCommand updateCmd =
                    new SqlCommand(updateQuery, con);

                updateCmd.Parameters.AddWithValue(
                    "@user_id",
                    userId);

                updateCmd.ExecuteNonQuery();

                ShowSuccess(
                    "Email verified successfully. You may now login.");

                btnLogin.Visible = true;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("LoginPage.aspx");
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;

            lblMessage.CssClass =
                "d-block text-danger text-center fw-bold mt-3";
        }

        private void ShowSuccess(string message)
        {
            lblMessage.Text = message;

            lblMessage.CssClass =
                "d-block text-success text-center fw-bold mt-3";
        }
    }
}