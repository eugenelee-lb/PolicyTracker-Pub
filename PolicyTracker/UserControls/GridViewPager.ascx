<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="GridViewPager.ascx.vb" Inherits="PolicyTracker.GridViewPager" %>

<div>
    <span style="float: left; margin-right: 4px;">
        <asp:ImageButton AlternateText="First page" ToolTip="First page" ID="ImageButtonFirst" runat="server" ImageUrl="~/Images/PgFirst.gif" Width="8" Height="9" CommandName="Page" CommandArgument="First" />
        &nbsp;
        <asp:ImageButton AlternateText="Previous page" ToolTip="Previous page" ID="ImageButtonPrev" runat="server" ImageUrl="~/Images/PgPrev.gif" Width="5" Height="9" CommandName="Page" CommandArgument="Prev" />
        &nbsp;
        Page
        <asp:TextBox ID="TextBoxPage" runat="server" Columns="10" AutoPostBack="true" ontextchanged="TextBoxPage_TextChanged" Width="25px" style="text-align: right" />
        of
        <asp:Label ID="LabelNumberOfPages" runat="server" />
        &nbsp;
        <asp:ImageButton AlternateText="Next page" ToolTip="Next page" ID="ImageButtonNext" runat="server" ImageUrl="~/Images/PgNext.gif" Width="5" Height="9" CommandName="Page" CommandArgument="Next" />
        &nbsp;
        <asp:ImageButton AlternateText="Last page" ToolTip="Last page" ID="ImageButtonLast" runat="server" ImageUrl="~/Images/PgLast.gif" Width="8" Height="9" CommandName="Page" CommandArgument="Last" />
        &nbsp;
        <asp:Label ID="LabelTotalRecordCount" runat="server" Font-Italic="true" />
    </span>
    <span style="float: right; margin-left: 4px;">
        Rows per page:
        <asp:DropDownList ID="DropDownListPageSize" runat="server" AutoPostBack="true" 
            onselectedindexchanged="DropDownListPageSize_SelectedIndexChanged" style="text-align: right">
            <asp:ListItem Value="5" />
            <asp:ListItem Value="10" />
            <asp:ListItem Value="20" />
            <asp:ListItem Value="50" />
            <asp:ListItem Value="100" />
        </asp:DropDownList>
    </span>
</div>