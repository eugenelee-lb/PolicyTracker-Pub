<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Login.aspx.vb" Inherits="PolicyTracker.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Log In</h1>

    <asp:label id="lblLoginInfo" Runat="Server" EnableViewState="true" Visible="true" CssClass="InfoMsg"></asp:label>
    <br />
    <br />
    <table class="DataWebControlStyle" cellspacing="0" cellpadding="4" rules="all" border="1" style="border-collapse:collapse;">
        <tr class="RowStyle">
            <td class="HeaderStyle">User ID</td>
            <td><asp:TextBox ID="txtUserID" runat="server" Columns="20" MaxLength="20"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvUserID" runat="server" ControlToValidate="txtUserID"
                    ErrorMessage="(Required)"></asp:RequiredFieldValidator></td>
        </tr>
        <tr class="AlternatingRowStyle">
            <td class="HeaderStyle">Password (Last 4 digits of SSN)</td>
            <td><asp:TextBox ID="txtPassword" runat="server" Columns="20" MaxLength="50" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                    ErrorMessage="(Required)"></asp:RequiredFieldValidator></td>
        </tr>
        <tr class="CommandRowStyle">
            <td colspan="2">
                <asp:Button ID="btnLogin" runat="server" Text="Log In" />
            </td>
        </tr>
    </table>
    <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
    <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
    <br />
    <br />
    <br />
    <br />
    <br />
    <br />
    &nbsp;
</asp:Content>
