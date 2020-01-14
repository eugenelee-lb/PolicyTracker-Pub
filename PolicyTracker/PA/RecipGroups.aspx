<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RecipGroups.aspx.vb"  MasterPageFile="~/Site.Master"  Inherits="PolicyTracker.RecipGroups" %>

<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function pageLoad() {
            // keep the current tab active bootstrap after a page reload/postback
            $('a[data-toggle="tab"]').on('shown.bs.tab', function () {
                //save the latest tab; use cookies if you like 'em better:
                sessionStorage.setItem('lastTab_RecipGroup', $(this).attr('href'));
            });
            //go to the latest tab, if it exists:
            var lastTab = sessionStorage.getItem('lastTab_RecipGroup');
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
                //var rfv_dept_code = document.getElementById('MainContent_dvRecipGroup_rfvDeptCode');
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
        }

        // ShareType, DeptCode
        function validateDeptCode(source, arguments) {
            //alert("args.Value:" + arguments.Value);
            arguments.IsValid = true;
            var ddlShareType = document.getElementById('MainContent_dvRecipGroup_ddlShareType');
            var ddlDeptCode = document.getElementById('MainContent_dvRecipGroup_ddlDeptCode');
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

    <h2>Recipient Groups</h2>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <table>
                <tr>
                    <td>Name:</td>
                    <td><asp:TextBox ID="txtFilterName" runat="server" Columns="40" MaxLength="50"></asp:TextBox></td>
                    <td>Owner:</td>
                    <td>
                        <asp:DropDownList ID="ddlFilterOwner" runat="server" AppendDataBoundItems="true"
                            AutoPostBack="false">
                            <asp:ListItem Value="MY">My Groups</asp:ListItem>
                            <asp:ListItem Value="ALL">All Groups</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>Status:</td>
                    <td>
                        <asp:DropDownList ID="ddlFilterStat" runat="server" AppendDataBoundItems="false"
                            DataSourceID="odsFilterStat" DataTextField="CMCodeDesc" DataValueField="CMCode"
                            AutoPostBack="false">
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="odsFilterStat" runat="server"
                            SelectMethod="GetCommonCodesByCatg"
                            TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression">
                            <SelectParameters>
                                <asp:Parameter Name="catg" Type="String" DefaultValue="FILTER_STAT" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                    <td>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-info btn-xs" />
                    </td>
                </tr>
            </table>

            <asp:GridView ID="gvRecipGroups" runat="server" AutoGenerateColumns="False" 
                ItemType="PolicyTracker.Lib.RecipGroup" SelectMethod="gvRecipGroups_GetData" DataKeyNames="RecipGroupId"
                AllowPaging="True" AllowSorting="True"
                EmptyDataText="No record was found.">
                <Columns>
                    <asp:TemplateField ShowHeader="False" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnSelect" runat="server" CausesValidation="False" ToolTip="Select"
                                CommandName="Select" AlternateText="Select" EnableViewState="false">Select</asp:LinkButton>
                            <asp:LinkButton ID="btnSettings" runat="server" CausesValidation="False" ToolTip="Settings" 
                                CommandArgument='<%# Eval("RecipGroupId")%>' 
                                CommandName="Settings" EnableViewState="true"><span class="glyphicon glyphicon-cog"></span></asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="RecipGroupId" HeaderText="ID" SortExpression="RecipGroupId" ReadOnly="True" Visible="false" />
                    <asp:BoundField DataField="GroupName" HeaderText="Group Name" SortExpression="GroupName" />
                    <asp:BoundField DataField="RecipGroupType" HeaderText="Group Type" SortExpression="RecipGroupType" />
                    <asp:BoundField DataField="ShareType" HeaderText="Share Type" SortExpression="ShareType" />
                    <asp:BoundField DataField="DeptCode" HeaderText="Dept" SortExpression="DeptCode" />
                    <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled?" SortExpression="Disabled" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" SortExpression="CreateUser" />
                    <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" />
                    <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="LastUpdateUser" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>
            <br />

            <ul class="nav nav-pills">
                <li class="" id="tliRecipGroup" runat="server"><a href="#tpRecipGroup" id="tlkRecipGroup" runat="server" data-toggle="tab" aria-expanded="true">Recipient Group</a></li>
                <li class="" id="tliOwners" runat="server">
                    <a href="#tpOwners" id="tlkOwners" runat="server" data-toggle="tab" aria-expanded="false">Owners 
                        <span runat="server" id="spnOwnCnt" class="badge">0</span>
                    </a>
                </li>
            </ul>

            <div class="tab-content" id="tcRecipGroup">
                <div class="tab-pane fade in" id="tpRecipGroup">
                    
                    <br />
                    <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
                    <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
                    <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />

                    <asp:DetailsView ID="dvRecipGroup" runat="server" AutoGenerateRows="False" DataKeyNames="RecipGroupId"
                        DataSourceID="odsRecipGroup" Caption="" DefaultMode="Insert">
                        <Fields>
                            <asp:BoundField DataField="RecipGroupId" HeaderText="ID" SortExpression="RecipGroupId" Visible="false" />
                            <asp:TemplateField HeaderText="Group Name" SortExpression="GroupName">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtRecipGroupName" runat="server" Columns="50" MaxLength="50" Text='<%# Bind("GroupName")%>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvRecipGroupName" runat="server" ControlToValidate="txtRecipGroupName"
                                        ErrorMessage="Group Name is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>&nbsp;
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtRecipGroupName" runat="server" Columns="50" MaxLength="50" Text='<%# Bind("GroupName")%>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvRecipGroupName" runat="server" ControlToValidate="txtRecipGroupName"
                                        ErrorMessage="Group Name is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>&nbsp;
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblRecipGroupName" runat="server" Text='<%# Bind("GroupName")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Group Type" SortExpression="RecipGroupType">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlRecipGroupType" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("RecipGroupType")%>'
                                        DataSourceID="odsRecipGroupType" DataTextField="CMCodeDesc" DataValueField="CMCode">
                                        <asp:ListItem Value="">-- Select Group Type --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvRecipGroupType" runat="server" ControlToValidate="ddlRecipGroupType"
                                        ErrorMessage="Group Type is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                                    <asp:ObjectDataSource ID="odsRecipGroupType" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="RCP_GRP_TYPE" Name="catg" Type="String" />
                                            <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="ddlRecipGroupType" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("RecipGroupType")%>'
                                        DataSourceID="odsRecipGroupType" DataTextField="CMCodeDesc" DataValueField="CMCode">
                                        <asp:ListItem Value="">-- Select Group Type --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvRecipGroupType" runat="server" ControlToValidate="ddlRecipGroupType"
                                        ErrorMessage="Group Type is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                                    <asp:ObjectDataSource ID="odsRecipGroupType" runat="server" OldValuesParameterFormatString="original_{0}"
                                        SelectMethod="GetCommonCodesByCatgStatus" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="RCP_GRP_TYPE" Name="catg" Type="String" />
                                            <asp:Parameter DefaultValue="true" Name="stat" Type="Boolean" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </InsertItemTemplate>
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
                            <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" InsertVisible="false" />
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
                                        Text="Add New Recipient Group" CssClass="btn btn-primary btn-xs"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                    <asp:ObjectDataSource ID="odsRecipGroup" runat="server" SelectMethod="GetRecipGroupById"
                        TypeName="PolicyTracker.Lib.PTRepository" DataObjectTypeName="PolicyTracker.Lib.RecipGroup"
                        DeleteMethod="DeleteRecipGroup" InsertMethod="InsertRecipGroup" OldValuesParameterFormatString="orig{0}"
                        UpdateMethod="UpdateRecipGroup" ConflictDetection="CompareAllValues">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="gvRecipGroups" DefaultValue="" Name="RecipGroupId" PropertyName="SelectedDataKey.Values[0]" Type="Int32" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="RecipGroup" Type="Object" />
                            <asp:Parameter Name="origRecipGroup" Type="Object" />
                        </UpdateParameters>
                    </asp:ObjectDataSource>
                    <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>
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
                                    AllowSorting="false" AutoGenerateColumns="False" DataKeyNames="Owner" Visible="false"
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
    </asp:UpdatePanel>
</asp:Content>
