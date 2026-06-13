<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="RegisterStudent.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.RegisterStudent" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Student Management</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server" />
    <asp:HiddenField ID="hfStudentID" runat="server" />
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
            <h2 class="fw-bold mb-1">Student Management</h2>
            <p class="text-muted mb-0">
                Register students, update records, drop students and manage email verification.
            </p>
        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Student Registration / Update Form</h5>
                <div class="mini-help">
                    Matric number is generated automatically using P + year + month + running number.
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
                            placeholder="Example: student001" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Password</label>
                        <asp:TextBox ID="txtPassword" runat="server"
                            CssClass="form-control"
                            TextMode="Password"
                            placeholder="Example: Student@123" />
                        <small class="text-muted">
                            Required for new student. Minimum 8 characters, uppercase, lowercase, number and special character.
                        </small>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Email Address</label>
                        <asp:TextBox ID="txtEmail" runat="server"
                            CssClass="form-control"
                            TextMode="Email"
                            placeholder="student@email.com" />
                    </div>

                    <div class="col-12 mt-3">
                        <h6 class="fw-bold text-primary mb-0">Student Personal Details</h6>
                        <hr class="mt-2" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Full Name</label>
                        <asp:TextBox ID="txtFullName" runat="server"
                            CssClass="form-control"
                            placeholder="Enter full name" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">NRIC / Passport No</label>
                        <asp:TextBox ID="txtNRIC" runat="server"
                            CssClass="form-control"
                            placeholder="Example: 010101-01-1234" />
                    </div>

                    <div class="col-md-2">
                        <label class="form-label">Date of Birth</label>
                        <asp:TextBox ID="txtDOB" runat="server"
                            CssClass="form-control"
                            TextMode="Date" />
                    </div>

                    <div class="col-md-2">
                        <label class="form-label">Matric Number</label>
                        <asp:TextBox ID="txtMatricNumber" runat="server"
                            CssClass="form-control readonly-matric"
                            ReadOnly="true"
                            placeholder="Auto generated" />
                    </div>

                    <div class="col-12 mt-3">
                        <h6 class="fw-bold text-primary mb-0">Admission & Academic Information</h6>
                        <hr class="mt-2" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Admission Year</label>
                        <asp:TextBox ID="txtAdmissionYear" runat="server"
                            CssClass="form-control"
                            TextMode="Number"
                            placeholder="Example: 2026" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Admission Month</label>
                        <asp:DropDownList ID="ddlAdmissionMonth" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="January" Value="01" />
                            <asp:ListItem Text="February" Value="02" />
                            <asp:ListItem Text="March" Value="03" />
                            <asp:ListItem Text="April" Value="04" />
                            <asp:ListItem Text="May" Value="05" />
                            <asp:ListItem Text="June" Value="06" />
                            <asp:ListItem Text="July" Value="07" />
                            <asp:ListItem Text="August" Value="08" />
                            <asp:ListItem Text="September" Value="09" />
                            <asp:ListItem Text="October" Value="10" />
                            <asp:ListItem Text="November" Value="11" />
                            <asp:ListItem Text="December" Value="12" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Programme</label>
                        <asp:DropDownList ID="ddlProgramme" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-2">
                        <label class="form-label">Academic Status</label>
                        <asp:DropDownList ID="ddlStatus" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="Active" Value="Active" />
                            <asp:ListItem Text="Inactive" Value="Inactive" />
                            <asp:ListItem Text="Dropped" Value="Dropped" />
                            <asp:ListItem Text="Graduated" Value="Graduated" />
                            <asp:ListItem Text="Suspended" Value="Suspended" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-12 mt-4">
                        <asp:Button ID="btnRegister" runat="server"
                            Text="Register Student"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnRegister_Click" />

                        <asp:Button ID="btnUpdate" runat="server"
                            Text="Update Student"
                            CssClass="btn btn-ios-warning ms-2"
                            OnClick="btnUpdate_Click" />

                        <asp:Button ID="btnDrop" runat="server"
                            Text="Drop Student"
                            CssClass="btn btn-ios-danger ms-2"
                            OnClick="btnDrop_Click" />

                        <asp:Button ID="btnReactivate" runat="server"
                            Text="Reactivate Student"
                            CssClass="btn btn-ios-primary ms-2"
                            OnClick="btnReactivate_Click" />

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

        <div class="glass-card form-card mb-4">
            <div class="form-body">
                <div class="row g-3 align-items-end">

                    <div class="col-md-3">
                        <label class="form-label">Search By</label>
                        <asp:DropDownList ID="ddlSearchBy" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="All Fields" Value="all" />
                            <asp:ListItem Text="Student ID" Value="student_id" />
                            <asp:ListItem Text="Matric Number" Value="matric_number" />
                            <asp:ListItem Text="NRIC / Passport" Value="nric_passport" />
                            <asp:ListItem Text="Student Name" Value="full_name" />
                            <asp:ListItem Text="Student Email" Value="email" />
                            <asp:ListItem Text="Username" Value="username" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Search Keyword</label>
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Type ID, NRIC, name, matric number, email or username" />
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
            </div>
        </div>

        <div class="glass-card table-card">
            <div class="table-header">
                <h5 class="fw-bold mb-1">Registered Students</h5>
                <div class="mini-help">
                    View student academic status, account status and email verification.
                </div>
            </div>

            <div class="p-3">
                <div class="table-responsive">

                    <asp:GridView ID="gvStudents" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="student_id,user_id"
                        GridLines="None"
                        OnSelectedIndexChanged="gvStudents_SelectedIndexChanged"
                        OnRowCommand="gvStudents_RowCommand">

                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Edit" />

                            <asp:BoundField DataField="student_id" HeaderText="ID" />
                            <asp:BoundField DataField="matric_number" HeaderText="Matric Number" />
                            <asp:BoundField DataField="nric_passport" HeaderText="NRIC / Passport" />
                            <asp:BoundField DataField="full_name" HeaderText="Full Name" />
                            <asp:BoundField DataField="programme_name" HeaderText="Programme" />
                            <asp:BoundField DataField="username" HeaderText="Username" />
                            <asp:BoundField DataField="email" HeaderText="Email" />
                            <asp:BoundField DataField="verification_status" HeaderText="Email Verification" />
                            <asp:BoundField DataField="account_status" HeaderText="Account Status" />
                            <asp:BoundField DataField="status" HeaderText="Academic Status" />
                            <asp:BoundField DataField="date_of_birth" HeaderText="DOB" DataFormatString="{0:yyyy-MM-dd}" />
                            <asp:BoundField DataField="admission_year" HeaderText="Admission Year" />
                            <asp:BoundField DataField="admission_month" HeaderText="Admission Month" />

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