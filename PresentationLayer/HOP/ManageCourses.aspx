<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ManageCourses.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.ManageCourses" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Manage Courses</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <asp:HiddenField ID="hfCourseID" runat="server" />

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
            <h2 class="fw-bold mb-1">Manage Courses</h2>
            <p class="text-muted mb-0">
                Create, update and manage programme courses.
            </p>
        </div>

        <div class="glass-card form-card mb-4">

            <div class="form-header">
                <h5 class="fw-bold mb-1">Course Information</h5>
                <div class="mini-help">
                    Assign courses under programmes and manage course details.
                </div>
            </div>

            <div class="form-body">

                <div class="row g-4">

                    <div class="col-md-4">
                        <label class="form-label">Programme</label>
                        <asp:DropDownList ID="ddlProgramme" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Course Name</label>
                        <asp:TextBox ID="txtCourseName" runat="server"
                            CssClass="form-control"
                            placeholder="Example: Database Systems" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Course Code</label>
                        <asp:TextBox ID="txtCourseCode" runat="server"
                            CssClass="form-control"
                            placeholder="Example: CSC102" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Credit Hours</label>
                        <asp:TextBox ID="txtCreditHours" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            placeholder="Example: 4" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Semester</label>
                        <asp:DropDownList ID="ddlSemester" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="Semester 1" Value="Semester 1" />
                            <asp:ListItem Text="Semester 2" Value="Semester 2" />
                            <asp:ListItem Text="Semester 3" Value="Semester 3" />
                            <asp:ListItem Text="Short Semester" Value="Short Semester" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="Active" Value="1" />
                            <asp:ListItem Text="Inactive" Value="0" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 mt-4">

                        <asp:Button ID="btnAdd" runat="server"
                            Text="Add Course"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnAdd_Click" />

                        <asp:Button ID="btnUpdate" runat="server"
                            Text="Update Course"
                            CssClass="btn btn-ios-warning ms-2"
                            OnClick="btnUpdate_Click" />

                        <asp:Button ID="btnDelete" runat="server"
                            Text="Deactivate Course"
                            CssClass="btn btn-ios-danger ms-2"
                            OnClick="btnDelete_Click"
                            OnClientClick="return confirm('Are you sure you want to deactivate this course? It will be hidden from active course lists.');" />

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
                <h5 class="fw-bold mb-1">Course List</h5>
                <div class="mini-help">
                    Select a course to edit or deactivate.
                </div>
            </div>

            <div class="p-3">

                <div class="table-responsive">

                    <asp:GridView ID="gvCourses" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="course_id"
                        GridLines="None"
                        OnSelectedIndexChanged="gvCourses_SelectedIndexChanged">

                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Edit" />

                            <asp:BoundField DataField="course_id" HeaderText="ID" />
                            <asp:BoundField DataField="programme_id" HeaderText="Programme ID" Visible="false" />
                            <asp:BoundField DataField="programme_name" HeaderText="Programme" />
                            <asp:BoundField DataField="course_name" HeaderText="Course Name" />
                            <asp:BoundField DataField="course_code" HeaderText="Course Code" />
                            <asp:BoundField DataField="credit_hours" HeaderText="Credit Hours" />
                            <asp:BoundField DataField="semester" HeaderText="Semester" />
                            <asp:BoundField DataField="status_text" HeaderText="Status" />
                        </Columns>

                    </asp:GridView>

                </div>

            </div>

        </div>

    </div>

</form>
</body>
</html>