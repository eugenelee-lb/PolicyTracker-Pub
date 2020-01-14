<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="RecipGroupMembers.ascx.vb" Inherits="PolicyTracker.RecipGroupMembers" %>
<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
    <ProgressTemplate>
        <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
    </ProgressTemplate>
</asp:UpdateProgress>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
    <ContentTemplate>

        <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>

        <asp:GridView ID="gvEmployees" runat="server" DataSourceID="" DataKeyNames="EmpId" SelectMethod="gvEmployees_GetData" 
            AutoGenerateColumns="false" AllowPaging="true" PageSize="10" AllowSorting="true" ItemType="Policytracker.Lib.vEmployee"
            EmptyDataText="No record was found." Visible="false">
            <Columns>
                <asp:BoundField DataField="EmpId" HeaderText="Emp ID" SortExpression="EmpId" />
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

    </ContentTemplate>
</asp:UpdatePanel>

