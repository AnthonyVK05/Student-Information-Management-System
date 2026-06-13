<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="MessageManagement.aspx.cs"
    Inherits="Student_Information_Management_System.HOP.MessageManagement" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Message Centre</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />
</head>

<body>
<form id="form1" runat="server">

    <asp:HiddenField ID="hfMessageID" runat="server" />
    <asp:HiddenField ID="hfReplyToUserID" runat="server" />

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
            <h2 class="fw-bold mb-1">Message Centre</h2>
            <p class="text-muted mb-0">
                Send messages, view inbox and reply internally inside SIMS.
            </p>
        </div>

        <div class="glass-card form-card mb-4">
            <div class="form-header">
                <h5 class="fw-bold mb-1">Compose Message</h5>
                <div class="mini-help">
                    Select a user, write subject and message.
                </div>
            </div>

            <div class="form-body">
                <div class="row g-4">

                    <div class="col-md-6">
                        <label class="form-label">Send To</label>
                        <asp:DropDownList ID="ddlReceiver" runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-6">
                        <label class="form-label">Subject</label>
                        <asp:TextBox ID="txtSubject" runat="server"
                            CssClass="form-control"
                            placeholder="Enter message subject" />
                    </div>

                    <div class="col-12">
                        <label class="form-label">Message</label>
                        <asp:TextBox ID="txtMessage" runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="6"
                            placeholder="Write your message here..." />
                    </div>

                    <div class="col-12 mt-3">
                        <asp:Button ID="btnSend" runat="server"
                            Text="Send Message"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnSend_Click" />

                        <asp:Button ID="btnReply" runat="server"
                            Text="Send Reply"
                            CssClass="btn btn-success ms-2"
                            OnClick="btnReply_Click" />

                        <asp:Button ID="btnMarkRead" runat="server"
                            Text="Mark as Read"
                            CssClass="btn btn-ios-warning ms-2"
                            OnClick="btnMarkRead_Click" />

                        <asp:Button ID="btnDelete" runat="server"
                            Text="Delete Message"
                            CssClass="btn btn-ios-danger ms-2"
                            OnClick="btnDelete_Click"
                            OnClientClick="return confirm('Are you sure you want to delete this message?');" />

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
                <h5 class="fw-bold mb-1">Inbox / Sent Messages</h5>
                <div class="mini-help">
                    Select a message to view or reply.
                </div>
            </div>

            <div class="p-3">

                <div class="row mb-3 g-3 align-items-end">

                    <div class="col-md-3">
                        <label class="form-label">View</label>
                        <asp:DropDownList ID="ddlBox" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="Inbox" Value="Inbox" />
                            <asp:ListItem Text="Sent" Value="Sent" />
                            <asp:ListItem Text="All Messages" Value="All" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-3">
                        <label class="form-label">Read Status</label>
                        <asp:DropDownList ID="ddlReadStatus" runat="server"
                            CssClass="form-select">
                            <asp:ListItem Text="All" Value="All" />
                            <asp:ListItem Text="Unread" Value="Unread" />
                            <asp:ListItem Text="Read" Value="Read" />
                        </asp:DropDownList>
                    </div>

                    <div class="col-md-4">
                        <label class="form-label">Search</label>
                        <asp:TextBox ID="txtSearch" runat="server"
                            CssClass="form-control"
                            placeholder="Search sender, receiver, subject or message" />
                    </div>

                    <div class="col-md-2">
                        <asp:Button ID="btnSearch" runat="server"
                            Text="Search"
                            CssClass="btn btn-ios-primary"
                            OnClick="btnSearch_Click" />

                        <asp:Button ID="btnResetSearch" runat="server"
                            Text="Reset"
                            CssClass="btn btn-ios-light mt-2"
                            OnClick="btnResetSearch_Click" />
                    </div>

                </div>

                <div class="table-responsive">
                    <asp:GridView ID="gvMessages" runat="server"
                        CssClass="table table-bordered table-hover"
                        AutoGenerateColumns="False"
                        DataKeyNames="message_id"
                        GridLines="None"
                        OnSelectedIndexChanged="gvMessages_SelectedIndexChanged">

                        <Columns>
                            <asp:CommandField ShowSelectButton="True" SelectText="View / Reply" />

                            <asp:BoundField DataField="message_id" HeaderText="ID" />
                            <asp:BoundField DataField="sender_name" HeaderText="Sender" />
                            <asp:BoundField DataField="receiver_name" HeaderText="Receiver" />
                            <asp:BoundField DataField="subject" HeaderText="Subject" />
                            <asp:BoundField DataField="read_status" HeaderText="Status" />
                            <asp:BoundField DataField="sent_at" HeaderText="Sent At" DataFormatString="{0:yyyy-MM-dd HH:mm}" />

                            <asp:TemplateField HeaderText="Message">
                                <ItemTemplate>
                                    <div style="max-width:450px; white-space:normal;">
                                        <%# Eval("message_body") %>
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