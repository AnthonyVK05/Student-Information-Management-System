<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AnalyticsDashboard.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.AnalyticsDashboard" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>SIMS Analytics Dashboard</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <nav class="navbar topbar px-4 py-3">
        <span class="navbar-brand fw-bold text-dark">
            <span class="blue-dot"></span>SIMS College Portal
        </span>

        <a href="AdminDashboard.aspx" class="btn btn-ios-light btn-sm">
            Back to Dashboard
        </a>
    </nav>

    <div class="container-fluid page-shell mt-4 mb-5 px-4">

        <div class="glass-card hero-card mb-4">
            <div class="d-flex justify-content-between align-items-start flex-wrap gap-3">
                <div>
                    <span class="pill">Head of Programme</span>
                    <h2 class="fw-bold mt-3 mb-1">Academic Analytics Dashboard</h2>
                    <p class="text-muted mb-0">
                        Monitor CGPA performance, enrolment health, attendance trends and programme statistics.
                    </p>
                </div>

                <div class="d-flex gap-2">
                    <a href="RiskStudents.aspx" class="btn btn-ios-danger">
                        View Risk Students
                    </a>

                    <a href="GPAReport.aspx" class="btn btn-ios-primary">
                        GPA / CGPA Report
                    </a>
                </div>
            </div>
        </div>

        <div class="row g-4 mb-4">

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">Average CGPA</div>
                    <div class="summary-value">
                        <asp:Label ID="lblAverageCGPA" runat="server" Text="0.00" />
                    </div>
                    <div class="mini-help">Overall student academic performance</div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">Risk Students</div>
                    <div class="summary-value text-danger">
                        <asp:Label ID="lblRiskStudents" runat="server" Text="0" />
                    </div>
                    <div class="mini-help">Students below academic requirement</div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">Average Attendance</div>
                    <div class="summary-value text-primary">
                        <asp:Label ID="lblAverageAttendance" runat="server" Text="0%" />
                    </div>
                    <div class="mini-help">Calculated from attendance records</div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">Approved Enrolments</div>
                    <div class="summary-value text-success">
                        <asp:Label ID="lblApprovedEnrolments" runat="server" Text="0" />
                    </div>
                    <div class="mini-help">Current approved course enrolments</div>
                </div>
            </div>

        </div>

        <div class="row g-4 mb-4">

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">Active Students</div>
                    <div class="summary-value">
                        <asp:Label ID="lblActiveStudents" runat="server" Text="0" />
                    </div>
                    <div class="mini-help">Students with active academic status</div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">Dropped Students</div>
                    <div class="summary-value text-danger">
                        <asp:Label ID="lblDroppedStudents" runat="server" Text="0" />
                    </div>
                    <div class="mini-help">Students no longer active</div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">Verified Accounts</div>
                    <div class="summary-value text-success">
                        <asp:Label ID="lblVerifiedAccounts" runat="server" Text="0" />
                    </div>
                    <div class="mini-help">Verified student and lecturer emails</div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">Pending Verification</div>
                    <div class="summary-value text-warning">
                        <asp:Label ID="lblPendingVerification" runat="server" Text="0" />
                    </div>
                    <div class="mini-help">Accounts waiting for email verification</div>
                </div>
            </div>

        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Analytics Filter</h5>
                <div class="mini-help">
                    Filter analytics records by academic year, semester and programme.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4 align-items-end">

                    <div class="col-md-3">
                        <label class="form-label">Academic Year</label>
                        <asp:DropDownList ID="ddlAcademicYear" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="All Academic Years" Value="All" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Semester</label>
                        <asp:DropDownList ID="ddlSemester" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="All Semesters" Value="All" />
                            <asp:ListItem Text="Semester 1" Value="Semester 1" />
                            <asp:ListItem Text="Semester 2" Value="Semester 2" />
                            <asp:ListItem Text="Semester 3" Value="Semester 3" />
                            <asp:ListItem Text="Short Semester" Value="Short Semester" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Programme</label>
                        <asp:DropDownList ID="ddlProgramme" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-3">
                        <asp:Button ID="btnFilter" runat="server"
                            Text="Apply Filter"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnFilter_Click" />

                        <asp:Button ID="btnReset" runat="server"
                            Text="Reset"
                            CssClass="btn btn-ios-light ms-2"
                            OnClick="btnReset_Click" />
                    </div>

                    <div class="col-12">
                        <asp:Label ID="lblMessage" runat="server"
                            CssClass="fw-bold" />
                    </div>

                </div>
            </div>
        </div>

        <div class="row g-4 mb-4">

            <div class="col-lg-6">
                <div class="glass-card table-card h-100">
                    <div class="table-header">
                        <h5 class="fw-bold mb-1">Students by Programme</h5>
                        <div class="mini-help">
                            Shows active, dropped and total students by programme.
                        </div>
                    </div>

                    <div class="p-3">
                        <div class="table-responsive">
                            <asp:GridView ID="gvProgrammeStats" runat="server"
                                CssClass="table table-bordered table-hover"
                                AutoGenerateColumns="False"
                                GridLines="None">

                                <Columns>
                                    <asp:BoundField DataField="programme_name" HeaderText="Programme" />
                                    <asp:BoundField DataField="active_students" HeaderText="Active" />
                                    <asp:BoundField DataField="dropped_students" HeaderText="Dropped" />
                                    <asp:BoundField DataField="total_students" HeaderText="Total" />
                                    <asp:BoundField DataField="average_cgpa" HeaderText="Avg CGPA" DataFormatString="{0:0.00}" />
                                </Columns>

                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div class="glass-card table-card h-100">
                    <div class="table-header">
                        <h5 class="fw-bold mb-1">Course Enrolment Statistics</h5>
                        <div class="mini-help">
                            Shows approved, pending, rejected and dropped enrolments by course.
                        </div>
                    </div>

                    <div class="p-3">
                        <div class="table-responsive">
                            <asp:GridView ID="gvCourseStats" runat="server"
                                CssClass="table table-bordered table-hover"
                                AutoGenerateColumns="False"
                                GridLines="None">

                                <Columns>
                                    <asp:BoundField DataField="course_code" HeaderText="Code" />
                                    <asp:BoundField DataField="course_name" HeaderText="Course" />
                                    <asp:BoundField DataField="credit_hours" HeaderText="Credits" />
                                    <asp:BoundField DataField="approved_count" HeaderText="Approved" />
                                    <asp:BoundField DataField="pending_count" HeaderText="Pending" />
                                    <asp:BoundField DataField="rejected_count" HeaderText="Rejected" />
                                    <asp:BoundField DataField="dropped_count" HeaderText="Dropped" />
                                    <asp:BoundField DataField="total_enrolments" HeaderText="Total" />
                                </Columns>

                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <div class="row g-4 mb-4">

            <div class="col-lg-6">
                <div class="glass-card table-card h-100">
                    <div class="table-header">
                        <h5 class="fw-bold mb-1">Grade Distribution</h5>
                        <div class="mini-help">
                            Based on completed course marks.
                        </div>
                    </div>

                    <div class="p-3">
                        <div class="table-responsive">
                            <asp:GridView ID="gvGradeDistribution" runat="server"
                                CssClass="table table-bordered table-hover"
                                AutoGenerateColumns="False"
                                GridLines="None">

                                <Columns>
                                    <asp:BoundField DataField="grade" HeaderText="Grade" />
                                    <asp:BoundField DataField="student_count" HeaderText="Count" />
                                    <asp:BoundField DataField="percentage" HeaderText="Percentage" DataFormatString="{0:0.00}%" />
                                </Columns>

                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-lg-6">
                <div class="glass-card table-card h-100">
                    <div class="table-header">
                        <h5 class="fw-bold mb-1">Attendance Status Summary</h5>
                        <div class="mini-help">
                            Present, absent, late and excused attendance breakdown.
                        </div>
                    </div>

                    <div class="p-3">
                        <div class="table-responsive">
                            <asp:GridView ID="gvAttendanceStats" runat="server"
                                CssClass="table table-bordered table-hover"
                                AutoGenerateColumns="False"
                                GridLines="None">

                                <Columns>
                                    <asp:BoundField DataField="attendance_status" HeaderText="Status" />
                                    <asp:BoundField DataField="total_records" HeaderText="Records" />
                                    <asp:BoundField DataField="percentage" HeaderText="Percentage" DataFormatString="{0:0.00}%" />
                                </Columns>

                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <div class="glass-card table-card">
            <div class="table-header">
                <h5 class="fw-bold mb-1">At-Risk Student Overview</h5>
                <div class="mini-help">
                    Shows students with low CGPA or poor attendance percentage.
                </div>
            </div>

            <div class="p-3">
                <div class="table-responsive">
                    <asp:GridView ID="gvRiskOverview" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        GridLines="None">

                        <Columns>
                            <asp:BoundField DataField="matric_number" HeaderText="Matric No" />
                            <asp:BoundField DataField="student_name" HeaderText="Student" />
                            <asp:BoundField DataField="programme_name" HeaderText="Programme" />
                            <asp:BoundField DataField="cgpa" HeaderText="CGPA" DataFormatString="{0:0.00}" />
                            <asp:BoundField DataField="attendance_percentage" HeaderText="Attendance %" DataFormatString="{0:0.00}%" />
                            <asp:BoundField DataField="risk_level" HeaderText="Risk Level" />
                        </Columns>

                    </asp:GridView>
                </div>
            </div>
        </div>

    </div>

</form>
</body>
</html>