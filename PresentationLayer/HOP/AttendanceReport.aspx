<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AttendanceReport.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.AttendanceReport" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Attendance Report</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
<form id="form1" runat="server">

<nav class="navbar navbar-dark bg-primary px-4">
    <span class="navbar-brand">SIMS - Attendance Report</span>
    <a href="AdminDashboard.aspx" class="btn btn-light btn-sm">Back</a>
</nav>

<div class="container mt-4">
    <div class="card shadow-sm">
        <div class="card-header bg-dark text-white">
            Attendance Summary
        </div>
        <div class="card-body">
            <asp:GridView ID="gvAttendanceReport" runat="server"
                CssClass="table table-bordered table-hover"
                AutoGenerateColumns="true"
                GridLines="None">
            </asp:GridView>
        </div>
    </div>
</div>

</form>
</body>
</html>