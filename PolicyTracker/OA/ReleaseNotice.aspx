<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="ReleaseNotice.aspx.vb" Inherits="PolicyTracker.ReleaseNotice" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

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
                sessionStorage.setItem('lastTab_RelNot', $(this).attr('href'));
            });
            //go to the latest tab, if it exists:
            var lastTab = sessionStorage.getItem('lastTab_RelNot');
            if (lastTab && $('a[href="' + lastTab + '"]').is(":visible")) {
                $('a[href="' + lastTab + '"]').tab('show');
            }
            else {
                // Set the first tab if cookie do not exist
                $('a[data-toggle="tab"]:first').tab('show');
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src='<%: Page.ResolveUrl("~/Scripts/chosen.jquery.min.js")%>' type="text/javascript"></script>
    <link href='<%: Page.ResolveClientUrl("~/Content/chosen.min.css")%>' rel="stylesheet" />

    <h2>Release Notice</h2>
    <a id="lnkBackToRelease" runat="server" href="~/OA/Release/">Back To Release</a>

            <asp:FormView ID="fvRelNotice" runat="server" DataKeyNames="ReleaseNoticeId"
                ItemType="PolicyTracker.Lib.ReleaseNotice" SelectMethod="fvRelNotice_GetItem"
                DefaultMode="ReadOnly" EmptyDataText="No record was found." style="border:hidden;">
                <ItemTemplate>
                    <table class="DataWebControlStyle">
                        <tr class="RowStyle">
                            <td class="HeaderStyle">Release Date</td>
                            <td>
                                <asp:Label ID="lblReleaseDate" runat="server" Text='<%# Eval("Release.ReleaseDate", "{0:M/d/yyyy}")%>'></asp:Label>
                            </td>
                            <td class="HeaderStyle">Deadline</td>
                            <td>
                                <asp:Label ID="lblDeadlineDate" runat="server" Text='<%# Eval("Release.DeadlineDate", "{0:M/d/yyyy}")%>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="AlternatingRowStyle">
                            <td class="HeaderStyle">From</td>
                            <td colspan="3">
                                <asp:Label ID="lblFrom" runat="server" Text='<%# Eval("Release.From")%>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="RowStyle">
                            <td class="HeaderStyle">To</td>
                            <td colspan="3">
                                <asp:Label ID="lblTo" runat="server" Text='<%# Eval("Release.To")%>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="AlternatingRowStyle">
                            <td class="HeaderStyle">Subject</td>
                            <td colspan="3">
                                <asp:Label ID="lblSubject" runat="server" Text='<%# Eval("Release.Subject")%>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="RowStyle">
                            <td class="HeaderStyle"># of Policies</td>
                            <td>
                                <asp:Label ID="lblNumberOfPol" runat="server" Text='<%#:Item.Release.ReleasePolicies.Count.ToString("#,##0")%>'></asp:Label>
                            </td>
                            <td class="HeaderStyle"># of Recipients</td>
                            <td>
                                <asp:Label ID="lblNumberOfRecip" runat="server" Text='<%#:Item.Release.ReleaseRecipients.Count.ToString("#,##0")%>'></asp:Label>
                            </td>
                        </tr>
                        <tr><td colspan="4"></td></tr>
                        <tr class="RowStyle">
                            <td class="HeaderStyle">Notice Type</td>
                            <td>
                                <asp:Label ID="lblNoticeType" runat="server" Text='<%# Eval("NoticeType") %>'></asp:Label>
                            </td>
                            <td class="HeaderStyle">Notice Date</td>
                            <td>
                                <asp:Label ID="lblNoticeDate" runat="server" Text='<%# Eval("NoticeDate", "{0:M/d/yyyy}") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="AlternatingRowStyle">
                            <td class="HeaderStyle">Start Sending</td>
                            <td>
                                <asp:Label ID="lblStartDT" runat="server" Text='<%# Eval("StartDT", "{0:M/d/yyyy h:mm:ss tt}") %>'></asp:Label>
                            </td>
                            <td class="HeaderStyle">Complete Sending</td>
                            <td>
                                <asp:Label ID="lblCompleteDT" runat="server" Text='<%# Eval("CompleteDT", "{0:M/d/yyyy h:mm:ss tt}") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="RowStyle">
                            <td class="HeaderStyle"># of Recipient Notices</td>
                            <td colspan="3">
                                <asp:Label ID="lblNumberOfNotices" runat="server" Text='<%#Eval("RecipientNotices.Count","{0:#,##0}")%>'></asp:Label>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:FormView>
            
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

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <div class="row" style="margin: 3px 0 3px 0;">
                <asp:Button ID="btnCreateMessages" runat="server" Text="Create Email Messages" CssClass="btn btn-success btn-xs" Visible="false" />
                <asp:Button ID="btnSendMessages" runat="server" Text="Send Email Messages" CssClass="btn btn-success btn-xs" Visible="false" />
            </div>

            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsMain" runat="server" ShowSummary="true" DisplayMode="List" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgMain" />

            <ul class="nav nav-pills">
                <li class="" id="tliRecipNotices" runat="server"><a href="#tpRecipNotices" id="tlkRecipNotices" runat="server" data-toggle="tab" aria-expanded="true">Recipient Notices <span runat="server" id="spnRecNotCnt" class="badge">0</span></a></li>
                <li class="" id="tliOrgAdminNotice" runat="server"><a href="#tpOrgAdminNotice" id="tlkOrgAdminNotice" runat="server" data-toggle="tab" aria-expanded="false">Org Admin Notice</a></li>
            </ul>

            <div class="tab-content" id="tcReleaseNotice">
                <div class="tab-pane fade" id="tpRecipNotices">
                    <br />
                    <asp:GridView ID="gvRecipientNotices" runat="server" AutoGenerateColumns="False" DataKeyNames="ReleaseNoticeId,RecipientId" 
                        AllowPaging="True" AllowSorting="True" EmptyDataText="No record was found." Caption=""
                        ItemType="PolicyTracker.Lib.RecipientNotice" SelectMethod="gvRecipientNotices_GetData">
                        <Columns>
                            <asp:CommandField ShowSelectButton="True" />
                            <asp:BoundField DataField="RecipientId" HeaderText="Recipient" SortExpression="RecipientId" Visible="true" />
                            <asp:BoundField DataField="From" HeaderText="From" SortExpression="From" />
                            <asp:BoundField DataField="To" HeaderText="To" SortExpression="To" />
                            <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" Visible="false" />
                            <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" Visible="false" />
                            <asp:BoundField DataField="SentDT" HeaderText="Sent Date" SortExpression="SentDT" Visible="true" />
                        </Columns>
                        <PagerTemplate>
                            <asp:GridViewPager ID="GridViewPager1" runat="server" />
                        </PagerTemplate>
                    </asp:GridView>
                    <br />

                    <asp:Label ID="lblInfoRecipN" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
                    <asp:Label ID="lblErrorRecipN" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
                    <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />
            
                    <asp:DetailsView ID="dvRecipNotice" runat="server" AutoGenerateRows="False" Caption="Recipient Notice Details"
                        DataKeyNames="ReleaseNoticeId,RecipientId" DataSourceID="odsRecipNotice" DefaultMode="ReadOnly" Visible="false">
                        <Fields>
                            <asp:BoundField DataField="ReleaseNoticeId" Visible="false" />
                            <asp:BoundField DataField="RecipientId" Visible="false" />
                            <asp:BoundField DataField="From" HeaderText="From" />
                            <asp:BoundField DataField="To" HeaderText="To" />
                            <asp:BoundField DataField="Subject" HeaderText="Subject" />
                            <asp:TemplateField HeaderText="Body" SortExpression="Body">
                                <ItemTemplate>
                                    <asp:Label ID="lblBody" runat="server" Text='<%# Bind("Body") %>'></asp:Label>
                                    <asp:Label ID="lblSentDT" runat="server" Text='<%# Eval("SentDT") %>' Visible="false"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="CreateDT" HeaderText="Create Date" InsertVisible="False" DataFormatString="{0:M/d/yyyy h:mm:ss.fff tt}"
                                ReadOnly="True" SortExpression="CreateDT" />
                            <asp:BoundField DataField="SentDT" HeaderText="Sent Date" InsertVisible="False" DataFormatString="{0:M/d/yyyy h:mm:ss.fff tt}"
                                ReadOnly="True" SortExpression="SentDT" />
                            <asp:TemplateField ShowHeader="False" ItemStyle-CssClass="CommandRowStyle" Visible="true">
                                <EditItemTemplate>
                                    <asp:LinkButton ID="lbtnUpdate" runat="server" CausesValidation="True" CommandName="Update"
                                        ValidationGroup="Edit" Text="Update"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                        Text="Cancel"></asp:LinkButton>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:LinkButton ID="lbtnInsert" runat="server" CausesValidation="True" CommandName="Insert"
                                        ValidationGroup="Insert" Text="Insert"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                        Text="Cancel"></asp:LinkButton>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbtnEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                        Text="Edit" Visible="false"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnNew" runat="server" CausesValidation="False" CommandName="New"
                                        Text="New" Visible="false"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="Delete" Visible="false" OnClientClick="return confirm('Are you sure you want to delete this record?');"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnSend" runat="server" CausesValidation="False" CommandName="Send"
                                        Text="Send" Visible="true" CssClass="btn btn-warning btn-xs" 
                                        OnClientClick="return confirm('Are you sure you want to send this message?');"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                    <asp:ObjectDataSource ID="odsRecipNotice" runat="server" ConflictDetection="CompareAllValues"
                        OldValuesParameterFormatString="orig{0}"
                        SelectMethod="GetRecipientNoticeById" TypeName="PolicyTracker.Lib.PTRepository" DataObjectTypeName="PolicyTracker.Lib.RecipientNotice"
                        UpdateMethod="UpdateRecipientNotice">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="gvRecipientNotices" DefaultValue="" Name="ReleaseNoticeId" PropertyName="SelectedDataKey(0)" Type="Int32" />
                            <asp:ControlParameter ControlID="gvRecipientNotices" DefaultValue="" Name="RecipientId" PropertyName="SelectedDataKey(1)" Type="String" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="recipientNotice" Type="Object" />
                            <asp:Parameter Name="origRecipientNotice" Type="Object" />
                        </UpdateParameters>
                    </asp:ObjectDataSource>
                    <asp:HiddenField ID="hidRecipNoticeMode" runat="server" />
                </div>

                <div class="tab-pane fade" id="tpOrgAdminNotice">
                    <br />
                    <asp:Label ID="lblInfoOAN" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
                    <asp:Label ID="lblErrorOAN" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
                    <asp:ValidationSummary ID="vsDetailsOAN" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />
            
                    <asp:DetailsView ID="dvOANotice" runat="server" AutoGenerateRows="False" Caption=""
                        DataKeyNames="ReleaseNoticeId" DataSourceID="odsOANotice" DefaultMode="ReadOnly" Visible="true"
                        EmptyDataText="No record was found.">
                        <Fields>
                            <asp:BoundField DataField="ReleaseNoticeId" Visible="false" />
                            <asp:BoundField DataField="From" HeaderText="From" />
                            <asp:BoundField DataField="To" HeaderText="To" />
                            <asp:BoundField DataField="Subject" HeaderText="Subject" />
                            <asp:TemplateField HeaderText="Body" SortExpression="Body">
                                <ItemTemplate>
                                    <asp:Label ID="lblBody" runat="server" Text='<%# Bind("Body") %>'></asp:Label>
                                    <asp:Label ID="lblSentDT" runat="server" Text='<%# Eval("SentDT") %>' Visible="false"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="CreateDT" HeaderText="Create Date" InsertVisible="False" DataFormatString="{0:M/d/yyyy h:mm:ss.fff tt}"
                                ReadOnly="True" SortExpression="CreateDT" />
                            <asp:BoundField DataField="SentDT" HeaderText="Sent Date" InsertVisible="False" DataFormatString="{0:M/d/yyyy h:mm:ss.fff tt}"
                                ReadOnly="True" SortExpression="SentDT" />
                            <asp:TemplateField ShowHeader="False" ItemStyle-CssClass="CommandRowStyle" Visible="true">
                                <EditItemTemplate>
                                    <asp:LinkButton ID="lbtnUpdate" runat="server" CausesValidation="True" CommandName="Update"
                                        ValidationGroup="Edit" Text="Update"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                        Text="Cancel"></asp:LinkButton>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:LinkButton ID="lbtnInsert" runat="server" CausesValidation="True" CommandName="Insert"
                                        ValidationGroup="Insert" Text="Insert"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnCancel" runat="server" CausesValidation="False" CommandName="Cancel"
                                        Text="Cancel"></asp:LinkButton>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:LinkButton ID="lbtnEdit" runat="server" CausesValidation="False" CommandName="Edit"
                                        Text="Edit" Visible="false"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnNew" runat="server" CausesValidation="False" CommandName="New"
                                        Text="New" Visible="false"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="Delete" Visible="false" OnClientClick="return confirm('Are you sure you want to delete this record?');"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnSend" runat="server" CausesValidation="False" CommandName="Send"
                                        Text="Send" Visible="true" CssClass="btn btn-warning btn-xs" 
                                        OnClientClick="return confirm('Are you sure you want to send this message?');"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                    <asp:ObjectDataSource ID="odsOANotice" runat="server" ConflictDetection="CompareAllValues"
                        OldValuesParameterFormatString="orig{0}"
                        SelectMethod="GetOrgAdminNoticeById" TypeName="PolicyTracker.Lib.PTRepository" DataObjectTypeName="PolicyTracker.Lib.OrgAdminNotice"
                        UpdateMethod="UpdateOrgAdminNotice">
                        <SelectParameters>
                            <asp:RouteParameter RouteKey="ReleaseNoticeId" Name="ReleaseNoticeId" Type="Int32" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="orgAdminNotice" Type="Object" />
                            <asp:Parameter Name="origOrgAdminNotice" Type="Object" />
                        </UpdateParameters>
                    </asp:ObjectDataSource>
                    <asp:HiddenField ID="hidOANoticeMode" runat="server" />

                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
