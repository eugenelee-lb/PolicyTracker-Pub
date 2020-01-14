<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Employees.aspx.vb" Inherits="PolicyTracker.Employees" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script>
        function pageLoad() {
            $(".select_chosen").chosen({
                disable_search_threshold: 10,
                no_results_text: "Nothing found!",
                search_contains: true
            });

            $(".select_chosen_multi").chosen({
                width: "400px",
                placeholder_text_multiple: "-- All --",
                search_contains: true
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src='<%: Page.ResolveUrl("~/Scripts/chosen.jquery.min.js")%>' type="text/javascript"></script>
    <link href='<%: Page.ResolveClientUrl("~/Content/chosen.min.css")%>' rel="stylesheet" />

    <h2>Employees</h2>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lblFilterName" runat="server" Text="Name:" AssociatedControlID="txtFilterName"></asp:Label>
                        <asp:TextBox ID="txtFilterName" runat="server" Columns="15" ClientIDMode="Static"></asp:TextBox>
                    </td>
                    <td>
                        <asp:Label ID="lblFilterOrg" runat="server" Text="Org:" AssociatedControlID="lstFilterOrg"></asp:Label>
                        <asp:ListBox ID="lstFilterOrg" runat="server" SelectionMode="Multiple"
                            DataSourceID="odsFilterOrg" DataTextField="OrgDesc" DataValueField="OrgCode"
                            AutoPostBack="false" CssClass="select_chosen_multi">
                        </asp:ListBox>
                        <asp:ObjectDataSource ID="odsFilterOrg" runat="server"
                            SelectMethod="GetOrgsByCodeDescStat"
                            TypeName="PolicyTracker.Lib.SettingsRepository" SortParameterName="sortExpression">
                            <SelectParameters>
                                <asp:Parameter Name="orgCode" Type="String" DefaultValue="" />
                                <asp:Parameter Name="orgDesc" Type="String" DefaultValue="" />
                                <asp:Parameter Name="stat" Type="String" DefaultValue="A" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblFilterStatus" runat="server" Text="Status:" AssociatedControlID="ddlFilterStat"></asp:Label>
                        <asp:DropDownList ID="ddlFilterStat" runat="server" AppendDataBoundItems="false"
                            DataSourceID="odsFilterStat" DataTextField="CMCodeDesc" DataValueField="CMCode"
                            AutoPostBack="false">
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsFilterStat" runat="server"
                            SelectMethod="GetCommonCodesByCatgStatus"
                            TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression">
                            <SelectParameters>
                                <asp:Parameter Name="catg" Type="String" DefaultValue="FILTER_STAT" />
                                <asp:Parameter Name="stat" Type="Boolean" DefaultValue="true" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td>
                        <asp:Label ID="lblFilterClass" runat="server" Text="Class:" AssociatedControlID="lstFilterClass"></asp:Label>
                        <asp:ListBox ID="lstFilterClass" runat="server" SelectionMode="Multiple"
                            DataSourceID="odsFilterClass" DataTextField="ClassCodeDesc" DataValueField="ClassCode"
                            AutoPostBack="false" CssClass="select_chosen_multi">
                        </asp:ListBox>
                        <asp:ObjectDataSource ID="odsFilterClass" runat="server"
                            SelectMethod="GetClassificationsByStatWithCodeDesc"
                            TypeName="PolicyTracker.Lib.SettingsRepository">
                            <SelectParameters>
                                <asp:Parameter Name="sortExpression" Type="String" DefaultValue="ClassCode" />
                                <asp:Parameter Name="stat" Type="String" DefaultValue="A" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-info btn-xs" />
                    </td>
                </tr>
            </table>

            <asp:GridView ID="gvEmployees" runat="server" AutoGenerateColumns="False" 
                itemtype="PolicyTracker.Lib.vEmployee" SelectMethod="gvEmployees_GetData" DataKeyNames="EmpId" 
                AllowPaging="True" AllowSorting="True"
                EmptyDataText="No record has been found." Visible="false">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" Visible="false" />
                    <asp:BoundField DataField="EmpId" HeaderText="Emp ID" ReadOnly="True" SortExpression="EmpId" />
                    <asp:BoundField DataField="PIN" HeaderText="PIN" SortExpression="PIN" Visible="false" />
                    <asp:BoundField DataField="FirstName" HeaderText="F Name" SortExpression="FirstName" />
                    <asp:BoundField DataField="MiddleName" HeaderText="M Name" SortExpression="MiddleName" />
                    <asp:BoundField DataField="LastName" HeaderText="L Name" SortExpression="LastName" />
                    <asp:BoundField DataField="Suffix" HeaderText="Suffix" SortExpression="Suffix" />
                    <asp:TemplateField HeaderText="Org" SortExpression="OrgCode">
                        <ItemTemplate>
                            <asp:Label ID="lblOrgCode" runat="server" Text='<%# Bind("OrgCode")%>' ToolTip='<%# Bind("OrgDesc")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Class" SortExpression="ClassCode">
                        <ItemTemplate>
                            <asp:Label ID="lblClassCode" runat="server" Text='<%# Bind("ClassCode")%>' ToolTip='<%# Bind("ClassDesc")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="HireDate" HeaderText="Hire Date" SortExpression="HireDate" DataFormatString="{0:MM/dd/yyyy}" />
                    <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled?" SortExpression="Disabled" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" />
                    <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>
            <br />

            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />

            <asp:DetailsView ID="dvEmployee" runat="server" AutoGenerateRows="False" DataKeyNames="EmpId"
                DataSourceID="odsEmployee" Caption="Employee Details" DefaultMode="ReadOnly" Visible="false">
                <Fields>
                    <asp:TemplateField HeaderText="Department Code" SortExpression="DeptCode">
                        <ItemTemplate>
                            <asp:Label ID="lblDeptCode" runat="server" Text='<%# Bind("DeptCode") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Department Desc" SortExpression="DeptDesc">
                        <ItemTemplate>
                            <asp:Label ID="lblDeptDesc" runat="server" Text='<%# Bind("DeptDesc") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Famis Org Code" SortExpression="FamisOrgCode">
                        <ItemTemplate>
                            <asp:Label ID="lblFamisOrgCode" runat="server" Text='<%# Bind("FamisOrgCode")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Active Directory OU" SortExpression="ADOU">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtADOU" runat="server" Text='<%# Bind("ADOU") %>' Columns="30" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvADOU" runat="server" ControlToValidate="txtADOU"
                                ErrorMessage="Active Directory OU is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>&nbsp;
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtADOU" runat="server" Text='<%# Bind("ADOU") %>' Columns="30" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvADOU" runat="server" ControlToValidate="txtADOU"
                                ErrorMessage="Active Directory OU is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>&nbsp;
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblADOU" runat="server" Text='<%# Bind("ADOU") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Default User Group" SortExpression="DefaultUserGroup">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDefaultUserGroup" runat="server" Text='<%# Bind("DefaultUserGroup") %>' Columns="30" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDefaultUserGroup" runat="server" ControlToValidate="txtDefaultUserGroup"
                                ErrorMessage="Default User Group is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>&nbsp;
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtDefaultUserGroup" runat="server" Text='<%# Bind("DefaultUserGroup") %>' Columns="30" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDefaultUserGroup" runat="server" ControlToValidate="txtDefaultUserGroup"
                                ErrorMessage="Default User Group is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>&nbsp;
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblDefaultUserGroup" runat="server" Text='<%# Bind("DefaultUserGroup") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled?" SortExpression="Disabled" InsertVisible="false" />
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" InsertVisible="False" DataFormatString="{0:M/d/yyyy h:mm:ss.fff tt}"
                        ReadOnly="True" SortExpression="CreateDT" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" InsertVisible="False"
                        ReadOnly="True" SortExpression="CreateUser" />
                    <asp:TemplateField HeaderText="Last Update Date" SortExpression="LastUpdateDate" InsertVisible="False">
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
                                Text="Edit" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnNew" runat="server" CausesValidation="False" CommandName="New"
                                Text="New" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                Text="Delete" CssClass="btn btn-default btn-xs" OnClientClick="return confirm('Are you sure you want to delete this record?');"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <asp:ObjectDataSource ID="odsEmployee" runat="server" 
                TypeName="PolicyTracker.Lib.SettingsRepository" DataObjectTypeName="PolicyTracker.Lib.Employee"
                SelectMethod="GetEmployeesByNameOrgClassStat" 
                OldValuesParameterFormatString="orig{0}" ConflictDetection="CompareAllValues">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvEmployees" DefaultValue="" Name="empId" PropertyName="SelectedValue"
                        Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
