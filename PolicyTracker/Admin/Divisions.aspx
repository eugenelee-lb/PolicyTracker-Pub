<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Divisions.aspx.vb" Inherits="PolicyTracker.Divisions" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Divisions</h2>

    <table>
        <tr>
            <td>Bureau:</td>
            <td>
                <asp:DropDownList ID="ddlFilterBureau" runat="server" AppendDataBoundItems="true"
                    DataSourceID="odsFilterBureau" DataTextField="BurDesc" DataValueField="BurCode"
                    AutoPostBack="false">
                    <asp:ListItem Value="">-- All Bureaus --</asp:ListItem>
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsFilterBureau" runat="server"
                    SelectMethod="GetBureausByDeptDescStat"
                    TypeName="PolicyTracker.Lib.SettingsRepository" SortParameterName="sortExpression">
                    <SelectParameters>
                        <asp:Parameter Name="deptCode" Type="String" DefaultValue="" />
                        <asp:Parameter Name="burDesc" Type="String" DefaultValue="" />
                        <asp:Parameter Name="stat" Type="String" DefaultValue="A" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </td>
            <td>Description:</td>
            <td>
                <asp:TextBox ID="txtFilterDivDesc" runat="server" Columns="20"></asp:TextBox>
            </td>
            <td>Status:</td>
            <td>
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
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-info btn-xs" />
            </td>
        </tr>
    </table>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <asp:GridView ID="gvDivisions" runat="server" DataSourceID="odsDivisions"
                AutoGenerateColumns="False" DataKeyNames="DivCode" AllowPaging="True" AllowSorting="True"
                EmptyDataText="No record has been found.">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" Visible="false" />
                    <asp:BoundField DataField="DivCode" HeaderText="Code" ReadOnly="True" SortExpression="DivCode" />
                    <asp:BoundField DataField="DivDesc" HeaderText="Description" SortExpression="DivDesc" />
                    <asp:BoundField DataField="BurCode" HeaderText="Bur Code" ReadOnly="True" SortExpression="BurCode" />
                    <asp:BoundField DataField="CMDept" HeaderText="CM Dept" SortExpression="CMDept" />
                    <asp:BoundField DataField="PersUnitCode" HeaderText="Pers Unit Code" SortExpression="PersUnitCode" />
                    <asp:BoundField DataField="FamisOrg" HeaderText="Famis Org" SortExpression="FamisOrg" />
                    <asp:BoundField DataField="FamisDefaultIndex" HeaderText="Famis Default Index" SortExpression="FamisDefaultIndex" />
                    <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled?" SortExpression="Disabled" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" SortExpression="CreateUser" Visible="false" />
                    <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" />
                    <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="LastUpdateUser" Visible="false" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsDivisions" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="GetDivisionsByDeptBurDescStat" TypeName="PolicyTracker.Lib.SettingsRepository" SortParameterName="sortExpression">
                <SelectParameters>
                    <asp:Parameter Name="deptCode" Type="String" DefaultValue="" />
                    <asp:ControlParameter ControlID="ddlFilterBureau" Name="burCode" PropertyName="SelectedValue" Type="String" />
                    <asp:ControlParameter ControlID="txtFilterDivDesc" Name="divDesc" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="ddlFilterStat" Name="stat" PropertyName="SelectedValue" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />

<%--
            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="List" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />

            <asp:DetailsView ID="dvDivision" runat="server" AutoGenerateRows="False" DataKeyNames="DivCode"
                DataSourceID="odsDivision" Caption="Add New Division" DefaultMode="Insert">
                <Fields>
                    <asp:TemplateField HeaderText="Division Code" SortExpression="DivCode">
                        <EditItemTemplate>
                            <asp:Label ID="lblDivCode" runat="server" Text='<%# Bind("DivCode") %>'></asp:Label>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtDivCode" runat="server" Text='<%# Bind("DivCode") %>' Columns="5"
                                MaxLength="50" onChange="javascript:this.value=this.value.toUpperCase();"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDivCode" runat="server" ControlToValidate="txtDivCode"
                                ErrorMessage="Division Code is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblDivCode" runat="server" Text='<%# Bind("DivCode") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Division Desc" SortExpression="divDesc">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtdivDesc" runat="server" Text='<%# Bind("divDesc") %>' Columns="50" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvdivDesc" runat="server" ControlToValidate="txtdivDesc"
                                ErrorMessage="Division Desc is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtdivDesc" runat="server" Text='<%# Bind("divDesc") %>' Columns="50" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvdivDesc" runat="server" ControlToValidate="txtdivDesc"
                                ErrorMessage="Division Desc is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lbldivDesc" runat="server" Text='<%# Bind("divDesc") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Bureau" SortExpression="BurCode">
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlBurCode" runat="server" AppendDataBoundItems="true" SelectedValue='<%# Bind("BurCode")%>'
                                DataSourceID="odsBurCode" DataTextField="BurDesc" DataValueField="BurCode"
                                AutoPostBack="false">
                                <asp:ListItem Value="">-- Select Bureau --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsBurCode" runat="server"
                                SelectMethod="GetBureaus"
                                TypeName="PolicyTracker.Lib.PTRepository" SortParameterName="sortExpression"></asp:ObjectDataSource>
                            <asp:RequiredFieldValidator ID="rfvBurCode" runat="server" ControlToValidate="ddlBurCode"
                                ErrorMessage="Bureau is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:DropDownList ID="ddlBurCode" runat="server" AppendDataBoundItems="true" SelectedValue='<%# Bind("BurCode")%>'
                                DataSourceID="odsBurCode" DataTextField="BurDesc" DataValueField="BurCode"
                                AutoPostBack="false">
                                <asp:ListItem Value="">-- Select Bureau --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsBurCode" runat="server"
                                SelectMethod="GetBureaus"
                                TypeName="PolicyTracker.Lib.PTRepository" SortParameterName="sortExpression"></asp:ObjectDataSource>
                            <asp:RequiredFieldValidator ID="rfvBurCode" runat="server" ControlToValidate="ddlBurCode"
                                ErrorMessage="Bureau is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblBurCode" runat="server" Text='<%# Bind("BurCode")%>'></asp:Label>
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
            <asp:ObjectDataSource ID="odsDivision" runat="server" DeleteMethod="DeleteDivision"
                TypeName="PolicyTracker.Lib.PTRepository" DataObjectTypeName="PolicyTracker.Lib.Division"
                InsertMethod="InsertDivision" SelectMethod="GetDivisionByCode" UpdateMethod="UpdateDivision"
                OldValuesParameterFormatString="orig{0}" ConflictDetection="CompareAllValues">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvDivisions" DefaultValue="" Name="DivCode" PropertyName="SelectedValue"
                        Type="String" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="division" Type="Object" />
                    <asp:Parameter Name="origDivision" Type="Object" />
                </UpdateParameters>
            </asp:ObjectDataSource>
            <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>
--%>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
