<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="NotificationManagement.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.NotificationManagement" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Notification Management</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <asp:HiddenField ID="hfNotificationID" runat="server" />

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
            <h2 class="fw-bold mb-1">Notification Centre</h2>
            <p class="text-muted mb-0">
                Create, view and manage internal SIMS notifications.
            </p>
        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Create Notification</h5>
                <div class="mini-help">
                    Send reminders or alerts to students, lecturers or HOP users.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4">

                    <div class="col-md-6">
                        <label class="form-label">Send To</label>
                        <asp:DropDownList ID="ddlUser" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label">Notification Type</label>
                        <asp:DropDownList ID="ddlType" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="General" Value="General" />
                            <asp:ListItem Text="Academic" Value="Academic" />
                            <asp:ListItem Text="Attendance" Value="Attendance" />
                            <asp:ListItem Text="Enrolment" Value="Enrolment" />
                            <asp:ListItem Text="Fee" Value="Fee" />
                            <asp:ListItem Text="System" Value="System" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-12">
                        <label class="form-label">Message</label>
                        <asp:TextBox ID="txtMessage" runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="5"
                            placeholder="Write notification message..." />
                    </div>

                    <div class="col-12 mt-3">
                        <asp:Button ID="btnSend" runat="server"
                            Text="Send Notification"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnSend_Click" />

                        <asp:Button ID="btnMarkRead" runat="server"
                            Text="Mark as Read"
                            CssClass="btn btn-ios-warning ms-2"
                            OnClick="btnMarkRead_Click" />

                        <asp:Button ID="btnDelete" runat="server"
                            Text="Delete Notification"
                            CssClass="btn btn-ios-danger ms-2"
                            OnClick="btnDelete_Click"
                            OnClientClick="return confirm('Are you sure you want to delete this notification?');" />

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
                <h5 class="fw-bold mb-1">Notification List</h5>
                <div class="mini-help">
                    Select a notification to mark as read or delete.
                </div>
            </div>

            <div class="p-3">

                <div class="row mb-3 g-3 align-items-end">

                    <div class="col-md-5">
                        <label class="form-label">Search</label>
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Search user, type or message" />
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Read Status</label>
                        <asp:DropDownList ID="ddlReadStatus" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="All Notifications" Value="All" />
                            <asp:ListItem Text="Unread" Value="Unread" />
                            <asp:ListItem Text="Read" Value="Read" />
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

                    <asp:GridView ID="gvNotifications" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="notification_id"
                        GridLines="None"
                        OnSelectedIndexChanged="gvNotifications_SelectedIndexChanged">

                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="View" />

                            <asp:BoundField DataField="notification_id" HeaderText="ID" />
                            <asp:BoundField DataField="username" HeaderText="User" />
                            <asp:BoundField DataField="role" HeaderText="Role" />
                            <asp:BoundField DataField="type" HeaderText="Type" />
                            <asp:BoundField DataField="read_status" HeaderText="Status" />
                            <asp:BoundField DataField="created_at" HeaderText="Created At" DataFormatString="{0:yyyy-MM-dd HH:mm}" />

                            <asp:TemplateField HeaderText="Message">
                                <ItemTemplate>
                                    <div style="max-width:480px; white-space:normal;">
                                        <%# Eval("message") %>
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