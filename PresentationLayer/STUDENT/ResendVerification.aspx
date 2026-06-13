<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ResendVerification.aspx.cs"
    Inherits="Student_Information_Management_System.Student.ResendVerification" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Resend Verification</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/LoginTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <div class="login-card">

        <h3 class="login-title">EMAIL VERIFICATION</h3>

        <div class="login-subtitle">
            Enter your registered email to receive a new verification link
        </div>

        <div class="mb-3">
            <label class="form-label fw-semibold">Email Address</label>

            <asp:TextBox ID="txtEmail"
                runat="server"
                CssClass="form-control"
                TextMode="Email"
                placeholder="Enter registered email" />
        </div>

        <asp:Button ID="btnSend"
            runat="server"
            Text="Send Verification Link"
            CssClass="btn-login"
            OnClick="btnSend_Click" />

        <asp:Button ID="btnBackLogin"
            runat="server"
            Text="Back To Login"
            CssClass="btn btn-outline-primary w-100 mt-3"
            OnClick="btnBackLogin_Click" />

        <asp:Label ID="lblMessage"
            runat="server"
            CssClass="d-block text-center mt-3 fw-bold" />

    </div>

</form>
</body>
</html>