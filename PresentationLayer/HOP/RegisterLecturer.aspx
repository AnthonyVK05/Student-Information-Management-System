<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="RegisterLecturer.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.RegisterLecturer" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Lecturer Management</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <asp:HiddenField ID="hfLecturerID" runat="server" />
    <asp:HiddenField ID="hfUserID" runat="server" />

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
            <h2 class="fw-bold mb-1">Lecturer Management</h2>
            <p class="text-muted mb-0">
                Register lecturers, update staff information, manage account status and resend email verification.
            </p>
        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Lecturer Registration / Update Form</h5>
                <div class="mini-help">
                    Staff ID is generated automatically. Email verification is required before login.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4">

                    <div class="col-12">
                        <h6 class="fw-bold text-primary mb-0">Login Account Details</h6>
                        <hr class="mt-2" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Username</label>
                        <asp:TextBox ID="txtUsername" runat="server"
                            CssClass="form-control"
                            placeholder="Example: lecturer001" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Password</label>
                        <asp:TextBox ID="txtPassword" runat="server"
                            CssClass="form-control"
                            TextMode="Password"
                            placeholder="Example: Lect@1234" />

                        <small class="text-muted">
                            Minimum 8 characters, uppercase, lowercase, number and special character.
                        </small>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Email Address</label>
                        <asp:TextBox ID="txtEmail" runat="server"
                            CssClass="form-control"
                            TextMode="Email"
                            placeholder="lecturer@email.com" />
                    </div>

                    <div class="col-12 mt-3">
                        <h6 class="fw-bold text-primary mb-0">Lecturer Profile Details</h6>
                        <hr class="mt-2" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Full Name</label>
                        <asp:TextBox ID="txtFullName" runat="server"
                            CssClass="form-control"
                            placeholder="Enter lecturer full name" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Staff ID</label>
                        <asp:TextBox ID="txtStaffID" runat="server"
                            CssClass="form-control"
                            ReadOnly="true"
                            placeholder="Auto Generated" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Specialisation</label>
                        <asp:TextBox ID="txtSpecialisation" runat="server"
                            CssClass="form-control"
                            placeholder="Example: Database Systems" />
                    </div>

                    <div class="col-md-2">
                        <label class="form-label">Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="Active" Value="Active" />
                            <asp:ListItem Text="Inactive" Value="Inactive" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 mt-4">
                        <asp:Button ID="btnRegister" runat="server"
                            Text="Register Lecturer"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnRegister_Click" />

                        <asp:Button ID="btnUpdate" runat="server"
                            Text="Update Lecturer"
                            CssClass="btn btn-ios-warning ms-2"
                            OnClick="btnUpdate_Click" />

                        <asp:Button ID="btnDeactivate" runat="server"
                            Text="Deactivate"
                            CssClass="btn btn-ios-danger ms-2"
                            OnClick="btnDeactivate_Click" />

                        <asp:Button ID="btnReactivate" runat="server"
                            Text="Reactivate"
                            CssClass="btn btn-ios-primary ms-2"
                            OnClick="btnReactivate_Click" />

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

        <div class="glass-card form-card mb-4">
            <div class="form-body">
                <div class="row g-3 align-items-end">

                    <div class="col-md-6">
                        <label class="form-label">Search Lecturer</label>
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Search by staff ID, name, username, email or specialisation" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Verification Filter</label>
                        <asp:DropDownList ID="ddlVerificationFilter" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="All" Value="All" />
                            <asp:ListItem Text="Verified" Value="Verified" />
                            <asp:ListItem Text="Pending Verification" Value="Pending Verification" />
                            <asp:ListItem Text="Expired" Value="Expired" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-3">
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
            </div>
        </div>

        <div class="glass-card table-card">
            <div class="table-header">
                <h5 class="fw-bold mb-1">Registered Lecturers</h5>
                <div class="mini-help">
                    View account verification status and resend verification email if needed.
                </div>
            </div>

            <div class="p-3">
                <div class="table-responsive">

                    <asp:GridView ID="gvLecturers" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        GridLines="None"
                        DataKeyNames="lecturer_id,user_id"
                        OnSelectedIndexChanged="gvLecturers_SelectedIndexChanged"
                        OnRowCommand="gvLecturers_RowCommand">

                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Edit" />

                            <asp:BoundField DataField="lecturer_id" HeaderText="ID" />
                            <asp:BoundField DataField="staff_id" HeaderText="Staff ID" />
                            <asp:BoundField DataField="full_name" HeaderText="Full Name" />
                            <asp:BoundField DataField="specialisation" HeaderText="Specialisation" />
                            <asp:BoundField DataField="username" HeaderText="Username" />
                            <asp:BoundField DataField="email" HeaderText="Email" />
                            <asp:BoundField DataField="verification_status" HeaderText="Email Verification" />
                            <asp:BoundField DataField="account_status" HeaderText="Account Status" />
                            <asp:BoundField DataField="status" HeaderText="Lecturer Status" />

                            <asp:TemplateField HeaderText="Verification Action">
                                <ItemTemplate>
                                    <asp:Button ID="btnResend" runat="server"
                                        Text="Resend Email"
                                        CssClass="btn btn-ios-warning btn-sm"
                                        CommandName="ResendVerification"
                                        CommandArgument='<%# Eval("user_id") %>' />
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