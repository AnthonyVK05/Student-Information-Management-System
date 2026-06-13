<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="VerifyEmail.aspx.cs"
    Inherits="Student_Information_Management_System.Student.VerifyEmail" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Email Verification</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/LoginTheme.css" rel="stylesheet" />
</head>

<body>

<form id="form1" runat="server">

    <div class="login-card">

        <h3 class="login-title">
            EMAIL VERIFICATION
        </h3>

        <div class="login-subtitle">
            Student Information Management System
        </div>

        <asp:Label ID="lblMessage"
            runat="server"
            CssClass="d-block text-center fw-bold mt-3">
        </asp:Label>

        <asp:Button ID="btnLogin"
            runat="server"
            Text="Go To Login"
            CssClass="btn btn-outline-primary w-100 mt-4"
            OnClick="btnLogin_Click"
            Visible="false" />

    </div>

</form>

</body>
</html>