<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AcademicCalendar.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.AcademicCalendar" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Academic Calendar</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <asp:HiddenField ID="hfCalendarID" runat="server" />

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
            <h2 class="fw-bold mb-1">Academic Calendar Management</h2>
            <p class="text-muted mb-0">
                Manage academic year, semester duration, registration period and calendar status.
            </p>
        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Academic Session Setup</h5>
                <div class="mini-help">
                    Select year and semester quickly instead of typing manually.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4">

                    <div class="col-md-3">
                        <label class="form-label">Academic Year</label>
                        <asp:DropDownList ID="ddlAcademicYear" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="2025/2026" Value="2025/2026" />
                            <asp:ListItem Text="2026/2027" Value="2026/2027" />
                            <asp:ListItem Text="2027/2028" Value="2027/2028" />
                            <asp:ListItem Text="2028/2029" Value="2028/2029" />
                            <asp:ListItem Text="2029/2030" Value="2029/2030" />
                        </asp:DropDownList>
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
                        <label class="form-label">Start Date</label>
                        <asp:TextBox ID="txtStartDate" runat="server"
                            CssClass="form-control"
                            TextMode="Date" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">End Date</label>
                        <asp:TextBox ID="txtEndDate" runat="server"
                            CssClass="form-control"
                            TextMode="Date" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Calendar Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="Upcoming" Value="Upcoming" />
                            <asp:ListItem Text="Open" Value="Open" />
                            <asp:ListItem Text="Closed" Value="Closed" />
                            <asp:ListItem Text="Completed" Value="Completed" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-9 d-flex align-items-end">
                        <asp:Button ID="btnAdd" runat="server"
                            Text="Create Calendar"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnAdd_Click" />

                        <asp:Button ID="btnUpdate" runat="server"
                            Text="Update Calendar"
                            CssClass="btn btn-ios-warning ms-2"
                            OnClick="btnUpdate_Click" />

                        <asp:Button ID="btnDelete" runat="server"
                            Text="Delete"
                            CssClass="btn btn-ios-danger ms-2"
                            OnClick="btnDelete_Click" />

                        <asp:Button ID="btnClear" runat="server"
                            Text="Clear"
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
                <h5 class="fw-bold mb-1">Academic Calendar Records</h5>
                <div class="mini-help">
                    Open and upcoming calendars are used in enrolment selection.
                </div>
            </div>

            <div class="p-3">
                <div class="table-responsive">

                    <asp:GridView ID="gvCalendar" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="calendar_id"
                        GridLines="None"
                        OnSelectedIndexChanged="gvCalendar_SelectedIndexChanged">

                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Edit" />

                            <asp:BoundField DataField="calendar_id" HeaderText="ID" />
                            <asp:BoundField DataField="academic_year" HeaderText="Academic Year" />
                            <asp:BoundField DataField="semester" HeaderText="Semester" />
                            <asp:BoundField DataField="start_date" HeaderText="Start Date" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="end_date" HeaderText="End Date" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="status" HeaderText="Status" />
                        </Columns>

                    </asp:GridView>

                </div>
            </div>
        </div>

    </div>

</form>
</body>
</html>