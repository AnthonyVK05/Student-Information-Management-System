<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="LoginPage.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.LoginPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>SIMS Login</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/LoginTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <div class="login-card">

        <h3 class="login-title">SIMS LOGIN</h3>

        <div class="login-subtitle">
            Student Information Management System
        </div>

        <div class="mb-3">
            <label class="form-label fw-semibold">Username</label>
            <asp:TextBox ID="txtUsername" runat="server"
                CssClass="form-control"
                placeholder="Enter username" />
        </div>

        <div class="mb-3">
            <label class="form-label fw-semibold">Password</label>
            <asp:TextBox ID="txtPassword" runat="server"
                CssClass="form-control"
                TextMode="Password"
                placeholder="Enter password" />
        </div>

        <asp:Button ID="btnLogin" runat="server"
            Text="Login"
            CssClass="btn-login"
            OnClick="btnLogin_Click" />

        <asp:Label ID="lblMessage" runat="server"
            CssClass="d-block text-danger text-center mt-3 fw-bold" />

        <div class="login-footer">
            Secure access for HOP, Lecturer and Student
        </div>

    </div>

</form>
</body>
</html>