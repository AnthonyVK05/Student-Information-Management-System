<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ManageProgrammes.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.ManageProgrammes" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Manage Programmes</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

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
            <h2 class="fw-bold mb-1">Manage Programmes</h2>
            <p class="text-muted mb-0">
                Add, update and manage academic programmes offered by the college.
            </p>
        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Programme Information</h5>
                <div class="mini-help">
                    Fill in programme name, code, duration and department.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4">

                    <div class="col-md-6">
                        <label class="form-label">Programme Name</label>
                        <asp:TextBox ID="txtProgrammeName" runat="server"
                            CssClass="form-control"
                            placeholder="Example: Diploma in Computer Science" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Programme Code</label>
                        <asp:TextBox ID="txtCode" runat="server"
                            CssClass="form-control"
                            placeholder="Example: DCS" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Duration Years</label>
                        <asp:TextBox ID="txtDuration" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            placeholder="Example: 3" />
                    </div>

                    <div class="col-md-6">
                        <label class="form-label">Department</label>
                        <asp:TextBox ID="txtDepartment" runat="server"
                            CssClass="form-control"
                            placeholder="Example: Computing" />
                    </div>

                    <div class="col-12 mt-4">
                        <asp:Button ID="btnAdd" runat="server"
                            Text="Add Programme"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnAdd_Click" />

                        <asp:Button ID="btnUpdate" runat="server"
                            Text="Update Programme"
                            CssClass="btn btn-ios-warning ms-2"
                            OnClick="btnUpdate_Click" />

                        <asp:Button ID="btnDelete" runat="server"
                            Text="Delete Programme"
                            CssClass="btn btn-ios-danger ms-2"
                            OnClick="btnDelete_Click" />

                        <asp:Button ID="btnClear" runat="server"
                            Text="Clear Form"
                            CssClass="btn btn-ios-light ms-2"
                            OnClick="btnClear_Click" />
                    </div>

                </div>
            </div>
        </div>

        <div class="glass-card table-card">
            <div class="table-header">
                <h5 class="fw-bold mb-1">Programme List</h5>
                <div class="mini-help">
                    Select a programme from the list to update or delete it.
                </div>
            </div>

            <div class="p-3">
                <div class="table-responsive">

                    <asp:GridView ID="gvProgrammes"
                        runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="programme_id"
                        GridLines="None"
                        OnSelectedIndexChanged="gvProgrammes_SelectedIndexChanged">

                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Edit" />

                            <asp:BoundField DataField="programme_id" HeaderText="ID" />
                            <asp:BoundField DataField="programme_name" HeaderText="Programme Name" />
                            <asp:BoundField DataField="code" HeaderText="Code" />
                            <asp:BoundField DataField="duration_years" HeaderText="Duration Years" />
                            <asp:BoundField DataField="department" HeaderText="Department" />
                        </Columns>

                    </asp:GridView>

                </div>
            </div>
        </div>

    </div>

</form>
</body>
</html>