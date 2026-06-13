<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AttendanceManagement.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.AttendanceManagement" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Attendance Management</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <asp:HiddenField ID="hfAttendanceID" runat="server" />

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
            <h2 class="fw-bold mb-1">Attendance Management</h2>
            <p class="text-muted mb-0">
                Record and monitor student attendance for approved course enrolments.
            </p>
        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Record Attendance</h5>
                <div class="mini-help">
                    Attendance should only be recorded for approved enrolments and active lecturers.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4">

                    <div class="col-12">
                        <h6 class="fw-bold text-primary mb-0">Class Attendance Details</h6>
                        <hr class="mt-2" />
                    </div>

                    <div class="col-md-5">
                        <label class="form-label">Enrolment</label>
                        <asp:DropDownList ID="ddlEnrolment" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Lecturer</label>
                        <asp:DropDownList ID="ddlLecturer" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Class Date</label>
                        <asp:TextBox ID="txtDate" runat="server"
                            CssClass="form-control"
                            TextMode="Date" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Attendance Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="Present" Value="Present" />
                            <asp:ListItem Text="Absent" Value="Absent" />
                            <asp:ListItem Text="Late" Value="Late" />
                            <asp:ListItem Text="Excused" Value="Excused" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-9">
                        <label class="form-label">Remarks</label>
                        <asp:TextBox ID="txtRemarks" runat="server"
                            CssClass="form-control"
                            placeholder="Optional remarks, example: Medical leave / late by 10 minutes" />
                    </div>

                    <div class="col-12 mt-4">
                        <asp:Button ID="btnAdd" runat="server"
                            Text="Add Attendance"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnAdd_Click" />

                        <asp:Button ID="btnUpdate" runat="server"
                            Text="Update Attendance"
                            CssClass="btn btn-ios-warning ms-2"
                            OnClick="btnUpdate_Click" />

                        <asp:Button ID="btnDelete" runat="server"
                            Text="Delete Record"
                            CssClass="btn btn-ios-danger ms-2"
                            OnClick="btnDelete_Click" />

                        <asp:Button ID="btnClear" runat="server"
                            Text="Clear Form"
                            CssClass="btn btn-ios-light ms-2"
                            OnClick="btnClear_Click" />
                    </div>

                    <div class="col-12">
                        <asp:Label ID="lblMessage" runat="server"
                            CssClass="fw-bold" />
                    </div>

                </div>
            </div>
        </div>

        <div class="glass-card table-card">
            <div class="table-header">
                <h5 class="fw-bold mb-1">Attendance Records</h5>
                <div class="mini-help">
                    NRIC / Passport is masked in the list for privacy. Select a record to update or delete.
                </div>
            </div>

            <div class="p-3">

                <div class="row mb-3 g-3 align-items-end">

                    <div class="col-md-4">
                        <label class="form-label">Search</label>
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Search matric, student, NRIC, course or lecturer" />
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

    </div>

</form>
</body>
</html>