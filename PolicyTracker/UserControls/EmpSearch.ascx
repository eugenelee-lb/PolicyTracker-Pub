<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EmpSearch.ascx.vb" Inherits="PolicyTracker.EmpSearch" %>
<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<script>
    function CheckFilter() {
        if ($("input[name$='EmpSearch$txtFilterName']").val() == ''
        && $("select[name$='EmpSearch$ddlFilterOrg']").val() == ''
        && $("select[name$='EmpSearch$ddlFilterClass']").val() == '') {
            window.alert("Please input at least one condition in Name, Org, and Class.");
            return false;
        }
        else { return true; }
    }
</script>

<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
    <ProgressTemplate>
        <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
    </ProgressTemplate>
</asp:UpdateProgress>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
    <ContentTemplate>

        <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>

        <table>
            <tr>
                <td>
                    <asp:Label ID="lblFilterName" runat="server" Text="Name:" AssociatedControlID="txtFilterName"></asp:Label>
                    <asp:TextBox ID="txtFilterName" runat="server" Columns="15" ClientIDMode="Static"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="lblFilterOrg" runat="server" Text="Org:" AssociatedControlID="ddlFilterOrg"></asp:Label>
                    <asp:DropDownList ID="ddlFilterOrg" runat="server" AppendDataBoundItems="true"
                        DataSourceID="odsFilterOrg" DataTextField="OrgDesc" DataValueField="OrgCode"
                        AutoPostBack="false" CssClass="select_chosen_e">
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
                        TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression" OldValuesParameterFormatString="original_{0}">
                        <SelectParameters>
                            <asp:Parameter Name="sortExpression" Type="String" />
                            <asp:Parameter Name="catg" Type="String" DefaultValue="FILTER_STAT" />
                            <asp:Parameter Name="stat" Type="Boolean" DefaultValue="true" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
                <td>
                    <asp:Label ID="lblFilterClass" runat="server" Text="Class:" AssociatedControlID="ddlFilterClass"></asp:Label>
                    <asp:DropDownList ID="ddlFilterClass" runat="server" AppendDataBoundItems="true"
                        DataSourceID="odsFilterClass" DataTextField="ClassCodeDesc" DataValueField="ClassCode"
                        AutoPostBack="false" CssClass="select_chosen_e">
                        <asp:ListItem Text="-- All --" Value=""></asp:ListItem>
                    </asp:DropDownList>
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
                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClientClick="return CheckFilter();" CssClass="btn btn-info btn-xs" />
                </td>
            </tr>
        </table>

        <asp:GridView ID="gvEmployees" runat="server" DataSourceID="odsEmployees" DataKeyNames="EmpId" 
            AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True" 
            EmptyDataText="No record was found." Visible="False">
            <Columns>
                <asp:TemplateField HeaderText="Emp ID" SortExpression="EmpId">
                    <ItemTemplate>
                        <asp:Label ID="lblEmpId" runat="server" Text='<%# GetAnchor() %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="FirstName" HeaderText="F Name" SortExpression="FirstName" />
                <asp:BoundField DataField="MiddleName" HeaderText="M Name" SortExpression="MiddleName" />
                <asp:BoundField DataField="LastName" HeaderText="L Name" SortExpression="LastName" />
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
            </Columns>
            <PagerTemplate>
                <asp:GridViewPager ID="GridViewPager1" runat="server" />
            </PagerTemplate>
        </asp:GridView>
        <asp:ObjectDataSource ID="odsEmployees" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="GetVEmployeesByNameOrgClassStat" TypeName="PolicyTracker.Lib.SettingsRepository" SortParameterName="sortExpression">
            <SelectParameters>
                <asp:Parameter Name="sortExpression" Type="String" />
                <asp:ControlParameter ControlID="txtFilterName" Name="name" PropertyName="Text" Type="String" />
                <asp:ControlParameter ControlID="ddlFilterOrg" Name="orgCode" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="ddlFilterClass" Name="classCode" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="ddlFilterStat" Name="stat" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>

    </ContentTemplate>
</asp:UpdatePanel>

