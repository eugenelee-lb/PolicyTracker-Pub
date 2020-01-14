<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="UserInfo.aspx.vb" Inherits="PolicyTracker.UserInfo" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>User Info</h2>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>

            <div class="alert alert-info">
                <asp:Label ID="lblUserInfo" runat="server"></asp:Label>
            </div>

            <div id="divGetHostname" runat="server">
                IP Address: <asp:TextBox ID="txtIPAddress" runat="server" Columns="20"></asp:TextBox>
                <asp:Button ID="btnGetHostName" runat="server" Text="Get Hostname" OnClick="btnGetHostName_Click" />
                <div class="alert alert-info">
                    <asp:Label ID="lblHostname" runat="server"></asp:Label>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
