Imports PolicyTracker.Lib
Imports System.Web.ModelBinding
Imports System.IO

Public Class Release
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Release - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Releases"
        Master.ActiveSubMenu = ""

        Try
            fvRelease.DataBind()
            If fvRelease.DataItemCount < 1 Then
                lblFilterOrg.Visible = False
                ddlFilterOrg.Visible = False
                tlMain.Visible = False
                Exit Sub
            End If

            If Not Page.IsPostBack Then
                ' any packet under org permission
                If Not CType(User, CustomPrincipal).IsInRole("SA") Then
                    Dim intReleaseId As Integer = Integer.Parse(RouteData.Values("ReleaseId"))
                    If Not PTBL.IsReleaseForAdmin(intReleaseId, CType(User, CustomPrincipal).UserId) Then
                        lblFilterOrg.Visible = False
                        ddlFilterOrg.Visible = False
                        tlMain.Visible = False
                        lblError.Text = "You have no packet to manage in this release."
                        lblError.Visible = True
                    End If
                End If

                ddlFilterOrg.DataBind()
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub

    Public Function fvRelease_GetItem(<RouteData> releaseId As Integer) As PolicyTracker.Lib.Release
        Dim db = New PTEntities
        Dim rel = (From a In db.Releases Where a.ReleaseId = releaseId Select a).FirstOrDefault()

        btnCreateData.Visible = False
        btnCreateNotices.Visible = False
        If (Not rel Is Nothing) AndAlso ConfigurationManager.AppSettings("DevUserID").ToLower.IndexOf(CType(Context.User, CustomPrincipal).UserId.ToLower) >= 0 Then
            btnCreateData.Visible = True
            btnCreateNotices.Visible = True
            btnCreateData.Enabled = False
            btnCreateNotices.Enabled = False
            Dim acked = (From pa In rel.ReleaseRecipients Where pa.AckDT.HasValue).Any()
            If Not acked Then
                btnCreateData.Enabled = True
            End If
            btnCreateNotices.Enabled = True
        End If

        Return rel
    End Function

    Private Sub btnCreateData_Click(sender As Object, e As EventArgs) Handles btnCreateData.Click
        Try
            Dim intReleaseId As Integer = Integer.Parse(RouteData.Values("ReleaseId"))

            Dim bl As PTBL = New PTBL
            If bl.CreateReleasePolicyRecip(intReleaseId, CType(User, CustomPrincipal).UserId) > 0 Then
                fvRelease.DataBind()
                gvRelPolicies.DataBind()
                gvRelRecipients.DataBind()
                gvRelNotices.DataBind()

                lblInfo.Text = "Recipients/Policies have been created."
                lblInfo.Visible = True
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = "There was a problem in creating Recipients/Policies."

            Dim strMessage As String = WebUtil.ProcessException(ex)
            Dim _customValidator = New CustomValidator()
            _customValidator.IsValid = False
            _customValidator.ErrorMessage = strMessage
            _customValidator.ValidationGroup = "vgMain"
            Page.Validators.Add(_customValidator)
        End Try
    End Sub

    Private Sub btnCreateNotices_Click(sender As Object, e As EventArgs) Handles btnCreateNotices.Click
        Try
            Dim intReleaseId As Integer = Integer.Parse(RouteData.Values("ReleaseId"))

            Dim bl As PTBL = New PTBL
            If bl.CreateReleaseNotices(intReleaseId, CType(User, CustomPrincipal).UserId) > 0 Then
                fvRelease.DataBind()
                gvRelNotices.DataBind()

                lblInfo.Text = "Notices have been created."
                lblInfo.Visible = True
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = "There was a problem in creating Notices."

            Dim strMessage As String = WebUtil.ProcessException(ex)
            Dim _customValidator = New CustomValidator()
            _customValidator.IsValid = False
            _customValidator.ErrorMessage = strMessage
            _customValidator.ValidationGroup = "vgMain"
            Page.Validators.Add(_customValidator)
        End Try
    End Sub

    Private Sub ddlFilterOrg_DataBound(sender As Object, e As EventArgs) Handles ddlFilterOrg.DataBound
        If CType(User, CustomPrincipal).IsInRole("SA") Then
            ddlFilterOrg.Items.Insert(0, New ListItem("-- All --", ""))
        End If
    End Sub

    Protected Sub odsOrg_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs)
        e.InputParameters("userRole") = CType(Page.User, CustomPrincipal).Role
        e.InputParameters("userId") = CType(Page.User, CustomPrincipal).UserId
    End Sub

    ' Rel Policies
    Public Function gvRelPolicies_GetData(<RouteData> releaseId As Integer) As IQueryable(Of PolicyTracker.Lib.ReleasePolicy)
        Dim db = New PTEntities
        Dim qry = (From a In db.ReleasePolicies Where a.ReleaseId = releaseId Select a)

        Dim cnt = qry.Count()
        spnPolCnt.InnerText = cnt.ToString

        Return qry
    End Function

    Private Sub gvRelPolicies_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gvRelPolicies.SelectedIndexChanged
        WebUtil.SetGridRowStyle(gvRelPolicies)
        gvFiles.DataBind()
        gvFiles.Visible = True
    End Sub

    ' RelPol - Upload Files
    Public Function gvFiles_GetData() As IQueryable(Of PolicyTracker.Lib.UploadFile)
        Dim intReleaseId As Integer = Integer.Parse(gvRelPolicies.SelectedDataKey.Values("ReleaseId"))
        Dim intPolicyId As Integer = Integer.Parse(gvRelPolicies.SelectedDataKey.Values("PolicyId"))

        Dim db = New PolicyTracker.Lib.PTEntities
        Dim qry = (From uf In db.UploadFiles
                   Where uf.ReleasePolicies.Where(Function(a) a.ReleaseId = intReleaseId And a.PolicyId = intPolicyId).Any()
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

    ' Rel Recipients
    Public Function gvRelRecipients_GetData(<RouteData> releaseId As Integer,
                                            <Control> ddlFilterOrg As String) As IQueryable(Of PolicyTracker.Lib.vPacket)
        Dim db = New PTEntities
        Dim qry = (From a In db.vPackets Where a.ReleaseId = releaseId Select a)
        If ddlFilterOrg <> "" Then
            qry = qry.Where(Function(a) a.OrgCode.StartsWith(ddlFilterOrg))
        End If

        Dim cnt = qry.Count()
        spnRecCnt.InnerText = cnt.ToString
        ViewState("gvRecordCount_Recip") = "Total rows: " & cnt.ToString()

        Return qry
    End Function

    Private Sub gvRelRecipients_DataBound(sender As Object, e As EventArgs) Handles gvRelRecipients.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount_Recip"))
        End If

        gv.SelectedIndex = -1
        dvPacket.ChangeMode(DetailsViewMode.Insert)
        dvPacket_ModeChanged(Nothing, Nothing)
        'dvPacket.Visible = False
    End Sub

    Private Sub gvRelRecipients_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvRelRecipients.RowCommand
        Try
            If e.CommandName = "EmpInfo" Then
                EmpInfo.GetEmpInfo(e.CommandArgument.ToString)
            End If

        Catch ex As Exception
            lblErrorRecipients.Visible = True
            lblErrorRecipients.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub

    Private Sub gvRelRecipients_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gvRelRecipients.SelectedIndexChanged
        dvPacket.ChangeMode(DetailsViewMode.ReadOnly)
        dvPacket.DataBind()
        dvPacket.Visible = True

        WebUtil.SetGridRowStyle(gvRelRecipients)
    End Sub

    Private Sub btnDownloadPackets_ServerClick(sender As Object, e As EventArgs) Handles btnDownloadPackets.ServerClick
        Dim lblSubject As Label = CType(fvRelease.FindControl("lblSubject"), Label)
        Dim strFolder As String = Request.PhysicalApplicationPath & "Export"
        Dim strFileNm As String = "Packets_" & WebUtil.EncodeAsFileName(lblSubject.Text & "_" & ddlFilterOrg.SelectedItem.Text, "_").Replace(" ", "_") & "_" & Today.ToString("yyyyMMdd") & ".csv"

        Try
            Dim dl = gvRelRecipients_GetData(Integer.Parse(RouteData.Values("ReleaseId").ToString), ddlFilterOrg.SelectedValue).ToList()

            ' create folder
            If Not Directory.Exists(strFolder) Then
                Directory.CreateDirectory(strFolder)
            End If

            Dim myExport As New Util.CsvExport

            For Each ro In dl
                myExport.AddRow()
                myExport("Recipient ID") = ro.RecipientId
                myExport("Recipient Name") = ro.RecipientName
                myExport("Recipient Email") = ro.RecipientEmail
                myExport("Org Code") = ro.OrgCode
                myExport("Org Desc") = ro.OrgDesc
                myExport("Class Code") = ro.ClassCode
                myExport("Class Desc") = ro.ClassDesc
                myExport("Exception") = ro.Exception
                myExport("Admin Memo") = ro.AdminMemo
                myExport("Ack DT") = IIf(ro.AckDT.HasValue, ro.AckDT.GetValueOrDefault().ToString("M-d-yyyy h:mm:ss tt"), "")
                myExport("Ack User ID") = ro.AckUserId
                myExport("Ack Client IP") = ro.AckClientIP
                myExport("Ack Auth Type") = ro.AckAuthType
                myExport("Recipient View DT") = IIf(ro.RecipientViewDT.HasValue, ro.RecipientViewDT.GetValueOrDefault().ToString("M-d-yyyy h:mm:ss tt"), "")
                myExport("Recipient View Client IP") = ro.RecipientViewClientIP
                myExport("Create DT") = ro.CreateDT.ToString("M-d-yyyy h:mm:ss tt")
                myExport("Create User") = ro.CreateUser
                myExport("Last Update DT") = ro.LastUpdateDT.ToString("M-d-yyyy h:mm:ss tt")
                myExport("Last Update User") = ro.LastUpdateUser
            Next

            myExport.ExportToFile(strFolder & "\" & strFileNm)

            ' get the file bytes to download to the browser
            Dim fileBytes As Byte()
            fileBytes = System.IO.File.ReadAllBytes(strFolder & "\" & strFileNm)

            ' download this file to the browser
            WebUtil.StreamFileToBrowser(WebUtil.GetMimeTypeByFileName(strFileNm), strFileNm, fileBytes)

        Catch ex As Exception
            lblErrorRecipients.Visible = True
            lblErrorRecipients.Text = WebUtil.ProcessException(ex)
        Finally
            If File.Exists(strFolder & "\" & strFileNm) Then File.Delete(strFolder & "\" & strFileNm)
        End Try
    End Sub

    ' dvPacket
    Protected Sub dvPacket_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvPacket.ItemUpdated
        Try
            If e.Exception IsNot Nothing Then
                Dim strMessage As String = WebUtil.ProcessException(e.Exception)

                ' Display a user-friendly message
                lblError.Visible = True
                lblError.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9111)) '"There was a problem updating the record. "

                Dim _customValidator = New CustomValidator()
                _customValidator.IsValid = False
                _customValidator.ErrorMessage = strMessage
                _customValidator.ValidationGroup = "vgRecipients"
                Page.Validators.Add(_customValidator)

                Dim inner As Exception = e.Exception
                If inner.InnerException IsNot Nothing Then inner = inner.InnerException
                If inner.InnerException IsNot Nothing Then inner = inner.InnerException
                If TypeOf inner Is System.Data.Entity.Core.OptimisticConcurrencyException Then
                    dvPacket.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInEditMode = True
            Else
                ' Select the record inserted
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("ReleaseId", e.Keys.Item("ReleaseId").ToString)
                colKeyValues.Add("RecipientId", e.Keys.Item("RecipientId").ToString)
                WebUtil.SelectGridRowByKeyValue(gvRelRecipients, colKeyValues)
                lblDetailsViewMode.Text = "ReadOnly"

                lblInfoRecipients.Visible = True
                lblInfoRecipients.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(12)) ' Record Updated

                fvRelease.DataBind()
                fvRelStat.DataBind()
            End If

        Catch ex As Exception
            lblErrorRecipients.Visible = True
            lblErrorRecipients.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Protected Sub dvPacket_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dvPacket.ModeChanged
        Select Case dvPacket.CurrentMode
            Case DetailsViewMode.Insert
                If gvRelRecipients.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
                    dvPacket.ChangeMode(DetailsViewMode.ReadOnly)
                    dvPacket.Caption = "Packet Details"
                    dvPacket.Visible = True
                Else
                    gvRelRecipients.SelectedIndex = -1
                    WebUtil.SetGridRowStyle(gvRelRecipients)
                    'dvPacket.Caption = "Add New Packet xx" & lblDetailsViewMode.Text
                    dvPacket.Visible = False
                End If
            Case DetailsViewMode.Edit
                dvPacket.Caption = "Edit Packet"
                dvPacket.Visible = True
            Case DetailsViewMode.ReadOnly
                dvPacket.Caption = "Packet Details"
                dvPacket.Visible = True
        End Select

    End Sub

    Protected Sub odsPacket_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsPacket.Updating
        Dim obj As ReleaseRecipient = e.InputParameters("releaseRecipient")
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()
    End Sub

    ' Rel Notices
    Public Function gvRelNotices_GetData(<RouteData> releaseId As Integer) As IQueryable(Of PolicyTracker.Lib.ReleaseNotice)
        Dim db = New PTEntities
        Dim relNotices = (From a In db.ReleaseNotices Where a.ReleaseId = releaseId Select a Order By a.NoticeDate, a.ReleaseNoticeId)

        Dim cnt = relNotices.Count()
        spnNotCnt.InnerText = cnt.ToString

        Return relNotices
    End Function

    Private Sub gvRelNotices_DataBound(sender As Object, e As EventArgs) Handles gvRelNotices.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        'If gv.Rows.Count > 0 Then
        '    gv.BottomPagerRow.Visible = True
        '     record count
        '    Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
        '    pager.SetTotalRecordCount(ViewState(""))
        'End If

        gv.SelectedIndex = -1
        dvRelNotice.ChangeMode(DetailsViewMode.Insert)
        dvRelNotice_ModeChanged(Nothing, Nothing)
    End Sub

    Private Sub gvRelNotices_SelectedIndexChanged(sender As Object, e As EventArgs) Handles gvRelNotices.SelectedIndexChanged
        dvRelNotice.ChangeMode(DetailsViewMode.ReadOnly)
        dvRelNotice_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvRelNotices)
    End Sub

    Private Sub dvRelNotice_ModeChanged(sender As Object, e As System.EventArgs) Handles dvRelNotice.ModeChanged
        Select Case dvRelNotice.CurrentMode
            Case DetailsViewMode.Insert
                If gvRelNotices.SelectedIndex > -1 And lblDetailsViewModeRelNotice.Text = "ReadOnly" Then
                    dvRelNotice.ChangeMode(DetailsViewMode.ReadOnly)
                    dvRelNotice.Caption = "Notice Details"
                Else
                    gvRelNotices.SelectedIndex = -1
                    WebUtil.SetGridRowStyle(gvRelNotices)
                    dvRelNotice.Caption = "Add New Notice"
                End If
            Case DetailsViewMode.Edit
                dvRelNotice.Caption = "Edit Notice"
            Case DetailsViewMode.ReadOnly
                dvRelNotice.Caption = "Notice Details"
        End Select
    End Sub

    Private Sub dvRelNotice_ItemDeleted(sender As Object, e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvRelNotice.ItemDeleted
        Try
            If e.Exception IsNot Nothing Then
                Dim strMessage As String = WebUtil.ProcessException(e.Exception)

                ' Display a user-friendly message
                lblErrorNotices.Visible = True
                lblErrorNotices.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9112)) '"There was a problem deleting the message. "

                Dim _customValidator = New CustomValidator()
                _customValidator.IsValid = False
                _customValidator.ErrorMessage = strMessage
                _customValidator.ValidationGroup = "vgNoticeDetails"
                Page.Validators.Add(_customValidator)

                Dim inner As Exception = e.Exception
                If inner.InnerException IsNot Nothing Then inner = inner.InnerException
                If inner.InnerException IsNot Nothing Then inner = inner.InnerException
                If TypeOf inner Is System.Data.Entity.Core.OptimisticConcurrencyException Then
                    dvRelNotice.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                dvRelNotice.DataBind()
            Else
                ' Success
                gvRelNotices.SelectedIndex = -1
                gvRelNotices.DataBind()
                dvRelNotice.ChangeMode(DetailsViewMode.Insert)

                lblInfoNotices.Visible = True
                lblInfoNotices.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
            End If

        Catch ex As Exception
            lblErrorNotices.Visible = True
            lblErrorNotices.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Protected Sub dvRelNotice_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvRelNotice.ItemInserted
        Try
            If e.Exception IsNot Nothing Then
                Dim strMessage As String = WebUtil.ProcessException(e.Exception)

                ' Display a user-friendly message
                lblErrorNotices.Visible = True
                lblErrorNotices.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9113)) '"There was a problem adding a new record. "

                Dim _customValidator = New CustomValidator()
                _customValidator.IsValid = False
                _customValidator.ErrorMessage = strMessage
                _customValidator.ValidationGroup = "vgNoticeDetails"
                Page.Validators.Add(_customValidator)

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in insert mode
                e.KeepInInsertMode = True
            Else
                ' Select the record inserted
                'Dim colKeyValues = New Collections.Specialized.NameValueCollection
                'colKeyValues.Add("ReleaseNoticeId", e.Values("ReleaseNoticeId").ToString)
                'WebUtil.SelectGridRowByKeyValue(gvRelNotices, colKeyValues)
                lblDetailsViewModeRelNotice.Text = "ReadOnly"

                lblInfoNotices.Visible = True
                lblInfoNotices.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(11)) ' Record Inserted.
            End If

        Catch ex As Exception
            lblErrorNotices.Visible = True
            lblErrorNotices.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Private Sub dvRelNotice_ItemUpdated(sender As Object, e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvRelNotice.ItemUpdated
        Try
            If e.Exception IsNot Nothing Then
                Dim strMessage As String = WebUtil.ProcessException(e.Exception)

                ' Display a user-friendly message
                lblErrorNotices.Visible = True
                lblErrorNotices.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9111)) '"There was a problem updating the record. "

                Dim _customValidator = New CustomValidator()
                _customValidator.IsValid = False
                _customValidator.ErrorMessage = strMessage
                _customValidator.ValidationGroup = "vgNoticeDetails"
                Page.Validators.Add(_customValidator)

                Dim inner As Exception = e.Exception
                If inner.InnerException IsNot Nothing Then inner = inner.InnerException
                If inner.InnerException IsNot Nothing Then inner = inner.InnerException
                If TypeOf inner Is System.Data.Entity.Core.OptimisticConcurrencyException Then
                    dvRelNotice.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInEditMode = True
            Else
                ' Select the record updated
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("ReleaseNoticeId", e.Keys.Item("ReleaseNoticeId").ToString)
                WebUtil.SelectGridRowByKeyValue(gvRelNotices, colKeyValues)
                lblDetailsViewModeRelNotice.Text = "ReadOnly"

                lblInfoNotices.Visible = True
                lblInfoNotices.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(12)) ' Record Updated
            End If

        Catch ex As Exception
            lblErrorNotices.Visible = True
            lblErrorNotices.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Private Sub dvRelNotice_ItemUpdating(sender As Object, e As DetailsViewUpdateEventArgs) Handles dvRelNotice.ItemUpdating
        If Not Me.IsValid Then e.Cancel = True
    End Sub

    Private Sub odsRelNotice_Inserted(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles odsRelNotice.Inserted
        If e.Exception Is Nothing Then
            ' Select the record inserted
            Dim colKeyValues = New Collections.Specialized.NameValueCollection
            colKeyValues.Add("NewReleaseNoticeId", e.ReturnValue)
            WebUtil.SelectGridRowByKeyValue(gvRelNotices, colKeyValues)
        End If
    End Sub

    Private Sub odsRelNotice_Inserting(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsRelNotice.Inserting
        Dim obj As [Lib].ReleaseNotice = e.InputParameters("releaseNotice")
        obj.CreateUser = CType(Context.User, CustomPrincipal).UserId
        obj.CreateDT = Now()
        obj.LastUpdateUser = CType(Context.User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()

        Dim intReleaseId As Integer = Integer.Parse(RouteData.Values("ReleaseId"))
        obj.ReleaseId = intReleaseId
    End Sub

    Private Sub odsRelNotice_Updating(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsRelNotice.Updating
        Dim obj As [Lib].ReleaseNotice = e.InputParameters("releaseNotice")
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()
    End Sub

    Private Sub dvRelNotice_DataBound(sender As Object, e As EventArgs) Handles dvRelNotice.DataBound
        If dvRelNotice.CurrentMode = DetailsViewMode.ReadOnly Then
            Dim lblRNCount As Label = gvRelNotices.SelectedRow.FindControl("lblRNCount")
            Dim intCount As Integer
            If lblRNCount IsNot Nothing AndAlso Integer.TryParse(lblRNCount.Text, intCount) Then
                If intCount > 0 Then
                    Dim lbtnEdit As LinkButton = dvRelNotice.FindControl("lbtnEdit")
                    lbtnEdit.Visible = False
                    Dim lbtnDelete As LinkButton = dvRelNotice.FindControl("lbtnDelete")
                    lbtnDelete.Visible = False
                End If
            End If
        End If
    End Sub

    Private Sub ddlFilterOrg_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlFilterOrg.SelectedIndexChanged
        fvRelStat.DataBind()
    End Sub

End Class