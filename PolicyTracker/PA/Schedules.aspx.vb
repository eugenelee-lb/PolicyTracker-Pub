Imports PolicyTracker.Lib
Imports System.Web.ModelBinding
Imports System.IO

Public Class Schedules
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Schedules - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "PolicyAdmin"
        Master.ActiveSubMenu = "Schedules"

        If Not Page.IsPostBack Then
            If RouteData.Values("ScheduleId") IsNot Nothing Then
                divSearchFilters.Visible = False
                spnHeader.InnerText = "Schedule"
                dvSchedule.DefaultMode = DetailsViewMode.ReadOnly
            End If
            ddlFilterStat.DataBind()
            'ddlFilterStat.SelectedValue = ""

            Me.Form.DefaultButton = btnSearch.UniqueID

            gvSchedules.Sort("LastUpdateDT", SortDirection.Descending)
        End If
    End Sub

    Public Function gvSchedules_GetData(<Control> txtFilterName As String,
                                        <Control> txtFilterDesc As String,
                                        <Control> ddlFilterStat As String,
                                        <RouteData> ScheduleId As Object) As IQueryable(Of PolicyTracker.Lib.Schedule)
        Dim db = New PolicyTracker.Lib.PTEntities
        Dim qry = (From s In db.Schedules Select s)

        ' Schedule Id
        If ScheduleId IsNot Nothing Then
            txtFilterName = ""
            txtFilterDesc = ""
            ddlFilterStat = ""
            Dim intScheduleId As Integer = Integer.Parse(ScheduleId.ToString)
            qry = qry.Where(Function(a) a.ScheduleId = intScheduleId)
        End If

        ' user role
        If Not CType(Page.User, CustomPrincipal).IsInRole("SA") Then
            qry = qry.Where(Function(s) (From o In s.ScheduleOwners
                                         Where o.UserId = CType(Page.User, CustomPrincipal).UserId).Any())
        End If

        ' filter name
        If Not String.IsNullOrWhiteSpace(txtFilterName) Then
            qry = qry.Where(Function(a) (a.ScheduleName.Contains(txtFilterName)))
        End If
        ' filter desc
        If Not String.IsNullOrWhiteSpace(txtFilterDesc) Then
            qry = qry.Where(Function(a) (a.ScheduleDesc.Contains(txtFilterDesc)))
        End If
        ' filter status
        If ddlFilterStat = "A" Then
            qry = qry.Where(Function(a) (a.Disabled = False))
        ElseIf ddlFilterStat = "D" Then
            qry = qry.Where(Function(a) (a.Disabled = True))
        End If

        ViewState("gvRecordCount") = "Total rows: " & qry.Count.ToString()

        Return qry
    End Function

    Private Sub gvSchedules_DataBound(sender As Object, e As System.EventArgs) Handles gvSchedules.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        If gv.Rows.Count = 1 And RouteData.Values("ScheduleId") IsNot Nothing Then
            gv.SelectRow(0)
            dvSchedule.DataBind()
            BindPolicies()
            BindRecipGroups()
            BindOwners()
            BindReleases()
        Else
            gv.SelectedIndex = -1
            dvSchedule.ChangeMode(DetailsViewMode.Insert)
            dvSchedule_ModeChanged(Nothing, Nothing)
        End If
    End Sub

    Protected Sub gvSchedules_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvSchedules.SelectedIndexChanged
        dvSchedule.ChangeMode(DetailsViewMode.ReadOnly)
        dvSchedule_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvSchedules)

    End Sub

    ' Schedule
    Protected Sub dvSchedule_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvSchedule.ItemDeleted
        Try
            If e.Exception IsNot Nothing Then
                Dim strMessage As String = WebUtil.ProcessException(e.Exception)

                ' Display a user-friendly message
                lblError.Visible = True
                lblError.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9112)) '"There was a problem deleting the message. "

                Dim _customValidator = New CustomValidator()
                _customValidator.IsValid = False
                _customValidator.ErrorMessage = strMessage
                _customValidator.ValidationGroup = "vgDetails"
                Page.Validators.Add(_customValidator)

                Dim inner As Exception = e.Exception
                If inner.InnerException IsNot Nothing Then inner = inner.InnerException
                If inner.InnerException IsNot Nothing Then inner = inner.InnerException
                If TypeOf inner Is System.Data.Entity.Core.OptimisticConcurrencyException Then
                    dvSchedule.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                dvSchedule.DataBind()
            Else
                ' Success
                gvSchedules.SelectedIndex = -1
                gvSchedules.DataBind()
                dvSchedule.ChangeMode(DetailsViewMode.Insert)

                lblInfo.Visible = True
                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try
    End Sub

    Protected Sub dvSchedule_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvSchedule.ItemInserted
        Try
            If e.Exception IsNot Nothing Then
                Dim strMessage As String = WebUtil.ProcessException(e.Exception)

                ' Display a user-friendly message
                lblError.Visible = True
                lblError.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9113)) '"There was a problem adding a new record. "

                Dim _customValidator = New CustomValidator()
                _customValidator.IsValid = False
                _customValidator.ErrorMessage = strMessage
                _customValidator.ValidationGroup = "vgDetails"
                Page.Validators.Add(_customValidator)

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInInsertMode = True
            Else
                ' Select the record inserted
                'Dim colKeyValues = New Collections.Specialized.NameValueCollection
                'colKeyValues.Add("NoticeId", e.Values("NoticeId").ToString)
                'WebUtil.SelectGridRowByKeyValue(gvNotices, colKeyValues)
                lblDetailsViewMode.Text = "ReadOnly"

                lblInfo.Visible = True
                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(11)) ' Record Inserted.
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Private Sub dvSchedule_ItemInserting(sender As Object, e As System.Web.UI.WebControls.DetailsViewInsertEventArgs) Handles dvSchedule.ItemInserting
        e.Values("LastUpdateDT") = Now()
        e.Values("LastUpdateUser") = CType(User, CustomPrincipal).UserId
    End Sub

    Protected Sub dvSchedule_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvSchedule.ItemUpdated
        Try
            If e.Exception IsNot Nothing Then
                Dim strMessage As String = WebUtil.ProcessException(e.Exception)

                ' Display a user-friendly message
                lblError.Visible = True
                lblError.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9111)) '"There was a problem updating the record. "

                Dim _customValidator = New CustomValidator()
                _customValidator.IsValid = False
                _customValidator.ErrorMessage = strMessage
                _customValidator.ValidationGroup = "vgDetails"
                Page.Validators.Add(_customValidator)

                Dim inner As Exception = e.Exception
                If inner.InnerException IsNot Nothing Then inner = inner.InnerException
                If inner.InnerException IsNot Nothing Then inner = inner.InnerException
                If TypeOf inner Is System.Data.Entity.Core.OptimisticConcurrencyException Then
                    dvSchedule.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInEditMode = True
            Else
                ' Select the record inserted
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("ScheduleID", e.Keys.Item("ScheduleID").ToString)
                WebUtil.SelectGridRowByKeyValue(gvSchedules, colKeyValues)
                lblDetailsViewMode.Text = "ReadOnly"

                lblInfo.Visible = True
                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(12)) ' Record Updated
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Protected Sub dvSchedule_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dvSchedule.ModeChanged
        Select Case dvSchedule.CurrentMode
            Case DetailsViewMode.Insert
                If gvSchedules.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
                    dvSchedule.ChangeMode(DetailsViewMode.ReadOnly)
                    'dvSchedule.Caption = "Schedule Details"
                    tlkSchedule.InnerText = "Schedule Details"
                    tliPolicies.Visible = True
                    tliRecipGroups.Visible = True
                    tliOwners.Visible = True
                    'tliReleases.Visible = True
                    BindReleases()
                Else
                    gvSchedules.SelectedIndex = -1
                    WebUtil.SetGridRowStyle(gvSchedules)
                    'dvSchedule.Caption = "Add New Schedule"
                    tlkSchedule.InnerText = "Add New Schedule"
                    tliPolicies.Visible = False
                    tliRecipGroups.Visible = False
                    tliOwners.Visible = False
                    tliReleases.Visible = False
                End If
            Case DetailsViewMode.Edit
                'dvSchedule.Caption = "Edit Schedule"
                tlkSchedule.InnerText = "Edit Schedule"
                tliPolicies.Visible = True
                tliRecipGroups.Visible = True
                tliOwners.Visible = True
                'tliReleases.Visible = True
            Case DetailsViewMode.ReadOnly
                'dvSchedule.Caption = "Schedule Details"
                tlkSchedule.InnerText = "Schedule Details"
                tliPolicies.Visible = True
                tliRecipGroups.Visible = True
                tliOwners.Visible = True
                'tliReleases.Visible = True
                BindReleases()
        End Select

    End Sub

    Private Sub dvSchedule_DataBound(sender As Object, e As EventArgs) Handles dvSchedule.DataBound
        Try
            If dvSchedule.CurrentMode = DetailsViewMode.ReadOnly And dvSchedule.DataKey.Value IsNot Nothing Then
                ' save ownership
                ViewState("IsOwner") = False
                Dim intScheduleId As Integer = Integer.Parse(dvSchedule.DataKey.Value)
                Dim bl = New PTBL
                If bl.IsScheduleOwner(intScheduleId, CType(User, CustomPrincipal).UserId) Then ViewState("IsOwner") = True

                BindPolicies()
                BindRecipGroups()
                BindOwners()
                BindReleases()
            End If

            ' only owners can edit/delete
            If dvSchedule.CurrentMode = DetailsViewMode.ReadOnly Then
                Dim lbtnEdit As LinkButton = dvSchedule.FindControl("lbtnEdit")
                Dim lbtnDelete As LinkButton = dvSchedule.FindControl("lbtnDelete")
                If ViewState("IsOwner") Then
                    lbtnEdit.Visible = True
                    lbtnDelete.Visible = True
                Else
                    lbtnEdit.Visible = False
                    lbtnDelete.Visible = False
                End If

                If RouteData.Values("ScheduleId") IsNot Nothing Then
                    Dim lbtnNew As LinkButton = dvSchedule.FindControl("lbtnNew")
                    lbtnNew.Visible = False
                End If
            End If

            If dvSchedule.CurrentMode = DetailsViewMode.Insert Then
                Dim ddlFrequency As DropDownList = dvSchedule.FindControl("ddlFrequency")
                ddlFrequency.SelectedValue = "ONE_TIME"
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
        End Try

    End Sub

    Private Sub odsScheduleDetails_Inserted(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceStatusEventArgs) Handles odsScheduleDetails.Inserted
        If e.Exception Is Nothing Then
            ' Select the record inserted
            Dim colKeyValues = New Collections.Specialized.NameValueCollection
            colKeyValues.Add("NewScheduleID", e.ReturnValue)
            If WebUtil.SelectGridRowByKeyValue(gvSchedules, colKeyValues) Then
                lblDetailsViewMode.Text = "ReadOnly"

                BindPolicies()
                BindRecipGroups()
                BindOwners()
                BindReleases()
            End If
        End If
    End Sub

    Protected Sub odsScheduleDetails_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsScheduleDetails.Inserting
        Dim obj As Schedule = e.InputParameters("schedule")
        obj.CreateUser = CType(User, CustomPrincipal).UserId
        obj.CreateDT = Now()
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()
    End Sub

    Protected Sub odsScheduleDetails_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsScheduleDetails.Updating
        Dim obj As Schedule = e.InputParameters("schedule")
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()
    End Sub

    Private Sub odsScheduleDetails_Deleting(sender As Object, e As ObjectDataSourceMethodEventArgs) Handles odsScheduleDetails.Deleting
        Dim obj As Schedule = e.InputParameters("schedule")
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        'obj.LastUpdateDT = Now()
    End Sub

    Protected Function EvalHtmlEncode(colname As String) As String
        Return WebUtil.HtmlMsgEncode(Eval(colname))
    End Function

    ' Policies
    Private Sub BindPolicies()
        Dim intScheduleID As Integer
        If Not Integer.TryParse(gvSchedules.SelectedDataKey("ScheduleID"), intScheduleID) Then Exit Sub

        Using context = New PTEntities()
            Dim qry = (From p In context.Policies Select p Where p.Disabled = False)
            Dim blSettings As New SettingsBL
            Dim myDepts = blSettings.GetDepartmentsByAdmin(CType(Page.User, CustomPrincipal).Role, CType(Page.User, CustomPrincipal).UserId)
            Dim listMyDepts = (From d In myDepts Select d.DeptCode)
            qry = qry.Where(Function(a) (a.CreateUser = CType(Page.User, CustomPrincipal).UserId _
                                         Or (a.ShareType = "DEPT" And listMyDepts.Contains(a.DeptCode)) _
                                         Or (a.ShareType = "PUB")))

            Dim allPolicies = (From p In qry Select p).ToList()
            Dim schedule = (From s In context.Schedules
                          Where s.ScheduleId = intScheduleID
                          Select s).First()
            Dim assignedPolicies = schedule.Policies.ToList()
            Dim unassignedPolicies = allPolicies.Except(assignedPolicies.AsEnumerable()).ToList()

            gvPolicies.DataSource = assignedPolicies
            gvPolicies.DataBind()
            spnPolCnt.InnerText = assignedPolicies.Count.ToString
            If assignedPolicies.Count = 0 Then
                tliPolicies.Attributes.Add("class", "showTooltip")
            Else
                tliPolicies.Attributes.Item("class") = tliPolicies.Attributes.Item("class").Replace("showTooltip", "")
            End If

            ddlPolicyToAdd.DataSource = unassignedPolicies
            ddlPolicyToAdd.DataBind()
            ddlPolicyToAdd.Items.Insert(0, New ListItem("-- Select --", ""))

            If unassignedPolicies.Count > 0 Then
                btnAddPolicy.Enabled = True
            Else
                btnAddPolicy.Enabled = False
            End If
        End Using
    End Sub

    Private Sub gvPolicies_DataBound(sender As Object, e As EventArgs) Handles gvPolicies.DataBound
        Dim gv As WebControls.GridView = sender
        gv.SelectedIndex = -1
        gvPolicies_SelectedIndexChanged(Nothing, Nothing)
        WebUtil.SetGridRowStyle(gv)
    End Sub

    Private Sub gvPolicies_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvPolicies.RowCommand
        If e.CommandName = "DeletePolicy" Then
            Try
                Dim intScheduleID As Integer = Integer.Parse(gvSchedules.SelectedDataKey("ScheduleID"))
                Dim intPolicyID As Integer = Integer.Parse(e.CommandArgument)

                Dim bl = New PTBL
                If bl.DeleteSchedulePolicy(intScheduleID, intPolicyID) Then
                    BindPolicies()
                    lblInfoPolicies.Text = "Policy has been deleted."
                    lblInfoPolicies.Visible = True
                Else
                    BindPolicies()
                    lblErrorPolicies.Visible = True
                    lblErrorPolicies.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9112)) '"There was a problem deleting a record. "
                End If

            Catch ex As Exception
                lblErrorPolicies.Visible = True
                lblErrorPolicies.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9112)) '"There was a problem deleting a record. "

                Dim strMessage As String = WebUtil.ProcessException(ex)
                Dim _customValidator = New CustomValidator()
                _customValidator.IsValid = False
                _customValidator.ErrorMessage = strMessage
                _customValidator.ValidationGroup = "vgPolicies"
                Page.Validators.Add(_customValidator)
            End Try
        End If

    End Sub

    Private Sub btnAddPolicy_Click(sender As Object, e As System.EventArgs) Handles btnAddPolicy.Click
        Try
            Dim intScheduleID As Integer = Integer.Parse(gvSchedules.SelectedDataKey("ScheduleID"))
            Dim intPolicyID As Integer = Integer.Parse(ddlPolicyToAdd.SelectedValue)

            Dim bl = New PTBL
            If bl.AddSchedulePolicy(intScheduleID, intPolicyID) Then
                BindPolicies()
                lblInfoPolicies.Text = "Policy has been added."
                lblInfoPolicies.Visible = True
            Else
                BindPolicies()
                lblErrorPolicies.Visible = True
                lblErrorPolicies.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9113)) '"There was a problem adding a new record. "
            End If

        Catch ex As Exception
            lblErrorPolicies.Visible = True
            lblErrorPolicies.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9113)) '"There was a problem adding a new record. "

            Dim strMessage As String = WebUtil.ProcessException(ex)
            Dim _customValidator = New CustomValidator()
            _customValidator.IsValid = False
            _customValidator.ErrorMessage = strMessage
            _customValidator.ValidationGroup = "vgPolicies"
            Page.Validators.Add(_customValidator)
        End Try
    End Sub

    Private Sub gvPolicies_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gvPolicies.SelectedIndexChanged
        WebUtil.SetGridRowStyle(gvPolicies)
        If gvPolicies.SelectedIndex = -1 Then
            gvFiles.Visible = False
        Else
            gvFiles.DataBind()
            gvFiles.Visible = True
        End If
    End Sub

    ' Pol - Upload Files
    Public Function gvFiles_GetData() As IQueryable(Of PolicyTracker.Lib.UploadFile)
        Dim intPolicyId As Integer = Integer.Parse(gvPolicies.SelectedDataKey.Values("PolicyId"))

        Dim db = New PolicyTracker.Lib.PTEntities
        Dim qry = (From uf In db.UploadFiles
                   Where uf.Policies.Where(Function(a) a.PolicyId = intPolicyId).Any()
                   Select uf Order By uf.OriginalName)
        Return qry
    End Function

    Private Sub gvFiles_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvFiles.RowCommand
        Try
            If e.CommandName = "Download" Then
                Dim repo As New PTRepository
                Dim uploadFile = repo.GetUploadFileById(e.CommandArgument)
                If uploadFile Is Nothing Then
                    Throw New ApplicationException("Could not find upload file record.")
                End If

                Dim fi As FileInfo = Nothing
                If Not String.IsNullOrWhiteSpace(uploadFile.FileUrl) Then
                    Dim uploadRoot As String = ConfigurationManager.AppSettings("UPLOAD_FOLDER") & "\"
                    If Not File.Exists(uploadRoot & uploadFile.FileUrl) Then
                        ' file not exists
                        Throw New ApplicationException("Could not find file in repository.")
                    End If
                    fi = New FileInfo(uploadRoot & uploadFile.FileUrl)
                End If

                Response.Clear()
                Response.ClearHeaders()
                Response.ClearContent()
                Response.AddHeader("Content-Disposition", "attachment; filename=""" + uploadFile.OriginalName + """")
                Response.AddHeader("Content-Length", uploadFile.Length.ToString())
                Response.ContentType = uploadFile.ContentType
                Response.Flush()
                If fi IsNot Nothing Then
                    Response.TransmitFile(fi.FullName)
                Else
                    Response.BinaryWrite(uploadFile.FileData)
                End If
                Response.End()
            End If

        Catch ex As Exception
            lblErrorUpload.Visible = True
            lblErrorUpload.Text = WebUtil.ProcessException(ex)

        Finally
            gvFiles.DataBind()

        End Try
    End Sub

    Private Sub gvFiles_RowDataBound(sender As Object, e As Web.UI.WebControls.GridViewRowEventArgs) Handles gvFiles.RowDataBound
        Dim row As GridViewRow = e.Row
        Dim btnDownload As LinkButton = row.FindControl("lbtnDownload")
        If btnDownload IsNot Nothing Then
            'lblInfoUpload.Text &= lnkDownload.UniqueID.Substring(lnkDownload.UniqueID.IndexOf("atcBiz$")) & "<br/>"
            Dim pbt As New PostBackTrigger
            pbt.ControlID = btnDownload.UniqueID.Substring(btnDownload.UniqueID.IndexOf("gvFiles$"))
            UpdatePanel1.Triggers.Add(pbt)
        End If
        'lblInfoUpload.Visible = True
    End Sub

    ' Recipient Groups
    Private Sub BindRecipGroups()
        Dim intScheduleID As Integer
        If Not Integer.TryParse(gvSchedules.SelectedDataKey("ScheduleID"), intScheduleID) Then Exit Sub

        Using context = New PTEntities()
            Dim qry = (From a In context.RecipGroups Select a Where a.Disabled = False)
            Dim blSettings As New SettingsBL
            Dim myDepts = blSettings.GetDepartmentsByAdmin(CType(Page.User, CustomPrincipal).Role, CType(Page.User, CustomPrincipal).UserId)
            Dim listMyDepts = (From d In myDepts Select d.DeptCode)
            qry = qry.Where(Function(a) (a.CreateUser = CType(Page.User, CustomPrincipal).UserId _
                                         Or (a.ShareType = "DEPT" And listMyDepts.Contains(a.DeptCode)) _
                                         Or (a.ShareType = "PUB")))

            Dim allRecipGroups = (From a In qry Select a).ToList()
            Dim schedule = (From s In context.Schedules
                          Where s.ScheduleId = intScheduleID
                          Select s).First()
            Dim assignedRGs = schedule.RecipGroups.ToList()
            Dim unassignedRGs = allRecipGroups.Except(assignedRGs.AsEnumerable()).ToList()

            gvRecipGroups.DataSource = assignedRGs
            gvRecipGroups.DataBind()
            spnRecGrpCnt.InnerText = assignedRGs.Count.ToString
            If assignedRGs.Count = 0 Then
                tliRecipGroups.Attributes.Add("class", "showTooltip")
            Else
                tliRecipGroups.Attributes.Item("class") = tliRecipGroups.Attributes.Item("class").Replace("showTooltip", "")
            End If

            ddlRecipGroupToAdd.DataSource = unassignedRGs
            ddlRecipGroupToAdd.DataBind()
            ddlRecipGroupToAdd.Items.Insert(0, New ListItem("-- Select --", ""))

            If unassignedRGs.Count > 0 Then
                btnAddRecipGroup.Enabled = True
            Else
                btnAddRecipGroup.Enabled = False
            End If
        End Using
    End Sub

    Private Sub gvRecipGroups_RowCommand(sender As Object, e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles gvRecipGroups.RowCommand
        If e.CommandName = "DeleteRecipGroup" Then
            Try
                Dim intScheduleId As Integer = Integer.Parse(gvSchedules.SelectedValue)
                Dim intRecipGroupID As Integer = Integer.Parse(e.CommandArgument)
                Dim intReturn As Integer
                Dim strMemberName As String

                Using db = New PolicyTracker.Lib.PTEntities
                    Dim sch = (From s In db.Schedules
                               Where s.ScheduleId = intScheduleId
                               Select s).FirstOrDefault()
                    If sch Is Nothing Then
                        Throw New ApplicationException("Schedule [" & intScheduleId.ToString & "] is not found.")
                    End If

                    Dim member = (From a In sch.RecipGroups
                                  Where a.RecipGroupId = intRecipGroupID
                                  Select a).FirstOrDefault()
                    If member Is Nothing Then
                        Throw New ApplicationException("Recipient Group [" & intRecipGroupID.ToString & "] is not found.")
                    End If
                    strMemberName = member.GroupName

                    sch.RecipGroups.Remove(member)
                    intReturn = db.SaveChanges()
                End Using

                If intReturn = 1 Then
                    BindRecipGroups()
                    lblInfoRecipGroups.Text = "Recipient Group [" & strMemberName & "] has been removed."
                    lblInfoRecipGroups.Visible = True
                End If

            Catch ex As Exception
                lblErrorRecipGroups.Visible = True
                lblErrorRecipGroups.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9112)) '"There was a problem deleting a record. "

                Dim strMessage As String = WebUtil.ProcessException(ex)
                Dim _customValidator = New CustomValidator()
                _customValidator.IsValid = False
                _customValidator.ErrorMessage = strMessage
                _customValidator.ValidationGroup = "vgRecipGroups"
                Page.Validators.Add(_customValidator)
            End Try

        ElseIf e.CommandName = "RecipGroupMembers" Then
            Try
                Dim intRecipGroupId As Integer = Integer.Parse(e.CommandArgument)
                RecipGroupMembers.GetRecipGroupMembers(intRecipGroupId)

            Catch ex As Exception
                lblErrorRecipGroups.Visible = True
                lblErrorRecipGroups.Text = "There was a problem in showing Recipient Group Members."

                Dim strMessage As String = WebUtil.ProcessException(ex)
                Dim _customValidator = New CustomValidator()
                _customValidator.IsValid = False
                _customValidator.ErrorMessage = strMessage
                _customValidator.ValidationGroup = "vgRecipGroups"
                Page.Validators.Add(_customValidator)
            End Try
        End If

    End Sub

    Private Sub btnAddRecipGroup_Click(sender As Object, e As System.EventArgs) Handles btnAddRecipGroup.Click
        Try
            Dim intScheduleID As Integer = Integer.Parse(gvSchedules.SelectedDataKey("ScheduleID"))
            Dim intRecipGroupID As Integer = Integer.Parse(ddlRecipGroupToAdd.SelectedValue)
            Dim intReturn As Integer

            Using db = New PolicyTracker.Lib.PTEntities
                ' search existing RG
                Dim sch = (From s In db.Schedules
                           Where s.ScheduleId = intScheduleID
                           Select s).FirstOrDefault()
                If sch Is Nothing Then
                    Throw New ApplicationException("Schedule [" & intScheduleID.ToString & "] is not found.")
                End If

                Dim rg = (From a In sch.RecipGroups
                          Where a.RecipGroupId = intRecipGroupID
                          Select a).FirstOrDefault()
                If rg IsNot Nothing Then
                    Throw New ApplicationException("Recipient Group [" & ddlRecipGroupToAdd.SelectedItem.Text & "] already exists.")
                End If

                Dim newRG = (From a In db.RecipGroups
                             Where a.RecipGroupId = intRecipGroupID
                             Select a).FirstOrDefault()
                If newRG Is Nothing Then
                    Throw New ApplicationException("Recipient Group [" & ddlRecipGroupToAdd.SelectedValue & "] is not found.")
                End If

                sch.RecipGroups.Add(newRG)
                intReturn = db.SaveChanges()
            End Using

            If intReturn = 1 Then
                lblInfoRecipGroups.Text = "Recipient Group [" & ddlRecipGroupToAdd.SelectedItem.Text & "] has been added."
                lblInfoRecipGroups.Visible = True
                BindRecipGroups()
            End If

        Catch ex As Exception
            lblErrorRecipGroups.Visible = True
            lblErrorRecipGroups.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9113)) '"There was a problem adding a new record. "

            Dim strMessage As String = WebUtil.ProcessException(ex)
            Dim _customValidator = New CustomValidator()
            _customValidator.IsValid = False
            _customValidator.ErrorMessage = strMessage
            _customValidator.ValidationGroup = "vgRecipGroups"
            Page.Validators.Add(_customValidator)
        End Try
    End Sub

    ' Owners
    Private Sub BindOwners()
        Dim intScheduleId As Integer
        If Not Integer.TryParse(gvSchedules.SelectedValue, intScheduleId) Then Exit Sub
        Using db = New PolicyTracker.Lib.PTEntities

            Dim aOwners = (From a In db.ScheduleOwners, u In db.AppUsers
                           Where a.ScheduleId = intScheduleId And a.UserId = u.UserId
                           Select Owner = a.UserId, OwnerName = u.UserName
                           Order By Owner).ToList()

            gvOwners.DataSource = aOwners
            gvOwners.DataBind()
            spnOwnCnt.InnerText = aOwners.Count.ToString

            Dim userRoles = {"PA", "SA"}
            Dim allOwners = (From au In db.AppUsers
                             Where userRoles.Contains(au.UserRole)
                             Select Owner = au.UserId, OwnerName = au.UserName
                             Order By Owner).ToList()

            Dim uOwners = allOwners.Except(aOwners.AsEnumerable()).ToList()
            ddlOwnerToAdd.DataSource = uOwners
            ddlOwnerToAdd.DataBind()
            ddlOwnerToAdd.Items.Insert(0, New ListItem("-- Select --", ""))

            If uOwners.Count > 0 Then
                btnAddOwner.Enabled = True
            Else
                btnAddOwner.Enabled = False
            End If
        End Using
    End Sub

    Private Sub btnAddOwner_Click(sender As Object, e As EventArgs) Handles btnAddOwner.Click
        Try
            Dim intScheduleId As Integer = Integer.Parse(gvSchedules.SelectedValue)
            Dim intReturn As Integer

            Using db = New PolicyTracker.Lib.PTEntities
                ' search existing Owner
                Dim so = (From a In db.ScheduleOwners
                          Where a.ScheduleId = intScheduleId And ddlOwnerToAdd.SelectedValue.Equals(a.UserId)
                           Select a).FirstOrDefault()
                If so IsNot Nothing Then
                    Throw New ApplicationException("Schedule Owner [" & ddlOwnerToAdd.SelectedValue & "] already exists.")
                End If

                Dim newOwner = (From u In db.AppUsers
                                 Where u.UserId = ddlOwnerToAdd.SelectedValue
                                 Select u).FirstOrDefault()
                If newOwner Is Nothing Then
                    Throw New ApplicationException("User [" & ddlOwnerToAdd.SelectedValue & "] is not found.")
                End If

                Dim sch = (From s In db.Schedules
                           Where s.ScheduleId = intScheduleId
                           Select s).FirstOrDefault()
                If sch Is Nothing Then
                    Throw New ApplicationException("Schedule [" & intScheduleId.ToString & "] is not found.")
                End If

                Dim newSO As New ScheduleOwner
                With newSO
                    .ScheduleId = intScheduleId
                    .UserId = newOwner.UserId
                End With
                sch.ScheduleOwners.Add(newSO)
                intReturn = db.SaveChanges()
            End Using

            If intReturn = 1 Then
                Dim strOwnerName As String = ddlOwnerToAdd.SelectedItem.Text
                lblInfoOwners.Text = "Schedule Owner [" & strOwnerName & "] has been added."
                lblInfoOwners.Visible = True
                dvSchedule.DataBind()
                'BindOwners()
            End If

        Catch ex As Exception
            lblErrorOwners.Visible = True
            lblErrorOwners.Text = "There was a problem in adding owner."

            Dim strMessage As String = WebUtil.ProcessException(ex)
            Dim _customValidator = New CustomValidator()
            _customValidator.IsValid = False
            _customValidator.ErrorMessage = strMessage
            _customValidator.ValidationGroup = "vgOwners"
            Page.Validators.Add(_customValidator)
        End Try
    End Sub

    Private Sub gvOwners_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvOwners.RowCommand
        Try
            If e.CommandName = "Remove" Then
                Dim intScheduleId As Integer = Integer.Parse(gvSchedules.SelectedValue)
                Dim strOwner As String = e.CommandArgument.ToString
                Dim intReturn As Integer

                Using db = New PolicyTracker.Lib.PTEntities
                    Dim member = (From a In db.ScheduleOwners
                                     Where a.ScheduleId = intScheduleId And a.UserId = strOwner
                                     Select a).FirstOrDefault()
                    If member Is Nothing Then
                        Throw New ApplicationException("Schedule Owner [" & strOwner & "] is not found.")
                    End If

                    Dim sch = (From s In db.Schedules
                               Where s.ScheduleId = intScheduleId
                               Select s).FirstOrDefault()
                    If sch Is Nothing Then
                        Throw New ApplicationException("Schedule [" & intScheduleId.ToString & "] is not found.")
                    End If

                    ' cannot remove yourself
                    If strOwner = CType(User, CustomPrincipal).UserId Then
                        Throw New ApplicationException("You cannot remove yourself from owner.")
                    End If
                    ' cannot remove creator from owner
                    If sch.CreateUser.ToLower = strOwner.ToLower Then
                        Throw New ApplicationException("You cannot remove schedule creator from owner.")
                    End If

                    sch.ScheduleOwners.Remove(member)
                    intReturn = db.SaveChanges()
                End Using

                If intReturn = 1 Then
                    lblInfoOwners.Text = "Schedule Owner [" & strOwner & "] has been removed."
                    lblInfoOwners.Visible = True
                    BindOwners()
                End If

            End If

        Catch ex As Exception
            lblErrorOwners.Visible = True
            lblErrorOwners.Text = "There was a problem in removing owner."

            Dim strMessage As String = WebUtil.ProcessException(ex)
            Dim _customValidator = New CustomValidator()
            _customValidator.IsValid = False
            _customValidator.ErrorMessage = strMessage
            _customValidator.ValidationGroup = "vgOwners"
            Page.Validators.Add(_customValidator)
        End Try
    End Sub

    ' Releases
    Private Sub BindReleases()
        If gvPolicies.Rows.Count > 0 And gvRecipGroups.Rows.Count > 0 Then
            tliReleases.Visible = True
        Else
            tliReleases.Visible = False
        End If
        If CType(User, CustomPrincipal).IsInRole("SA") Then btnCreateNextRelease.Visible = True

        Dim intScheduleId As Integer
        If Not Integer.TryParse(gvSchedules.SelectedValue, intScheduleId) Then Exit Sub

        Using db = New PolicyTracker.Lib.PTEntities

            Dim rels = (From a In db.Releases
                        Where a.ScheduleId = intScheduleId
                        Select a
                        Order By a.ReleaseDate Descending).ToList()
            If rels.Count > 0 Then tliReleases.Visible = True
            spnRelCnt.InnerText = rels.Count.ToString

            gvReleases.DataSource = rels
            gvReleases.DataBind()

            'ViewState("gvRelCount") = "Total rows: " & rels.Count.ToString()
        End Using
    End Sub

    Private Sub gvReleases_DataBound(sender As Object, e As EventArgs) Handles gvReleases.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        'If gv.Rows.Count > 0 Then
        '    gv.BottomPagerRow.Visible = True
        '    ' record count
        '    Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
        '    pager.SetTotalRecordCount(ViewState("gvRelCount"))
        'End If
    End Sub

    Private Sub btnCreateNextRelease_Click(sender As Object, e As EventArgs) Handles btnCreateNextRelease.Click
        Try
            Dim intScheduleId As Integer = Integer.Parse(gvSchedules.SelectedValue)
            Dim intLeadDaysToCreateRelease As Integer = Integer.Parse(ConfigurationManager.AppSettings("LeadDaysToCreateRelease"))

            Dim bl As PTBL = New PTBL
            If bl.CreateRelease(intScheduleId, CType(Context.User, CustomPrincipal).UserId, intLeadDaysToCreateRelease) > 0 Then
                BindReleases()
                lblInfoReleases.Text = "Next release has been created."
                lblInfoReleases.Visible = True
            Else
                lblErrorReleases.Text = "No release was created. Only release from today to " & intLeadDaysToCreateRelease.ToString & " days is created."
                lblErrorReleases.Visible = True
            End If

        Catch ex As Exception
            lblErrorReleases.Visible = True
            lblErrorReleases.Text = "There was a problem in creating next release."

            Dim strMessage As String = WebUtil.ProcessException(ex)
            Dim _customValidator = New CustomValidator()
            _customValidator.IsValid = False
            _customValidator.ErrorMessage = strMessage
            _customValidator.ValidationGroup = "vgReleases"
            Page.Validators.Add(_customValidator)
        End Try
    End Sub

End Class