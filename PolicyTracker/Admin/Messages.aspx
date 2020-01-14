<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Messages.aspx.vb" Inherits="PolicyTracker.Messages" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Messages</h2>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <table>
                <tr>
                    <td>Message Text:</td>
                    <td><asp:TextBox ID="txtMsgText" runat="server" Columns="40" MaxLength="500"></asp:TextBox></td>
                    <td></td>
                </tr>
                <tr>
                    <td>Message Title:</td>
                    <td><asp:TextBox ID="txtMsgTitle" runat="server" Columns="40" MaxLength="50"></asp:TextBox></td>
                    <td><asp:Button ID="btnSearch" runat="server" Text="Search" ValidationGroup="Search" CssClass="btn btn-info btn-xs" /></td>
                </tr>
            </table>

            <asp:GridView ID="gvMessages" runat="server" AutoGenerateColumns="False" DataKeyNames="MsgNo"
                DataSourceID="odsMessages" AllowPaging="True" EmptyDataText="No record has been found.">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="MsgNo" HeaderText="Number" ReadOnly="True" SortExpression="MsgNo" />
                    <asp:BoundField DataField="MsgText" HeaderText="Message Text" SortExpression="MsgText" />
                    <asp:BoundField DataField="MsgTitle" HeaderText="Message Title" SortExpression="MsgTitle" />
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" SortExpression="CreateUser" />
                    <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" />
                    <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="LastUpdateUser" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsMessages" runat="server" SelectMethod="GetMessagesByTextAndTitle"
                TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression">
                <SelectParameters>
                    <asp:ControlParameter ControlID="txtMsgText" Name="msgText" PropertyName="Text" Type="String" />
                    <asp:ControlParameter ControlID="txtMsgTitle" Name="msgTitle" PropertyName="Text"
                        Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <br />

            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" ShowModelStateErrors="true" />

            <asp:DetailsView ID="dvMessage" runat="server" AutoGenerateRows="False" Caption="Add New Message" 
                DataKeyNames="MsgNo" DefaultMode="Insert" ItemType="PolicyTracker.Lib.Message"
                SelectMethod="GetMessageByNo" InsertMethod="InsertMessage" UpdateMethod="UpdateMessage" DeleteMethod="DeleteMessage">
                <Fields>
                    <asp:TemplateField HeaderText="Number" SortExpression="MsgNo">
                        <EditItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# BindItem.MsgNo%>'></asp:Label>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtMsgNo" runat="server" Columns="10" MaxLength="10" Text='<%# BindItem.MsgNo%>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvID" runat="server" ControlToValidate="txtMsgNo"
                                ErrorMessage="Message Number is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>&nbsp;
                            <asp:RangeValidator ID="rvID" runat="server" ControlToValidate="txtMsgNo" ErrorMessage="Only numbers are allowed for Message Number" Text="*"
                                Type="Integer" MaximumValue="999999999" MinimumValue="-999999999" ValidationGroup="vgDetails"></asp:RangeValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label3" runat="server" Text='<%# Item.MsgNo%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Message Text" SortExpression="MsgText">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtMsgText" runat="server" Columns="100" onkeyup="return ismaxlength(this, 1000);"
                                TextMode="MultiLine" Rows="3" Text='<%# BindItem.MsgText%>' ValidateRequestMode="Disabled"></asp:TextBox>
                            <%--<asp:RequiredFieldValidator ID="rfvText" runat="server" ControlToValidate="txtMsgText"
                                ErrorMessage="Message Text is required" Text="*" Display="dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>--%>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtMsgText" runat="server" Columns="100" onkeyup="return ismaxlength(this, 1000);"
                                TextMode="MultiLine" Rows="3" Text='<%# BindItem.MsgText%>'></asp:TextBox>
                            <%--<asp:RequiredFieldValidator ID="rfvText" runat="server" ControlToValidate="txtMsgText"
                                ErrorMessage="Message Text is required" Text="*" Display="dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>--%>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label6" runat="server" Text='<%# Item.MsgText%>' Visible="true"></asp:Label>
                            <asp:Label ID="Label1" runat="server" Text='<%#: Item.MsgText%>' Visible="false"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Message Title" SortExpression="MsgTitle">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtMsgTitle" runat="server" Columns="50" MaxLength="50" Text='<%# BindItem.MsgTitle%>'></asp:TextBox>
                            <%--<asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtMsgTitle" Enabled="true"
                                ValidationGroup="vgDetails" ErrorMessage="Message Title is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>--%>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtMsgTitle" runat="server" Columns="50" MaxLength="50" Text='<%# BindItem.MsgTitle%>'></asp:TextBox>
                            <%--<asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtMsgTitle" Enabled="true"
                                ValidationGroup="vgDetails" ErrorMessage="Message Title is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>--%>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Item.MsgTitle%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" InsertVisible="False" DataFormatString="{0:M/d/yyyy h:mm:ss.fff tt}"
                        ReadOnly="True" SortExpression="CreateDT" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" InsertVisible="False"
                        ReadOnly="True" SortExpression="CreateUser" />
                    <asp:TemplateField HeaderText="Last Update Date" SortExpression="LastUpdateDT"
                        InsertVisible="False">
                        <EditItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# BindItem.LastUpdateDT%>'></asp:Label>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# BindItem.LastUpdateDT%>'></asp:Label>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label4" runat="server" Text='<%# Item.LastUpdateDT%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Last Update User" SortExpression="LastUpdateUser"
                        InsertVisible="False">
                        <EditItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# BindItem.LastUpdateUser%>'></asp:Label>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# BindItem.LastUpdateUser%>'></asp:Label>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label5" runat="server" Text='<%# Item.LastUpdateUser%>'></asp:Label>
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
            <asp:ObjectDataSource ID="odsMessageDetails" runat="server" ConflictDetection="CompareAllValues"
                DeleteMethod="DeleteMessage" InsertMethod="AddMessage" OldValuesParameterFormatString="orig{0}"
                SelectMethod="GetMessageByMsgNo" TypeName="PolicyTracker.Lib.SettingsBL" DataObjectTypeName="PolicyTracker.Lib.Message"
                UpdateMethod="UpdateMessage">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvMessages" DefaultValue="" Name="msgNo" PropertyName="SelectedValue"
                        Type="Int32" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="message" Type="Object" />
                    <asp:Parameter Name="origMessage" Type="Object" />
                </UpdateParameters>
            </asp:ObjectDataSource>

            <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
