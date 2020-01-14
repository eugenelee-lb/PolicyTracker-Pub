<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="EmpInfo.ascx.vb" Inherits="PolicyTracker.EmpInfo" %>
<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>

<asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
    <ProgressTemplate>
        <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
    </ProgressTemplate>
</asp:UpdateProgress>

<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
    <ContentTemplate>

        <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>

        <asp:FormView ID="fvEmployee" runat="server" AutoGenerateRows="False" DataKeyNames="EmpId" ItemType="PolicyTracker.Lib.vEmployee"
            Caption="" DefaultMode="ReadOnly" Visible="false" SelectMethod="fvEmployee_GetItem" style="border:hidden;">
            <ItemTemplate>
                <table class="DataWebControlStyle">
                    <tr class="RowStyle">
                        <td class="HeaderStyle">Employee ID</td><td colspan="3"><%#:Item.EmpId%></td>
                        <td class="HeaderStyle">Hire Date</td><td><%#:String.Format("{0:M/d/yyyy}", Item.HireDate)%></td>
                        <td class="HeaderStyle">Status</td><td><%#:IIf(Item.Disabled, "Disabled", "Active")%></td>
                    </tr>
                    <tr class="AlternatingRowStyle">
                        <td class="HeaderStyle">First Name</td><td class="ItemStyle"><%#:Item.FirstName%></td>
                        <td class="HeaderStyle">Middle</td><td class="ItemStyle"><%#:Item.MiddleName%></td>
                        <td class="HeaderStyle">Last Name</td><td class="ItemStyle"><%#:Item.LastName%></td>
                        <td class="HeaderStyle">Suffix</td><td class="ItemStyle"><%#:Item.Suffix%></td>
                    </tr>
                    <tr class="RowStyle">
                        <td class="HeaderStyle">Organization</td><td colspan="7" class="ItemStyle"><%#:Item.OrgCode%> - <%#:Item.OrgDesc%></td>
                    </tr>
                    <tr class="AlternatingRowStyle">
                        <td class="HeaderStyle">Classification</td><td colspan="7"><%#:Item.ClassCode%> - <%#:Item.ClassDesc%></td>
                    </tr>
                    <tr class="RowStyle">
                        <td class="HeaderStyle">Email Address</td><td colspan="7"><%#EvalEmail()%></td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:FormView>
    </ContentTemplate>
</asp:UpdatePanel>

