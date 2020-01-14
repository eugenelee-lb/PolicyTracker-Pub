<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Classifications.aspx.vb" Inherits="PolicyTracker.Classifications" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Classifications</h2>

    <table>
        <tr>
            <td>Class Code:</td>
            <td>
                <asp:TextBox ID="txtClassCodeFilter" runat="server" Columns="10"></asp:TextBox>
            </td>
            <td>Description:</td>
            <td>
                <asp:TextBox ID="txtClassDescFilter" runat="server" Columns="20"></asp:TextBox>
            </td>
            <td>Class Type:</td>
            <td>
                <asp:DropDownList ID="ddlClassTypeFilter" runat="server" AppendDataBoundItems="true"
                    DataSourceID="odsClassTypeFilter" DataTextField="ClassType" DataValueField="ClassType"
                    AutoPostBack="false">
                    <asp:ListItem Value="">-- All --</asp:ListItem>
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsClassTypeFilter" runat="server"
                    SelectMethod="GetClassTypes"
                    TypeName="PolicyTracker.Lib.SettingsRepository">
                </asp:ObjectDataSource>
            </td>
            <td>Status:</td>
            <td>
                <asp:DropDownList ID="ddlStatFilter" runat="server" AppendDataBoundItems="false"
                    DataSourceID="odsStat" DataTextField="CMCodeDesc" DataValueField="CMCode"
                    AutoPostBack="false">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsStat" runat="server"
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

            <asp:GridView ID="gvClassifications" runat="server" DataSourceID="odsClassifications"
                AutoGenerateColumns="False" DataKeyNames="ClassCode" AllowPaging="True" AllowSorting="True"
                EmptyDataText="No record has been found.">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" Visible="false" />
                    <asp:BoundField DataField="ClassCode" HeaderText="Code" ReadOnly="True" SortExpression="ClassCode" />
                    <asp:BoundField DataField="ClassDesc" HeaderText="Description" SortExpression="ClassDesc" />
                    <asp:BoundField DataField="ClassType" HeaderText="Class Type" ReadOnly="True" SortExpression="ClassType" />
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
            <asp:ObjectDataSource ID="odsClassifications" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="GetClassificationsByCodeDescTypeStat" TypeName="PolicyTracker.Lib.SettingsRepository" SortParameterName="sortExpression">
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtClassCodeFilter" Name="classCode" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="txtClassDescFilter" Name="classDesc" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="ddlClassTypeFilter" Name="classType" PropertyName="SelectedValue" Type="String" />
                    <asp:ControlParameter ControlID="ddlStatFilter" Name="stat" PropertyName="SelectedValue" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />

<%--
            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="List" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />

            <asp:DetailsView ID="dvClassification" runat="server" AutoGenerateRows="False" DataKeyNames="ClassCode"
                DataSourceID="odsClassification" Caption="Add New Classification" DefaultMode="Insert">
                <Fields>
                    <asp:TemplateField HeaderText="Class Code" SortExpression="ClassCode">
                        <EditItemTemplate>
                            <asp:Label ID="lblClassCode" runat="server" Text='<%# Bind("ClassCode") %>'></asp:Label>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtClassCode" runat="server" Text='<%# Bind("ClassCode") %>' Columns="5"
                                MaxLength="50" onChange="javascript:this.value=this.value.toUpperCase();"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvClassCode" runat="server" ControlToValidate="txtClassCode"
                                ErrorMessage="Class Code is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblClassCode" runat="server" Text='<%# Bind("ClassCode") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Class Desc" SortExpression="ClassDesc">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtClassDesc" runat="server" Text='<%# Bind("ClassDesc") %>' Columns="50" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvClassDesc" runat="server" ControlToValidate="txtClassDesc"
                                ErrorMessage="Class Desc is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtClassDesc" runat="server" Text='<%# Bind("ClassDesc") %>' Columns="50" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvClassDesc" runat="server" ControlToValidate="txtClassDesc"
                                ErrorMessage="Class Desc is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblClassDesc" runat="server" Text='<%# Bind("ClassDesc") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Class Type" SortExpression="ClassType">
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlClassType" runat="server" AppendDataBoundItems="true" SelectedValue='<%# Bind("ClassType")%>'
                                DataSourceID="odsClassType" DataTextField="ClassType" DataValueField="ClassType"
                                AutoPostBack="false">
                                <asp:ListItem Value="">-- Select --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsClassType" runat="server"
                                SelectMethod="GetClassTypes"
                                TypeName="PolicyTracker.Lib.SettingsRepository"></asp:ObjectDataSource>
                            <asp:RequiredFieldValidator ID="rfvClassType" runat="server" ControlToValidate="ddlClassType"
                                ErrorMessage="Class Type is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:DropDownList ID="ddlClassType" runat="server" AppendDataBoundItems="true" SelectedValue='<%# Bind("ClassType")%>'
                                DataSourceID="odsClassType" DataTextField="ClassType" DataValueField="ClassType"
                                AutoPostBack="false">
                                <asp:ListItem Value="">-- Select --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsClassType" runat="server"
                                SelectMethod="GetClassTypes"
                                TypeName="PolicyTracker.Lib.SettingsRepository"></asp:ObjectDataSource>
                            <asp:RequiredFieldValidator ID="rfvClassType" runat="server" ControlToValidate="ddlClassType"
                                ErrorMessage="Class Type is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblClassType" runat="server" Text='<%# Bind("ClassType")%>'></asp:Label>
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
            <asp:ObjectDataSource ID="odsClassification" runat="server" DeleteMethod="DeleteClassification"
                TypeName="PolicyTracker.Lib.SettingsRepository" DataObjectTypeName="PolicyTracker.Lib.Classification"
                InsertMethod="InsertClassification" SelectMethod="GetClassificationByClassCode" UpdateMethod="UpdateClassification"
                OldValuesParameterFormatString="orig{0}" ConflictDetection="CompareAllValues">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvClassifications" DefaultValue="" Name="classCode" PropertyName="SelectedValue"
                        Type="String" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="cls" Type="Object" />
                    <asp:Parameter Name="origCls" Type="Object" />
                </UpdateParameters>
            </asp:ObjectDataSource>
            <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>
--%>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
