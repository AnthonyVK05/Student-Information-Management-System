<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="CourseMarks.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.CourseMarks" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Course Marks Management</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <asp:HiddenField ID="hfMarkID" runat="server" />

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
            <h2 class="fw-bold mb-1">Course Marks Management</h2>
            <p class="text-muted mb-0">
                Enter coursework and final exam marks. Total marks, grade and grade point are calculated automatically.
            </p>
        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Enter Student Course Marks</h5>
                <div class="mini-help">
                    Marks are recorded only for approved enrolments. Coursework is 40 marks and final exam is 60 marks.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4">

                    <div class="col-12">
                        <h6 class="fw-bold text-primary mb-0">Student & Course Information</h6>
                        <hr class="mt-2" />
                    </div>

                    <div class="col-md-12">
                        <label class="form-label">Approved Enrolment</label>
                        <asp:DropDownList ID="ddlEnrolment" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                        <small class="text-muted">
                            Format: Matric Number - Student Name - Course Code - Course Name
                        </small>
                    </div>

                    <div class="col-12 mt-3">
                        <h6 class="fw-bold text-primary mb-0">Assessment Marks</h6>
                        <hr class="mt-2" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Coursework Marks / 40</label>
                        <asp:TextBox ID="txtCoursework" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            placeholder="0 - 40" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Final Exam Marks / 60</label>
                        <asp:TextBox ID="txtFinal" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            placeholder="0 - 60" />
                    </div>

                    <div class="col-md-2">
                        <label class="form-label">Total / 100</label>
                        <asp:TextBox ID="txtTotal" runat="server"
                            CssClass="form-control"
                            ReadOnly="true"
                            placeholder="Auto" />
                    </div>

                    <div class="col-md-2">
                        <label class="form-label">Grade</label>
                        <asp:TextBox ID="txtGrade" runat="server"
                            CssClass="form-control"
                            ReadOnly="true"
                            placeholder="Auto" />
                    </div>

                    <div class="col-md-2">
                        <label class="form-label">Grade Point</label>
                        <asp:TextBox ID="txtGradePoint" runat="server"
                            CssClass="form-control"
                            ReadOnly="true"
                            placeholder="Auto" />
                    </div>

                    <div class="col-12">
                        <div class="alert alert-light border mb-0">
                            GPA calculation follows:
                            <strong>GPA = SUM(Grade Point × Credit Hours) / SUM(Credit Hours)</strong>.
                            This page stores grade point for each completed course.
                        </div>
                    </div>

                    <div class="col-12 mt-4">
                        <asp:Button ID="btnAdd" runat="server"
                            Text="Add Marks"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnAdd_Click" />

                        <asp:Button ID="btnUpdate" runat="server"
                            Text="Update Marks"
                            CssClass="btn btn-ios-warning ms-2"
                            OnClick="btnUpdate_Click" />

                        <asp:Button ID="btnDelete" runat="server"
                            Text="Delete Record"
                            CssClass="btn btn-ios-danger ms-2"
                            OnClick="btnDelete_Click"
                            OnClientClick="return confirm('Are you sure you want to delete this marks record? This action cannot be undone.');" />

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
                <h5 class="fw-bold mb-1">Course Marks Records</h5>
                <div class="mini-help">
                    NRIC / Passport is masked for privacy. Select a record to update or delete.
                </div>
            </div>

            <div class="p-3">

                <div class="row mb-3 g-3 align-items-end">

                    <div class="col-md-4">
                        <label class="form-label">Search</label>
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Search matric, student, NRIC, programme or course" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Grade Filter</label>
                        <asp:DropDownList ID="ddlGradeFilter" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="All Grades" Value="All" />
                            <asp:ListItem Text="A" Value="A" />
                            <asp:ListItem Text="A-" Value="A-" />
                            <asp:ListItem Text="B+" Value="B+" />
                            <asp:ListItem Text="B" Value="B" />
                            <asp:ListItem Text="B-" Value="B-" />
                            <asp:ListItem Text="C+" Value="C+" />
                            <asp:ListItem Text="C" Value="C" />
                            <asp:ListItem Text="D" Value="D" />
                            <asp:ListItem Text="F" Value="F" />
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

                    <asp:GridView ID="gvMarks" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="mark_id"
                        GridLines="None"
                        OnSelectedIndexChanged="gvMarks_SelectedIndexChanged">

                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Edit" />

                            <asp:BoundField DataField="mark_id" HeaderText="ID" />
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
                            <asp:BoundField DataField="credit_hours" HeaderText="Credits" />
                            <asp:BoundField DataField="academic_year" HeaderText="Academic Year" />
                            <asp:BoundField DataField="semester" HeaderText="Semester" />
                            <asp:BoundField DataField="coursework_marks" HeaderText="Coursework" DataFormatString="{0:0.00}" />
                            <asp:BoundField DataField="final_exam_marks" HeaderText="Final Exam" DataFormatString="{0:0.00}" />
                            <asp:BoundField DataField="total_marks" HeaderText="Total" DataFormatString="{0:0.00}" />
                            <asp:BoundField DataField="grade" HeaderText="Grade" />
                            <asp:BoundField DataField="grade_point" HeaderText="Grade Point" DataFormatString="{0:0.00}" />
                        </Columns>

                    </asp:GridView>

                </div>
            </div>
        </div>

    </div>

</form>
</body>
</html>