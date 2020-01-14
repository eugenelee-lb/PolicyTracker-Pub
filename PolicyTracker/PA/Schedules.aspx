<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Schedules.aspx.vb" Inherits="PolicyTracker.Schedules" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/RecipGroupMembers.ascx" TagPrefix="asp" TagName="RecipGroupMembers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        function pageLoad() {
            // keep the current tab active bootstrap after a page reload/postback
            $('a[data-toggle="tab"]').on('shown.bs.tab', function () {
                //save the latest tab; use cookies if you like 'em better:
                sessionStorage.setItem('lastTab_Sch', $(this).attr('href'));
            });
            //go to the latest tab, if it exists:
            var lastTab = sessionStorage.getItem('lastTab_Sch');
            if (lastTab && $('a[href="' + lastTab + '"]').is(":visible")) {
                $('a[href="' + lastTab + '"]').tab('show');
            }
            else {
                // Set the first tab if cookie do not exist
                $('a[data-toggle="tab"]:first').tab('show');
            }

            $("input[name$='txtFixedReleaseDate']").inputmask("m/d/y");
            $("input[name$='txtFixedReleaseDate']").datepicker({
                changeMonth: true,
                changeYear: true,
                minDate: 0
            });

            $(".select_chosen_150").chosen({
                disable_search_threshold: 5,
                no_results_text: "Nothing found!",
                width: "150px",
                search_contains: true
            });
            $(".select_chosen_300").chosen({
                disable_search_threshold: 5,
                no_results_text: "Nothing found!",
                width: "300px",
                search_contains: true
            });

            $('select[name$="ddlFrequency"]').change(function () {
                freq = $(this).val();
                if (freq == "ONE_TIME") {
                    $('select[name$="ddlRepeatMonth"]').prop('disabled', true);
                    $('input[name$="txtRepeatDay"]').prop('disabled', true);
                    $('input[name$="txtFixedReleaseDate"]').prop('disabled', false);
                }
                else if (freq == "ANNUAL") {
                    $('select[name$="ddlRepeatMonth"]').prop('disabled', false);
                    $('input[name$="txtRepeatDay"]').prop('disabled', false);
                    $('input[name$="txtFixedReleaseDate"]').prop('disabled', true);
                }
                else if (freq == "SEMI_ANNUAL") {
                    $('select[name$="ddlRepeatMonth"]').prop('disabled', false);
                    $('input[name$="txtRepeatDay"]').prop('disabled', false);
                    $('input[name$="txtFixedReleaseDate"]').prop('disabled', true);
                }
                else if (freq == "QUARTERLY") {
                    $('select[name$="ddlRepeatMonth"]').prop('disabled', false);
                    $('input[name$="txtRepeatDay"]').prop('disabled', false);
                    $('input[name$="txtFixedReleaseDate"]').prop('disabled', true);
                }
                else {
                    $('select[name$="ddlRepeatMonth"]').prop('disabled', true);
                    $('input[name$="txtRepeatDay"]').prop('disabled', true);
                    $('input[name$="txtFixedReleaseDate"]').prop('disabled', true);
                }
            }).change();

            //$('[data-toggle="tooltip"]').tooltip('hide');
            $(".tooltip").hide();
            $(".showTooltip").tooltip('show');
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

    <h2><span id="spnHeader" runat="server">Schedules</span></h2>

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
                    <td>
                        <asp:TextBox ID="txtFilterName" runat="server" Columns="20"></asp:TextBox>
                    </td>
                    <td>Description:</td>
                    <td>
                        <asp:TextBox ID="txtFilterDesc" runat="server" Columns="20"></asp:TextBox>
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
            </div>

            <asp:GridView ID="gvSchedules" runat="server" AutoGenerateColumns="False" 
                ItemType="PolicyTracker.Lib.Schedule" SelectMethod="gvSchedules_GetData" DataKeyNames="ScheduleID"
                AllowPaging="True" AllowSorting="True" EmptyDataText="No record was found.">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" Visible="false" />
                    <asp:TemplateField ShowHeader="False" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnSelect" runat="server" CausesValidation="False" ToolTip="Select"
                                CommandName="Select" AlternateText="Select" EnableViewState="false">Select</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="ScheduleID" HeaderText="ID" Visible="false" />
                    <asp:BoundField DataField="ScheduleName" HeaderText="Schedule Name" SortExpression="ScheduleName" />
                    <asp:BoundField DataField="Frequency" HeaderText="Frequency" SortExpression="Frequency" />
                    <asp:BoundField DataField="RepeatMonth" HeaderText="Month" SortExpression="RepeatMonth" />
                    <asp:BoundField DataField="RepeatDay" HeaderText="Day" SortExpression="RepeatDay" />
                    <asp:BoundField DataField="FixedReleaseDate" HeaderText="Fixed Release Date" SortExpression="FixedReleaseDate" DataFormatString="{0:M/d/yyyy}" />
                    <%--<asp:TemplateField HeaderText="Releases" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:HyperLink ID="hlkReleases" runat="server" Target="_self" ToolTip="Releases"
                                NavigateUrl='<%#: "~/OA/Releases/" & Item.ScheduleId.ToString%>'
                                Visible='<%#: IIf(Item.Releases.Count > 0, True, False)%>'>
                                <%#:Item.Releases.Count%>
                            </asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>--%>
                    <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" >
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
                <li class="" id="tliSchedule" runat="server"><a href="#tpSchedule" id="tlkSchedule" runat="server" data-toggle="tab" aria-expanded="true">Schedule Details</a></li>
                <li class="" id="tliReleases" runat="server"><a href="#tpReleases" id="tlkReleases" runat="server" data-toggle="tab" aria-expanded="false">Releases <span runat="server" id="spnRelCnt" class="badge">0</span></a></li>
                <li class="" id="tliPolicies" runat="server" 
                    data-toggle="tooltip" data-placement="bottom" data-container="body" title="Assign policy">
                    <a href="#tpPolicies" id="tlkPolicies" runat="server" data-toggle="tab" aria-expanded="false">Policies 
                        <span runat="server" id="spnPolCnt" class="badge">0</span>
                    </a>
                </li>
                <li class="" id="tliRecipGroups" runat="server"
                    data-toggle="tooltip" data-placement="bottom" data-container="body" title="Assign recipient group">
                    <a href="#tpRecipGroups" id="tlkRecipGroups" runat="server" data-toggle="tab" aria-expanded="false">Recipient Groups 
                        <span runat="server" id="spnRecGrpCnt" class="badge">0</span>
                    </a>
                </li>
                <li class="" id="tliOwners" runat="server"><a href="#tpOwners" id="tlkOwners" runat="server" data-toggle="tab" aria-expanded="false">Owners <span runat="server" id="spnOwnCnt" class="badge">0</span></a></li>
            </ul>

            <div class="tab-content" id="tcSchedule">
                <div class="tab-pane fade in" id="tpSchedule">

                    <br />
                    <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
                    <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
                    <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="List" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />

                    <asp:DetailsView ID="dvSchedule" runat="server" AutoGenerateRows="False" Caption=""
                        DataKeyNames="ScheduleID" DataSourceID="odsScheduleDetails" DefaultMode="Insert">
                        <Fields>
                            <asp:TemplateField HeaderText="Schedule Name" SortExpression="ScheduleName">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtScheduleName" runat="server" Columns="70" MaxLength="100" Text='<%# Bind("ScheduleName") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvScheduleName" runat="server" ControlToValidate="txtScheduleName"
                                        ValidationGroup="vgDetails" ErrorMessage="Schedule Name is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtScheduleName" runat="server" Columns="70" MaxLength="100" Text='<%# Bind("ScheduleName") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvScheduleName" runat="server" ControlToValidate="txtScheduleName"
                                        ValidationGroup="vgDetails" ErrorMessage="Schedule Name is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblScheduleName" runat="server" Text='<%# Bind("ScheduleName") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Description" SortExpression="ScheduleDesc">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtScheduleDesc" runat="server" Columns="100" onkeyup="return ismaxlength(this, 10000);"
                                        TextMode="MultiLine" Rows="3" Text='<%# Bind("ScheduleDesc") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ControlToValidate="txtScheduleDesc"
                                        ErrorMessage="Description is required" Text="*" Display="dynamic" ValidationGroup="vgDetails" Visible="false"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtScheduleDesc" runat="server" Columns="100" onkeyup="return ismaxlength(this, 10000);"
                                        TextMode="MultiLine" Rows="3" Text='<%# Bind("ScheduleDesc") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ControlToValidate="txtScheduleDesc"
                                        ErrorMessage="Description is required" Display="dynamic" ValidationGroup="vgDetails" Visible="false"></asp:RequiredFieldValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("ScheduleDesc") %>' Visible="false"></asp:Label>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("ScheduleDesc")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Frequency" SortExpression="Frequency">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlFrequency" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("Frequency") %>'
                                        DataSourceID="odsFrequency" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="true">
                                        <asp:ListItem Value="">-- Select --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsFrequency" runat="server" 
                                        SelectMethod="GetCommonCodesByCatg" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="FREQ" Name="catg" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <asp:RequiredFieldValidator ID="rfvFrequency" runat="server" ControlToValidate="ddlFrequency"
                                        ValidationGroup="vgDetails" ErrorMessage="Frequency is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="ddlFrequency" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("Frequency") %>'
                                        DataSourceID="odsFrequency" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="true">
                                        <asp:ListItem Value="">-- Select --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsFrequency" runat="server" 
                                        SelectMethod="GetCommonCodesByCatg" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="FREQ" Name="catg" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                    <asp:RequiredFieldValidator ID="rfvFrequency" runat="server" ControlToValidate="ddlFrequency"
                                        ValidationGroup="vgDetails" ErrorMessage="Frequency is required" Text="*" Display="dynamic"></asp:RequiredFieldValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlFrequency" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("Frequency") %>'
                                        DataSourceID="odsFrequency" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="false">
                                        <asp:ListItem Value="">-- N/A --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsFrequency" runat="server" 
                                        SelectMethod="GetCommonCodesByCatg" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="FREQ" Name="catg" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Repeat Month" SortExpression="RepeatMonth">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlRepeatMonth" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("RepeatMonth") %>'
                                        DataSourceID="odsRepeatMonth" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="true">
                                        <asp:ListItem Value="">-- Select --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsRepeatMonth" runat="server" 
                                        SelectMethod="GetCommonCodesByCatg" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="MONTHS" Name="catg" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:DropDownList ID="ddlRepeatMonth" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("RepeatMonth") %>'
                                        DataSourceID="odsRepeatMonth" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="true">
                                        <asp:ListItem Value="">-- Select --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsRepeatMonth" runat="server" 
                                        SelectMethod="GetCommonCodesByCatg" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="MONTHS" Name="catg" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddlRepeatMonth" runat="server" AppendDataBoundItems="True" SelectedValue='<%# Bind("RepeatMonth") %>'
                                        DataSourceID="odsRepeatMonth" DataTextField="CMCodeDesc" DataValueField="CMCode" Enabled="false">
                                        <asp:ListItem Value="">-- N/A --</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="odsRepeatMonth" runat="server" 
                                        SelectMethod="GetCommonCodesByCatg" TypeName="PolicyTracker.Lib.SettingsBL">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="" Name="sortExpression" Type="String" />
                                            <asp:Parameter DefaultValue="MONTHS" Name="catg" Type="String" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Repeat Day" SortExpression="RepeatDay">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtRepeatDay" runat="server" Columns="3" MaxLength="2" Text='<%# Bind("RepeatDay") %>'></asp:TextBox>
                                    <asp:RangeValidator ID="rvRepeatDay" runat="server" ControlToValidate="txtRepeatDay" ErrorMessage="Repeat Day should be a number between 1 to 31" Text="*"
                                        Type="Integer" MaximumValue="31" MinimumValue="1" ValidationGroup="vgDetails"></asp:RangeValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtRepeatDay" runat="server" Columns="3" MaxLength="2" Text='<%# Bind("RepeatDay") %>'></asp:TextBox>
                                    <asp:RangeValidator ID="rvRepeatDay" runat="server" ControlToValidate="txtRepeatDay" ErrorMessage="Repeat Day should be a number between 1 to 31" Text="*"
                                        Type="Integer" MaximumValue="31" MinimumValue="1" ValidationGroup="vgDetails"></asp:RangeValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblRepeatDay" runat="server" Text='<%# Bind("RepeatDay") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Fixed Release Date" SortExpression="FixedReleaseDate">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtFixedReleaseDate" runat="server" Text='<%# Bind("FixedReleaseDate", "{0:MM/dd/yyyy}") %>' Columns="12" MaxLength="10" />
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtFixedReleaseDate" runat="server" Text='<%# Bind("FixedReleaseDate", "{0:MM/dd/yyyy}") %>' Columns="12" MaxLength="10" />
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblFixedReleaseDate" runat="server" Text='<%# Eval("FixedReleaseDate", "{0:M/d/yyyy}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Days to Deadline" SortExpression="DaysToDeadline">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtDaysToDeadline" runat="server" Columns="4" MaxLength="3" Text='<%# Bind("DaysToDeadline") %>'></asp:TextBox>
                                    <asp:RangeValidator ID="rvDaysToDeadline" runat="server" ControlToValidate="txtDaysToDeadline" 
                                        ErrorMessage="Days to Deadline should be a number between 1 to 365)" Text="*"
                                        Type="Integer" MaximumValue="365" MinimumValue="1" ValidationGroup="vgDetails"></asp:RangeValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtDaysToDeadline" runat="server" Columns="4" MaxLength="3" Text='<%# Bind("DaysToDeadline") %>'></asp:TextBox>
                                    <asp:RangeValidator ID="rvDaysToDeadline" runat="server" ControlToValidate="txtDaysToDeadline" 
                                        ErrorMessage="Days to Deadline should be a number between 1 to 365)" Text="*"
                                        Type="Integer" MaximumValue="365" MinimumValue="1" ValidationGroup="vgDetails"></asp:RangeValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDaysToDeadline" runat="server" Text='<%# Bind("DaysToDeadline") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Days to Reminder" SortExpression="DaysToReminder">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtDaysToReminder" runat="server" Columns="20" MaxLength="30" Text='<%# Bind("DaysToReminder") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="rexvDaysToReminder" runat="server" 
                                        ControlToValidate="txtDaysToReminder" ErrorMessage="Days to Reminder is in invalid format" Text="*"
                                        ValidationExpression="\d{1,3}(,\d{1,3})*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RegularExpressionValidator>
                                        <em>You can input multiple # of days separated by comma, e.g., (15,30,45).</em>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtDaysToReminder" runat="server" Columns="20" MaxLength="30" Text='<%# Bind("DaysToReminder") %>'></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="rexvDaysToReminder" runat="server" 
                                        ControlToValidate="txtDaysToReminder" ErrorMessage="Days to Reminder is in invalid format" Text="*"
                                        ValidationExpression="\d{1,3}(,\d{1,3})*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RegularExpressionValidator>
                                        <em>You can input multiple # of days separated by comma, e.g., (15,30,45).</em>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDaysToReminder" runat="server" Text='<%# Bind("DaysToReminder") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="(Header) From" SortExpression="From">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtFrom" runat="server" Columns="100" MaxLength="500" Text='<%# Bind("From") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvFrom" runat="server" ControlToValidate="txtFrom"
                                        ValidationGroup="vgDetails" ErrorMessage="(Header) From is required" Text="*" Display="dynamic" Visible="true"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtFrom" runat="server" Columns="100" MaxLength="500" Text='<%# Bind("From") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtFrom"
                                        ValidationGroup="vgDetails" ErrorMessage="(Header) From is required" Text="*" Display="dynamic" Visible="true"></asp:RequiredFieldValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblFrom" runat="server" Text='<%# Bind("From") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="(Header) To" SortExpression="To">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtTo" runat="server" Columns="100" MaxLength="500" Text='<%# Bind("To") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTo" runat="server" ControlToValidate="txtTo"
                                        ValidationGroup="vgDetails" ErrorMessage="(Header) To is required" Text="*" Display="dynamic" Visible="true"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtTo" runat="server" Columns="100" MaxLength="500" Text='<%# Bind("To") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTo" runat="server" ControlToValidate="txtTo"
                                        ValidationGroup="vgDetails" ErrorMessage="(Header) To is required" Text="*" Display="dynamic" Visible="true"></asp:RequiredFieldValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblTo" runat="server" Text='<%# Bind("To") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="(Header) Subject" SortExpression="Subject">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSubject" runat="server" Columns="100" MaxLength="500" Text='<%# Bind("Subject") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject"
                                        ValidationGroup="vgDetails" ErrorMessage="(Header) Subject is required" Text="*" Display="dynamic" Visible="true"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtSubject" runat="server" Columns="100" MaxLength="500" Text='<%# Bind("Subject") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvSubject" runat="server" ControlToValidate="txtSubject"
                                        ValidationGroup="vgDetails" ErrorMessage="(Header) Subject is required" Text="*" Display="dynamic" Visible="true"></asp:RequiredFieldValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblSubject" runat="server" Text='<%# Bind("Subject") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Disclaimer" SortExpression="Disclaimer">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtDisclaimer" runat="server" Columns="100" onkeyup="return ismaxlength(this, 1000);"
                                        TextMode="MultiLine" Rows="3" Text='<%# Bind("Disclaimer") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDisclaimer" runat="server" ControlToValidate="txtDisclaimer"
                                        ValidationGroup="vgDetails" ErrorMessage="(Required)" Display="dynamic" Visible="false"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <InsertItemTemplate>
                                    <asp:TextBox ID="txtDisclaimer" runat="server" Columns="100" onkeyup="return ismaxlength(this, 1000);"
                                        TextMode="MultiLine" Rows="3" Text='<%# Bind("Disclaimer") %>'></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDisclaimer" runat="server" ControlToValidate="txtDisclaimer"
                                        ValidationGroup="vgDetails" ErrorMessage="(Required)" Display="dynamic" Visible="false"></asp:RequiredFieldValidator>
                                </InsertItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDisclaimer" runat="server" Text='<%# Bind("Disclaimer") %>' Visible="false"></asp:Label>
                                    <asp:Label ID="lblDisclaimerEval" runat="server" Text='<%# EvalHtmlEncode("Disclaimer") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" InsertVisible="false">
                            </asp:CheckBoxField>
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
                                        Text="Add New Schedule" CssClass="btn btn-primary btn-xs"></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Fields>
                    </asp:DetailsView>
                    <asp:ObjectDataSource ID="odsScheduleDetails" runat="server" ConflictDetection="CompareAllValues"
                        DeleteMethod="DeleteSchedule" InsertMethod="InsertSchedule" OldValuesParameterFormatString="orig{0}"
                        SelectMethod="GetScheduleByID" TypeName="PolicyTracker.Lib.PTRepository" DataObjectTypeName="PolicyTracker.Lib.Schedule"
                        UpdateMethod="UpdateSchedule">
                        <SelectParameters>
                            <asp:ControlParameter ControlID="gvSchedules" DefaultValue="" Name="scheduleID" PropertyName="SelectedValue" Type="Int32" />
                        </SelectParameters>
                        <UpdateParameters>
                            <asp:Parameter Name="schedule" Type="Object" />
                            <asp:Parameter Name="origSchedule" Type="Object" />
                        </UpdateParameters>
                    </asp:ObjectDataSource>

                    <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>

                </div>

                <div class="tab-pane fade" id="tpPolicies">
                    <br />
                    <table>
                        <tr>
                            <td colspan="2">
                                <asp:label id="lblInfoPolicies" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:label>
                                <asp:label id="lblErrorPolicies" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
                                <asp:ValidationSummary ID="vsPolicies" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgPolicies" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 0 10px 0 10px; vertical-align:top;">
                                <asp:GridView ID="gvPolicies" runat="server" AutoGenerateColumns="False" DataKeyNames="PolicyId"
                                    AllowPaging="false" AllowSorting="false" EmptyDataText="No record was found."
                                    ItemType="PolicyTracker.Lib.Policy" Caption="Assigned Policies">
                                    <Columns>
                                        <asp:CommandField ShowSelectButton="True" />
                                        <asp:TemplateField ShowHeader="False">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ibtnDeletePolicy" runat="server" CausesValidation="False" CommandName="DeletePolicy" CommandArgument='<%# Eval("PolicyID") %>'
                                                    AlternateText="Delete" ToolTip="Delete" 
                                                    OnClientClick="return confirm('Are you sure you want to delete this record?');">
                                                    <span class="glyphicon glyphicon-trash"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="PolicyId" HeaderText="ID" Visible="false" />
                                        <asp:BoundField DataField="PolicyName" HeaderText="Policy Name" Visible="false" />
                                        <asp:TemplateField HeaderText="Policy Name">
                                            <ItemTemplate>
                                                <%#: Item.PolicyName %>
                                                <asp:HyperLink ID="hlkOpenPolicy" runat="server" Target="_blank" ToolTip="Open"
                                                    NavigateUrl='<%#: "~/PA/Policy/" & Item.PolicyId.ToString%>'>
                                                    <span class="glyphicon glyphicon-new-window"></span>
                                                </asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" >
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:CheckBoxField>
                                        <asp:TemplateField HeaderText="# of Files" ItemStyle-HorizontalAlign="Right">
                                            <ItemTemplate><%#: Item.UploadFiles.Count.ToString%></ItemTemplate>
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
                            </td>
                            <td style="padding: 0 10px 0 10px; vertical-align:top;">
                                <asp:Panel ID="panAddPolicy" runat="server" GroupingText="Assign Policy">
                                    <table cellpadding="3px">
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlPolicyToAdd" runat="server" 
                                                    DataValueField="PolicyID" DataTextField="PolicyName" 
                                                    CssClass="select_chosen_300">
                                                    <asp:ListItem Text="-- Select --" Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvPolicyToAdd" runat="server" ControlToValidate="ddlPolicyToAdd"
                                                    ErrorMessage="Please select a policy" Text="*" Display="Dynamic" ValidationGroup="vgPolicies"></asp:RequiredFieldValidator>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnAddPolicy" runat="server" Text="Assign" ValidationGroup="vgPolicies" CssClass="btn btn-primary btn-xs" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel> 
                            </td>
                        </tr>
                    </table>
                </div>

                <div class="tab-pane fade" id="tpRecipGroups">
                    <br />
                    <table>
                        <tr>
                            <td colspan="2">
                                <asp:label id="lblInfoRecipGroups" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:label>
                                <asp:label id="lblErrorRecipGroups" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
                                <asp:ValidationSummary ID="vsRecipGroups" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgRecipGroups" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding: 0 10px 0 10px; vertical-align:top;">
                                <asp:GridView ID="gvRecipGroups" runat="server" AutoGenerateColumns="False" DataKeyNames="RecipGroupID"
                                    AllowPaging="false" AllowSorting="false" EmptyDataText="No record was found."
                                    Caption="Assigned Recipient Groups">
                                    <Columns>
                                        <asp:TemplateField ShowHeader="False">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="ibtnDeleteRecipGroup" runat="server" CausesValidation="False" 
                                                    CommandName="DeleteRecipGroup" CommandArgument='<%# Eval("RecipGroupID") %>'
                                                    AlternateText="Delete" ToolTip="Delete" 
                                                    OnClientClick="return confirm('Are you sure you want to delete this record?');">
                                                    <span class="glyphicon glyphicon-trash"></span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="RecipGroupID" HeaderText="ID" Visible="false" />
                                        <asp:BoundField DataField="GroupName" HeaderText="Recipient Group Name" Visible="false" />
                                        <asp:TemplateField HeaderText="Recipient Group Name" SortExpression="GroupName">
                                            <ItemTemplate>
                                                <asp:Label ID="lblGroupName" runat="server" Text='<%# Bind("GroupName")%>'></asp:Label>
                                                <asp:LinkButton ID="lbtnReviewGroupMembers" runat="server" CausesValidation="false"
                                                    AlternateText="Group Members" ToolTip="" OnClientClick="return reviewGroupMembers();" 
                                                    CommandName="RecipGroupMembers" CommandArgument='<%# Eval("RecipGroupId") %>'>
                                                    <span style="letter-spacing:-3px;">
                                                        <span class="glyphicon glyphicon-user"></span>
                                                        <span class="glyphicon glyphicon-user"></span>
                                                    </span>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" >
                                            <ItemStyle HorizontalAlign="Center" />
                                        </asp:CheckBoxField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                            <td style="padding: 0 10px 0 10px; vertical-align:top;">
                                <asp:Panel ID="panAddRecipGroup" runat="server" GroupingText="Assign Recipient Group">
                                    <table cellpadding="3px">
                                        <tr>
                                            <td>
                                                <asp:DropDownList ID="ddlRecipGroupToAdd" runat="server" 
                                                    DataValueField="RecipGroupID" DataTextField="GroupName" 
                                                    CssClass="select_chosen_300">
                                                    <asp:ListItem Text="-- Select --" Value=""></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvRecipGroupToAdd" runat="server" ControlToValidate="ddlRecipGroupToAdd"
                                                    ErrorMessage="Please select a RecipGroup" Text="*" Display="Dynamic" ValidationGroup="vgRecipGroups"></asp:RequiredFieldValidator>
                                            </td>
                                            <td>
                                                <asp:Button ID="btnAddRecipGroup" runat="server" Text="Assign" ValidationGroup="vgRecipGroups" CssClass="btn btn-primary btn-xs" />
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel> 
                            </td>
                        </tr>
                    </table>
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

                <div class="tab-pane fade" id="tpReleases">
                    <br />
                    <asp:Button ID="btnCreateNextRelease" runat="server" Text="Create Next Release" CssClass="btn btn-success btn-xs" Visible="false" />
                    <asp:label id="lblInfoReleases" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:label>
                    <asp:label id="lblErrorReleases" Runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:label>
                    <asp:ValidationSummary ID="vsReleases" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgReleases" />
                    <br />
                    <asp:GridView ID="gvReleases" runat="server" AutoGenerateColumns="False" 
                        ItemType="PolicyTracker.Lib.Release" SelectMethod="" DataKeyNames="ReleaseId"
                        AllowPaging="False" AllowSorting="False" EmptyDataText="No release was found.">
                        <Columns>
                            <asp:TemplateField ShowHeader="False" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlkOpenRelease" runat="server" Target="_self" ToolTip="Open"
                                        NavigateUrl='<%#: "~/OA/Release/" & Item.ReleaseId.ToString%>'>
                                        <span class="glyphicon glyphicon-folder-open"></span>
                                    </asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="ReleaseId" HeaderText="ID" Visible="false" />
                            <asp:BoundField DataField="ReleaseDate" HeaderText="Release Date" SortExpression="ReleaseDate" DataFormatString="{0:M/d/yyyy}" />
                            <asp:BoundField DataField="DeadlineDate" HeaderText="Deadline Date" SortExpression="DeadlineDate" DataFormatString="{0:M/d/yyyy}" />
                            <asp:BoundField DataField="To" HeaderText="To" SortExpression="To" Visible="false" />
                            <asp:BoundField DataField="From" HeaderText="From" SortExpression="From" Visible="false" />
                            <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" />
                            <asp:TemplateField HeaderText="Policies" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate><%#: Item.ReleasePolicies.Count.ToString("#,##0")%></ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Recipients" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate><%#: Item.ReleaseRecipients.Count.ToString("#,##0")%></ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" Visible="true" />
                            <asp:BoundField DataField="CreateUser" HeaderText="Create User" SortExpression="CreateUser" Visible="true" />
                            <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" Visible="false" />
                            <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="LastUpdateUser" Visible="false" />
                        </Columns>
                        <PagerTemplate>
                            <asp:GridViewPager ID="GridViewPager1" runat="server" />
                        </PagerTemplate>
                    </asp:GridView>
                    <br />

                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
