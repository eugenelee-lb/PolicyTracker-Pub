<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Policies.aspx.vb" Inherits="PolicyTracker.Policies" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function pageLoad() {
            // keep the current tab active bootstrap after a page reload/postback
            $('a[data-toggle="tab"]').on('shown.bs.tab', function () {
                //save the latest tab; use cookies if you like 'em better:
                sessionStorage.setItem('lastTab_Pol', $(this).attr('href'));
            });
            //go to the latest tab, if it exists:
            var lastTab = sessionStorage.getItem('lastTab_Pol');
            if (lastTab && $('a[href="' + lastTab + '"]').is(":visible")) {
                $('a[href="' + lastTab + '"]').tab('show');
            }
            else {
                // Set the first tab if cookie do not exist
                $('a[data-toggle="tab"]:first').tab('show');
            }

            $(".select_chosen_150").chosen({
                disable_search_threshold: 5,
                no_results_text: "Nothing found!",
                width: "150px",
                search_contains: true
            });

            $('select[name$="ddlShareType"]').change(function () {
                share_type = $(this).val();
                //var rfv_dept_code = document.getElementById('MainContent_dvEmpGroup_rfvDeptCode');
                //var vs_details = document.getElementById("MainContent_vsDetails");
                if (share_type == "DEPT") {
                    $('select[name$="ddlDeptCode"]').prop('disabled', false);
                    $('select[name$="ddlDeptCode"]').prop('hidden', false);
                    //if (rfv_dept_code != null) {
                    //    rfv_dept_code.enabled = true;
                    //    //ValidatorEnable(rfv_dept_code, true);
                    //}
                }
                else {
                    $('select[name$="ddlDeptCode"]').prop('disabled', true);
                    $('select[name$="ddlDeptCode"]').prop('hidden', true);
                    //if (rfv_dept_code != null && vs_details != null) {
                    //    rfv_dept_code.enabled = false;
                    //    rfv_dept_code.isvalid = true;
                    //    //ValidatorEnable(rfv_dept_code, false);
                    //    ValidatorUpdateDisplay(rfv_dept_code);
                    //    ValidationSummaryOnSubmit();
                    //}
                }
            }).change();

            $(".tooltip").hide();
            $(".showTooltip").tooltip('show');

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
        }

        // ShareType, DeptCode
        function validateDeptCode(source, arguments) {
            //alert("args.Value:" + arguments.Value);
            arguments.IsValid = true;
            var ddlShareType = document.getElementById('MainContent_dvPolicy_ddlShareType');
            var ddlDeptCode = document.getElementById('MainContent_dvPolicy_ddlDeptCode');
            //alert(ddlShareType.value);
            if (ddlShareType.value == "DEPT" && ddlDeptCode.value == "") {
                arguments.IsValid = false;
                //alert("false");
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script src='<%: Page.ResolveUrl("~/Scripts/chosen.jquery.min.js")%>' type="text/javascript"></script>
    <link href='<%: Page.ResolveClientUrl("~/Content/chosen.min.css")%>' rel="stylesheet" />

    <h2><span id="spnHeader" runat="server">Policies</span></h2>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <div id="divSearchFilters" runat="server">
            <table>
                <tr>
                    <td>Name:</td>
                    <td><asp:TextBox ID="txtFilterName" runat="server" Columns="40" MaxLength="50"></asp:TextBox></td>
                    <td>Status:</td>
                    <td>
                        <asp:DropDownList ID="ddlFilterStat" runat="server" AppendDataBoundItems="false"
                            DataSourceID="odsFilterStat" DataTextField="CMCodeDesc" DataValueField="CMCode"
                            AutoPostBack="false">
                        </asp:DropDownList><asp:ObjectDataSource ID="odsFilterStat" runat="server"
                            SelectMethod="GetCommonCodesByCatg"
                            TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression">
                            <SelectParameters>
                                <asp:Parameter Name="catg" Type="String" DefaultValue="FILTER_STAT" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td></td>
                </tr>
                <tr>
                    <td>Description:</td>
                    <td>
                        <asp:TextBox ID="txtFilterDesc" runat="server" Columns="40" MaxLength="1000"></asp:TextBox></td>
                    <td>Owner:</td>
                    <td>
                        <asp:DropDownList ID="ddlFilterOwner" runat="server" AppendDataBoundItems="true"
                            AutoPostBack="false">
                            <asp:ListItem Value="MY">My Policies</asp:ListItem>
                            <asp:ListItem Value="ALL">All Policies</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-info btn-xs" />
                    </td>
                </tr>
            </table>
            </div>

            <asp:GridView ID="gvPolicies" runat="server" AutoGenerateColumns="False" 
                ItemType="PolicyTracker.Lib.Policy" DataKeyNames="PolicyId" SelectMethod="gvPolicies_GetData"
                AllowPaging="True" AllowSorting="True" EmptyDataText="No record was found.">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" Visible="true" CausesValidation="false" />
                    <asp:BoundField DataField="PolicyId" HeaderText="ID" ReadOnly="True" SortExpression="PolicyId" Visible="false" />
                    <asp:BoundField DataField="PolicyName" HeaderText="Policy Name" SortExpression="PolicyName" />
                    <asp:BoundField DataField="PolicyDesc" HeaderText="Description" SortExpression="PolicyDesc" Visible="false" />
                    <asp:CheckBoxField DataField="ShowDisclaimer" HeaderText="Show Disclaimer" SortExpression="ShowDisclaimer">
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:CheckBoxField>
                    <asp:BoundField DataField="ShareType" HeaderText="Share Type" SortExpression="ShareType" />
                    <asp:BoundField DataField="DeptCode" HeaderText="Dept" SortExpression="DeptCode" />
                    <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled">
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:CheckBoxField>
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" Visible="false" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" SortExpression="CreateUser" Visible="false" />
                    <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" />
                    <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="LastUpdateUser" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>
            <br />

            <ul class="nav nav-pills">
                <li class="" id="tliPolicy" runat="server"><a href="#tpPolicy" id="tlkPolicy" runat="server" data-toggle="tab" aria-expanded="true">Policy</a></li>
                <li class="" id="tliFiles" runat="server"
                    data-toggle="tooltip" data-placement="bottom" data-container="body" title="Upload file">
                    <a href="#tpFiles" id="tlkFiles" runat="server" data-toggle="tab" aria-expanded="false">Files 
                        <span runat="server" id="spnFileCnt" class="badge">0</span>
                    </a>
                </li>
                <li class="" id="tliSchedules" runat="server">
                    <a href="#tpSchedules" id="tlkSchedules" runat="server" data-toggle="tab" aria-expanded="false">Schedules 
                        <span runat="server" id="spnSchCnt" class="badge">0</span>
                    </a>
                </li>
                <li class="" id="tliOwners" runat="server">
                    <a href="#tpOwners" id="tlkOwners" runat="server" data-toggle="tab" aria-expanded="false">Owners 
                        <span runat="server" id="spnOwnCnt" class="badge">0</span>
                    </a>
                </li>
            </ul>

            <div class="tab-content" id="tcPolicy">
                <div class="tab-pane fade in" id="tpPolicy">
                    
                    <br />
                    <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
                    <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
                    <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="List" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />

                    <asp:DetailsView ID="dvPolicy" runat="server" AutoGenerateRows="False" Caption=""
                        DataKeyNames="PolicyId" DataSourceID="odsPolicyDetails" DefaultMode="Insert" Visible="true">
                        <Fields>
                            <asp:TemplateField HeaderText="Policy Name" SortExpression="PolicyName">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtPolicyName" runat="server" Columns="80" MaxLength="100" Text='<%# Bind("PolicyName") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPolicyName" runat="server" ControlToValidate="txtPolicyName"
                                        ValidationGroup="vgDetails" ErrorMessage="Policy Name is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtPolicyName" runat="server" Columns="80" MaxLength="100" Text='<%# Bind("PolicyName") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvPolicyName" runat="server" ControlToValidate="txtPolicyName"
                                        ValidationGroup="vgDetails" ErrorMessage="Policy Name is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblPolicyName" runat="server" Text='<%# Bind("PolicyName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description" SortExpression="PolicyDescription">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtPolicyDescription" runat="server" Columns="100" onkeyup="return ismaxlength(this, 1000);"
                                        TextMode="MultiLine" Rows="8" Text='<%# Bind("PolicyDesc")%>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ControlToValidate="txtPolicyDescription"
                                        ErrorMessage="(Required)" Display="dynamic" ValidationGroup="vgDetails" Visible="false"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtPolicyDescription" runat="server" Columns="100" onkeyup="return ismaxlength(this, 1000);"
                                        TextMode="MultiLine" Rows="8" Text='<%# Bind("PolicyDesc")%>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ControlToValidate="txtPolicyDescription"
                                        ErrorMessage="(Required)" Display="dynamic" ValidationGroup="vgDetails" Visible="false"></asp:RequiredFieldValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("PolicyDesc")%>' Visible="false"></asp:Label>
                                    <asp:Label ID="Label1" runat="server" Text='<%# EvalHtmlEncode("PolicyDesc") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CheckBoxField DataField="ShowDisclaimer" HeaderText="Show Disclaimer" SortExpression="ShowDisclaimer"></asp:CheckBoxField>
                            <asp:TemplateField HeaderText="Disclaimer" SortExpression="Disclaimer">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtDisclaimer" runat="server" Columns="100" onkeyup="return ismaxlength(this, 1000);"
                                        TextMode="MultiLine" Rows="5" Text='<%# Bind("Disclaimer") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDisclaimer" runat="server" ControlToValidate="txtDisclaimer"
                                        ValidationGroup="vgDetails" ErrorMessage="(Required)" Display="dynamic" Visible="false"></asp:RequiredFieldValidator>
                                    <br />
                                    <em>You can include URL's in this field to refer to web resources.</em>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtDisclaimer" runat="server" Columns="100" onkeyup="return ismaxlength(this, 1000);"
                                        TextMode="MultiLine" Rows="5" Text='<%# Bind("Disclaimer") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDisclaimer" runat="server" ControlToValidate="txtDisclaimer"
                                        ValidationGroup="vgDetails" ErrorMessage="(Required)" Display="dynamic" Visible="false"></asp:RequiredFieldValidator>
                                    <br />
                                    <em>You can include URL's in this field to refer to web resources.</em>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDisclaimer" runat="server" Text='<%# Bind("Disclaimer") %>' Visible="false"></asp:Label>
                                    <asp:Label ID="lblDisclaimerEval" runat="server" Text='<%# EvalHtmlEncode("Disclaimer") %>' CssClass="content-with-url"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Share Type">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlShareType" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("ShareType") %>'
                                        DataSourceID="odsShareType" DataTextField="CMCodeDesc" DataValueField="CMCode">
                                        <asp:ListItem Value="">-- Select Share Type --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvShareType" runat="server" ControlToValidate="ddlShareType"
                                        ErrorMessage="Share Type is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                                    <asp:ObjectDataSource ID="odsShareType" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="SHARE_TYPE" Name="catg" Type="String" />
                                            <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <asp:DropDownList ID="ddlDeptCode" runat="server" AppendDataBoundItems="true" SelectedValue='<%# Bind("DeptCode")%>'
                                        DataSourceID="odsDeptCode" DataTextField="DeptDesc" DataValueField="DeptCode"
                                        AutoPostBack="false">
                                        <asp:ListItem Value="">-- Select Department --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CustomValidator ID="cvDeptCode" runat="server" ControlToValidate="ddlShareType" 
                                        ClientValidationFunction="validateDeptCode" OnServerValidate="cvDeptCode_ServerValidate"
                                        ErrorMessage="Department is required when Share Type is Department" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:CustomValidator>
                                    <asp:ObjectDataSource ID="odsDeptCode" runat="server"
                                        SelectMethod="GetDepartmentsByAdmin" OnSelecting="odsDeptCode_Selecting"
                                        TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter Name="userRole" Type="String" />
                                            <asp:Parameter Name="userId" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="ddlShareType" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("ShareType") %>'
                                        DataSourceID="odsShareType" DataTextField="CMCodeDesc" DataValueField="CMCode">
                                        <asp:ListItem Value="">-- Select Share Type --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvShareType" runat="server" ControlToValidate="ddlShareType"
                                        ErrorMessage="Share Type is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                                    <asp:ObjectDataSource ID="odsShareType" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="SHARE_TYPE" Name="catg" Type="String" />
                                            <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <asp:DropDownList ID="ddlDeptCode" runat="server" AppendDataBoundItems="true" SelectedValue='<%# Bind("DeptCode")%>'
                                        DataSourceID="odsDeptCode" DataTextField="DeptDesc" DataValueField="DeptCode"
                                        AutoPostBack="false">
                                        <asp:ListItem Value="">-- Select Department --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:CustomValidator ID="cvDeptCode" runat="server" ControlToValidate="ddlShareType" 
                                        ClientValidationFunction="validateDeptCode" OnServerValidate="cvDeptCode_ServerValidate"
                                        ErrorMessage="Department is required when Share Type is Department" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:CustomValidator>
                                    <asp:ObjectDataSource ID="odsDeptCode" runat="server"
                                        SelectMethod="GetDepartmentsByAdmin" OnSelecting="odsDeptCode_Selecting"
                                        TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter Name="userRole" Type="String" />
                                            <asp:Parameter Name="userId" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </InsertItemTemplate>
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
                                        <asp:ListItem Value="">-- N/A --</asp:ListItem>
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
                            <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" InsertVisible="false"></asp:CheckBoxField>
                            <asp:BoundField DataField="CreateDT" HeaderText="Create Date" InsertVisible="False" DataFormatString="{0:M/d/yyyy h:mm:ss.fff tt}"
                                ReadOnly="True" SortExpression="CreateDT" />
                            <asp:BoundField DataField="CreateUser" HeaderText="Create User" InsertVisible="False"
                                ReadOnly="True" SortExpression="CreateUser" />
                            <asp:TemplateField HeaderText="Last Update Date" SortExpression="LastUpdateDT" InsertVisible="False">
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
                            <asp:TemplateField HeaderText="Last Update User" SortExpression="LastUpdateUser" InsertVisible="False">
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
                                    <asp:LinkButton ID="lbtnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                        Text="Delete" CssClass="btn btn-default btn-xs" OnClientClick="return confirm('Are you sure you want to delete this record?');"></asp:LinkButton>
                                    <asp:LinkButton ID="lbtnNew" runat="server" CausesValidation="False" CommandName="New"
                                        Text="Add New Policy" CssClass="btn btn-primary btn-xs"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                    <asp:ObjectDataSource ID="odsPolicyDetails" runat="server" ConflictDetection="CompareAllValues"
                        DeleteMethod="DeletePolicy" InsertMethod="InsertPolicy" OldValuesParameterFormatString="orig{0}"
                        SelectMethod="GetPolicyByID" TypeName="PolicyTracker.Lib.PTRepository" DataObjectTypeName="PolicyTracker.Lib.Policy"
                        UpdateMethod="UpdatePolicy">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="gvPolicies" DefaultValue="" Name="policyId" PropertyName="SelectedValue" Type="Int32" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="policy" Type="Object" />
                            <asp:Parameter Name="origPolicy" Type="Object" />
                        </UpdateParameters>
                    </asp:ObjectDataSource>
                    <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>
                </div>

                <div class="tab-pane fade" id="tpSchedules">
                    <br />
                    <table>
                        <tr>
                            <td colspan="2">
                                <asp:label id="lblInfoSchedules" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:label>
                                <asp:label id="lblErrorSchedules" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
                                <asp:ValidationSummary ID="vsSchedules" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgSchedules" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 0 10px 0 10px; vertical-align:top;">
                                <asp:GridView ID="gvSchedules" runat="server" AutoGenerateColumns="False" DataKeyNames="ScheduleId"
                                    AllowPaging="false" AllowSorting="false" EmptyDataText="No record was found."
                                    ItemType="PolicyTracker.Lib.Schedule" Caption="Assigned Schedules">
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False" Visible="false">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ibtnDeleteSchedule" runat="server" CausesValidation="False" CommandName="DeleteSchedule" CommandArgument='<%# Eval("ScheduleId") %>'
                                                    AlternateText="Delete" ToolTip="Delete" 
                                                    OnClientClick="return confirm('Are you sure you want to delete this record?');">
                                                    <span class="glyphicon glyphicon-trash"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="ScheduleId" HeaderText="ID" Visible="false" />
                                        <asp:TemplateField HeaderText="Schedule Name">
                                            <ItemTemplate>
                                                <%#: Item.ScheduleName %>
                                                <asp:HyperLink ID="hlkOpenSchedule" runat="server" Target="_blank" ToolTip="Open"
                                                    NavigateUrl='<%#: "~/PA/Schedule/" & Item.ScheduleId.ToString%>'>
                                                    <span class="glyphicon glyphicon-new-window"></span>
                                                </asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" >
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:CheckBoxField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                            <td style="padding: 0 10px 0 10px; vertical-align:top;">
                                <asp:Panel ID="panAddSchedule" runat="server" GroupingText="Add Schedule" Visible="false">
                                    <table cellpadding="3px">
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlScheduleToAdd" runat="server" 
                                                    DataValueField="ScheduleID" DataTextField="ScheduleName" 
                                                    CssClass="select_chosen_300">
                                                    <asp:ListItem Text="-- Select --" Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvScheduleToAdd" runat="server" ControlToValidate="ddlScheduleToAdd"
                                                    ErrorMessage="Please select a Schedule" Text="*" Display="Dynamic" ValidationGroup="vgSchedules"></asp:RequiredFieldValidator>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnAddSchedule" runat="server" Text="Add" ValidationGroup="vgSchedules" CssClass="btn btn-primary btn-xs" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel> 
                            </td>
                        </tr>
                    </table>
                </div>

                <div class="tab-pane fade" id="tpFiles">
                    <br />
                    <asp:GridView ID="gvFiles" runat="server" EmptyDataText="No file was found." AllowPaging="false" 
                        AllowSorting="false" AutoGenerateColumns="False" DataKeyNames="FileId" 
                        Caption="" ItemType="PolicyTracker.Lib.UploadFile">
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
                            <asp:BoundField DataField="CreateDT" HeaderText="Upload Date" SortExpression="CreateDT" />
                            <asp:BoundField DataField="CreateUser" HeaderText="Upload User" SortExpression="CreateUser" />
                        </Columns> 
                    </asp:GridView>

                    <asp:Label ID="lblInfoUpload" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
                    <asp:Label ID="lblErrorUpload" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
                    <br />
                    <asp:FileUpload ID="fileUpload" runat="server" Width="500px" style="margin:3px 0 3px 0;" />
                    <asp:Button ID="btnUpload" runat="server" Text="Upload File" CssClass="btn btn-primary btn-xs" />
                </div>

                <div class="tab-pane fade" id="tpOwners">
                    <br />
                    <table>
                        <tr>
                            <td colspan="2">
                                <asp:label id="lblInfoOwners" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:label>
                                <asp:label id="lblErrorOwners" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
                                <asp:ValidationSummary ID="vsOwners" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgOwners" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 0 10px 0 10px; vertical-align:top;">
                                <asp:GridView ID="gvOwners" runat="server" EmptyDataText="No record was found." AllowPaging="true" 
                                    AllowSorting="false" AutoGenerateColumns="False" DataKeyNames="Owner" Visible="true"
                                    Caption="">
                                    <Columns>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ibtnDelete" runat="server" CausesValidation="False" CommandName="Remove" CommandArgument='<%# Eval("Owner")%>'
                                                    AlternateText="Delete" ToolTip="Delete" 
                                                    OnClientClick="return confirm('Are you sure you want to delete this record?');">
                                                    <span class="glyphicon glyphicon-trash"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Owner" HeaderText="Owner" SortExpression="Owner" />
                                        <asp:BoundField DataField="OwnerName" HeaderText="Owner Name" SortExpression="OwnerName" />
                                    </Columns> 
                                </asp:GridView>
                            </td>
                            <td style="padding: 0 10px 0 10px; vertical-align:top;">
                                <asp:Panel ID="panAddOwner" runat="server" GroupingText="Add Owner">
                                    <table cellpadding="3px">
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlOwnerToAdd" runat="server" AppendDataBoundItems="false"
                                                    DataSourceID="" DataTextField="OwnerName" DataValueField="Owner"
                                                    AutoPostBack="false" CssClass="select_chosen_150">
                                                    <asp:ListItem Text="-- Select --" Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvOwnerToAdd" runat="server" ControlToValidate="ddlOwnerToAdd"
                                                    ErrorMessage="Please select a user" Text="*" Display="Dynamic" ValidationGroup="vgOwners"></asp:RequiredFieldValidator>
                                            </td>
                                            <td><asp:Button ID="btnAddOwner" runat="server" Text="Add" ValidationGroup="vgOwners" CssClass="btn btn-primary btn-xs" /></td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnUpload" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
