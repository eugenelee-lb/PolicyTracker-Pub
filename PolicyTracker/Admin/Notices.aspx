<%@ Page Title="Notices" Language="vb" AutoEventWireup="false" MasterPageFile="~/Site.Master" CodeBehind="Notices.aspx.vb" Inherits="PolicyTracker.Notices" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<%@ Register Src="~/UserControls/UpdateProgressPanel.ascx" TagName="ProgressPanel" TagPrefix="asp" %>
<%@ Register Src="~/UserControls/GridViewPager.ascx" TagName="GridViewPager" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script>
        function pageLoad() {
            $("input[name$='txtStartDate']").inputmask("m/d/y");
            $("input[name$='txtStartTime']").inputmask("hh:mm t");
            $("input[name$='txtEndDate']").inputmask("m/d/y");
            $("input[name$='txtEndTime']").inputmask("hh:mm t");

            $("input[name$='txtStartDate']").datepicker({
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("input[name$='txtEndDate']").datepicker("option", "minDate", selectedDate);
                }
            });
            $("input[name$='txtEndDate']").datepicker({
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("input[name$='txtStartDate']").datepicker("option", "maxDate", selectedDate);
                }
            });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Notices</h1>

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <asp:ProgressPanel ID="ProgressPanel1" runat="server" />
        </ProgressTemplate>
    </asp:UpdateProgress>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="always">
        <ContentTemplate>

            <asp:GridView ID="gvNotices" runat="server" AutoGenerateColumns="False" DataKeyNames="NoticeId"
                DataSourceID="odsNotices" AllowPaging="True" AllowSorting="True" EmptyDataText="No record has been found.">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="NoticeId" HeaderText="ID" ReadOnly="True" SortExpression="NoticeId" />
                    <asp:BoundField DataField="NoticeTitle" HeaderText="Title" SortExpression="NoticeTitle" />
                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" SortExpression="StartDate" />
                    <asp:BoundField DataField="EndDate" HeaderText="End Date" SortExpression="EndDate" />
                    <asp:BoundField DataField="DispOrder" HeaderText="Order" SortExpression="DispOrder" ItemStyle-HorizontalAlign="Right" />
                    <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" ItemStyle-HorizontalAlign="Center" />
                    <asp:BoundField DataField="CreateDT" HeaderText="Create Date" SortExpression="CreateDT" />
                    <asp:BoundField DataField="CreateUser" HeaderText="Create User" SortExpression="CreateUser" />
                    <asp:BoundField DataField="LastUpdateDT" HeaderText="Last Update Date" SortExpression="LastUpdateDT" />
                    <asp:BoundField DataField="LastUpdateUser" HeaderText="Last Update User" SortExpression="LastUpdateUser" />
                </Columns>
                <PagerTemplate>
                    <asp:GridViewPager ID="GridViewPager1" runat="server" />
                </PagerTemplate>
            </asp:GridView>
            <asp:ObjectDataSource ID="odsNotices" runat="server" SelectMethod="GetNotices"
                TypeName="PolicyTracker.Lib.SettingsBL" SortParameterName="sortExpression">
            </asp:ObjectDataSource>
            <br />

            <asp:Label ID="lblInfo" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-success"></asp:Label>
            <asp:Label ID="lblError" runat="Server" EnableViewState="False" Visible="false" CssClass="label label-danger"></asp:Label>
            <asp:ValidationSummary ID="vsDetails" runat="server" ShowSummary="true" DisplayMode="BulletList" CssClass="alert alert-dismissable alert-danger" ValidationGroup="vgDetails" />

            <asp:DetailsView ID="dvNotice" runat="server" AutoGenerateRows="False" Caption="Add New Notice"
                DataKeyNames="NoticeId" DataSourceID="odsNoticeDetails" DefaultMode="Insert">
                <Fields>
                    <asp:BoundField DataField="NoticeId" HeaderText="ID" SortExpression="NoticeId" Visible="false" />
                    <asp:TemplateField HeaderText="Notice Title" SortExpression="NoticeTitle">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtNoticeTitle" runat="server" Columns="50" MaxLength="50" Text='<%# Bind("NoticeTitle") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvNoticeTitle" runat="server" ControlToValidate="txtNoticeTitle"
                                ErrorMessage="Notice Title is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>&nbsp;
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtNoticeTitle" runat="server" Columns="50" MaxLength="50" Text='<%# Bind("NoticeTitle") %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvNoticeTitle" runat="server" ControlToValidate="txtNoticeTitle"
                                ErrorMessage="Notice Title is required" Text="*" Display="Dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>&nbsp;
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblNoticeTitle" runat="server" Text='<%# Bind("NoticeTitle") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Notice Text" SortExpression="NoticeText">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtNoticeText" runat="server" Columns="100" 
                                TextMode="MultiLine" Rows="6" Text='<%# Bind("NoticeText") %>' ValidateRequestMode="Disabled"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvText" runat="server" ControlToValidate="txtNoticeText"
                                ErrorMessage="Notice Text is required" Text="*" Display="dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtNoticeText" runat="server" Columns="100" 
                                TextMode="MultiLine" Rows="6" Text='<%# Bind("NoticeText") %>' ValidateRequestMode="Disabled"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvText" runat="server" ControlToValidate="txtNoticeText"
                                ErrorMessage="Notice Text is required" Text="*" Display="dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="Label6" runat="server" Text='<%# Bind("NoticeText") %>' Visible="true"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Start Date" SortExpression="StartDate">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtStartDate" runat="server" Text='<%# Bind("StartDate", "{0:MM/dd/yyyy}") %>' Columns="12" MaxLength="10"
                            /><%--<asp:MaskedEditExtender 
                                ID="mkeStartDate" runat="server" OnInvalidCssClass="validationError"
                                TargetControlID="txtStartDate" Mask="99/99/9999" MessageValidatorTip="true"
                                CultureName="en-US" MaskType="Date" ErrorTooltipEnabled="false" 
                            /><asp:MaskedEditValidator 
                                ID="mkevStartDate" runat="server"
                                ControlExtender="mkeStartDate" ControlToValidate="txtStartDate"
                                Display="Dynamic" TooltipMessage="" IsValidEmpty="false"
                                EmptyValueMessage="Start Date is required" EmptyValueBlurredText="*"
                                InvalidValueMessage="Start Date is invalid" InvalidValueBlurredMessage="*"
                                ValidationGroup="vgDetails"
                            /><asp:CalendarExtender 
                                ID="caleStartDate" runat="server" Format="MM/dd/yyyy" TargetControlID="txtStartDate" PopupButtonID="ibtnStartDate" CssClass="MyCalendar"/>
                            <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/images/Calendar_scheduleHS.png" CausesValidation="False" ImageAlign="AbsMiddle" TabIndex="1000" />--%>
                            <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="txtStartDate"
                                ErrorMessage="Start Date is required" Text="*" Display="dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="rvStartDate" runat="server" ControlToValidate="txtStartDate" 
                                ErrorMessage="Start Date should be between 1/1/2000 and 12/31/2050" Text="*"
                                Type="Date" MinimumValue="1/1/2000" MaximumValue="12/31/2050" ValidationGroup="vgDetails"></asp:RangeValidator>
                            <asp:TextBox ID="txtStartTime" runat="server" Columns="8" Text='<%# Eval("StartDate", "{0:hh:mm tt}") %>' 
                            /><%--<asp:MaskedEditExtender ID="mkeStartTime" runat="server"
                                TargetControlID="txtStartTime" Mask="99:99" 
                                MessageValidatorTip="true"
                                OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="validationError"
                                MaskType="Time" AcceptAMPM="true"
                                ErrorTooltipEnabled="false" 
                            /><asp:MaskedEditValidator ID="mkevStartTime" runat="server"
                                ControlExtender="mkeStartTime"
                                ControlToValidate="txtStartTime"
                                IsValidEmpty="false" Text="*"
                                EmptyValueMessage="Start Time is required" EmptyValueBlurredText="*"
                                InvalidValueMessage="Start Time is invalid" InvalidValueBlurredMessage="*"
                                Display="Dynamic" ValidationGroup="vgDetails" />--%>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtStartDate" runat="server" Text='<%# Bind("StartDate") %>' Columns="12" MaxLength="10"
                            /><%--<asp:MaskedEditExtender 
                                ID="mkeStartDate" runat="server" OnInvalidCssClass="validationError"
                                TargetControlID="txtStartDate" Mask="99/99/9999" MessageValidatorTip="true"
                                CultureName="en-US" MaskType="Date" ErrorTooltipEnabled="false" 
                            /><asp:MaskedEditValidator 
                                ID="mkevStartDate" runat="server"
                                ControlExtender="mkeStartDate" ControlToValidate="txtStartDate"
                                Display="Dynamic" TooltipMessage="" IsValidEmpty="false"
                                EmptyValueMessage="Start Date is required" EmptyValueBlurredText="*"
                                InvalidValueMessage="Start Date is invalid" InvalidValueBlurredMessage="*"
                                ValidationGroup="vgDetails"
                            /><asp:CalendarExtender 
                                ID="caleStartDate" runat="server" Format="MM/dd/yyyy" TargetControlID="txtStartDate" PopupButtonID="ibtnStartDate" CssClass="MyCalendar"/>
                            <asp:ImageButton ID="ibtnStartDate" runat="server" ImageUrl="~/images/Calendar_scheduleHS.png" CausesValidation="False" ImageAlign="AbsMiddle" TabIndex="1000" />--%>
                            <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="txtStartDate"
                                ErrorMessage="Start Date is required" Text="*" Display="dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="rvStartDate" runat="server" ControlToValidate="txtStartDate" 
                                ErrorMessage="Start Date should be between 1/1/2000 and 12/31/2050" Text="*"
                                Type="Date" MinimumValue="1/1/2000" MaximumValue="12/31/2050" ValidationGroup="vgDetails"></asp:RangeValidator>
                            <asp:TextBox ID="txtStartTime" runat="server" Columns="8"
                            /><%--<asp:MaskedEditExtender ID="mkeStartTime" runat="server"
                                TargetControlID="txtStartTime" Mask="99:99" 
                                MessageValidatorTip="true"
                                OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="validationError"
                                MaskType="Time" AcceptAMPM="true"
                                ErrorTooltipEnabled="false" 
                            /><asp:MaskedEditValidator ID="mkevStartTime" runat="server"
                                ControlExtender="mkeStartTime"
                                ControlToValidate="txtStartTime"
                                IsValidEmpty="false" Text="*"
                                EmptyValueMessage="Start Time is required" EmptyValueBlurredText="*"
                                InvalidValueMessage="Start Time is invalid" InvalidValueBlurredMessage="*"
                                Display="Dynamic" ValidationGroup="vgDetails" />--%>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblStartDate" runat="server" Text='<%# Bind("StartDate", "{0:M/d/yyyy h:mm tt}") %>' Visible="true"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="End Date" SortExpression="EndDate">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtEndDate" runat="server" Text='<%# Bind("EndDate", "{0:MM/dd/yyyy}")%>' Columns="12" MaxLength="10"
                            /><%--<asp:MaskedEditExtender 
                                ID="mkeEndDate" runat="server" OnInvalidCssClass="validationError"
                                TargetControlID="txtEndDate" Mask="99/99/9999" MessageValidatorTip="true"
                                CultureName="en-US" MaskType="Date" ErrorTooltipEnabled="false" 
                            /><asp:MaskedEditValidator 
                                ID="mkevEndDate" runat="server"
                                ControlExtender="mkeEndDate" ControlToValidate="txtEndDate"
                                Display="Dynamic" TooltipMessage="" IsValidEmpty="false"
                                EmptyValueMessage="End Date is required" EmptyValueBlurredText="*"
                                InvalidValueMessage="End Date is invalid" InvalidValueBlurredMessage="*"
                                ValidationGroup="vgDetails"
                            /><asp:CalendarExtender 
                                ID="caleEndDate" runat="server" Format="MM/dd/yyyy" TargetControlID="txtEndDate" PopupButtonID="ibtnEndDate" CssClass="MyCalendar"/>
                            <asp:ImageButton ID="ibtnEndDate" runat="server" ImageUrl="~/images/Calendar_scheduleHS.png" CausesValidation="False" ImageAlign="AbsMiddle" TabIndex="1000" />--%>
                            <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="txtEndDate"
                                ErrorMessage="End Date is required" Text="*" Display="dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="rvEndDate" runat="server" ControlToValidate="txtEndDate" 
                                ErrorMessage="End Date should be between 1/1/2000 and 12/31/2050" Text="*"
                                Type="Date" MinimumValue="1/1/2000" MaximumValue="12/31/2050" ValidationGroup="vgDetails"></asp:RangeValidator>
                            <asp:TextBox ID="txtEndTime" runat="server" Columns="8" Text='<%# Eval("EndDate", "{0:hh:mm tt}") %>' 
                            /><%--<asp:MaskedEditExtender ID="mkeEndTime" runat="server"
                                TargetControlID="txtEndTime" Mask="99:99" 
                                MessageValidatorTip="true"
                                OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="validationError"
                                MaskType="Time" AcceptAMPM="true"
                                ErrorTooltipEnabled="false" 
                            /><asp:MaskedEditValidator ID="mkevEndTime" runat="server"
                                ControlExtender="mkeEndTime"
                                ControlToValidate="txtEndTime"
                                IsValidEmpty="false" Text="*"
                                EmptyValueMessage="End Time is required" EmptyValueBlurredText="*"
                                InvalidValueMessage="End Time is invalid" InvalidValueBlurredMessage="*"
                                Display="Dynamic" ValidationGroup="vgDetails" />--%>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtEndDate" runat="server" Text='<%# Bind("EndDate") %>' Columns="12" MaxLength="10"
                            /><%--<asp:MaskedEditExtender 
                                ID="mkeEndDate" runat="server" OnInvalidCssClass="validationError"
                                TargetControlID="txtEndDate" Mask="99/99/9999" MessageValidatorTip="true"
                                CultureName="en-US" MaskType="Date" ErrorTooltipEnabled="false" 
                            /><asp:MaskedEditValidator 
                                ID="mkevEndDate" runat="server"
                                ControlExtender="mkeEndDate" ControlToValidate="txtEndDate"
                                Display="Dynamic" TooltipMessage="" IsValidEmpty="false"
                                EmptyValueMessage="End Date is required" EmptyValueBlurredText="*"
                                InvalidValueMessage="End Date is invalid" InvalidValueBlurredMessage="*"
                                ValidationGroup="vgDetails"
                            /><asp:CalendarExtender 
                                ID="caleEndDate" runat="server" Format="MM/dd/yyyy" TargetControlID="txtEndDate" PopupButtonID="ibtnEndDate" CssClass="MyCalendar"/>
                            <asp:ImageButton ID="ibtnEndDate" runat="server" ImageUrl="~/images/Calendar_scheduleHS.png" CausesValidation="False" ImageAlign="AbsMiddle" TabIndex="1000" />--%>
                            <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="txtEndDate"
                                ErrorMessage="End Date is required" Text="*" Display="dynamic" ValidationGroup="vgDetails"></asp:RequiredFieldValidator>
                            <asp:RangeValidator ID="rvEndDate" runat="server" ControlToValidate="txtEndDate" 
                                ErrorMessage="End Date should be between 1/1/2000 and 12/31/2050" Text="*"
                                Type="Date" MinimumValue="1/1/2000" MaximumValue="12/31/2050" ValidationGroup="vgDetails"></asp:RangeValidator>
                            <asp:TextBox ID="txtEndTime" runat="server" Columns="8"
                            /><%--<asp:MaskedEditExtender ID="mkeEndTime" runat="server"
                                TargetControlID="txtEndTime" Mask="99:99" 
                                MessageValidatorTip="true"
                                OnFocusCssClass="MaskedEditFocus" OnInvalidCssClass="validationError"
                                MaskType="Time" AcceptAMPM="true"
                                ErrorTooltipEnabled="false" 
                            /><asp:MaskedEditValidator ID="mkevEndTime" runat="server"
                                ControlExtender="mkeEndTime"
                                ControlToValidate="txtEndTime"
                                IsValidEmpty="false" Text="*"
                                EmptyValueMessage="End Time is required" EmptyValueBlurredText="*"
                                InvalidValueMessage="End Time is invalid" InvalidValueBlurredMessage="*"
                                Display="Dynamic" ValidationGroup="vgDetails" />--%>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblEndDate" runat="server" Text='<%# Bind("EndDate", "{0:M/d/yyyy h:mm tt}") %>' Visible="true"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Display Order" SortExpression="DispOrder">
                        <EditItemTemplate>
                            <asp:TextBox ID="txtDispOrder" runat="server" Columns="10" MaxLength="10" Text='<%# Bind("DispOrder") %>'></asp:TextBox>
                            <asp:RangeValidator ID="rvID" runat="server" ControlToValidate="txtDispOrder" 
                                ErrorMessage="Display Order should be a number between 0 to 999,999" Text="*"
                                Type="Integer" MaximumValue="999999" MinimumValue="0" ValidationGroup="vgDetails"></asp:RangeValidator>
                        </EditItemTemplate>
                        <InsertItemTemplate>
                            <asp:TextBox ID="txtDispOrder" runat="server" Columns="10" MaxLength="10" Text='<%# Bind("DispOrder") %>'></asp:TextBox>
                            <asp:RangeValidator ID="rvID" runat="server" ControlToValidate="txtDispOrder" 
                                ErrorMessage="Display Order should be a number between 0 to 999,999" Text="*"
                                Type="Integer" MaximumValue="999999" MinimumValue="0" ValidationGroup="vgDetails"></asp:RangeValidator>
                        </InsertItemTemplate>
                        <ItemTemplate>
                            <asp:Label ID="lblDispOrder" runat="server" Text='<%# Bind("DispOrder") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:CheckBoxField DataField="Disabled" HeaderText="Disabled" SortExpression="Disabled" InsertVisible="false" />
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
                            <asp:LinkButton ID="lbtnNew" runat="server" CausesValidation="False" CommandName="New"
                                Text="New" CssClass="btn btn-default btn-xs"></asp:LinkButton>
                            <asp:LinkButton ID="lbtnDelete" runat="server" CausesValidation="False" CommandName="Delete"
                                Text="Delete" CssClass="btn btn-default btn-xs" OnClientClick="return confirm('Are you sure you want to delete this record?');"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Fields>
            </asp:DetailsView>
            <asp:ObjectDataSource ID="odsNoticeDetails" runat="server" ConflictDetection="CompareAllValues"
                DeleteMethod="DeleteNotice" InsertMethod="AddNotice" OldValuesParameterFormatString="orig{0}"
                SelectMethod="GetNoticeById" TypeName="PolicyTracker.Lib.SettingsBL" DataObjectTypeName="PolicyTracker.Lib.Notice"
                UpdateMethod="UpdateNotice">
                <SelectParameters>
                    <asp:ControlParameter ControlID="gvNotices" DefaultValue="" Name="Id" PropertyName="SelectedValue"
                        Type="Int32" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:Parameter Name="notice" Type="Object" />
                    <asp:Parameter Name="origNotice" Type="Object" />
                </UpdateParameters>
            </asp:ObjectDataSource>

            <asp:Label ID="lblDetailsViewMode" runat="server" EnableViewState="false" Text="" Visible="false"></asp:Label>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
