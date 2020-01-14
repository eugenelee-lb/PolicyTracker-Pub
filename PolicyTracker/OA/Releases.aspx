<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Releases.aspx.vb" Inherits="PolicyTracker.Releases" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function pageLoad() {
            $(".select_chosen").chosen({
                disable_search_threshold: 10,
                no_results_text: "Nothing found!",
                search_contains: true
            });
            $(".select_chosen_300").chosen({
                disable_search_threshold: 10,
                no_results_text: "Nothing found!",
                width: "300px",
                search_contains: true
            });

            $("input[name$='txtFilterRelDateFrom']").inputmask("m/d/y");
            $("input[name$='txtFilterRelDateTo']").inputmask("m/d/y");

            $("input[name$='txtFilterRelDateFrom']").datepicker({
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("input[name$='txtFilterRelDateTo']").datepicker("option", "minDate", selectedDate);
                }
            });
            $("input[name$='txtFilterRelDateTo']").datepicker({
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("input[name$='txtFilterRelDateFrom']").datepicker("option", "maxDate", selectedDate);
                }
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src='<%: Page.ResolveUrl("~/Scripts/chosen.jquery.min.js")%>' type="text/javascript"></script>
    <link href='<%: Page.ResolveClientUrl("~/Content/chosen.min.css")%>' rel="stylesheet" />

    <h2>Releases</h2>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>
            <table>
                <tr>
                    <td>Subject:</td>
                    <td>
                        <asp:TextBox ID="txtFilterSubject" runat="server" Columns="20"></asp:TextBox>
                    </td>
                    <td>Release Date:</td>
                    <td>
                        <asp:TextBox ID="txtFilterRelDateFrom" runat="server" Columns="12" MaxLength="10" />
                        to
                        <asp:TextBox ID="txtFilterRelDateTo" runat="server" Columns="12" MaxLength="10" />
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td>Organization:</td>
                    <td colspan="3">
                        <asp:DropDownList ID="ddlFilterOrg" runat="server" AppendDataBoundItems="true"
                            DataSourceID="odsFilterOrg" DataTextField="OrgDesc" DataValueField="OrgCode"
                            AutoPostBack="false" CssClass="select_chosen">
                            <%--<asp:ListItem Text="-- All --" Value=""></asp:ListItem>--%>
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsFilterOrg" runat="server"
                            SelectMethod="GetOrgsForOA" OnSelecting="odsOrg_Selecting"
                            TypeName="PolicyTracker.Lib.SettingsBL">
                            <SelectParameters>
                                <asp:Parameter Name="userRole" Type="String" />
                                <asp:Parameter Name="userId" Type="String" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-info btn-xs" />
                    </td>
                </tr>
            </table>

            <asp:GridView ID="gvReleases" runat="server" AutoGenerateColumns="False" DataKeyNames="ReleaseId" Caption=""
                ItemType="PolicyTracker.Lib.Release" SelectMethod="gvReleases_GetData"
                AllowPaging="true" AllowSorting="false" EmptyDataText="No release was found.">
                <Columns>
                    <asp:TemplateField ShowHeader="False" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:HyperLink ID="hlkOpenRelease" runat="server" Target="_self" ToolTip="Open"
                                NavigateUrl='<%#: "~/OA/Release/" & Item.ReleaseId.ToString%>'>
                                        <span class="glyphicon glyphicon-folder-open"></span>
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ReleaseId" HeaderText="ID" Visible="false" />
                    <asp:BoundField DataField="ReleaseDate" HeaderText="Release Date" SortExpression="ReleaseDate" DataFormatString="{0:M/d/yyyy}" />
                    <asp:BoundField DataField="DeadlineDate" HeaderText="Deadline Date" SortExpression="DeadlineDate" DataFormatString="{0:M/d/yyyy}" />
                    <asp:BoundField DataField="To" HeaderText="To" SortExpression="To" Visible="false" />
                    <asp:BoundField DataField="From" HeaderText="From" SortExpression="From" Visible="false" />
                    <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" />
                    <asp:TemplateField HeaderText="# of Policies" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate><%#: Item.ReleasePolicies.Count %></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="# of Recipients<br />(Acked / Viewed / Total)" HeaderStyle-HorizontalAlign="Right" SortExpression="" Visible="True" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate><%# Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue) And a.AckDT.HasValue).Count.ToString("#,##0") & "/" _
                & Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue) And a.RecipientViewDT.HasValue).Count.ToString("#,##0") & "/" _
                & Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue)).Count.ToString("#,##0")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="# of <br />Exceptions" HeaderStyle-HorizontalAlign="Right" SortExpression="" Visible="True" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate><%# Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue) And a.Exception <> "").Count%></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Acked %" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate><%# IIf( _
    Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue)).Count = 0, "", _
    String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:##0.00%}", _
            Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue) And a.Exception Is Nothing And a.AckDT.HasValue).Count _
            / Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue) And a.Exception Is Nothing).Count))%></ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Viewed %" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate><%# IIf( _
    Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue)).Count = 0, "", _
    String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:##0.00%}", _
            Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue) And a.Exception Is Nothing And a.RecipientViewDT.HasValue).Count _
            / Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue) And a.Exception Is Nothing).Count))%></ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" Visible="false" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" SortExpression="CreateUser" Visible="false" />
                    <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" Visible="false" />
                    <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="LastUpdateUser" Visible="false" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
