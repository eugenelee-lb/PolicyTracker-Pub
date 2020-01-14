<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ADUserSearch.ascx.vb" Inherits="PolicyTracker.ADUserSearch" %>
<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<script>
    function CheckFilter() {
        if ($("input[name$='ADUserSearch$txtLogonName']").val() == ''
        && $("input[name$='ADUserSearch$txtFirstName']").val() == ''
        && $("input[name$='ADUserSearch$txtLastName']").val() == '') {
            window.alert("Please input at least one name condition.");
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
                <td>Department:</td>
                <td>
                    <asp:DropDownList ID="dropDept" runat="server" AppendDataBoundItems="True" DataSourceID="" DataTextField="DeptDesc" DataValueField="ADOU" Enabled="false">
                        <asp:ListItem Value="All">-- All --</asp:ListItem>
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsDepts" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="GetDepartments" TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression"></asp:ObjectDataSource>
                </td>
                <td>User ID:</td>
                <td>
                    <asp:TextBox ID="txtLogonName" runat="server" Columns="15"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>First Name:</td>
                <td>
                    <asp:TextBox ID="txtFirstName" runat="server" Columns="15"></asp:TextBox>
                <td>Last Name:</td>
                <td>
                    <asp:TextBox ID="txtLastName" runat="server" Columns="15"></asp:TextBox>
                </td>
                <td>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" OnClientClick="return CheckFilter();"></asp:Button>
                </td>
            </tr>
        </table>

        <asp:GridView ID="gridADUsers" runat="server" Visible="True" AutoGenerateColumns="false"
            AllowPaging="true" PageSize="10" AllowSorting="true" EmptyDataText="There is no user match with the condition.">
            <Columns>
                <asp:TemplateField HeaderText="User ID" SortExpression="UserName">
                    <ItemTemplate>
                        <asp:Label ID="lblUserName" runat="server" Text='<%# GetAnchor() %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="FirstName" HeaderText="First Name" SortExpression="FirstName"></asp:BoundField>
                <asp:BoundField DataField="MiddleInitial" HeaderText="MI"></asp:BoundField>
                <asp:BoundField DataField="LastName" HeaderText="Last Name" SortExpression="LastName"></asp:BoundField>
                <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title"></asp:BoundField>
                <asp:BoundField DataField="Email" HeaderText="E-mail" SortExpression="Email"></asp:BoundField>
                <asp:BoundField DataField="IsAccountActive" HeaderText="Active" SortExpression="IsAccountActive"></asp:BoundField>
            </Columns>
            <PagerTemplate>
                <asp:GridViewPager ID="GridViewPager1" runat="server" />
            </PagerTemplate>
        </asp:GridView>

    </ContentTemplate>
</asp:UpdatePanel>
