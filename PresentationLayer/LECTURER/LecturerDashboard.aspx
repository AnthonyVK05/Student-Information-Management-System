<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="LecturerDashboard.aspx.cs"
    Inherits="Student_Information_Management_System.Lecturer.LecturerDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Lecturer Dashboard</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <nav class="navbar topbar px-4 py-3">
        <span class="navbar-brand fw-bold text-dark">
            <span class="blue-dot"></span>SIMS Lecturer Portal
        </span>

        <asp:Label ID="lblLecturerName"
            runat="server"
            CssClass="fw-semibold">
        </asp:Label>

        <asp:Button ID="btnLogout"
            runat="server"
            Text="Logout"
            CssClass="btn btn-ios-light btn-sm"
            OnClick="btnLogout_Click" />
    </nav>

    <div class="container-fluid">

        <div class="row">

            <div class="col-md-2 sidebar p-3">

                <div class="section-title">Lecturer</div>

                <a href="LecturerDashboard.aspx" class="active">
                    Dashboard
                </a>

                <a href="MyProfile.aspx">
                    My Profile
                </a>

                <a href="MyCourses.aspx">
                    My Courses
                </a>

                <a href="CourseStudents.aspx">
                    Course Students
                </a>

                <div class="section-title">Academic</div>

                <a href="Attendance.aspx">
                    Attendance
                </a>

                <a href="GradeManagement.aspx">
                    Grade Management
                </a>

                <a href="CourseAnalytics.aspx">
                    Course Analytics
                </a>

                <a href="RiskStudents.aspx">
                    Risk Students
                </a>

                <div class="section-title">Communication</div>

                <a href="Announcements.aspx">
                    Announcements
                </a>

                <a href="Messages.aspx">
                    Messages
                </a>

                <a href="Notifications.aspx">
                    Notifications
                </a>

                <a href="Reports.aspx">
                    Reports
                </a>

            </div>

            <div class="col-md-10 p-4">

                <div class="glass-card hero-card mb-4">

                    <h2 class="fw-bold mb-1">
                        Welcome,
                        <asp:Label ID="lblWelcome"
                            runat="server">
                        </asp:Label>
                    </h2>

                    <p class="text-muted mb-0">
                        Lecturer Academic Management Dashboard
                    </p>

                </div>

                <div class="row g-4 mb-4">

                    <div class="col-md-3">
                        <div class="glass-card p-4">

                            <div class="mini-help">
                                Assigned Courses
                            </div>

                            <h2 class="fw-bold mb-0">
                                <asp:Label ID="lblCourses"
                                    runat="server"
                                    Text="0">
                                </asp:Label>
                            </h2>

                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="glass-card p-4">

                            <div class="mini-help">
                                Registered Students
                            </div>

                            <h2 class="fw-bold mb-0">
                                <asp:Label ID="lblStudents"
                                    runat="server"
                                    Text="0">
                                </asp:Label>
                            </h2>

                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="glass-card p-4">

                            <div class="mini-help">
                                Pending Grades
                            </div>

                            <h2 class="fw-bold mb-0">
                                <asp:Label ID="lblPendingGrades"
                                    runat="server"
                                    Text="0">
                                </asp:Label>
                            </h2>

                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="glass-card p-4">

                            <div class="mini-help">
                                Attendance Alerts
                            </div>

                            <h2 class="fw-bold mb-0">
                                <asp:Label ID="lblAlerts"
                                    runat="server"
                                    Text="0">
                                </asp:Label>
                            </h2>

                        </div>
                    </div>

                </div>

                <div class="row g-4 mb-4">

                    <div class="col-lg-6">

                        <div class="glass-card table-card h-100">

                            <div class="table-header">
                                <h5 class="fw-bold mb-1">
                                    Assigned Classes
                                </h5>

                                <div class="mini-help">
                                    Courses assigned to you for the academic session.
                                </div>
                            </div>

                            <div class="p-3 table-responsive">

                                <asp:GridView ID="gvClasses"
                                    runat="server"
                                    CssClass="table table-bordered table-hover"
                                    AutoGenerateColumns="true"
                                    GridLines="None">
                                </asp:GridView>

                            </div>

                        </div>

                    </div>

                    <div class="col-lg-6">

                        <div class="glass-card table-card h-100">

                            <div class="table-header">
                                <h5 class="fw-bold mb-1">
                                    Risk Students
                                </h5>

                                <div class="mini-help">
                                    Students requiring academic or attendance attention.
                                </div>
                            </div>

                            <div class="p-3 table-responsive">

                                <asp:GridView ID="gvRiskStudents"
                                    runat="server"
                                    CssClass="table table-bordered table-hover"
                                    AutoGenerateColumns="true"
                                    GridLines="None">
                                </asp:GridView>

                            </div>

                        </div>

                    </div>

                </div>

                <div class="glass-card p-4">

                    <h5 class="fw-bold mb-3">
                        Quick Actions
                    </h5>

                    <asp:Button ID="btnAttendance"
                        runat="server"
                        Text="Take Attendance"
                        CssClass="btn btn-ios-primary me-2"
                        OnClick="btnAttendance_Click" />

                    <asp:Button ID="btnGrades"
                        runat="server"
                        Text="Manage Grades"
                        CssClass="btn btn-ios-warning me-2"
                        OnClick="btnGrades_Click" />

                    <asp:Button ID="btnAnnouncement"
                        runat="server"
                        Text="Post Announcement"
                        CssClass="btn btn-ios-light me-2"
                        OnClick="btnAnnouncement_Click" />

                    <asp:Button ID="btnMessages"
                        runat="server"
                        Text="Messages"
                        CssClass="btn btn-ios-light"
                        OnClick="btnMessages_Click" />

                </div>

            </div>

        </div>

    </div>

</form>
</body>
</html>