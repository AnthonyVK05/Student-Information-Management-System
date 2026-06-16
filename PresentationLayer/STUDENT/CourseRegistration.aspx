<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CourseRegistration.aspx.cs" Inherits="Student_Information_Management_System.STUDENT.CourseRegistration" %>

<!DOCTYPE html>
    
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <title>Course Registration</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="../../Content/SIMSTheme.css" rel="stylesheet" />

    <style>

        /*Menu*/
        .menu
        {
            background-color: darkblue;
            padding: 15px;
        }

        .menu a
        {
            color: white;
            text-decoration: none;
            margin-right: 25px;
            font-weight: bold;
            font-size: 16px;
        }
        
        .menu a:hover {
          border-bottom: 3px solid white;
        }

        .menu a.active {
          border-bottom: 3px solid white;
        }

        .dashboard
        {
            padding: 30px;
        }

        .title
        {
            color: darkblue;
        }

    </style>

</head>

<body>

<form id="form2" runat="server">
        <!-- MENU -->

    <div class="menu">
        <a class="active" href="CourseRegistration.aspx">Course Registration</a>
        <a href="ViewEnrolledCourses.aspx">View Enrolled Courses</a>
        <a href="EnrollmentStatus.aspx">Status</a>
    </div>

    <h1 class="title">
        Course Registration
    </h1>

        <!-- SESSION -->

    <asp:Label ID="Label1"
        runat="server"
        Text="Select Session">
    </asp:Label>

    <br />

    <asp:DropDownList ID="ddlSession"
        runat="server"
        Width="250px"
        AutoPostBack="True"
        OnSelectedIndexChanged="ddlSession_SelectedIndexChanged">
    </asp:DropDownList>

    <br /><br />

    <!-- SEMESTER -->

    <asp:Label ID="Label2"
        runat="server"
        Text="Select Semester">
    </asp:Label>

    <br />

    <asp:DropDownList ID="ddlSemester"
        runat="server"
        Width="250px"
        AutoPostBack="True"
        OnSelectedIndexChanged="ddlSemester_SelectedIndexChanged">

        <asp:ListItem>Semester 1</asp:ListItem>

        <asp:ListItem>Semester 2</asp:ListItem>

    </asp:DropDownList>

    <br /><br />

        <!-- COURSE GRID -->

    <asp:GridView ID="gvCourses"
        runat="server"
        AutoGenerateColumns="False"
        Width="1200px"
        CssClass="grid">

        <Columns>

            <asp:TemplateField HeaderText="Select">

                <ItemTemplate>

                    <asp:CheckBox ID="chkSelect"
                        runat="server" />

                </ItemTemplate>

            </asp:TemplateField>

            <asp:BoundField DataField="CourseID"
                HeaderText="Course ID" />

            <asp:BoundField DataField="CourseCode"
                HeaderText="Course Code" />

            <asp:BoundField DataField="CourseName"
                HeaderText="Course Name" />

            <asp:BoundField DataField="CreditHours"
                HeaderText="Credit Hours" />

        </Columns>

    </asp:GridView>

    <br />

    <!-- SUBMIT BUTTON -->

    <asp:Button ID="btnSubmit"
        runat="server"
        Text="Submit Enrollment"
        Width="200px"
        OnClick="btnSubmit_Click" />

    <br /><br />

    <asp:Label ID="lblMessage"
        runat="server"
        ForeColor="Green">
    </asp:Label>

    <hr />

    <h2>Submitted Enrollment Details</h2>

    <!-- ENROLLMENT GRID -->

    <asp:GridView ID="gvEnrollment"
        runat="server"
        AutoGenerateColumns="True"
        Width="1200px">

    </asp:GridView>

</form>

</body>

</html>
