<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Lecturer-CourseAssignment.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.Lecturer_CourseAssignment" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Lecturer Course Assignment</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <asp:HiddenField ID="hfLecturerID" runat="server" />
    <asp:HiddenField ID="hfProgrammeID" runat="server" />

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
            <h2 class="fw-bold mb-1">Lecturer Course Assignment</h2>
            <p class="text-muted mb-0">
                Assign multiple programme courses to a lecturer for an academic semester.
            </p>
        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Assignment Details</h5>
                <div class="mini-help">
                    Select programme first, then assign one or more courses to the lecturer.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4">

                    <div class="col-md-4">
                        <label class="form-label">Programme</label>
                        <asp:DropDownList ID="ddlProgramme" runat="server"
                            CssClass="form-select"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlProgramme_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Lecturer</label>
                        <asp:DropDownList ID="ddlLecturer" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-2">
                        <label class="form-label">Academic Year</label>
                        <asp:TextBox ID="txtAcademicYear" runat="server"
                            CssClass="form-control"
                            placeholder="2026/2027" />
                    </div>

                    <div class="col-md-2">
                        <label class="form-label">Semester</label>
                        <asp:TextBox ID="txtSemester" runat="server"
                            CssClass="form-control"
                            placeholder="Semester 1" />
                    </div>

                    <div class="col-12">
                        <label class="form-label">Programme Courses</label>

                        <div class="glass-card p-3">
                            <asp:CheckBoxList ID="cblCourses" runat="server"
                                CssClass="course-check-list"
                                RepeatDirection="Vertical">
                            </asp:CheckBoxList>
                        </div>
                    </div>

                    <div class="col-12 mt-4">
                        <asp:Button ID="btnAssign" runat="server"
                            Text="Assign Selected Courses"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnAssign_Click" />

                        <asp:Button ID="btnRemoveSelected" runat="server"
                            Text="Remove Selected Courses"
                            CssClass="btn btn-ios-danger ms-2"
                            OnClick="btnRemoveSelected_Click" />

                        <asp:Button ID="btnClear" runat="server"
                            Text="Clear Form"
                            CssClass="btn btn-ios-light ms-2"
                            OnClick="btnClear_Click" />
                    </div>

                    <div class="col-12">
                        <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold"></asp:Label>
                    </div>

                </div>
            </div>
        </div>

        <div class="glass-card table-card">
            <div class="table-header">
                <h5 class="fw-bold mb-1">Grouped Lecturer Course Assignments</h5>
                <div class="mini-help">
                    Each lecturer appears once per programme, academic year and semester.
                </div>
            </div>

            <div class="p-3">
                <div class="table-responsive">

                    <asp:GridView ID="gvAssignments"
                        runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        GridLines="None"
                        OnRowCommand="gvAssignments_RowCommand">

                        <Columns>
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:Button ID="btnManage" runat="server"
                                        Text="Manage"
                                        CssClass="btn btn-ios-primary btn-sm"
                                        CommandName="ManageGroup"
                                        CommandArgument='<%# Eval("lecturer_id") + "|" + Eval("programme_id") + "|" + Eval("academic_year") + "|" + Eval("semester") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="lecturer_name" HeaderText="Lecturer" />
                            <asp:BoundField DataField="programme_name" HeaderText="Programme" />
                            <asp:BoundField DataField="academic_year" HeaderText="Academic Year" />
                            <asp:BoundField DataField="semester" HeaderText="Semester" />
                            <asp:BoundField DataField="assigned_courses" HeaderText="Assigned Courses" />
                            <asp:BoundField DataField="course_count" HeaderText="No. of Courses" />
                        </Columns>

                    </asp:GridView>

                </div>
            </div>
        </div>

    </div>

</form>
</body>
</html>