<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Packet.aspx.vb" Inherits="PolicyTracker.Packet" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/EmpInfo.ascx" TagPrefix="asp" TagName="EmpInfo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var idVerifyMode = ""; // View or Ack

        function validateAck() {
            if (document.getElementById("MainContent_chbAcknowledge").checked == false) {
                alert("Please accept the acknowledge statement by selecting the check box and click on the button again.");
                return false;
            }
            else {
                var hidUserName = document.getElementById('MainContent_hidUserName');
                if (hidUserName.value != '' && confirm('Are you ' + hidUserName.value + '?') == false) {
                    window.location = "../../../SignOut.html";
                    return false;
                }
                else if (confirm('Are you sure you want to acknowledge this record?') == false) {
                    return false;
                }
                else {
                    return true;
                }
            }
        }

        function showIDVerify() {
            $("#id-verify-modal").modal({
                backdrop: 'static',
                keyboard: false
            });
            //alert("A");
            //return true;
        }
        function submitIDVerify() {
            if ($('#radIDVerifyYes').is(':checked')) {
                if (idVerifyMode == "View") {
                    //alert("save view DT");
                    saveViewDT();
                }
            }
            else {
                // redirect to close app page
                //alert("close the app, logoff & login again!")
                window.location = "../../../SignOut.html";
            }
        }

        function saveViewDT() {
            var pathname = window.location.pathname.split("/");
            var pRecipId = pathname[pathname.length - 1];
            var pRelId = pathname[pathname.length - 2];
            var hidClientIP = document.getElementById('MainContent_hidClientIP');
            $.ajax({
                type: 'POST',
                url: '../../../Service/PacketService.asmx/UpdateRecipientView',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: '{"relId":"' + pRelId + '","recipId":"' + pRecipId + '","clientIP":"' + hidClientIP.value + '"}',
                success: function (response) {
                    //alert(response);
                    //alert(JSON.stringify(response));
                    var updateResult = JSON.parse(JSON.stringify(response));
                    //alert(updateResult);
                    var dd = JSON.parse(JSON.stringify(updateResult.d));
                    //alert(dd.ClientIp + ", " + dd.DateTime);
                    var lblRecipientViewDT = document.getElementById('MainContent_fvPacket_lblRecipientViewDT');
                    lblRecipientViewDT.innerHTML = dd.DateTime;
                    var lblRecipientViewClientIP = document.getElementById('MainContent_fvPacket_lblRecipientViewClientIP');
                    lblRecipientViewClientIP.innerHTML = dd.ClientIp;
                },
                error: function(error) {
                    console.log(error);
                    alert('error in save view dt')
                }
            });
        }

        function pageLoad() {
            // Get each div
            $('.content-with-url').each(function () {
                // Get the content
                var str = $(this).html();
                //alert(str);
                // Set the regex string
                //var regex = /(https?:\/\/([-\w\.]+)+(:\d+)?(\/([-\w\/_\.]*(\?\S+)?)?)?)/ig
                // http://stackoverflow.com/questions/3809401/what-is-a-good-regular-expression-to-match-a-url
                // Code behind function WebUtil.HtmlMsgEncode is used for encoding. 
                // "&" is encoded to "&amp;", I had to add ";" in the regex.
                var regex = /(https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}[-a-zA-Z0-9@:%_\+.~#?&//=;]*)/g
                // Replace plain text links by hyperlinks
                var replaced_text = str.replace(regex, "<a href='$1' target='_blank'>$1</a>");
                // Echo link
                $(this).html(replaced_text);
            });

            var hidVerifyIDView = document.getElementById('MainContent_hidVerifyIDView');
            var hidVerifyIDAck = document.getElementById('MainContent_hidVerifyIDAck');
            if (hidVerifyIDView.value == "Y") {
                idVerifyMode = "View"
                showIDVerify();
            } else if (hidVerifyIDAck.value == "Y") {
                idVerifyMode = "Ack"
                showIDVerify();
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Packet Details</h2>

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

    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="always">
        <ContentTemplate>

            <input type="hidden" id="hidVerifyIDView" runat="server" />
            <input type="hidden" id="hidVerifyIDAck" runat="server" />
            <input type="hidden" id="hidUserName" runat="server" />
            <input type="hidden" id="hidClientIP" runat="server" />
            <asp:FormView ID="fvPacket" runat="server" DataKeyNames="ReleaseId,RecipientId" 
                ItemType="PolicyTracker.Lib.ReleaseRecipient" SelectMethod="fvPacket_GetItem" 
                DefaultMode="ReadOnly" EmptyDataText="Packet is not found." style="border:hidden;">
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
                        <tr><td colspan="4"></td></tr>
                        <tr class="RowStyle">
                            <td class="HeaderStyle">Recipient ID</td>
                            <td>
                                <asp:Label ID="lblRecipientID" runat="server" Text='<%# Eval("RecipientId")%>'></asp:Label>
                            </td>
                            <td class="HeaderStyle">Recipient Name</td>
                            <td>
                                <asp:Label ID="lblUserName" runat="server" Text='<%# Eval("RecipientName")%>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="AlternatingRowStyle">
                            <td class="HeaderStyle">Email Address</td>
                            <td colspan="3">
                                <asp:Label ID="Label1" runat="server" Text='<%# Eval("RecipientEmail")%>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="RowStyle">
                            <td class="HeaderStyle">Organization</td>
                            <td colspan="3"><%# Item.Division.DivCode & " " & Item.Division.DivDesc%></td>
                        </tr>
                        <tr class="RowStyle">
                            <td class="HeaderStyle">Recipient View Date Time</td>
                            <td colspan="3">
                                <asp:Label ID="lblRecipientViewDT" runat="server" Text='<%# Eval("RecipientViewDT") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="AlternatingRowStyle">
                            <td class="HeaderStyle">Recipient View Client IP</td>
                            <td colspan="3">
                                <asp:Label ID="lblRecipientViewClientIP" runat="server" Text='<%# Eval("RecipientViewClientIP") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="RowStyle">
                            <td class="HeaderStyle">Acknowledge Date Time/User</td>
                            <td colspan="3">
                                <asp:Label ID="lblAckDate" runat="server" Text='<%# Eval("AckDT") & " " & Eval("AckUserID") %>'></asp:Label>
                            </td>
                        </tr>
                        <tr class="AlternatingRowStyle">
                            <td class="HeaderStyle">Acknowledge Client IP</td>
                            <td colspan="3">
                                <asp:Label ID="lblAckUserIP" runat="server" Text='<%# Eval("AckClientIP") %>'></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <p style="font-style:italic;"><asp:Label ID="lblDisclaimer" runat="server" Text='<%# EvalHtmlEncode("Release.Disclaimer") %>'></asp:Label></p>
                </ItemTemplate>
            </asp:FormView>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpload" />
        </Triggers>
    </asp:UpdatePanel>

    <asp:Panel ID="panCoverSheet" runat="server">
        <p><asp:Label ID="lblPolicyHeader" runat="server"></asp:Label></p><%--[MSG#1002] The packet contains the following document(s):--%>

        <asp:ListView ID="lvReleasePolicies" runat="server" DataKeyNames="ReleaseId,PolicyId" 
            ItemType="PolicyTracker.Lib.ReleasePolicy" SelectMethod="lvReleasePolicies_GetData">
            <ItemTemplate>
                <div class="alert alert-info">
                <%--<li style="line-height: 2em">--%>
                    <p style="margin-bottom:10px;"><asp:Label ID="lblPolicyName" runat="server" Text='<%# Eval("PolicyName") %>' Font-Bold="true"></asp:Label></p>
                    <p style="margin-bottom:10px;"><asp:Label ID="lblPolicyDesc" runat="server" Text='<%# EvalDesc() %>'></asp:Label></p>
                    <asp:HiddenField ID="PolicyId" runat="server" Value="<%# Item.PolicyId%>" />
                    <span class="content-with-url" style="font-style:italic;"><%# EvalDisclaimer()%></span>
                    <ul>
                    <asp:ListView ID="lvFiles" runat="server" DataKeyNames="FileId"
                        ItemType="PolicyTracker.Lib.UploadFile" SelectMethod="lvFiles_GetData" 
                        OnItemCommand="lvFiles_ItemCommand" OnItemDataBound="lvFiles_ItemDataBound">
                        <ItemTemplate>
                            <li>
                            <asp:LinkButton ID="lbtnDownload" runat="server" CausesValidation="False"
                                CommandArgument='<%# Eval("FileId")%>' CommandName="Download"
                                AlternateText="Download" ToolTip="Download">
                                <%#: Item.OriginalName%>
                            </asp:LinkButton>
                            </li>
                        </ItemTemplate>
                    </asp:ListView>
                    </ul>
                <%--</li>--%>
                </div>
            </ItemTemplate>
        </asp:ListView>

        <asp:Label ID="lblInfoDownload" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
        <asp:Label ID="lblErrorDownload" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>

    </asp:Panel>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <asp:Label ID="lblAckWarning" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-warning"></asp:Label><%--[MSG#1003]--%>
            <br />
            <asp:CheckBox ID="chbAcknowledge" runat="server" Text="" />
            <asp:Label ID="lblAckText" runat="server" style="font-size:120%; font-weight:bold"></asp:Label><%--[MSG#1001]--%>
            <br />
            <asp:Button ID="btnAck" runat="server" CausesValidation="False" Text="Submit" CssClass="btn btn-primary btn-xs" 
                OnClientClick="return validateAck();"></asp:Button>
            <br />

            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsMain" runat="server" ShowSummary="true" DisplayMode="List" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgMain" />
            <br />

            <asp:Panel ID="panUploadAckFile" runat="server" Visible="false">
                <asp:GridView ID="gvFiles" runat="server" EmptyDataText="No file was found. This is required only if the recipient cannot acknowledge online." style="min-width:300px;"
                    AllowPaging="false" AllowSorting="false" AutoGenerateColumns="False" DataKeyNames="FileId" Visible="true"
                    Caption="Acknowledge Signature Files" ItemType="PolicyTracker.Lib.UploadFile" SelectMethod="gvFiles_GetData">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnDownload" runat="server" CausesValidation="False" CommandName="Download" CommandArgument='<%# Eval("FileId")%>'
                                    AlternateText="Download" ToolTip="Download">
                                    <span class="glyphicon glyphicon-download-alt"></span>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton ID="lbtnDelete" runat="server" CausesValidation="False" CommandName="DeleteFile" CommandArgument='<%# Eval("FileId")%>'
                                    AlternateText="Delete" ToolTip="Delete" 
                                    OnClientClick="return confirm('Are you sure you want to delete this file?');">
                                    <span class="glyphicon glyphicon-trash"></span>
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
                        <asp:TemplateField HeaderText="Disabled?" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox ID="chbDisabled" runat="server" Checked="<%# Item.PacketAckFiles.FirstOrDefault().Disabled%>" Enabled="false" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="CreateDT" HeaderText="Upload Date" SortExpression="CreateDT" />
                        <asp:BoundField DataField="CreateUser" HeaderText="Upload User" SortExpression="CreateUser" />
                    </Columns> 
                </asp:GridView>
                <br />
                <asp:Label ID="lblInfoUpload" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
                <asp:Label ID="lblErrorUpload" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>

                <asp:FileUpload ID="fileUpload" runat="server" Width="500px" style="margin:3px 0 3px 0;" />
                <asp:Button ID="btnUpload" runat="server" Text="Upload Ack Signature File" CssClass="btn btn-primary btn-xs" />
            </asp:Panel>

            <div id="id-verify-modal" class="modal fade">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title">Please Verify Your Identity <span class="glyphicon glyphicon-user"></span></h4>
                        </div>
                        <div class="modal-body">
                            <asp:FormView ID="fvEmployee" runat="server" AutoGenerateRows="False" DataKeyNames="EmpId" ItemType="PolicyTracker.Lib.vEmployee"
                                Caption="" DefaultMode="ReadOnly" Visible="true" SelectMethod="fvEmployee_GetItem" style="border:hidden;">
                                <ItemTemplate>
                                    <table class="DataWebControlStyle">
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
                                    </table>
                                </ItemTemplate>
                            </asp:FormView>
                        </div>
                        <div class="modal-footer">
                            <div style="font-size:120%; font-weight:bold; color: red">
                                Are you the person displayed above?<br />
                                <input type="radio" name="radIDVerify" id="radIDVerifyYes" value="Y" title="Yes" /> Yes
                                <input type="radio" name="radIDVerify" id="radIDVerifyNo" value="N" title="No" checked /> No
                                <input type="submit" name="btnSubmitIDVerify" value="Submit" class="btn btn-primary btn-sm" data-dismiss="modal" onclick="submitIDVerify()" />
                                <%--<asp:RadioButton id="radIDYes" runat="server" text="Yes" GroupName="IDVerify" />--%>
                                <%--<asp:RadioButton id="radIDNo" runat="server" text="No" GroupName="IDVerify" checked="true" />--%>
                                <%--<button id="btnIDVSubmit" runat="server" type="button" class="btn btn-primary btn-sm" data-dismiss="modal" onserverclick="btnIDVSubmit_ServerClick">Submit</button>--%>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpload" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
