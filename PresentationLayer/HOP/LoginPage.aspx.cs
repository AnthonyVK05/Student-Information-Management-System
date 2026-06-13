using System;

namespace Student_Information_Management_System.HOP
{
    public partial class LoginPage : System.Web.UI.Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (username == "hopadmin" && password == "Hop@12345")
            {
                Session["UserID"] = 1;
                Session["User"] = username;
                Session["Role"] = "HOP_ADMIN";

                Response.Redirect("~/PresentationLayer/HOP/AdminDashboard.aspx");
                return;
            }

            lblMessage.Text = "Invalid username or password";
        }
    }
}