<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Release.aspx.vb" Inherits="PolicyTracker.Release" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/EmpInfo.ascx" TagPrefix="asp" TagName="EmpInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function pageLoad() {
            $(".select_chosen").chosen({
                disable_search_threshold: 10,
                no_results_text: "Nothing found!",
                search_contains: true
            });

            // keep the current tab active bootstrap after a page reload/postback
            $('a[data-toggle="tab"]').on('shown.bs.tab', function () {
                //save the latest tab; use cookies if you like 'em better:
                sessionStorage.setItem('lastTab_Rel', $(this).attr('href'));
            });
            //go to the latest tab, if it exists:
            var lastTab = sessionStorage.getItem('lastTab_Rel');
            if (lastTab && $('a[href="' + lastTab + '"]').is(":visible")) {
                $('a[href="' + lastTab + '"]').tab('show');
            }
            else {
                // Set the first tab if cookie do not exist
                $('a[data-toggle="tab"]:first').tab('show');
            }

            $("input[name$='txtNoticeDate']").inputmask("m/d/y");
            $("input[name$='txtNoticeDate']").datepicker({
                changeMonth: true,
                changeYear: true
            });
        }

        function showEmpInfo() {
            $("#emp-info-modal").modal('show');
            return true;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src='<%: Page.ResolveUrl("~/Scripts/chosen.jquery.min.js")%>' type="text/javascript"></script>
    <link href='<%: Page.ResolveClientUrl("~/Content/chosen.min.css")%>' rel="stylesheet" />

    <h2>Release Details</h2>

    <div id="emp-info-modal" class="modal fade">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Employee Details</h4>
                </div>
                <div class="modal-body">
                    <asp:EmpInfo runat="server" ID="EmpInfo" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default btn-sm" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <asp:FormView ID="fvRelease" runat="server" DataKeyNames="ReleaseId" 
                ItemType="PolicyTracker.Lib.Release" SelectMethod="fvRelease_GetItem"
                DefaultMode="ReadOnly" EmptyDataText="Release is not found." style="border:hidden;">
                <ItemTemplate>
                    <table class="DataWebControlStyle">
                        <tr class="RowStyle">
                            <td class="HeaderStyle">Release Date</td>
                            <td>
                                <asp:Label ID="lblReleaseDate" runat="server" Text='<%# Eval("ReleaseDate", "{0:M/d/yyyy}") %>'></asp:Label>
                            </td>
                            <td class="HeaderStyle">Deadline</td>
                            <td>
                                <asp:Label ID="lblDeadlineDate" runat="server" Text='<%# Eval("DeadlineDate", "{0:M/d/yyyy}") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="AlternatingRowStyle">
                            <td class="HeaderStyle">From</td>
                            <td colspan="3">
                                <asp:Label ID="lblFrom" runat="server" Text='<%# Eval("From") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="RowStyle">
                            <td class="HeaderStyle">To</td>
                            <td colspan="3">
                                <asp:Label ID="lblTo" runat="server" Text='<%# Eval("To") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="AlternatingRowStyle">
                            <td class="HeaderStyle">Subject</td>
                            <td colspan="3">
                                <asp:Label ID="lblSubject" runat="server" Text='<%# Eval("Subject") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="RowStyle">
                            <td class="HeaderStyle"># of Policies</td>
                            <td><%#:Item.ReleasePolicies.Count.ToString("#,##0")%></td>
                            <td class="HeaderStyle"># of Notices</td>
                            <td><%#:Item.ReleaseNotices.Count.ToString("#,##0")%></td>
                        </tr>
                        <tr class="AlternatingRowStyle">
                            <td class="HeaderStyle"># of Recipients<br />(Acked / Viewed / Total)</td>
                            <td><%# Item.ReleaseRecipients.Where(Function(a) a.AckDT.HasValue).Count.ToString("#,##0") & "/" _
                                                    & Item.ReleaseRecipients.Where(Function(a) a.RecipientViewDT.HasValue).Count.ToString("#,##0") & "/" _
                                                    & Item.ReleaseRecipients.Count.ToString("#,##0")%>
                            </td>
                            <td class="HeaderStyle"># of Exceptions</td>
                            <td>
                                <%#:Item.ReleaseRecipients.Where(Function(a) a.Exception <> "").Count.ToString("#,##0")%>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:FormView>
            
            <hr />
            <div class="row" style="margin: 3px 0 3px 0;">
                <asp:Label ID="lblFilterOrg" runat="server" Text="Organization:" AssociatedControlID="ddlFilterOrg"></asp:Label>
                <asp:DropDownList ID="ddlFilterOrg" runat="server" AppendDataBoundItems="true"
                    DataSourceID="odsFilterOrg" DataTextField="OrgDesc" DataValueField="OrgCode"
                    AutoPostBack="true" CssClass="select_chosen">
                </asp:DropDownList>
                <asp:ObjectDataSource ID="odsFilterOrg" runat="server"
                    SelectMethod="GetOrgsForOA" OnSelecting="odsOrg_Selecting"
                    TypeName="PolicyTracker.Lib.SettingsBL">
                    <SelectParameters>
                        <asp:Parameter Name="userRole" Type="String" />
                        <asp:Parameter Name="userId" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </div>

            <asp:FormView ID="fvRelStat" runat="server" DataKeyNames="ReleaseId" 
                ItemType="PolicyTracker.Lib.Release" SelectMethod="fvRelease_GetItem"
                DefaultMode="ReadOnly" EmptyDataText="Release is not found." style="border:hidden;">
                <ItemTemplate>
                    <table class="DataWebControlStyle">
                        <tr class="RowStyle">
                            <td class="HeaderStyle"># of Recipients<br />(Acked / Viewed / Total)</td>
                            <td>
                                <asp:Label ID="lblPackets" runat="server" Text='<%# _
    Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue) And a.AckDT.HasValue).Count.ToString("#,##0") & "/" _
    & Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue) And a.RecipientViewDT.HasValue).Count.ToString("#,##0") & "/" _
    & Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue)).Count.ToString("#,##0")%>'></asp:Label>
                            </td>
                            <td class="HeaderStyle"># of Exceptions</td>
                            <td>
                                <asp:Label ID="lblExc" runat="server" Text='<%# _
    Item.ReleaseRecipients.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg.SelectedValue) And a.Exception <> "").Count.ToString("#,##0")%>'></asp:Label>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:FormView>

            <div class="row" style="margin: 3px 0 3px 0;">
                <asp:Button ID="btnCreateData" runat="server" Text="Create Recipients & Policies" CssClass="btn btn-success btn-xs" Visible="false" />
                <asp:Button ID="btnCreateNotices" runat="server" Text="Create Notices" CssClass="btn btn-success btn-xs" Visible="false" />
            </div>

            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsMain" runat="server" ShowSummary="true" DisplayMode="List" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgMain" />

            <ul class="nav nav-pills" id="tlMain" runat="server">
                <li class="" id="tliPolicies" runat="server"><a href="#tpPolicies" id="tlkPolicies" runat="server" data-toggle="tab" aria-expanded="true">Policies <span runat="server" id="spnPolCnt" class="badge">0</span></a></li>
                <li class="" id="tliRecipients" runat="server"><a href="#tpRecipients" id="tlkRecipients" runat="server" data-toggle="tab" aria-expanded="false">Recipients <span runat="server" id="spnRecCnt" class="badge">0</span></a></li>
                <li class="" id="tliNotices" runat="server"><a href="#tpNotices" id="tlkNotices" runat="server" data-toggle="tab" aria-expanded="false">Notices <span runat="server" id="spnNotCnt" class="badge">0</span></a></li>
            </ul>

            <div class="tab-content" id="tcRelease">
                <div class="tab-pane fade" id="tpPolicies">
                    <br />
                    <asp:label id="lblInfoPolicies" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:label>
                    <asp:label id="lblErrorPolicies" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
                    <asp:ValidationSummary ID="vsPolicies" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgPolicies" />

                    <asp:GridView ID="gvRelPolicies" runat="server" AutoGenerateColumns="False" DataKeyNames="ReleaseId,PolicyId"
                        AllowPaging="false" AllowSorting="false" EmptyDataText="No record was found." Caption=""
                        ItemType="PolicyTracker.Lib.ReleasePolicy" SelectMethod="gvRelPolicies_GetData">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" />
                            <asp:BoundField DataField="ReleaseId" HeaderText="Release ID" Visible="false" />
                            <asp:BoundField DataField="PolicyId" HeaderText="Policy ID" Visible="false" />
                            <asp:BoundField DataField="PolicyName" HeaderText="Policy Name" />
                            <asp:TemplateField HeaderText="# of Files" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate><%#: Item.UploadFiles.Count.ToString("#,##0")%></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <br />

                    <asp:GridView ID="gvFiles" runat="server" EmptyDataText="No file was found." AllowPaging="false" 
                        AllowSorting="false" AutoGenerateColumns="False" DataKeyNames="FileId" Visible="false"
                        Caption="Policy Files" ItemType="PolicyTracker.Lib.UploadFile" SelectMethod="gvFiles_GetData">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbtnDownload" runat="server" CausesValidation="False" CommandName="Download" CommandArgument='<%# Eval("FileId")%>'
                                        AlternateText="Download" ToolTip="Download">
                                        <span class="glyphicon glyphicon-download-alt"></span>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="FileId" HeaderText="File ID" SortExpression="FileId" Visible="false" />
                            <asp:BoundField DataField="OriginalName" HeaderText="File Name" SortExpression="OriginalName" />
                            <asp:TemplateField HeaderText="Size" SortExpression="Length" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <asp:Label ID="lblSize" runat="server" Text='<%# FormatNumber(Eval("Length") / 1024, 0) & " KB"%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="CreateDT" HeaderText="Upload Date" SortExpression="CreateDT" />
                            <asp:BoundField DataField="CreateUser" HeaderText="Upload User" SortExpression="CreateUser" />
                        </Columns> 
                    </asp:GridView>
                    <br />
                    <asp:Label ID="lblInfoUpload" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
                    <asp:Label ID="lblErrorUpload" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
                    <asp:Button ID="btnUpload" runat="server" Text="Upload File" CssClass="btn btn-primary btn-xs" Visible="false" />
                </div>

                <div class="tab-pane fade" id="tpRecipients">
                    <br />
                    <asp:label id="lblInfoRecipients" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:label>
                    <asp:label id="lblErrorRecipients" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
                    <asp:ValidationSummary ID="vsRecipients" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgRecipients" />

                    <asp:GridView ID="gvRelRecipients" runat="server" AutoGenerateColumns="False" DataKeyNames="ReleaseId,RecipientId"
                        AllowPaging="true" AllowSorting="true" EmptyDataText="No record was found." Caption=""
                        ItemType="PolicyTracker.Lib.vPacket" SelectMethod="gvRelRecipients_GetData">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" />
                            <asp:TemplateField ShowHeader="False" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlkOpenPacket" runat="server" Target="_self" ToolTip="Open Packet"
                                        NavigateUrl='<%#: "~/USER/Packet/" & Item.ReleaseId.ToString & "/" & Item.RecipientId%>'>
                                        <span class="glyphicon glyphicon-folder-open"></span>
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ReleaseId" HeaderText="Release ID" Visible="false" />
                            <asp:BoundField DataField="RecipientId" HeaderText="Recipient ID" SortExpression="RecipientId" Visible="false" />
                            <asp:TemplateField HeaderText="Recipient ID" SortExpression="RecipientId">
                                <ItemTemplate>
                                    <asp:Label ID="lblRecipientId" runat="server" Text='<%# Eval("RecipientId")%>'></asp:Label>
                                    <asp:LinkButton ID="lbtnEmpInfo" runat="server" CausesValidation="false" CommandName="EmpInfo" CommandArgument='<%# Eval("RecipientId")%>'
                                        AlternateText="Employee Details" ToolTip="Employee Details" 
                                        OnClientClick="return showEmpInfo();">
                                        <span class="glyphicon glyphicon-user" id="emp-info"></span>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="RecipientName" HeaderText="Recipient Name" SortExpression="RecipientName" />
                            <asp:BoundField DataField="RecipientEmail" HeaderText="Recipient Email" SortExpression="RecipientEmail" />
                            <asp:BoundField DataField="OrgCode" HeaderText="Org" SortExpression="OrgCode" />
                            <asp:BoundField DataField="ClassCode" HeaderText="Class" SortExpression="ClassCode" />
							<asp:BoundField DataField="Exception" HeaderText="Exception" SortExpression="Exception" Visible="true" />
							<asp:BoundField DataField="RecipientViewDT" HeaderText="View Date Time" SortExpression="RecipientViewDT" Visible="true" />
							<asp:BoundField DataField="AckDT" HeaderText="Ack Date Time" SortExpression="AckDT" Visible="true" />
                            <asp:TemplateField HeaderText="# of Ack Files" SortExpression="">
                                <ItemTemplate>
                                    <asp:Label ID="lblAckFiles" runat="server" 
                                        Text='<%# Item.AckFilesCount %>'
                                        Visible='<%# IIf(Item.AckFilesCount > 0, True, False)%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerTemplate>
                            <asp:GridViewPager ID="GridViewPager1" runat="server" />
                        </PagerTemplate>
                    </asp:GridView>

                    <div class="row" style="margin: 3px 0 3px 0;">
                        <button id="btnDownloadPackets" runat="server" type="button" class="btn btn-primary btn-sm" causesvalidation="false"
                            tooltip="Download packets as CSV file">
                            Download <span class="glyphicon glyphicon-download"></span>
                        </button>
                    </div>
                                        
                    <asp:DetailsView ID="dvPacket" runat="server" AutoGenerateRows="False" Caption="Packet Details"
                        DataKeyNames="ReleaseId,RecipientId" DataSourceID="odsPacket" DefaultMode="ReadOnly" Visible="false">
                        <Fields>
                            <asp:BoundField DataField="ReleaseId" Visible="false" />
                            <asp:BoundField DataField="RecipientId" HeaderText="Recipient ID" SortExpression="RecipientId" Visible="true" ReadOnly="true" />
                            <asp:BoundField DataField="RecipientName" HeaderText="Recipient Name" SortExpression="RecipientName" ReadOnly="true" />
                            <asp:TemplateField HeaderText="Recipient Email" SortExpression="RecipientEmail">
                                <ItemTemplate>
                                    <asp:Label ID="lblRecipientEmail" runat="server" Text='<%# Eval("RecipientEmail")%>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtRecipientEmail" runat="server" Columns="50" MaxLength="500" 
                                        Text='<%# Bind("RecipientEmail")%>'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Organization" SortExpression="OrgCode">
                                <ItemTemplate>
                                    <asp:Label ID="lblDivisionOrg" runat="server" Text='<%# Eval("OrgCode") & " " & Eval("Division.DivDesc")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Recipient View Date Time">
                                <ItemTemplate>
                                    <asp:Label ID="lblViewDT" runat="server" Text='<%# Eval("RecipientViewDT") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblViewDT" runat="server" Text='<%# Eval("RecipientViewDT") %>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Recipient View Client IP">
                                <ItemTemplate>
                                    <asp:Label ID="lblViewIP" runat="server" Text='<%# Eval("RecipientViewClientIP") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblViewIP" runat="server" Text='<%# Eval("RecipientViewClientIP") %>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ack Date Time/User">
                                <ItemTemplate>
                                    <asp:Label ID="lblAckDT" runat="server" Text='<%# Eval("AckDT") & " " & Eval("AckUserID") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblAckDT" runat="server" Text='<%# Eval("AckDT") & " " & Eval("AckUserID") %>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ack Client IP">
                                <ItemTemplate>
                                    <asp:Label ID="lblAckIP" runat="server" Text='<%# Eval("AckClientIP") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblAckIP" runat="server" Text='<%# Eval("AckClientIP") %>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ack Auth Type">
                                <ItemTemplate>
                                    <asp:Label ID="lblAckAuthType" runat="server" Text='<%# Eval("AckAuthType") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblAckAuthType" runat="server" Text='<%# Eval("AckAuthType") %>'></asp:Label>
                                </EditItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Exception" SortExpression="Exception">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlException" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("Exception") %>'
                                        DataSourceID="odsException" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="true">
                                        <asp:ListItem Value="">-- N/A --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsException" runat="server" 
                                        SelectMethod="GetCommonCodesByCatg" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="RECIP_EXCEPTION" Name="catg" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlException" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("Exception") %>'
                                        DataSourceID="odsException" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="false">
                                        <asp:ListItem Value="">-- N/A --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsException" runat="server" 
                                        SelectMethod="GetCommonCodesByCatg" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="RECIP_EXCEPTION" Name="catg" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Memo" SortExpression="AdminMemo">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAdminMemo" runat="server" Columns="100" onkeyup="return ismaxlength(this, 10000);"
                                        TextMode="MultiLine" Rows="3" Text='<%# Bind("AdminMemo") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblAdminMemo" runat="server" Text='<%# Bind("AdminMemo") %>' Visible="true"></asp:Label>
                                    <%--<asp:Label ID="lblAdminMemoEval" runat="server" Text='<%# EvalHtmlEncode("AdminMemo") %>'></asp:Label>--%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="CreateDT" HeaderText="Create Date" InsertVisible="False" DataFormatString="{0:M/d/yyyy h:mm:ss.fff tt}"
                                ReadOnly="True" SortExpression="CreateDT" />
                            <asp:BoundField DataField="CreateUser" HeaderText="Create User" InsertVisible="False"
                                ReadOnly="True" SortExpression="CreateUser" />
                            <asp:TemplateField HeaderText="Last Update Date" SortExpression="LastUpdateDT"
                                InsertVisible="False">
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
                            <asp:TemplateField HeaderText="Last Update User" SortExpression="LastUpdateUser"
                                InsertVisible="False">
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
                                        Text="Update" ValidationGroup="vgRecipients" CssClass="btn btn-primary btn-xs"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                        Text="Cancel" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:LinkButton ID="lbtnInsert" runat="server" CausesValidation="True" CommandName="Insert"
                                        Text="Insert" ValidationGroup="vgRecipients" CssClass="btn btn-primary btn-xs"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                        Text="Cancel" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbtnEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                        Text="Edit" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnNew" runat="server" CausesValidation="False" CommandName="New"
                                        Text="New" CssClass="btn btn-default btn-xs" Visible="false"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="Delete" CssClass="btn btn-default btn-xs" OnClientClick="return confirm('Are you sure you want to delete this record?');" Visible="false"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                    <asp:ObjectDataSource ID="odsPacket" runat="server" ConflictDetection="CompareAllValues"
                        OldValuesParameterFormatString="orig{0}"
                        SelectMethod="GetReleaseRecipientById" TypeName="PolicyTracker.Lib.PTRepository" DataObjectTypeName="PolicyTracker.Lib.ReleaseRecipient"
                        UpdateMethod="UpdateReleaseRecipient">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="gvRelRecipients" DefaultValue="" Name="ReleaseId" PropertyName="SelectedDataKey(0)" Type="Int32" />
                            <asp:ControlParameter ControlID="gvRelRecipients" DefaultValue="" Name="RecipientId" PropertyName="SelectedDataKey(1)" Type="String" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="releaseRecipient" Type="Object" />
                            <asp:Parameter Name="origReleaseRecipient" Type="Object" />
                        </UpdateParameters>
                    </asp:ObjectDataSource>
                    <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="ReadOnly" Visible="false"></asp:Label>

                </div>

                <div class="tab-pane fade in" id="tpNotices">
                    <br />
                    <asp:GridView ID="gvRelNotices" runat="server" AutoGenerateColumns="False" DataKeyNames="ReleaseNoticeId"
                        AllowPaging="false" AllowSorting="false" EmptyDataText="No record was found." Caption=""
                        ItemType="PolicyTracker.Lib.ReleaseNotice" SelectMethod="gvRelNotices_GetData">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" />
                            <asp:TemplateField ShowHeader="False" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlkOpenNotice" runat="server" Target="_self" ToolTip="Open Notice"
                                        NavigateUrl='<%#: "~/OA/ReleaseNotice/" & Item.ReleaseNoticeId.ToString%>'>
                                        <span class="glyphicon glyphicon-folder-open"></span>
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ReleaseNoticeId" HeaderText="Release Notice ID" Visible="false" />
                            <asp:BoundField DataField="ReleaseId" HeaderText="Release ID" Visible="false" />
                            <asp:BoundField DataField="NoticeType" HeaderText="Notice Type" />
							<asp:BoundField DataField="NoticeDate" HeaderText="Notice Date" DataFormatString="{0:M/d/yyyy}" />
                            <asp:TemplateField HeaderText="# of Recipient Notices" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <asp:Label ID="lblRNCount" runat="server" Text='<%#: Item.RecipientNotices.Count.ToString("#,##0")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="StartDT" HeaderText="Start Sending" SortExpression="" DataFormatString="{0:M/d/yyyy h:mm:ss tt}" />
                            <asp:BoundField DataField="CompleteDT" HeaderText="Complete Sending" SortExpression="" DataFormatString="{0:M/d/yyyy h:mm:ss tt}" />
                        </Columns>
                    </asp:GridView>
                    <br />

                    <asp:label id="lblInfoNotices" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:label>
                    <asp:label id="lblErrorNotices" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
                    <asp:ValidationSummary ID="vsNotices" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgNoticeDetails" />

                    <asp:DetailsView ID="dvRelNotice" runat="server" AutoGenerateRows="False" Caption="Add New Notice" ItemType="PolicyTracker.Lib.ReleaseNotice"
                        DataKeyNames="ReleaseNoticeId" DataSourceID="odsRelNotice" DefaultMode="Insert">
                        <Fields>
                            <asp:TemplateField HeaderText="Notice Type">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlNoticeType" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("NoticeType") %>'
                                        DataSourceID="odsNoticeType" DataTextField="CMCodeDesc" DataValueField="CMCode">
                                        <asp:ListItem Value="">-- Select Notice Type --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvNoticeType" runat="server" ControlToValidate="ddlNoticeType"
                                        ErrorMessage="Notice Type is required" Text="*" Display="Dynamic" ValidationGroup="vgNoticeDetails"></asp:RequiredFieldValidator>
                                    <asp:ObjectDataSource ID="odsNoticeType" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="NOTICE_TYPE" Name="catg" Type="String" />
                                            <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="ddlNoticeType" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("NoticeType") %>'
                                        DataSourceID="odsNoticeType" DataTextField="CMCodeDesc" DataValueField="CMCode">
                                        <asp:ListItem Value="">-- Select Notice Type --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvNoticeType" runat="server" ControlToValidate="ddlNoticeType"
                                        ErrorMessage="Notice Type is required" Text="*" Display="Dynamic" ValidationGroup="vgNoticeDetails"></asp:RequiredFieldValidator>
                                    <asp:ObjectDataSource ID="odsNoticeType" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="NOTICE_TYPE" Name="catg" Type="String" />
                                            <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlNoticeType" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("NoticeType") %>'
                                        DataSourceID="odsNoticeType" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="false">
                                        <asp:ListItem Value="">-- Select Notice Type --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsNoticeType" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="NOTICE_TYPE" Name="catg" Type="String" />
                                            <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Notice Date" SortExpression="NoticeDate">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtNoticeDate" runat="server" Text='<%# Bind("NoticeDate", "{0:MM/dd/yyyy}") %>' Columns="12" MaxLength="10" />
                                    <asp:RequiredFieldValidator ID="rfvNoticeDate" runat="server" ControlToValidate="txtNoticeDate"
                                        ErrorMessage="Notice Date is required" Text="*" Display="dynamic" ValidationGroup="vgNoticeDetails"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvReleaseDate" runat="server" ControlToValidate="txtNoticeDate"
                                        ErrorMessage="Notice Date should be between 1/1/2000 and 12/31/2050" Text="*"
                                        Type="Date" MinimumValue="1/1/2000" MaximumValue="12/31/2050" ValidationGroup="vgNoticeDetails"></asp:RangeValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtNoticeDate" runat="server" Text='<%# Bind("NoticeDate", "{0:MM/dd/yyyy}") %>' Columns="12" MaxLength="10" />
                                    <asp:RequiredFieldValidator ID="rfvNoticeDate" runat="server" ControlToValidate="txtNoticeDate"
                                        ErrorMessage="Notice Date is required" Text="*" Display="dynamic" ValidationGroup="vgNoticeDetails"></asp:RequiredFieldValidator>
                                    <asp:RangeValidator ID="rvReleaseDate" runat="server" ControlToValidate="txtNoticeDate"
                                        ErrorMessage="Notice Date should be between 1/1/2000 and 12/31/2050" Text="*"
                                        Type="Date" MinimumValue="1/1/2000" MaximumValue="12/31/2050" ValidationGroup="vgNoticeDetails"></asp:RangeValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblNoticeDate" runat="server" Text='<%# Eval("NoticeDate", "{0:M/d/yyyy}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <%--<asp:TemplateField HeaderText="# of Recipient Notices" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <asp:Label ID="lblRNCount" runat="server"><%#: Item.RecipientNotices.Count.ToString%></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <asp:BoundField DataField="CreateDT" HeaderText="Create Date" InsertVisible="False" DataFormatString="{0:M/d/yyyy h:mm:ss.fff tt}"
                                ReadOnly="True" SortExpression="CreateDT" />
                            <asp:BoundField DataField="CreateUser" HeaderText="Create User" InsertVisible="False"
                                ReadOnly="True" SortExpression="CreateUser" />
                            <asp:TemplateField HeaderText="Last Update Date" SortExpression="LastUpdateDT" InsertVisible="False">
                                <EditItemTemplate>
                                    <asp:Label ID="lblLastUpdateDT" runat="server" Text='<%# Bind("LastUpdateDT", "{0:M/d/yyyy h:mm:ss.fff tt}") %>'></asp:Label>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:Label ID="lblLastUpdateDT" runat="server" Text='<%# Bind("LastUpdateDT") %>'></asp:Label>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblLastUpdateDT" runat="server" Text='<%# Bind("LastUpdateDT", "{0:M/d/yyyy h:mm:ss.fff tt}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Last Update User" SortExpression="LastUpdateUser" InsertVisible="False">
                                <EditItemTemplate>
                                    <asp:Label ID="lblLastUpdateUser" runat="server" Text='<%# Bind("LastUpdateUser") %>'></asp:Label>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:Label ID="lblLastUpdateUser" runat="server" Text='<%# Bind("LastUpdateUser") %>'></asp:Label>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblLastUpdateUser" runat="server" Text='<%# Bind("LastUpdateUser") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ShowHeader="False" ItemStyle-CssClass="CommandRowStyle">
                                <EditItemTemplate>
                                    <asp:LinkButton ID="lbtnUpdate" runat="server" CausesValidation="True" CommandName="Update"
                                        Text="Update" ValidationGroup="vgNoticeDetails" CssClass="btn btn-primary btn-xs"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                        Text="Cancel" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:LinkButton ID="lbtnInsert" runat="server" CausesValidation="True" CommandName="Insert"
                                        Text="Insert" ValidationGroup="vgNoticeDetails" CssClass="btn btn-primary btn-xs"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                        Text="Cancel" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbtnEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                        Text="Edit" CssClass="btn btn-default btn-xs" Visible="true"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="Delete" CssClass="btn btn-default btn-xs" OnClientClick="return confirm('Are you sure you want to delete this record?');"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnNew" runat="server" CausesValidation="False" CommandName="New"
                                        Text="New Notice" CssClass="btn btn-primary btn-xs"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                    <asp:ObjectDataSource ID="odsRelNotice" runat="server" ConflictDetection="CompareAllValues"
                        DeleteMethod="DeleteReleaseNotice" InsertMethod="InsertReleaseNotice" OldValuesParameterFormatString="orig{0}"
                        SelectMethod="GetReleaseNoticeByID" TypeName="PolicyTracker.Lib.PTRepository" DataObjectTypeName="PolicyTracker.Lib.ReleaseNotice"
                        UpdateMethod="UpdateReleaseNotice">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="gvRelNotices" DefaultValue="" Name="ReleaseNoticeID" PropertyName="SelectedValue" Type="Int32" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="ReleaseNotice" Type="Object" />
                            <asp:Parameter Name="origReleaseNotice" Type="Object" />
                        </UpdateParameters>
                    </asp:ObjectDataSource>
                    <asp:Label ID="lblDetailsViewModeRelNotice" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>

                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpload" />
            <asp:PostBackTrigger ControlID="btnDownloadPackets" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
