<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="RiskStudents.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.RiskStudents" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Academic Risk Dashboard</title>

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
            <h2 class="fw-bold mb-1">Student Academic Risk Dashboard</h2>
            <p class="text-muted mb-0">
                Monitor CGPA, attendance and recommended intervention actions.
            </p>
        </div>

        <div class="row g-4 mb-4">

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">Total Students</div>
                    <div class="summary-value">
                        <asp:Label ID="lblTotalStudents" runat="server" Text="0" />
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">High Risk</div>
                    <div class="summary-value text-danger">
                        <asp:Label ID="lblHighRisk" runat="server" Text="0" />
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">Academic Warning</div>
                    <div class="summary-value text-warning">
                        <asp:Label ID="lblWarning" runat="server" Text="0" />
                    </div>
                </div>
            </div>

            <div class="col-md-3">
                <div class="summary-card">
                    <div class="summary-label">Dean's List</div>
                    <div class="summary-value text-success">
                        <asp:Label ID="lblDeansList" runat="server" Text="0" />
                    </div>
                </div>
            </div>

        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Search & Filter</h5>
                <div class="mini-help">
                    Filter students by name, matric number, programme or academic risk level.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4 align-items-end">

                    <div class="col-md-4">
                        <label class="form-label">Search Student</label>
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Name or matric number" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Programme</label>
                        <asp:DropDownList ID="ddlProgramme" runat="server"
                            CssClass="form-select" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Risk Level</label>
                        <asp:DropDownList ID="ddlRiskLevel" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="All Risk Levels" Value="All" />
                            <asp:ListItem Text="High Risk" Value="High Risk" />
                            <asp:ListItem Text="Academic Warning" Value="Academic Warning" />
                            <asp:ListItem Text="Good Standing" Value="Good Standing" />
                            <asp:ListItem Text="Dean's List" Value="Dean's List" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-12">
                        <asp:Button ID="btnFilter" runat="server"
                            Text="Apply Filter"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnFilter_Click" />

                        <asp:Button ID="btnReset" runat="server"
                            Text="Reset"
                            CssClass="btn btn-ios-light ms-2"
                            OnClick="btnReset_Click" />
                    </div>

                </div>
            </div>
        </div>

        <div class="glass-card table-card">
            <div class="table-header">
                <h5 class="fw-bold mb-1">Student Risk List</h5>
                <div class="mini-help">
                    Risk level is based on CGPA and attendance percentage.
                </div>
            </div>

            <div class="p-3">
                <div class="table-responsive">

                    <asp:GridView ID="gvRiskStudents" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        GridLines="None"
                        OnRowCommand="gvRiskStudents_RowCommand">

                        <Columns>
                            <asp:BoundField DataField="matric_number" HeaderText="Matric No" />
                            <asp:BoundField DataField="full_name" HeaderText="Student Name" />
                            <asp:BoundField DataField="programme_name" HeaderText="Programme" />
                            <asp:BoundField DataField="current_semester" HeaderText="Current Semester" />
                            <asp:BoundField DataField="cgpa" HeaderText="CGPA" DataFormatString="{0:0.00}" />
                            <asp:BoundField DataField="attendance_percentage" HeaderText="Attendance %" DataFormatString="{0:0.00}" />
                            <asp:BoundField DataField="risk_level" HeaderText="Risk Level" />
                            <asp:BoundField DataField="recommended_action" HeaderText="Recommended Action" />

                            <asp:TemplateField HeaderText="Transcript">
                                <ItemTemplate>
                                   <asp:Button
                                       ID="btnTranscript"
                                       runat="server"
                                       Text="View Transcript"
                                       CssClass="btn btn-ios-primary btn-sm"
                                       CommandName="ViewTranscript"
                                       CommandArgument='<%# Eval("student_id") %>' />
    
    
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>

                    </asp:GridView>

                </div>
            </div>
        </div>

    </div>

</form>
</body>
</html>