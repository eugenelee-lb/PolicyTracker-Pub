<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="MyPackets.aspx.vb" Inherits="PolicyTracker.MyPackets" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>My Packets</h2>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            Status: 
            <asp:DropDownList ID="ddlFilterStat" runat="server" AutoPostBack="true">
                <asp:ListItem Text="Pending" Value="True" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Acknowledged" Value="False"></asp:ListItem>
            </asp:DropDownList>
            <br /><br />
            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsMain" runat="server" ShowSummary="true" DisplayMode="List" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgMain" />

            <asp:GridView ID="gvPackets" runat="server" AutoGenerateColumns="False" DataKeyNames="ReleaseId,RecipientId"
                AllowPaging="true" AllowSorting="true" EmptyDataText="No record was found." Caption=""
                ItemType="PolicyTracker.Lib.vPacket" SelectMethod="gvPackets_GetData">
                <Columns>
                    <asp:TemplateField ShowHeader="False" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:HyperLink ID="hlkOpenPacket" runat="server" Target="_self" ToolTip="Open Packet"
                                NavigateUrl='<%#: "~/USER/Packet/" & Item.ReleaseId.ToString & "/" & Item.RecipientId%>'>
                                        <span class="glyphicon glyphicon-folder-open"></span>
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ReleaseId" HeaderText="Release ID" Visible="false" />
                    <asp:BoundField DataField="RecipientId" HeaderText="Recipient ID" SortExpression="RecipientId" Visible="false" />
                    <asp:BoundField DataField="ReleaseDate" HeaderText="Release Date" SortExpression="ReleaseDate" DataFormatString="{0:M/d/yyyy}" />
                    <asp:BoundField DataField="DeadlineDate" HeaderText="Deadline Date" SortExpression="DeadlineDate" DataFormatString="{0:M/d/yyyy}" />
                    <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" />
                    <asp:BoundField DataField="To" HeaderText="To" SortExpression="To" Visible="false" />
                    <asp:BoundField DataField="From" HeaderText="From" SortExpression="From" Visible="true" />
                    <asp:BoundField DataField="Exception" HeaderText="Exception" SortExpression="Exception" Visible="false" />
                    <asp:BoundField DataField="RecipientViewDT" HeaderText="View Date Time" SortExpression="RecipientViewDT" Visible="true" />
                    <asp:BoundField DataField="AckDT" HeaderText="Ack Date Time" SortExpression="AckDT" Visible="true" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
