<%@ Page Title="Users" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="AppUsers.aspx.vb" Inherits="PolicyTracker.AppUsers" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/ADUserSearch.ascx" TagPrefix="asp" TagName="ADUserSearch" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script>
        function pageLoad() {
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

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Administrators</h2>

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

            <table>
                <tr>
                    <td>Role:</td>
                    <td>
                        <asp:DropDownList ID="ddlRoleFilter" runat="server" AppendDataBoundItems="true"
                            DataSourceID="odsRoleFilter" DataTextField="CMCodeDesc" DataValueField="CMCode"
                            AutoPostBack="false">
                            <asp:ListItem Value="ALL">-- All --</asp:ListItem>
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsRoleFilter" runat="server"
                            SelectMethod="GetCommonCodesByCatgStatus"
                            TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression">
                            <SelectParameters>
                                <asp:Parameter Name="catg" Type="String" DefaultValue="USER_ROLE" />
                                <asp:Parameter Name="stat" Type="Boolean" DefaultValue="true" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td>Name:</td>
                    <td>
                        <asp:TextBox ID="txtNameFilter" runat="server" Columns="30"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-info btn-xs" />
                        <%--<input type="button" id="bSearch" runat="server"  value="Search"  />--%>
                    </td>
                </tr>
            </table>

            <asp:GridView ID="gvUsers" runat="server" DataSourceID="odsUsersGrid"
                AutoGenerateColumns="False" DataKeyNames="UserId" AllowPaging="True" AllowSorting="True"
                EmptyDataText="No record has been found.">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="UserId" HeaderText="User ID" ReadOnly="True" SortExpression="UserId" />
                    <asp:BoundField DataField="UserName" HeaderText="Name" SortExpression="UserName" />
                    <asp:BoundField DataField="UserRole" HeaderText="Role" ReadOnly="True" SortExpression="UserRole" />
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" SortExpression="CreateUser" />
                    <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" />
                    <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="LastUpdateUser" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsUsersGrid" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="SearchAppUsersByRoleName" TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression">
                <SelectParameters>
                    <asp:ControlParameter ControlID="ddlRoleFilter" Name="role" PropertyName="SelectedValue" Type="String" />
                    <asp:ControlParameter ControlID="txtNameFilter" Name="name" PropertyName="Text" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />

            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="List" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />

            <asp:DetailsView ID="dvUser" runat="server" AutoGenerateRows="False" DataKeyNames="UserId"
                DataSourceID="odsUserDetails" Caption="Add New User" DefaultMode="Insert">
                <Fields>
                    <asp:TemplateField HeaderText="User" SortExpression="UserId">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtUserId" runat="server" Columns="15" MaxLength="50" Text='<%# Bind("UserId") %>'
                                Enabled="false" />
                            <asp:TextBox ID="txtUserName" runat="server" Columns="30" MaxLength="50" Text='<%# Bind("UserName") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="txtUserName"
                                ErrorMessage="User Name is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtUserId" runat="server" Columns="15" MaxLength="50" Text='<%# Bind("UserId") %>' />
                            <asp:RequiredFieldValidator ID="rfvUserId" runat="server" ControlToValidate="txtUserId"
                                ErrorMessage="User ID is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <span class="glyphicon glyphicon-search" id="user-search"></span>
                            <asp:TextBox ID="txtUserName" runat="server" Columns="30" MaxLength="50" Text='<%# Bind("UserName") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="txtUserName"
                                ErrorMessage="User Name is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            (<asp:Label ID="lblUserId" runat="server" Text='<%# Bind("UserId") %>'></asp:Label>)
                    <asp:Label ID="lblUserName" runat="server" Text='<%# Bind("UserName") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="User Role" SortExpression="UserRole">
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlUserRole" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("UserRole") %>'
                                DataSourceID="odsUserRole" DataTextField="CMCodeDesc" DataValueField="CMCode">
                                <asp:ListItem Value="">-- Select User Role --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvUserRole" runat="server" ControlToValidate="ddlUserRole"
                                ErrorMessage="User Role is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <asp:ObjectDataSource ID="odsUserRole" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                    <asp:Parameter DefaultValue="USER_ROLE" Name="catg" Type="String" />
                                    <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:DropDownList ID="ddlUserRole" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("UserRole") %>'
                                DataSourceID="odsUserRole" DataTextField="CMCodeDesc" DataValueField="CMCode">
                                <asp:ListItem Value="">-- Select User Role --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvUserRole" runat="server" ControlToValidate="ddlUserRole"
                                ErrorMessage="User Role is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <asp:ObjectDataSource ID="odsUserRole" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                    <asp:Parameter DefaultValue="USER_ROLE" Name="catg" Type="String" />
                                    <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlUserRole" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("UserRole") %>'
                                DataSourceID="odsUserRole" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="false">
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsUserRole" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                    <asp:Parameter DefaultValue="USER_ROLE" Name="catg" Type="String" />
                                    <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
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
                                Text="Update" ValidationGroup="vgDetails" CssClass="btn btn-primary btn-xs"><span class="glyphicon glyphicon-ok"></span> Update</asp:LinkButton>
                            <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                Text="Cancel" CssClass="btn btn-warning btn-xs"><span class="glyphicon glyphicon-remove"></span> Cancel</asp:LinkButton>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:LinkButton ID="lbtnInsert" runat="server" CausesValidation="True" CommandName="Insert"
                                Text="Insert" ValidationGroup="vgDetails" CssClass="btn btn-primary btn-xs"><span class="glyphicon glyphicon-ok"></span> Insert</asp:LinkButton>
                            <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                Text="Cancel" CssClass="btn btn-warning btn-xs"><span class="glyphicon glyphicon-remove"></span> Cancel</asp:LinkButton>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtnEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                Text="" CssClass="btn btn-primary btn-xs"><span class="glyphicon glyphicon-pencil"></span> Edit</asp:LinkButton>
                            <asp:LinkButton ID="lbtnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                Text="Delete" CssClass="btn btn-danger btn-xs" OnClientClick="return confirm('Are you sure you want to delete this record?');"><span class="glyphicon glyphicon-trash"></span> Delete</asp:LinkButton>
                            <asp:LinkButton ID="lbtnNew" runat="server" CausesValidation="False" CommandName="New"
                                Text="New" CssClass="btn btn-success btn-xs"><span class="glyphicon glyphicon-plus"></span> New</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <asp:ObjectDataSource ID="odsUserDetails" runat="server" SelectMethod="GetAppUserByID"
                TypeName="PolicyTracker.Lib.SettingsBL" DataObjectTypeName="PolicyTracker.Lib.AppUser"
                DeleteMethod="DeleteAppUser" InsertMethod="AddAppUser" OldValuesParameterFormatString="orig{0}"
                UpdateMethod="UpdateAppUser" ConflictDetection="CompareAllValues">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvUsers" DefaultValue="" Name="userId" PropertyName="SelectedDataKey.Values[0]"
                        Type="String" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="appUser" Type="Object" />
                    <asp:Parameter Name="origAppUser" Type="Object" />
                </UpdateParameters>
            </asp:ObjectDataSource>

            <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
