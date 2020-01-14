<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Departments.aspx.vb" Inherits="PolicyTracker.Departments" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Departments</h2>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <asp:GridView ID="gvDepartments" runat="server" DataSourceID="odsDepartmentsGrid"
                AutoGenerateColumns="False" DataKeyNames="DeptCode" AllowPaging="True" AllowSorting="True"
                EmptyDataText="No record has been found.">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="DeptCode" HeaderText="Code" ReadOnly="True" SortExpression="DeptCode" />
                    <asp:BoundField DataField="DeptDesc" HeaderText="Description" SortExpression="DeptDesc" />
                    <asp:BoundField DataField="FamisOrgCode" HeaderText="Famis Org Code" ReadOnly="True" SortExpression="FamisOrg" />
                    <asp:BoundField DataField="ADOU" HeaderText="AD OU" SortExpression="ADOU" />
                    <asp:BoundField DataField="DefaultUserGroup" HeaderText="Default User Group" SortExpression="DefaultUserGroup" />
                    <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled?" SortExpression="Disabled" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" SortExpression="CreateUser" />
                    <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" />
                    <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="LastUpdateUser" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsDepartmentsGrid" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="GetDepartments" TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression">
            </asp:ObjectDataSource>
            <br />

            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />

            <asp:DetailsView ID="dvDepartment" runat="server" AutoGenerateRows="False" DataKeyNames="DeptCode"
                DataSourceID="odsDepartmentDetails" Caption="Add New Department" DefaultMode="Insert">
                <Fields>
                    <asp:TemplateField HeaderText="Department Code" SortExpression="DeptCode">
                        <EditItemTemplate>
                            <asp:Label ID="lblDeptCode" runat="server" Text='<%# Bind("DeptCode") %>'></asp:Label>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtDeptCode" runat="server" Text='<%# Bind("DeptCode") %>' Columns="5"
                                MaxLength="50" onChange="javascript:this.value=this.value.toUpperCase();"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDeptCode" runat="server" ControlToValidate="txtDeptCode"
                                ErrorMessage="Department Code is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblDeptCode" runat="server" Text='<%# Bind("DeptCode") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Department Desc" SortExpression="DeptDesc">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDeptDesc" runat="server" Text='<%# Bind("DeptDesc") %>' Columns="50" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDeptDesc" runat="server" ControlToValidate="txtDeptDesc"
                                ErrorMessage="Department Desc is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtDeptDesc" runat="server" Text='<%# Bind("DeptDesc") %>' Columns="50" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDeptDesc" runat="server" ControlToValidate="txtDeptDesc"
                                ErrorMessage="Department Desc is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblDeptDesc" runat="server" Text='<%# Bind("DeptDesc") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Famis Org Code" SortExpression="FamisOrgCode">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtFamisOrgCode" runat="server" Text='<%# Bind("FamisOrgCode")%>' Columns="5" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFamisOrgCode" runat="server" ControlToValidate="txtFamisOrgCode"
                                ErrorMessage="Famis Org Code is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtFamisOrgCode" runat="server" Text='<%# Bind("FamisOrgCode")%>' Columns="5" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvFamisOrgCode" runat="server" ControlToValidate="txtFamisOrgCode"
                                ErrorMessage="Famis Org Code is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
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
            <asp:ObjectDataSource ID="odsDepartmentDetails" runat="server" DeleteMethod="DeleteDepartment"
                TypeName="PolicyTracker.Lib.SettingsBL" DataObjectTypeName="PolicyTracker.Lib.Department"
                InsertMethod="AddDepartment" SelectMethod="GetDeptByDeptCode" UpdateMethod="UpdateDepartment"
                OldValuesParameterFormatString="orig{0}" ConflictDetection="CompareAllValues">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvDepartments" DefaultValue="" Name="deptCode" PropertyName="SelectedValue"
                        Type="String" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="department" Type="Object" />
                    <asp:Parameter Name="origDepartment" Type="Object" />
                </UpdateParameters>
            </asp:ObjectDataSource>
            <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
