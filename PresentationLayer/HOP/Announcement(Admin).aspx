<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Announcement(Admin).aspx.cs"
    Inherits="Student_Information_Management_System.HOP.Announcement_Admin_" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Announcement Management</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <asp:HiddenField ID="hfAnnouncementID" runat="server" />

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
            <h2 class="fw-bold mb-1">Announcement Management</h2>
            <p class="text-muted mb-0">
                Post academic announcements for all users or for a specific course.
            </p>
        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Create / Update Announcement</h5>
                <div class="mini-help">
                    Select a course only if the announcement is course-specific. Leave as General for portal-wide announcements.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4">

                    <div class="col-md-8">
                        <label class="form-label">Announcement Title</label>
                        <asp:TextBox ID="txtTitle" runat="server"
                            CssClass="form-control"
                            placeholder="Example: Semester 1 Registration Now Open" />
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Course Optional</label>
                        <asp:DropDownList ID="ddlCourse" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-12">
                        <label class="form-label">Announcement Content</label>
                        <asp:TextBox ID="txtContent" runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="6"
                            placeholder="Write the announcement details here..." />
                    </div>

                    <div class="col-12 mt-3">
                        <asp:Button ID="btnPost" runat="server"
                            Text="Post Announcement"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnPost_Click" />

                        <asp:Button ID="btnUpdate" runat="server"
                            Text="Update Announcement"
                            CssClass="btn btn-ios-warning ms-2"
                            OnClick="btnUpdate_Click" />

                        <asp:Button ID="btnDelete" runat="server"
                            Text="Delete Announcement"
                            CssClass="btn btn-ios-danger ms-2"
                            OnClick="btnDelete_Click"
                            OnClientClick="return confirm('Are you sure you want to delete this announcement? This action cannot be undone.');" />

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
                <h5 class="fw-bold mb-1">Announcement List</h5>
                <div class="mini-help">
                    Select an announcement to update or delete it.
                </div>
            </div>

            <div class="p-3">

                <div class="row mb-3 g-3 align-items-end">

                    <div class="col-md-5">
                        <label class="form-label">Search</label>
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Search title, content or course" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Course Filter</label>
                        <asp:DropDownList ID="ddlSearchCourse" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
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

                    <asp:GridView ID="gvAnnouncements" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="announcement_id"
                        GridLines="None"
                        OnSelectedIndexChanged="gvAnnouncements_SelectedIndexChanged">

                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="Edit" />

                            <asp:BoundField DataField="announcement_id" HeaderText="ID" />
                            <asp:BoundField DataField="title" HeaderText="Title" />
                            <asp:BoundField DataField="course_code" HeaderText="Course Code" />
                            <asp:BoundField DataField="course_name" HeaderText="Course" />
                            <asp:BoundField DataField="posted_by_name" HeaderText="Posted By" />
                            <asp:BoundField DataField="posted_at" HeaderText="Posted At" DataFormatString="{0:yyyy-MM-dd HH:mm}" />

                            <asp:TemplateField HeaderText="Content">
                                <ItemTemplate>
                                    <div style="max-width: 420px; white-space: normal;">
                                        <%# Eval("content") %>
                                    </div>
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