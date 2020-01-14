<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="OrgAdmins.aspx.vb" Inherits="PolicyTracker.OrgAdmins" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/ADUserSearch.ascx" TagPrefix="asp" TagName="ADUserSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function pageLoad() {
            $(".select_chosen").chosen({
                disable_search_threshold: 10,
                no_results_text: "Nothing found!",
                search_contains: true
            });

            $("#user-search").click(function () {
                $("#user-search-modal").modal('show');
            });
            $('#user-search-modal').on('shown.bs.modal', function () {
                $("input[name$='txtLogonName']").focus();
            })
        }

        function SelectUser(varUserId, varUserName) {
            $("input[name$='txtUserId']").val(varUserId);
            $("input[name$='txtUserName']").val(varUserName);
            $("#user-search-modal").modal('hide');
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src='<%: Page.ResolveUrl("~/Scripts/chosen.jquery.min.js")%>' type="text/javascript"></script>
    <link href='<%: Page.ResolveClientUrl("~/Content/chosen.min.css")%>' rel="stylesheet" />

    <h2>Organization Administrators</h2>

    <div id="user-search-modal" class="modal fade">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Search User</h4>
                </div>
                <div class="modal-body">
                    <asp:ADUserSearch runat="server" ID="ADUserSearch" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default btn-sm" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <asp:Label ID="lblFilterDepartment" runat="server" AssociatedControlID="ddlFilterDepartment" Text="Department:"></asp:Label>
            <asp:DropDownList ID="ddlFilterDepartment" runat="server" AppendDataBoundItems="true"
                DataSourceID="odsFilterDepartment" DataTextField="DeptDesc" DataValueField="DeptCode"
                AutoPostBack="false" CssClass="select_chosen">
            </asp:DropDownList>
            <asp:ObjectDataSource ID="odsFilterDepartment" runat="server"
                SelectMethod="GetDepartmentsByAdmin" OnSelecting="odsDepartment_Selecting"
                TypeName="PolicyTracker.Lib.SettingsBL">
                <SelectParameters>
                    <asp:Parameter Name="userRole" Type="String" />
                    <asp:Parameter Name="userId" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-info btn-xs" />

            <asp:GridView ID="gvOrgAdmins" runat="server" AutoGenerateColumns="False" 
                ItemType="PolicyTracker.Lib.vOrgAdmin" SelectMethod="gvOrgAdmins_GetData" DataKeyNames="OrgCode,UserId"
                AllowPaging="True" AllowSorting="True"
                EmptyDataText="No record has been found.">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="OrgCode" HeaderText="Dept" SortExpression="OrgCode" Visible="false" />
                    <asp:BoundField DataField="OrgDesc" HeaderText="Organization" SortExpression="OrgDesc" />
                    <asp:BoundField DataField="UserId" HeaderText="User ID" SortExpression="UserId" />
                    <asp:BoundField DataField="UserName" HeaderText="User Name" SortExpression="UserName" />
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" SortExpression="CreateUser" />
                    <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" />
                    <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="LastUpdateUser" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>
            <br />

            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />

            <asp:DetailsView ID="dvOrgAdmin" runat="server" AutoGenerateRows="False" DataKeyNames="UserId,OrgCode"
                DataSourceID="odsOrgAdmin" ItemType="PolicyTracker.Lib.UserOrg" Caption="Add New Org Admin" DefaultMode="Insert">
                <Fields>
                    <asp:TemplateField HeaderText="Organization" SortExpression="OrgCode">
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlOrganization" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("OrgCode")%>'
                                DataSourceID="odsOrganization" DataTextField="OrgDesc" DataValueField="OrgCode" Enabled="false">
                                <asp:ListItem Value="">-- Select Organization --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvOrganization" runat="server" ControlToValidate="ddlOrganization"
                                ErrorMessage="Organization is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <asp:ObjectDataSource ID="odsOrganization" runat="server"
                                SelectMethod="GetOrgsForPA" OnSelecting="odsDepartment_Selecting"
                                TypeName="PolicyTracker.Lib.SettingsBL">
                                <SelectParameters>
                                    <asp:Parameter Name="userRole" Type="String" />
                                    <asp:Parameter Name="userId" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:DropDownList ID="ddlOrganization" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("OrgCode")%>'
                                DataSourceID="odsOrganization" DataTextField="OrgDesc" DataValueField="OrgCode" Enabled="true" CssClass="select_chosen">
                                <asp:ListItem Value="">-- Select Organization --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvOrganization" runat="server" ControlToValidate="ddlOrganization"
                                ErrorMessage="Organization is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <asp:ObjectDataSource ID="odsOrganization" runat="server"
                                SelectMethod="GetOrgsForPA" OnSelecting="odsDepartment_Selecting"
                                TypeName="PolicyTracker.Lib.SettingsBL">
                                <SelectParameters>
                                    <asp:Parameter Name="userRole" Type="String" />
                                    <asp:Parameter Name="userId" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlOrganization" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("OrgCode")%>'
                                DataSourceID="odsOrganization" DataTextField="OrgDesc" DataValueField="OrgCode" Enabled="false">
                                <asp:ListItem Value="">-- Select Organization --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvOrganization" runat="server" ControlToValidate="ddlOrganization"
                                ErrorMessage="Organization is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <asp:ObjectDataSource ID="odsOrganization" runat="server"
                                SelectMethod="GetOrgsForPA" OnSelecting="odsDepartment_Selecting"
                                TypeName="PolicyTracker.Lib.SettingsBL">
                                <SelectParameters>
                                    <asp:Parameter Name="userRole" Type="String" />
                                    <asp:Parameter Name="userId" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="User" SortExpression="UserId">
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlUser" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("UserId")%>'
                                DataSourceID="odsUser" DataTextField="UserName" DataValueField="UserId" Enabled="false">
                                <asp:ListItem Value="">-- Select User --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvUser" runat="server" ControlToValidate="ddlUser"
                                ErrorMessage="User is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <asp:ObjectDataSource ID="odsUser" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="SearchAppUsersByRoleName" TypeName="PolicyTracker.Lib.SettingsBL">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                    <asp:Parameter DefaultValue="ALL" Name="role" Type="String" />
                                    <asp:Parameter DefaultValue="" Name="name" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtUserId" runat="server" Columns="15" MaxLength="50" Text='<%# Bind("UserId") %>' />
                            <asp:RequiredFieldValidator ID="rfvUserId" runat="server" ControlToValidate="txtUserId"
                                ErrorMessage="User ID is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <span class="glyphicon glyphicon-search" id="user-search" style="cursor: pointer;"></span>
                            <asp:TextBox ID="txtUserName" runat="server" Columns="30" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="txtUserName"
                                ErrorMessage="User Name is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            (<asp:Label ID="lblUserId" runat="server" Text='<%# Bind("UserId")%>'></asp:Label>)
                            <asp:Label ID="lblUserName" runat="server" Text='<%# EvalUserName()%>'></asp:Label>
                            <asp:HiddenField ID="hidAccessLevel" runat="server" Value='<%# Bind("AccessLevel")%>'></asp:HiddenField>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" InsertVisible="False" DataFormatString="{0:M/d/yyyy h:mm:ss.fff tt}"
                        ReadOnly="True" SortExpression="CreateDT" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" InsertVisible="False"
                        ReadOnly="True" SortExpression="CreateUser" />
                    <asp:TemplateField HeaderText="Last Update Date" SortExpression="LastUpdateDT" InsertVisible="False">
                        <EditItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("LastUpdateDT", "{0:M/d/yyyy h:mm:ss.fff tt}") %>'></asp:Label>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("LastUpdateDT") %>'></asp:Label>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("LastUpdateDT", "{0:M/d/yyyy h:mm:ss.fff tt}") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Last Update User" SortExpression="LastUpdateUser" InsertVisible="False">
                        <EditItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("LastUpdateUser") %>'></asp:Label>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("LastUpdateUser") %>'></asp:Label>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("LastUpdateUser") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ShowHeader="False" ItemStyle-CssClass="CommandRowStyle">
                        <EditItemTemplate>
                            <asp:LinkButton ID="lbtnUpdate" runat="server" CausesValidation="True" CommandName="Update"
                                Text="Update" ValidationGroup="vgDetails" CssClass="btn btn-primary btn-xs"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                Text="Cancel" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:LinkButton ID="lbtnInsert" runat="server" CausesValidation="True" CommandName="Insert"
                                Text="Insert" ValidationGroup="vgDetails" CssClass="btn btn-primary btn-xs"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                Text="Cancel" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtnEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                Text="Edit" CssClass="btn btn-default btn-xs" Visible="false"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                Text="Delete" CssClass="btn btn-default btn-xs" OnClientClick="return confirm('Are you sure you want to delete this record?');"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnNew" runat="server" CausesValidation="False" CommandName="New"
                                Text="Add New Org Admin" CssClass="btn btn-primary btn-xs"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <asp:ObjectDataSource ID="odsOrgAdmin" runat="server" SelectMethod="GetUserOrg"
                TypeName="PolicyTracker.Lib.SettingsRepository" DataObjectTypeName="PolicyTracker.Lib.UserOrg"
                DeleteMethod="DeleteUserOrg" InsertMethod="InsertUserOrg" OldValuesParameterFormatString="orig{0}"
                UpdateMethod="UpdateUserOrg" ConflictDetection="CompareAllValues">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvOrgAdmins" DefaultValue="" Name="userId" PropertyName="SelectedDataKey.Values[1]" Type="String" />
                    <asp:ControlParameter ControlID="gvOrgAdmins" DefaultValue="" Name="orgCode" PropertyName="SelectedDataKey.Values[0]" Type="String" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="userOrg" Type="Object" />
                    <asp:Parameter Name="origUserOrg" Type="Object" />
                </UpdateParameters>
            </asp:ObjectDataSource>

            <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
