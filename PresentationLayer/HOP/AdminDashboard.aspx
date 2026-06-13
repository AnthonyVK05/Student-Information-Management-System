<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AdminDashboard.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.AdminDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>HOP Dashboard</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.min.css">

    <style>
        .dashboard-shell {
            min-height: calc(100vh - 65px);
        }

        .metric-card, .health-card, .glass-panel {
            background: rgba(255,255,255,.42);
            backdrop-filter: blur(30px);
            border: 1px solid rgba(255,255,255,.55);
            border-radius: 26px;
            box-shadow: 0 12px 30px rgba(15,23,42,.06);
            transition: .25s;
        }

        .metric-card {
            padding: 24px;
            min-height: 155px;
        }

        .metric-card:hover,
        .health-card:hover,
        .action-tile:hover,
        .clickable-row:hover {
            transform: translateY(-4px);
            border-color: rgba(37,99,235,.35);
            box-shadow: 0 16px 35px rgba(37,99,235,.12);
        }

        .clickable-card, .clickable-row {
            cursor: pointer;
        }

        .metric-icon {
            width: 46px;
            height: 46px;
            border-radius: 16px;
            background: rgba(37,99,235,.12);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 22px;
            margin-bottom: 16px;
        }

        .metric-title {
            color: #64748b;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 6px;
        }

        .metric-value {
            font-size: 36px;
            font-weight: 850;
            color: #0f172a;
            line-height: 1;
        }

        .metric-note {
            font-size: 13px;
            color: #64748b;
            margin-top: 10px;
        }

        .health-card {
            padding: 22px;
            min-height: 130px;
        }

        .health-label {
            font-size: 13px;
            color: #64748b;
            font-weight: 700;
        }

        .health-value {
            font-size: 30px;
            font-weight: 850;
            color: #0f172a;
        }

        .health-bar {
            height: 8px;
            border-radius: 999px;
            background: rgba(148,163,184,.22);
            overflow: hidden;
            margin-top: 12px;
        }

        .health-fill {
            height: 100%;
            border-radius: 999px;
            background: rgba(37,99,235,.75);
        }

        .pill {
            font-size: 12px;
            font-weight: 700;
            padding: 5px 10px;
            border-radius: 999px;
            background: rgba(37,99,235,.12);
            color: #2563eb;
        }

        .chart-placeholder {
            height: 240px;
            border-radius: 22px;
            background:
                linear-gradient(180deg, rgba(37,99,235,.10), rgba(255,255,255,.28)),
                repeating-linear-gradient(
                    to top,
                    rgba(148,163,184,.16) 0,
                    rgba(148,163,184,.16) 1px,
                    transparent 1px,
                    transparent 48px
                );
            display: flex;
            align-items: center;
            justify-content: center;
            color: #64748b;
            font-weight: 700;
        }

        .alert-item {
            padding: 14px 0;
            border-bottom: 1px solid rgba(148,163,184,.18);
            transition: .25s;
        }

        .alert-item:last-child {
            border-bottom: none;
        }

        .action-tile {
            display: block;
            text-decoration: none;
            color: #0f172a;
            background: rgba(255,255,255,.36);
            border: 1px solid rgba(255,255,255,.55);
            border-radius: 22px;
            padding: 18px;
            transition: .25s;
            font-weight: 600;
            min-height: 75px;
        }

        .action-tile:hover {
            color: #2563eb;
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

<nav class="navbar topbar px-4 py-3">

    <span class="navbar-brand fw-bold text-dark">
        <span class="blue-dot"></span>SIMS College Portal
    </span>

    <div class="d-flex gap-2">

        <asp:Button ID="btnNotification" runat="server"
            Text="🔔 Notifications"
            CssClass="btn btn-ios-light btn-sm"
            OnClick="btnNotification_Click" />

        <asp:Button ID="btnMessages" runat="server"
            Text="💬 Messages"
            CssClass="btn btn-ios-light btn-sm"
            OnClick="btnMessages_Click" />

        <asp:Button ID="btnLogout" runat="server"
            Text="Logout"
            CssClass="btn btn-ios-light btn-sm"
            OnClick="btnLogout_Click" />

    </div>

</nav>
    <div class="container-fluid dashboard-shell">
        <div class="row">

            <div class="col-md-2 sidebar p-3">

                <div class="section-title">Main</div>
                <a href="AdminDashboard.aspx">Dashboard</a>

                <div class="section-title">Registration</div>
                <a href="RegisterStudent.aspx">Register Student</a>
                <a href="RegisterLecturer.aspx">Register Lecturer</a>

                <div class="section-title">Academic Setup</div>
                <a href="ManageProgrammes.aspx">Manage Programmes</a>
                <a href="ManageCourses.aspx">Manage Courses</a>
                <a href="AcademicCalendar.aspx">Academic Calendar</a>

                <div class="section-title">Management</div>
                <a href="EnrolmentManagement.aspx">Enrolment Management</a>
                <a href="Lecturer-CourseAssignment.aspx">Lecturer Course Assignment</a>
                <a href="AttendanceManagement.aspx">Attendance Management</a>
                <a href="CourseMarks.aspx">Course Marks</a>

                <div class="section-title">Reports</div>
                <a href="GPAReport.aspx">GPA / CGPA Report</a>
                <a href="RiskStudents.aspx">Risk Students</a>
                <a href="AttendanceReport.aspx">Attendance Report</a>
                <a href="AnalyticsDashboard.aspx">Analytics Dashboard</a>
                <a href="Announcement(Admin).aspx">Announcements</a>

            </div>

            <div class="col-md-10 p-4">

                <div class="hero-card mb-4">
                    <div class="d-flex justify-content-between align-items-start flex-wrap gap-3">
                        <div>
                            <span class="pill">Head of Programme</span>
                            <h2 class="fw-bold mt-3 mb-1">Academic Management Dashboard</h2>
                            <p class="text-muted mb-0">
                                Monitor academic activity, student progress, enrolment health and operational alerts.
                            </p>
                        </div>

                        <div class="d-flex gap-2">
                            <a href="RegisterStudent.aspx" class="btn btn-ios-primary">Register Student</a>
                            <a href="AnalyticsDashboard.aspx" class="btn btn-ios-light">View Analytics</a>
                        </div>
                    </div>
                </div>

                <div class="row g-4 mb-4">

                    <div class="col-md-3">
                        <div class="metric-card clickable-card"
                             onclick="location.href='RegisterStudent.aspx'">
                            <div class="metric-icon">🎓</div>
                            <div class="metric-title">Active Students</div>
                            <div class="metric-value">
                                <asp:Label ID="lblStudents" runat="server" Text="0" />
                            </div>
                            <div class="metric-note">Click to view student records</div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="metric-card clickable-card"
                             onclick="location.href='RegisterLecturer.aspx'">
                            <div class="metric-icon">👨‍🏫</div>
                            <div class="metric-title">Lecturers</div>
                            <div class="metric-value">
                                <asp:Label ID="lblLecturers" runat="server" Text="0" />
                            </div>
                            <div class="metric-note">Click to manage lecturers</div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="metric-card clickable-card"
                             onclick="location.href='ManageCourses.aspx'">
                            <div class="metric-icon">📚</div>
                            <div class="metric-title">Courses</div>
                            <div class="metric-value">
                                <asp:Label ID="lblCourses" runat="server" Text="0" />
                            </div>
                            <div class="metric-note">Click to manage courses</div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="metric-card clickable-card"
                             onclick="location.href='EnrolmentManagement.aspx'">
                            <div class="metric-icon">📝</div>
                            <div class="metric-title">Current Enrolments</div>
                            <div class="metric-value">
                                <asp:Label ID="lblEnrolment" runat="server" Text="0" />
                            </div>
                            <div class="metric-note">Click to view enrolments</div>
                        </div>
                    </div>

                </div>

                <div class="row g-4 mb-4">

                    <div class="col-md-3">
                        <div class="health-card clickable-card"
                             onclick="location.href='GPAReport.aspx'">
                            <div class="health-label">Average CGPA</div>
                            <div class="health-value">
                                <asp:Label ID="lblAverageGPA" runat="server" Text="0.00" />
                            </div>
                            <div class="health-bar">
                                <div class="health-fill" style="width:70%;"></div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="health-card clickable-card"
                             onclick="location.href='RiskStudents.aspx'">
                            <div class="health-label">At-Risk Students</div>
                            <div class="health-value">
                                <asp:Label ID="lblRiskStudents" runat="server" Text="0" />
                            </div>
                            <div class="health-bar">
                                <div class="health-fill" style="width:35%;"></div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="health-card clickable-card"
                             onclick="location.href='GPAReport.aspx'">
                            <div class="health-label">Pass Rate</div>
                            <div class="health-value">
                                <asp:Label ID="lblPassRate" runat="server" Text="0%" />
                            </div>
                            <div class="health-bar">
                                <div class="health-fill" style="width:85%;"></div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="health-card clickable-card"
                             onclick="location.href='AttendanceReport.aspx'">
                            <div class="health-label">Attendance Rate</div>
                            <div class="health-value">
                                <asp:Label ID="lblAttendanceRate" runat="server" Text="0%" />
                            </div>
                            <div class="health-bar">
                                <div class="health-fill" style="width:80%;"></div>
                            </div>
                        </div>
                    </div>

                </div>

                <div class="row g-4 mb-4">

                    <div class="col-md-8">
                        <div class="glass-panel p-4">
                            <div class="d-flex justify-content-between align-items-center mb-3">
                                <div>
                                    <h5 class="fw-bold mb-1">Student Growth Trend</h5>
                                    <p class="text-muted mb-0">
                                        Monthly registration and enrolment overview.
                                    </p>
                                </div>
                                <span class="pill">Chart Area</span>
                            </div>

                            <div class="chart-placeholder">
                                Chart.js graph can be connected here
                            </div>
                        </div>
                    </div>

                    <div class="col-md-4">
                        <div class="glass-panel p-4 h-100">
                            <h5 class="fw-bold mb-1">Action Required</h5>
                            <p class="text-muted mb-3">
                                Click an item to view exact records.
                            </p>

                            <div class="alert-item clickable-row"
                                 onclick="location.href='EnrolmentManagement.aspx?status=Pending'">
                                <strong>Pending Enrolments</strong>
                                <div class="text-muted small">
                                    <asp:Label ID="lblPending" runat="server" Text="0" />
                                    enrolments waiting for review.
                                </div>
                            </div>

                            <div class="alert-item clickable-row"
                                 onclick="location.href='EnrolmentManagement.aspx?status=Rejected'">
                                <strong>Rejected Enrolments</strong>
                                <div class="text-muted small">
                                    <asp:Label ID="lblRejected" runat="server" Text="0" />
                                    enrolments rejected this cycle.
                                </div>
                            </div>

                            <div class="alert-item clickable-row"
                                 onclick="location.href='Lecturer-CourseAssignment.aspx'">
                                <strong>Course Assignments</strong>
                                <div class="text-muted small">
                                    <asp:Label ID="lblAssignments" runat="server" Text="0" />
                                    lecturer-course assignments recorded.
                                </div>
                            </div>

                            <div class="alert-item clickable-row"
                                 onclick="location.href='Announcement(Admin).aspx'">
                                <strong>Unread Notifications</strong>
                                <div class="text-muted small">
                                    <asp:Label ID="lblNotifications" runat="server" Text="0" />
                                    system notifications unread.
                                </div>
                            </div>

                        </div>
                    </div>

                </div>

                <div class="glass-panel p-4">
                    <h5 class="fw-bold mb-1">Quick Actions</h5>
                    <p class="text-muted mb-3">
                        Common HOP actions for academic administration.
                    </p>

                    <div class="row g-3">
                        <div class="col-md-2">
                            <a class="action-tile" href="RegisterStudent.aspx">➕ Register Student</a>
                        </div>

                        <div class="col-md-2">
                            <a class="action-tile" href="RegisterLecturer.aspx">➕ Register Lecturer</a>
                        </div>

                        <div class="col-md-2">
                            <a class="action-tile" href="ManageCourses.aspx">📚 Add Course</a>
                        </div>

                        <div class="col-md-2">
                            <a class="action-tile" href="EnrolmentManagement.aspx">📝 Enrolment</a>
                        </div>

                        <div class="col-md-2">
                            <a class="action-tile" href="GPAReport.aspx">📄 GPA Report</a>
                        </div>

                        <div class="col-md-2">
                            <a class="action-tile" href="Announcement(Admin).aspx">📢 Announcement</a>
                        </div>
                    </div>
                </div>

                <asp:Label ID="lblUsers" runat="server" Text="0" Visible="false" />
                <asp:Label ID="lblApproved" runat="server" Text="0" Visible="false" />
                <asp:Label ID="lblAttendance" runat="server" Text="0" Visible="false" />
                <asp:Label ID="lblMarks" runat="server" Text="0" Visible="false" />

            </div>

        </div>
    </div>

</form>
</body>
</html>