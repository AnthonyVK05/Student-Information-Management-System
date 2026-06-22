<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyAttendance.aspx.cs" Inherits="Student_Information_Management_System.STUDENT.MyAttendance" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MyAttendance</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />

</head>
<body>
    <form id="form1" runat="server">

        <asp:HiddenField ID="hfAttendanceID" runat="server" />

        <nav class="navbar topbar px-4 py-3 no-print">
        <span class="navbar-brand fw-bold text-dark">
            <span class="blue-dot"></span>SIMS College Portal
        </span>

        <a href="AdminDashboard.aspx" class="btn btn-ios-light btn-sm">
            Back to Dashboard
        </a>
    </nav>

    <div class="container-fluid page-shell mt-4 mb-5 px-4">

        <div class="glass-card hero-card mb-4 no-print">
            <h2 class="fw-bold mb-1">Attendance</h2>
        </div>

                <!-- Attendance Percentage Cards -->
        <div class="row mb-4">
            <div class="col-md-3">
                <div class="card text-white bg-primary">
                    <div class="card-body">
                        <h6 class="card-title">Total Classes</h6>
                        <h2 class="card-text"><asp:Label ID="lblTotalClasses" runat="server" Text="0" /></h2>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card text-white bg-success">
                    <div class="card-body">
                        <h6 class="card-title">Present</h6>
                        <h2 class="card-text"><asp:Label ID="lblPresent" runat="server" Text="0" /></h2>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card text-white bg-warning">
                    <div class="card-body">
                        <h6 class="card-title">Late</h6>
                        <h2 class="card-text"><asp:Label ID="lblLate" runat="server" Text="0" /></h2>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card text-white bg-danger">
                    <div class="card-body">
                        <h6 class="card-title">Absent</h6>
                        <h2 class="card-text"><asp:Label ID="lblAbsent" runat="server" Text="0" /></h2>
                    </div>
                </div>
            </div>
        </div>

        <!-- Attendance Percentage Progress Bar -->
        <div class="card shadow-sm mb-4">
            <div class="card-header bg-dark text-white">
                <div class="row">
                    <div class="col-md-6">
                        <span>Attendance Percentage</span>
                    </div>
                    <div class="col-md-6 text-end">
                        <asp:Label ID="lblAttendancePercentage" runat="server" CssClass="fw-bold" Text="0%" />
                    </div>
                </div>
            </div>
            <div class="card-body">
                <div class="progress" style="height: 30px;">
                    <asp:Label ID="lblProgressBar" runat="server" 
                        CssClass="progress-bar fw-bold d-flex align-items-center justify-content-center"
                        Style="width: 0%; background-color: #dc3545;" 
                        Text="0%" />
                </div>
                <div class="row mt-2 text-center">
                    <div class="col-md-6 text-start">
                        <small class="text-muted">Excused: <asp:Label ID="lblExcused" runat="server" Text="0" /></small>
                    </div>
                    <div class="col-md-6 text-end">
                        <small class="text-muted">Required: 80% to pass</small>
                    </div>
                </div>
            </div>
        </div>

        <div class="container mt-4">
    <div class="card shadow-sm">
        <div class="card-header bg-dark text-white">
            Attendance Records
        </div>
                <div class="row mb-3 g-3 align-items-end">

                    <div class="col-md-4">
                        <label class="form-label">Search</label>
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Search matric, student, NRIC, course or lecturer" />
                    </div>

                    <!-- Course Filter Dropdown -->
                    <div class="col-md-2">
                        <label class="form-label">Course</label>
                        <asp:DropDownList ID="ddlCourseFilter" runat="server"
                            CssClass="form-select"
                            AppendDataBoundItems="True">
                            <asp:ListItem Text="All Courses" Value="All" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Attendance Status</label>
                        <asp:DropDownList ID="ddlSearchStatus" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="All Status" Value="All" />
                            <asp:ListItem Text="Present" Value="Present" />
                            <asp:ListItem Text="Absent" Value="Absent" />
                            <asp:ListItem Text="Late" Value="Late" />
                            <asp:ListItem Text="Excused" Value="Excused" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Class Date</label>
                        <asp:TextBox ID="txtSearchDate" runat="server"
                            CssClass="form-control"
                            TextMode="Date" />
                    </div>

                    <div class="col-md-2">
                        <asp:Button ID="btnSearch" runat="server"
                            Text="Search"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnSearch_Click" />

                        <asp:Button ID="btnResetSearch" runat="server"
                            Text="Reset"
                            CssClass="btn btn-ios-light ms-2"
                            OnClick="btnResetSearch_Click" />
                    </div>

                </div>

                <div class="table-responsive">

                    <asp:GridView ID="gvAttendance" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="attendance_id"
                        GridLines="None"
                        OnSelectedIndexChanged="gvAttendance_SelectedIndexChanged">

                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Edit" />

                            <asp:BoundField DataField="attendance_id" HeaderText="ID" />
                            <asp:BoundField DataField="matric_number" HeaderText="Matric No" />

                            <asp:TemplateField HeaderText="NRIC / Passport">
                                <ItemTemplate>
                                    <%# MaskNRIC(Eval("nric_passport")) %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="student_name" HeaderText="Student" />
                            <asp:BoundField DataField="programme_name" HeaderText="Programme" />
                            <asp:BoundField DataField="course_code" HeaderText="Course Code" />
                            <asp:BoundField DataField="course_name" HeaderText="Course" />
                            <asp:BoundField DataField="enrolment_status" HeaderText="Enrolment Status" />
                            <asp:BoundField DataField="lecturer_name" HeaderText="Lecturer" />
                            <asp:BoundField DataField="class_date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="status" HeaderText="Attendance" />
                            <asp:BoundField DataField="remarks" HeaderText="Remarks" />
                        </Columns>

                    </asp:GridView>

                </div>
    </div>
</div>

    <</div>
    </form>
</body>
</html>
