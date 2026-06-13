<%@ Page Title="" Language="C#" MasterPageFile="~/HomeMaster.master"
AutoEventWireup="true" CodeBehind="NewItem.aspx.cs"
Inherits="UniversitySystem.NewItem" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<h2>New Item Page</h2>

<div style="background:white;padding:20px;border-radius:10px;width:400px;">

    <asp:TextBox ID="txtName" runat="server" placeholder="Enter Name"
        style="width:100%;padding:10px;margin-bottom:10px;"></asp:TextBox>

    <asp:TextBox ID="txtCode" runat="server" placeholder="Enter Code"
        style="width:100%;padding:10px;margin-bottom:10px;"></asp:TextBox>

    <asp:Button ID="btnSave" runat="server" Text="Save"
        style="background:#1e3c72;color:white;border:none;padding:10px;width:100%;cursor:pointer;" />

</div>

</asp:Content>