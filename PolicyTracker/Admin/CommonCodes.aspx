<%@ Page Title="Common Codes" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="CommonCodes.aspx.vb" Inherits="PolicyTracker.CommonCodes" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Common Codes</h2>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <table>
                <tr>
                    <td>Category:</td>
                    <td>
                        <asp:DropDownList ID="dropCatg" runat="server" AppendDataBoundItems="false"
                            DataSourceID="odsCommonCodeDropdown" DataTextField="CMCodeDesc" DataValueField="CMCode"
                            AutoPostBack="false">
                            <asp:ListItem Value="00">-- Categories --</asp:ListItem>
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsCommonCodeDropdown" runat="server"
                            SelectMethod="GetCommonCodesByCatg"
                            TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression">
                            <SelectParameters>
                                <asp:Parameter Name="catg" Type="String" DefaultValue="00" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td>Description:</td>
                    <td>
                        <asp:TextBox ID="txtDesc" runat="server" Columns="20"></asp:TextBox>
                    </td>
                    <td>Status:</td>
                    <td>
                        <asp:DropDownList ID="dropStat" runat="server" AppendDataBoundItems="false"
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

            <asp:GridView ID="gvCommonCodes" runat="server" AutoGenerateColumns="False" DataKeyNames="CMId"
                DataSourceID="odsCommonCodeList" AllowPaging="True" AllowSorting="True" EmptyDataText="No record has been found.">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="CMId" HeaderText="ID" Visible="false" />
                    <asp:BoundField DataField="CMCatg" HeaderText="Category" />
                    <asp:BoundField DataField="CMCode" HeaderText="Code" SortExpression="CMCode" />
                    <asp:BoundField DataField="CMCodeDesc" HeaderText="Description" SortExpression="CMCodeDesc" />
                    <asp:BoundField DataField="DispOrder" HeaderText="Order" SortExpression="DispOrder" ItemStyle-HorizontalAlign="Right" />
                    <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" SortExpression="CreateUser" />
                    <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" />
                    <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="LastUpdateUser" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsCommonCodeList" runat="server" SelectMethod="GetCommonCodesByCatgDesc"
                TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression">
                <SelectParameters>
                    <asp:ControlParameter ControlID="dropCatg" Name="catg" PropertyName="SelectedValue" Type="String" />
                    <asp:ControlParameter ControlID="txtDesc" Name="desc" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="dropStat" Name="stat" PropertyName="SelectedValue" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />

            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />

            <asp:DetailsView ID="dvCommonCode" runat="server" AutoGenerateRows="False" DataKeyNames="CmId"
                DataSourceID="odsCommonCodeDetails" Caption="Add New Common Code" DefaultMode="Insert">
                <Fields>
                    <asp:BoundField DataField="CmId" HeaderText="ID" SortExpression="CmId" Visible="false" />
                    <asp:TemplateField HeaderText="Category" SortExpression="CMCatg">
                        <EditItemTemplate>
                            <asp:DropDownList ID="ddlCMCatg" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("CMCatg") %>'
                                DataSourceID="odsCMCatg" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="false">
                                <asp:ListItem Value="00">-- Root Category --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsCMCatg" runat="server"
                                SelectMethod="GetCommonCodesByCatg" TypeName="PolicyTracker.Lib.SettingsBL">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                    <asp:Parameter DefaultValue="00" Name="catg" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:DropDownList ID="ddlCMCatg" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("CMCatg") %>'
                                DataSourceID="odsCMCatg" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="false">
                                <asp:ListItem Value="00">-- Root Category --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsCMCatg" runat="server"
                                SelectMethod="GetCommonCodesByCatg" TypeName="PolicyTracker.Lib.SettingsBL">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                    <asp:Parameter DefaultValue="00" Name="catg" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlCMCatg" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("CMCatg") %>'
                                DataSourceID="odsCMCatg" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="false">
                                <asp:ListItem Value="00">-- Root Category --</asp:ListItem>
                            </asp:DropDownList>
                            <asp:ObjectDataSource ID="odsCMCatg" runat="server"
                                SelectMethod="GetCommonCodesByCatg" TypeName="PolicyTracker.Lib.SettingsBL">
                                <SelectParameters>
                                    <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                    <asp:Parameter DefaultValue="00" Name="catg" Type="String" />
                                </SelectParameters>
                            </asp:ObjectDataSource>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Code" SortExpression="CMCode">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCMCode" runat="server" Columns="50" MaxLength="100" Text='<%# Bind("CMCode") %>'
                                onChange="javascript:this.value=this.value.toUpperCase();" Enabled="false"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCMCode" runat="server" ControlToValidate="txtCMCode"
                                ValidationGroup="vgDetails" ErrorMessage="Code is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtCMCode" runat="server" Columns="50" MaxLength="100" Text='<%# Bind("CMCode") %>'
                                onChange="javascript:this.value=this.value.toUpperCase();"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCMCode" runat="server" ControlToValidate="txtCMCode"
                                ValidationGroup="vgDetails" ErrorMessage="Code is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblCMCode" runat="server" Text='<%# Bind("CMCode") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description" SortExpression="CMCodeDesc">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtCMCodeDesc" runat="server" Columns="100" MaxLength="255" Text='<%# Bind("CMCodeDesc") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCMCodeDesc" runat="server" ControlToValidate="txtCMCodeDesc"
                                ValidationGroup="vgDetails" ErrorMessage="Description is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtCMCodeDesc" runat="server" Columns="100" MaxLength="255" Text='<%# Bind("CMCodeDesc") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvCMCodeDesc" runat="server" ControlToValidate="txtCMCodeDesc"
                                ValidationGroup="vgDetails" ErrorMessage="Description is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblCMCodeDesc" runat="server" Text='<%# Bind("CMCodeDesc") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Display Order" SortExpression="DispOrder">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDispOrder" runat="server" Columns="10" MaxLength="10" Text='<%# Bind("DispOrder") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDispOrder" runat="server" ControlToValidate="txtDispOrder"
                                ErrorMessage="Display Order is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>&nbsp;
                            <asp:RangeValidator ID="rvID" runat="server" ControlToValidate="txtDispOrder" ErrorMessage="(number between 0 to 999,999 only)"
                                Type="Integer" MaximumValue="999999" MinimumValue="0" ValidationGroup="vgDetails"></asp:RangeValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtDispOrder" runat="server" Columns="10" MaxLength="10" Text='<%# Bind("DispOrder") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDispOrder" runat="server" ControlToValidate="txtDispOrder"
                                ErrorMessage="Display Order is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>&nbsp;
                            <asp:RangeValidator ID="rvID" runat="server" ControlToValidate="txtDispOrder" ErrorMessage="(number between 0 to 999,999 only)"
                                Type="Integer" MaximumValue="999999" MinimumValue="0" ValidationGroup="vgDetails"></asp:RangeValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblDispOrder" runat="server" Text='<%# Bind("DispOrder") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" InsertVisible="false" />
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
                                Text="Edit" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnNew" runat="server" CausesValidation="False" CommandName="New"
                                Text="New" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                Text="Delete" CssClass="btn btn-default btn-xs" OnClientClick="return confirm('Are you sure you want to delete this record?');"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <asp:ObjectDataSource ID="odsCommonCodeDetails" runat="server" DeleteMethod="DeleteCommonCode"
                TypeName="PolicyTracker.Lib.SettingsBL" DataObjectTypeName="PolicyTracker.Lib.CommonCode"
                InsertMethod="AddCommonCode" SelectMethod="GetCommonCodeByCMId" UpdateMethod="UpdateCommonCode"
                OldValuesParameterFormatString="orig{0}" ConflictDetection="CompareAllValues">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvCommonCodes" DefaultValue="" Name="cmId" PropertyName="SelectedValue"
                        Type="Int32" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="commonCode" Type="Object" />
                    <asp:Parameter Name="origCommonCode" Type="Object" />
                </UpdateParameters>
            </asp:ObjectDataSource>
            <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
