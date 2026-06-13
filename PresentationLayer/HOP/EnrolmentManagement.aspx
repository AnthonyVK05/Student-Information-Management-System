<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="EnrolmentManagement.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.EnrolmentManagement" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Enrolment Management</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <asp:HiddenField ID="hfEnrolmentID" runat="server" />

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
            <h2 class="fw-bold mb-1">Student Course Enrolment</h2>
            <p class="text-muted mb-0">
                Enrol active students into programme-related courses and manage enrolment status.
            </p>
        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Enrolment Form</h5>
                <div class="mini-help">
                    Courses are filtered automatically based on the selected student's programme.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4">

                    <div class="col-12">
                        <h6 class="fw-bold text-primary mb-0">Student & Course Information</h6>
                        <hr class="mt-2" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Student</label>
                        <asp:DropDownList ID="ddlStudent" runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlStudent_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Course</label>
                        <asp:DropDownList ID="ddlCourse" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Academic Calendar</label>
                        <asp:DropDownList ID="ddlCalendar" runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlCalendar_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 mt-3">
                        <h6 class="fw-bold text-primary mb-0">Academic Session</h6>
                        <hr class="mt-2" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Semester</label>
                        <asp:TextBox ID="txtSemester" runat="server"
                            CssClass="form-control"
                            placeholder="Example: Semester 1" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Academic Year</label>
                        <asp:TextBox ID="txtAcademicYear" runat="server"
                            CssClass="form-control"
                            placeholder="Example: 2026/2027" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Enrol Date</label>
                        <asp:TextBox ID="txtEnrolDate" runat="server"
                            CssClass="form-control"
                            TextMode="Date" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="Pending" Value="Pending" />
                            <asp:ListItem Text="Approved" Value="Approved" />
                            <asp:ListItem Text="Rejected" Value="Rejected" />
                            <asp:ListItem Text="Dropped" Value="Dropped" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 mt-4">
                        <asp:Button ID="btnAdd" runat="server"
                            Text="Add Enrolment"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnAdd_Click" />

                        <asp:Button ID="btnUpdate" runat="server"
                            Text="Update Enrolment"
                            CssClass="btn btn-ios-warning ms-2"
                            OnClick="btnUpdate_Click" />

                        <asp:Button ID="btnApprove" runat="server"
                            Text="Approve"
                            CssClass="btn btn-success ms-2"
                            OnClick="btnApprove_Click" />

                        <asp:Button ID="btnReject" runat="server"
                            Text="Reject"
                            CssClass="btn btn-danger ms-2"
                            OnClick="btnReject_Click" />

                        <asp:Button ID="btnDrop" runat="server"
                            Text="Drop Enrolment"
                            CssClass="btn btn-dark ms-2"
                            OnClick="btnDrop_Click" />

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
                        <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold" />
                    </div>

                </div>
            </div>
        </div>

        <div class="glass-card table-card">
            <div class="table-header">
                <h5 class="fw-bold mb-1">Enrolment Records</h5>
                <div class="mini-help">
                    NRIC / Passport is masked in the list for privacy. Select a record to manage status.
                </div>
            </div>

            <div class="p-3">

                <div class="row mb-3 g-3 align-items-end">

                    <div class="col-md-4">
                        <label class="form-label">Search</label>
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Search matric, name, NRIC, course" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Enrolment Status</label>
                        <asp:DropDownList ID="ddlSearchStatus" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="All Status" Value="All" />
                            <asp:ListItem Text="Pending" Value="Pending" />
                            <asp:ListItem Text="Approved" Value="Approved" />
                            <asp:ListItem Text="Rejected" Value="Rejected" />
                            <asp:ListItem Text="Dropped" Value="Dropped" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-5">
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

                    <asp:GridView ID="gvEnrolments"
                        runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="enrolment_id"
                        GridLines="None"
                        OnSelectedIndexChanged="gvEnrolments_SelectedIndexChanged">

                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Edit" />

                            <asp:BoundField DataField="enrolment_id" HeaderText="ID" />
                            <asp:BoundField DataField="matric_number" HeaderText="Matric No" />

                            <asp:TemplateField HeaderText="NRIC / Passport">
                                <ItemTemplate>
                                    <%# MaskNRIC(Eval("nric_passport")) %>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="student_name" HeaderText="Student" />
                            <asp:BoundField DataField="programme_name" HeaderText="Programme" />
                            <asp:BoundField DataField="student_status" HeaderText="Student Status" />
                            <asp:BoundField DataField="verification_status" HeaderText="Email Verification" />
                            <asp:BoundField DataField="course_code" HeaderText="Course Code" />
                            <asp:BoundField DataField="course_name" HeaderText="Course" />
                            <asp:BoundField DataField="credit_hours" HeaderText="Credits" />
                            <asp:BoundField DataField="calendar_name" HeaderText="Calendar" />
                            <asp:BoundField DataField="semester" HeaderText="Semester" />
                            <asp:BoundField DataField="academic_year" HeaderText="Academic Year" />
                            <asp:BoundField DataField="enrol_date" HeaderText="Enrol Date" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="status" HeaderText="Enrolment Status" />
                        </Columns>

                    </asp:GridView>

                </div>
            </div>
        </div>

    </div>

</form>
</body>
</html>