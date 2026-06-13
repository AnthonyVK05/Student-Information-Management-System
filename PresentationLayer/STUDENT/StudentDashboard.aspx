<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="StudentDashboard.aspx.cs"
    Inherits="Student_Information_Management_System.Student.StudentDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Student Dashboard</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />

    <style>
        .student-shell {
            min-height: calc(100vh - 65px);
        }

        .student-card {
            background: rgba(255,255,255,.45);
            backdrop-filter: blur(25px);
            border: 1px solid rgba(255,255,255,.55);
            border-radius: 24px;
            box-shadow: 0 12px 30px rgba(15,23,42,.07);
            padding: 22px;
        }

        .metric-title {
            color: #64748b;
            font-size: 13px;
            font-weight: 700;
        }

        .metric-value {
            font-size: 32px;
            font-weight: 850;
            color: #0f172a;
        }

        .mini-text {
            color: #64748b;
            font-size: 13px;
        }

        .warning-box {
            border-radius: 18px;
            padding: 16px;
            background: rgba(220, 38, 38, .08);
            border: 1px solid rgba(220, 38, 38, .18);
            color: #991b1b;
            font-weight: 700;
        }

        .safe-box {
            border-radius: 18px;
            padding: 16px;
            background: rgba(22, 163, 74, .08);
            border: 1px solid rgba(22, 163, 74, .18);
            color: #166534;
            font-weight: 700;
        }

        .action-tile {
            display: block;
            text-decoration: none;
            color: #0f172a;
            background: rgba(255,255,255,.40);
            border: 1px solid rgba(255,255,255,.55);
            border-radius: 18px;
            padding: 16px;
            font-weight: 700;
            transition: .25s;
            min-height: 70px;
        }

        .action-tile:hover {
            transform: translateY(-3px);
            color: #2563eb;
            box-shadow: 0 12px 28px rgba(37,99,235,.12);
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <nav class="navbar topbar px-4 py-3">
        <span class="navbar-brand fw-bold text-dark">
            <span class="blue-dot"></span>SIMS Student Portal
        </span>

        <asp:Button ID="btnLogout" runat="server"
            Text="Logout"
            CssClass="btn btn-ios-light btn-sm"
            OnClick="btnLogout_Click" />
    </nav>

    <div class="container-fluid student-shell">
        <div class="row">

            <div class="col-md-2 sidebar p-3">

                <div class="section-title">Student</div>
                <a href="StudentDashboard.aspx">Dashboard</a>
                <a href="MyProfile.aspx">My Profile</a>
                <a href="CourseEnrolment.aspx">Course Registration</a>

                <div class="section-title">Academic</div>
                <a href="MyAttendance.aspx">My Attendance</a>
                <a href="MyResults.aspx">My Results</a>
                <a href="AcademicTranscript.aspx">Transcript</a>
                <a href="Performance.aspx">Performance</a>

                <div class="section-title">Services</div>
                <a href="FeeStatement.aspx">Fee Statement</a>
                <a href="Notifications.aspx">Notifications</a>
                <a href="Messages.aspx">Messages</a>
                <a href="Announcements.aspx">Announcements</a>

            </div>

            <div class="col-md-10 p-4">

                <div class="student-card mb-4">
                    <div class="d-flex justify-content-between align-items-start flex-wrap gap-3">
                        <div>
                            <span class="pill">Student Portal</span>
                            <h2 class="fw-bold mt-3 mb-1">
                                Welcome, <asp:Label ID="lblStudentName" runat="server" Text="Student" />
                            </h2>

                            <p class="text-muted mb-0">
                                <asp:Label ID="lblMatric" runat="server" Text="Matric No" />
                                ·
                                <asp:Label ID="lblProgramme" runat="server" Text="Programme" />
                            </p>
                        </div>

                        <div>
                            <a href="MyProfile.aspx" class="btn btn-ios-primary">View Profile</a>
                            <a href="CourseEnrolment.aspx" class="btn btn-ios-light ms-2">Register Course</a>
                        </div>
                    </div>
                </div>

                <asp:Panel ID="pnlWarning" runat="server" CssClass="warning-box mb-4" Visible="false">
                    Academic Warning:
                    Your CGPA or attendance is below the recommended academic requirement.
                    Please contact your lecturer or HOP for guidance.
                </asp:Panel>

                <asp:Panel ID="pnlGoodStanding" runat="server" CssClass="safe-box mb-4" Visible="false">
                    Academic Standing: Good. Keep maintaining your performance and attendance.
                </asp:Panel>

                <div class="row g-4 mb-4">

                    <div class="col-md-3">
                        <div class="student-card">
                            <div class="metric-title">Current CGPA</div>
                            <div class="metric-value">
                                <asp:Label ID="lblCGPA" runat="server" Text="0.00" />
                            </div>
                            <div class="mini-text">Calculated from course marks</div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="student-card">
                            <div class="metric-title">Attendance</div>
                            <div class="metric-value">
                                <asp:Label ID="lblAttendance" runat="server" Text="0%" />
                            </div>
                            <div class="mini-text">Present attendance percentage</div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="student-card">
                            <div class="metric-title">Active Courses</div>
                            <div class="metric-value">
                                <asp:Label ID="lblCourses" runat="server" Text="0" />
                            </div>
                            <div class="mini-text">Approved enrolments</div>
                        </div>
                    </div>

                    <div class="col-md-3">
                        <div class="student-card">
                            <div class="metric-title">Fee Balance</div>
                            <div class="metric-value">
                                RM <asp:Label ID="lblFeeBalance" runat="server" Text="0.00" />
                            </div>
                            <div class="mini-text">Outstanding balance</div>
                        </div>
                    </div>

                </div>

                <div class="row g-4 mb-4">

                    <div class="col-lg-6">
                        <div class="student-card h-100">
                            <h5 class="fw-bold mb-2">Current Courses</h5>
                            <p class="mini-text">Approved enrolled courses for current academic records.</p>

                            <asp:GridView ID="gvCurrentCourses" runat="server"
                                CssClass="table table-bordered table-hover"
                                AutoGenerateColumns="False"
                                GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="course_code" HeaderText="Code" />
                                    <asp:BoundField DataField="course_name" HeaderText="Course" />
                                    <asp:BoundField DataField="credit_hours" HeaderText="Credits" />
                                    <asp:BoundField DataField="semester" HeaderText="Semester" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                    <div class="col-lg-6">
                        <div class="student-card h-100">
                            <h5 class="fw-bold mb-2">Latest Results</h5>
                            <p class="mini-text">Recently published academic marks.</p>

                            <asp:GridView ID="gvLatestResults" runat="server"
                                CssClass="table table-bordered table-hover"
                                AutoGenerateColumns="False"
                                GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="course_code" HeaderText="Code" />
                                    <asp:BoundField DataField="course_name" HeaderText="Course" />
                                    <asp:BoundField DataField="total_marks" HeaderText="Total" DataFormatString="{0:0.00}" />
                                    <asp:BoundField DataField="grade" HeaderText="Grade" />
                                    <asp:BoundField DataField="grade_point" HeaderText="Point" DataFormatString="{0:0.00}" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                </div>

                <div class="row g-4 mb-4">

                    <div class="col-lg-6">
                        <div class="student-card h-100">
                            <h5 class="fw-bold mb-2">Latest Notifications</h5>
                            <p class="mini-text">Academic updates and alerts.</p>

                            <asp:GridView ID="gvNotifications" runat="server"
                                CssClass="table table-bordered table-hover"
                                AutoGenerateColumns="False"
                                GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="type" HeaderText="Type" />
                                    <asp:BoundField DataField="message" HeaderText="Message" />
                                    <asp:BoundField DataField="created_at" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                    <div class="col-lg-6">
                        <div class="student-card h-100">
                            <h5 class="fw-bold mb-2">Announcements</h5>
                            <p class="mini-text">General and course-specific announcements.</p>

                            <asp:GridView ID="gvAnnouncements" runat="server"
                                CssClass="table table-bordered table-hover"
                                AutoGenerateColumns="False"
                                GridLines="None">
                                <Columns>
                                    <asp:BoundField DataField="title" HeaderText="Title" />
                                    <asp:BoundField DataField="course_name" HeaderText="Course" />
                                    <asp:BoundField DataField="posted_at" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>

                </div>

                <div class="student-card">
                    <h5 class="fw-bold mb-2">Quick Actions</h5>
                    <p class="mini-text">Access your main academic services quickly.</p>

                    <div class="row g-3">
                        <div class="col-md-2">
                            <a class="action-tile" href="CourseEnrolment.aspx">Course Registration</a>
                        </div>

                        <div class="col-md-2">
                            <a class="action-tile" href="MyAttendance.aspx">Attendance</a>
                        </div>

                        <div class="col-md-2">
                            <a class="action-tile" href="MyResults.aspx">Results</a>
                        </div>

                        <div class="col-md-2">
                            <a class="action-tile" href="AcademicTranscript.aspx">Transcript</a>
                        </div>

                        <div class="col-md-2">
                            <a class="action-tile" href="FeeStatement.aspx">Fees</a>
                        </div>

                        <div class="col-md-2">
                            <a class="action-tile" href="Messages.aspx">Messages</a>
                        </div>
                    </div>
                </div>

            </div>

        </div>
    </div>

</form>
</body>
</html>