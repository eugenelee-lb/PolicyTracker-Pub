<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RecipGroupSettings.aspx.vb" MasterPageFile="~/Site.Master"  Inherits="PolicyTracker.RecipGroupSettings" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/EmpSearch.ascx" TagPrefix="asp" TagName="EmpSearch" %>
<%@ Register Src="~/UserControls/EmpInfo.ascx" TagPrefix="asp" TagName="EmpInfo" %>
<%@ Register Src="~/UserControls/RecipGroupMembers.ascx" TagPrefix="asp" TagName="RecipGroupMembers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script>
        function pageLoad() {
            $("#emp-search").click(function () {
                $("#emp-search-modal").modal('show');
            });
            $('#emp-search-modal').on('shown.bs.modal', function () {
                $("input[name$='$EmpSearch$txtFilterName']").focus();
            })

            $(".select_chosen").chosen({
                disable_search_threshold: 10,
                no_results_text: "Nothing found!",
                search_contains: true
            });

            $(".select_chosen_e").chosen({
                disable_search_threshold: 10,
                no_results_text: "Nothing found!",
                width: "350px",
                search_contains: true
            });
        }

        function SelectEmp(varEmpId, varEmpName) {
            $("input[name$='txtEmpId']").val(varEmpId);
            $("input[name$='txtEmpName']").val(varEmpName);
            $("#emp-search-modal").modal('hide');
        }

        function showEmpInfo() {
            $("#emp-info-modal").modal('show');
            return true;
        }

        function reviewGroupMembers() {
            $("#group-members-modal").modal('show');
            return true;
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src='<%: Page.ResolveUrl("~/Scripts/chosen.jquery.min.js")%>' type="text/javascript"></script>
    <link href='<%: Page.ResolveClientUrl("~/Content/chosen.min.css")%>' rel="stylesheet" />

    <h2>Recipient Group Settings</h2>

    <div id="emp-search-modal" class="modal fade">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Search Employee</h4>
                </div>
                <div class="modal-body">
                    <asp:EmpSearch runat="server" ID="EmpSearch" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default btn-sm" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <div id="emp-info-modal" class="modal fade">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Employee Details <span class="glyphicon glyphicon-user"></span></h4>
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

    <div id="group-members-modal" class="modal fade">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Group Members
                        <span style="letter-spacing:-3px;">
                            <span class="glyphicon glyphicon-user"></span>
                            <span class="glyphicon glyphicon-user"></span>
                        </span>
                    </h4>
                </div>
                <div class="modal-body">
                    <asp:RecipGroupMembers runat="server" ID="RecipGroupMembers" />
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default btn-sm" data-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <a id="lnkBackToList" runat="server" href="~/PA/RecipGroups" >Back To Recipient Groups</a>
    <asp:DetailsView ID="dvRecipGroup" runat="server" AutoGenerateRows="False" DataKeyNames="RecipGroupId"
        DataSourceID="odsRecipGroup">
        <Fields>
            <asp:BoundField DataField="RecipGroupId" HeaderText="ID" SortExpression="RecipGroupId" Visible="false" />
            <asp:TemplateField HeaderText="Group Name" SortExpression="GroupName">
                <ItemTemplate>
                    <asp:Label ID="lblRecipGroupName" runat="server" Text='<%# Bind("GroupName")%>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Group Type" SortExpression="RecipGroupType">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlRecipGroupType" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("RecipGroupType")%>'
                        DataSourceID="odsRecipGroupType" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="false">
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsRecipGroupType" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                        <SelectParameters>
                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                            <asp:Parameter DefaultValue="RCP_GRP_TYPE" Name="catg" Type="String" />
                            <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Share Type">
                <ItemTemplate>
                    <asp:DropDownList ID="ddlShareType" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("ShareType") %>'
                        DataSourceID="odsShareType" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="false">
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsShareType" runat="server" OldValuesParameterFormatString="original_{0}"
                        SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                        <SelectParameters>
                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                            <asp:Parameter DefaultValue="SHARE_TYPE" Name="catg" Type="String" />
                            <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                    <asp:DropDownList ID="ddlDeptCodeV" runat="server" AppendDataBoundItems="true" SelectedValue='<%# Bind("DeptCode")%>'
                        DataSourceID="odsDeptCode" DataTextField="DeptDesc" DataValueField="DeptCode" Enabled="false"
                        AutoPostBack="false">
                        <asp:ListItem Value="">-- Select Department --</asp:ListItem>
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="odsDeptCode" runat="server"
                        SelectMethod="GetDepartmentsByStat"
                        TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression">
                        <SelectParameters>
                            <asp:Parameter Name="stat" Type="String" DefaultValue="A" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </ItemTemplate>
            </asp:TemplateField>
        </Fields>
    </asp:DetailsView>
    <asp:ObjectDataSource ID="odsRecipGroup" runat="server" SelectMethod="GetRecipGroupById"
        TypeName="PolicyTracker.Lib.PTRepository" DataObjectTypeName="PolicyTracker.Lib.RecipGroup"
        DeleteMethod="DeleteRecipGroup" InsertMethod="InsertRecipGroup" OldValuesParameterFormatString="original_{0}"
        UpdateMethod="UpdateRecipGroup" ConflictDetection="CompareAllValues">
        <SelectParameters>
            <asp:RouteParameter RouteKey="RecipGroupId" DefaultValue="" Name="RecipGroupId" Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="recipgroup" Type="Object" />
            <asp:Parameter Name="origRecipGroup" Type="Object" />
        </UpdateParameters>
    </asp:ObjectDataSource>
    <br />

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <table><tr><td>
            <asp:Panel ID="panIND" runat="server" GroupingText="Individuals" Visible="false">
                <asp:label id="lblInfoMembers" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:label>
                <asp:label id="lblErrorMembers" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
                <asp:ValidationSummary ID="vsMembers" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgMembers" />
                <table>
                    <tr>
                        <td style="padding: 0 10px 0 10px; vertical-align:top;">
                            <asp:GridView ID="gvMembers" runat="server" AutoGenerateColumns="False" DataKeyNames="EmpId"
                                AllowPaging="false" AllowSorting="false" EmptyDataText="No record was found."
                                Caption="Assigned Members">
                                <Columns>
                                    <asp:TemplateField ShowHeader="False">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ibtnDeleteMember" runat="server" CausesValidation="False" CommandName="DeleteMember" CommandArgument='<%# Eval("EmpId")%>'
                                                AlternateText="Delete" ToolTip="Delete" 
                                                OnClientClick="return confirm('Are you sure you want to delete this record?');">
                                                <span class="glyphicon glyphicon-trash"></span>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Emp ID">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEmpId" runat="server" Text='<%# Eval("EmpId")%>'></asp:Label>
                                            <asp:LinkButton ID="lbtnEmpInfo" runat="server" CausesValidation="false" CommandName="EmpInfo" CommandArgument='<%# Eval("EmpId")%>'
                                                AlternateText="Employee Details" ToolTip="Employee Details" 
                                                OnClientClick="return showEmpInfo();">
                                                <span class="glyphicon glyphicon-user" id="emp-info"></span>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Name" HeaderText="Name" />
                                </Columns>
                            </asp:GridView>
                            <div class="row" style="margin: 3px 0 3px 0; font-size: 18px;">
                                <%--<asp:LinkButton ID="lbtnDownloadMembers" runat="server" CausesValidation="false" tooltip="Download members as CSV file">
                                    Download <span class="glyphicon glyphicon-download"></span>
                                </asp:LinkButton>&nbsp;&nbsp;
                                <asp:LinkButton ID="lbtnUploadMembers" runat="server" CausesValidation="false" tooltip="Upload members from CSV file">
                                    Upload <span class="glyphicon glyphicon-upload"></span>
                                </asp:LinkButton>--%>
                                <button id="btnDownload" runat="server" type="button" class="btn btn-primary btn-sm" causesvalidation="false"
                                    tooltip="Download members as CSV file">
                                    Download <span class="glyphicon glyphicon-download"></span>
                                </button>
                            </div>
                        </td>
                        <td style="padding: 0 10px 0 10px; vertical-align:top;">
                            <asp:Panel ID="panAddMember" runat="server" GroupingText="Add Member">
                                <table cellpadding="3px">
                                    <tr><td>Emp ID</td><td>Name</td></tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txtEmpId" runat="server" Columns="7" MaxLength="10" />
                                            <asp:RequiredFieldValidator ID="rfvEmpId" runat="server" ControlToValidate="txtEmpId" 
                                                ErrorMessage="Please select an employee" Text="*"
                                                Display="Dynamic" ValidationGroup="vgMembers"></asp:RequiredFieldValidator>
                                            <span class="glyphicon glyphicon-search" id="emp-search" style="cursor: pointer;"></span>
                                        </td>
                                        <td><asp:TextBox ID="txtEmpName" runat="server" Columns="25" MaxLength="50" ReadOnly="true" /></td>
                                        <td><asp:Button ID="btnAddMember" runat="server" Text="Add" ValidationGroup="vgMembers" CssClass="btn btn-primary btn-xs"/></td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <asp:label id="lblErrorInd" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
            </asp:Panel> 
            </td></tr></table>

            <table><tr><td>
            <asp:Panel ID="panOrgs" runat="server" GroupingText="Attributes" Visible="false">
                <table>
                    <tr>
                        <td colspan="2">
                            <asp:label id="lblInfoOrgs" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:label>
                            <asp:label id="lblErrorOrgs" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
                            <asp:ValidationSummary ID="vsOrgs" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgOrgs" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 0 10px 0 10px; vertical-align:top;">
                            <asp:GridView ID="gvOrgs" runat="server" AutoGenerateColumns="False" DataKeyNames="OrgCode"
                                AllowPaging="false" AllowSorting="false" EmptyDataText="No record was found."
                                Caption="Assigned Organizations">
                                <Columns>
                                    <asp:TemplateField ShowHeader="False">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ibtnDeleteOrg" runat="server" CausesValidation="False" CommandName="Remove" CommandArgument='<%# Eval("OrgCode")%>'
                                                AlternateText="Delete" ToolTip="Delete" 
                                                OnClientClick="return confirm('Are you sure you want to delete this record?');">
                                                <span class="glyphicon glyphicon-trash"></span>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="OrgCode" HeaderText="Org" Visible="false" />
                                    <asp:BoundField DataField="OrgDesc" HeaderText="Organization" Visible="true" />
                                </Columns>
                            </asp:GridView>
                        </td>
                        <td style="padding: 0 10px 0 10px; vertical-align:top;">
                            <asp:Panel ID="panAddOrg" runat="server" GroupingText="Assign Organization">
                                <table cellpadding="3px">
                                    <tr><td>Organization</td></tr>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="ddlOrgToAdd" runat="server" AppendDataBoundItems="false"
                                                DataSourceID="" DataTextField="OrgDesc" DataValueField="OrgCode"
                                                AutoPostBack="false" CssClass="select_chosen">
                                                <asp:ListItem Text="-- Select --" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvOrgToAdd" runat="server" ControlToValidate="ddlOrgToAdd"
                                                ErrorMessage="Please select an Organization" Text="*" Display="Dynamic" ValidationGroup="vgOrgs"></asp:RequiredFieldValidator>
                                        </td>
                                        <td><asp:Button ID="btnAddOrg" runat="server" Text="Assign" ValidationGroup="vgOrgs" CssClass="btn btn-primary btn-xs" /></td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:label id="lblInfoClasses" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:label>
                            <asp:label id="lblErrorClasses" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
                            <asp:ValidationSummary ID="vsClasses" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgClasses" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 0 10px 0 10px; vertical-align:top;">
                            <asp:GridView ID="gvClasses" runat="server" AutoGenerateColumns="False" DataKeyNames="ClassCode"
                                AllowPaging="false" AllowSorting="false" EmptyDataText="No record was found."
                                Caption="Assigned Classifications">
                                <Columns>
                                    <asp:TemplateField ShowHeader="False">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="ibtnDeleteClass" runat="server" CausesValidation="False" CommandName="Remove" CommandArgument='<%# Eval("ClassCode")%>'
                                                AlternateText="Delete" ToolTip="Delete" 
                                                OnClientClick="return confirm('Are you sure you want to delete this record?');">
                                                <span class="glyphicon glyphicon-trash"></span>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ClassCode" HeaderText="Class" Visible="false" />
                                    <asp:BoundField DataField="ClassDesc" HeaderText="Classification" Visible="true" />
                                </Columns>
                            </asp:GridView>
                        </td>
                        <td style="padding: 0 10px 0 10px; vertical-align:top;">
                            <asp:Panel ID="panAddClass" runat="server" GroupingText="Assign Classification">
                                <table cellpadding="3px">
                                    <tr><td>Classification</td></tr>
                                    <tr>
                                        <td>
                                            <asp:DropDownList ID="ddlClassToAdd" runat="server" AppendDataBoundItems="false"
                                                DataSourceID="" DataTextField="ClassDesc" DataValueField="ClassCode"
                                                AutoPostBack="false" CssClass="select_chosen">
                                                <asp:ListItem Text="-- Select --" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="rfvClassToAdd" runat="server" ControlToValidate="ddlClassToAdd"
                                                ErrorMessage="Please select a Classification" Text="*" Display="Dynamic" ValidationGroup="vgClasses"></asp:RequiredFieldValidator>
                                        </td>
                                        <td><asp:Button ID="btnAddClass" runat="server" Text="Assign" ValidationGroup="vgClasses" CssClass="btn btn-primary btn-xs" /></td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <%--<tr>
                        <td colspan="2" style="padding: 20px 10px 0 10px; vertical-align:top;">
                        </td>
                    </tr>--%>
                </table>
                <br />

                <div id="divFormula" runat="server" class="alert alert-info" role="alert" visible="false">
                    <b>Selection Formula</b><br />
                    <asp:Literal ID="litFormula" runat="server"></asp:Literal><br /><br />
                    <b>Review Group Members</b>
                    <asp:LinkButton ID="lbtnReviewGroupMembers" runat="server" CausesValidation="false"
                        AlternateText="Group Members" ToolTip="" OnClientClick="return reviewGroupMembers();">
                        <span style="letter-spacing:-3px;">
                            <span class="glyphicon glyphicon-user"></span>
                            <span class="glyphicon glyphicon-user"></span>
                        </span>
                    </asp:LinkButton>
                </div>
                
            </asp:Panel> 
            </td></tr></table>

        </ContentTemplate>
        <Triggers>
            <%--<asp:PostBackTrigger ControlID="btnUpload" />--%>
            <asp:PostBackTrigger ControlID="btnDownload" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

