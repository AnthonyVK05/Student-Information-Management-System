<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="VerifyEmail.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.VerifyEmail" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Email Verification</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>

<body class="bg-light">
<form id="form1" runat="server">

<div class="container mt-5">
    <div class="card shadow-sm p-4">
        <h3>Email Verification</h3>
        <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold" />
    </div>
</div>

</form>
</body>
</html>