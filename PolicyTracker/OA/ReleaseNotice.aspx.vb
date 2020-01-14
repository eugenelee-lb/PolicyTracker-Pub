Imports PolicyTracker.Lib
Imports System.Web.ModelBinding

Public Class ReleaseNotice
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Release Notice - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Releases"
        Master.ActiveSubMenu = ""

        If Not Page.IsPostBack Then
            ddlFilterOrg.DataBind()
        End If
    End Sub

    Public Function fvRelNotice_GetItem(<RouteData> releaseNoticeId As Integer) As PolicyTracker.Lib.ReleaseNotice
        Dim db = New PTEntities
        Dim relNotice = (From a In db.ReleaseNotices Where a.ReleaseNoticeId = releaseNoticeId Select a).FirstOrDefault()
        If relNotice Is Nothing Then
            btnCreateMessages.Visible = False
        Else
            lnkBackToRelease.HRef = "~/OA/Release/" & relNotice.ReleaseId.ToString
        End If

        btnCreateMessages.Visible = False
        btnSendMessages.Visible = False
        If ConfigurationManager.AppSettings("DevUserID").ToLower.IndexOf(CType(Context.User, CustomPrincipal).UserId.ToLower) >= 0 Then
            btnCreateMessages.Visible = True
            btnSendMessages.Visible = True
            btnCreateMessages.Enabled = False
            btnSendMessages.Enabled = False
            If Not relNotice.CompleteDT.HasValue Then
                btnCreateMessages.Enabled = True
                If relNotice.RecipientNotices.Count > 0 Then
                    btnSendMessages.Enabled = True
                End If
            End If
        End If

        Return relNotice
    End Function

    Private Sub ddlFilterOrg_DataBound(sender As Object, e As EventArgs) Handles ddlFilterOrg.DataBound
        If CType(User, CustomPrincipal).IsInRole("SA") Then
            ddlFilterOrg.Items.Insert(0, New ListItem("-- All --", ""))
        End If
    End Sub

    Protected Sub odsOrg_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs)
        e.InputParameters("userRole") = CType(Page.User, CustomPrincipal).Role
        e.InputParameters("userId") = CType(Page.User, CustomPrincipal).UserId
    End Sub

    Private Sub btnCreateMessages_Click(sender As Object, e As System.EventArgs) Handles btnCreateMessages.Click
        Try
            Dim intReleaseNoticeId As Integer = Integer.Parse(RouteData.Values("ReleaseNoticeId"))

            Dim bl As PTBL = New PTBL
            If bl.CreateEmailNotices(intReleaseNoticeId) > 0 Then
                lblInfo.Text = "Email Messages have been created."
                lblInfo.Visible = True
            End If
            fvRelNotice.DataBind()
            gvRecipientNotices.DataBind()
            dvOANotice.DataBind()

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = "There was a problem in creating Email Messages."

            Dim strMessage As String = WebUtil.ProcessException(ex)
            Dim _customValidator = New CustomValidator()
            _customValidator.IsValid = False
            _customValidator.ErrorMessage = strMessage
            _customValidator.ValidationGroup = "vgMain"
            Page.Validators.Add(_customValidator)
        End Try
    End Sub

    Private Sub btnSendMessages_Click(sender As Object, e As EventArgs) Handles btnSendMessages.Click
        Try
            Dim intReleaseNoticeId As Integer = Integer.Parse(RouteData.Values("ReleaseNoticeId"))

            Dim bl As PTBL = New PTBL
            If bl.SendReleaseNotice(intReleaseNoticeId) = True Then
                lblInfo.Text = "Email Messages have been sent."
                lblInfo.Visible = True
            End If
            fvRelNotice.DataBind()
            gvRecipientNotices.DataBind()
            dvOANotice.DataBind()

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = "There was a problem in sending Email Messages."

            Dim strMessage As String = WebUtil.ProcessException(ex)
            Dim _customValidator = New CustomValidator()
            _customValidator.IsValid = False
            _customValidator.ErrorMessage = strMessage
            _customValidator.ValidationGroup = "vgMain"
            Page.Validators.Add(_customValidator)
        End Try
    End Sub

    ' RecipientNotices
    Public Function gvRecipientNotices_GetData(<RouteData> releaseNoticeId As Integer, _
                                           <Control> ddlFilterOrg As String) As IQueryable(Of PolicyTracker.Lib.RecipientNotice)
        Dim db = New PTEntities
        Dim qry = (From a In db.RecipientNotices Where a.ReleaseNoticeId = releaseNoticeId Select a)
        If ddlFilterOrg <> "" Then
            qry = qry.Where(Function(a) (From p In db.ReleaseRecipients
                                         Where p.ReleaseId = a.ReleaseNotice.ReleaseId _
                                         And p.RecipientId = a.RecipientId _
                                         And p.OrgCode.StartsWith(ddlFilterOrg)).Any())
        End If

        Dim cnt = qry.Count()
        spnRecNotCnt.InnerText = cnt.ToString

        Return qry
    End Function

    Private Sub gvRecipientNotices_DataBound(sender As Object, e As EventArgs) Handles gvRecipientNotices.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then gv.BottomPagerRow.Visible = True

        gv.SelectedIndex = -1
        dvRecipNotice.Visible = False
        'dvRecipNotice.ChangeMode(DetailsViewMode.Insert)
        'dvRecipNotice_ModeChanged(Nothing, Nothing)
    End Sub

    Protected Sub gvRecipientNotices_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvRecipientNotices.SelectedIndexChanged
        dvRecipNotice.Visible = True
        dvRecipNotice.ChangeMode(DetailsViewMode.ReadOnly)
        'dvRecipNotice_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvRecipientNotices)
    End Sub

    Private Sub dvRecipNotice_DataBound(sender As Object, e As System.EventArgs) Handles dvRecipNotice.DataBound
        If dvRecipNotice.DataItemCount > 0 AndAlso dvRecipNotice.CurrentMode = DetailsViewMode.ReadOnly Then
            Dim lbtnSend As LinkButton = dvRecipNotice.FindControl("lbtnSend")
            Dim lblSentDT As Label = dvRecipNotice.FindControl("lblSentDT")
            If lblSentDT.Text = "" Then
                lbtnSend.Visible = True
            Else
                lbtnSend.Visible = False
            End If
        End If
    End Sub

    Private Sub dvRecipNotice_ItemCommand(sender As Object, e As System.Web.UI.WebControls.DetailsViewCommandEventArgs) Handles dvRecipNotice.ItemCommand
        If e.CommandName = "Send" Then
            Try
                Dim intReleaseNoticeId As Integer = Integer.Parse(RouteData.Values("ReleaseNoticeId"))
                Dim strRecipientId As String = gvRecipientNotices.SelectedDataKey.Values("RecipientId")

                Dim bl As New PTBL
                Dim repo As New PTRepository
                Dim rn As RecipientNotice = repo.GetRecipientNoticeById(intReleaseNoticeId, strRecipientId)
                'Dim el1 As EmailLog = bl.GetEmailLogById(intEmailLogID)
                If bl.SendNoticeToRecip(rn) Then
                    ' Select the record updated
                    Dim colKeyValues = New Collections.Specialized.NameValueCollection
                    colKeyValues.Add("ReleaseNoticeId", intReleaseNoticeId.ToString)
                    colKeyValues.Add("RecipientId", strRecipientId)
                    WebUtil.SelectGridRowByKeyValue(gvRecipientNotices, colKeyValues)
                    hidRecipNoticeMode.Value = "ReadOnly"
                    gvRecipientNotices_SelectedIndexChanged(Nothing, Nothing)
                    dvRecipNotice.DataBind()

                    lblInfoRecipN.Visible = True
                    lblInfoRecipN.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(1041)) ' E-Mail message has been sent.
                End If

            Catch ex As Exception
                lblErrorRecipN.Visible = True
                lblErrorRecipN.Text = WebUtil.ProcessException(ex)

            End Try
        End If
    End Sub

    ' OrgAdminNotice
    Private Sub dvOANotice_DataBound(sender As Object, e As EventArgs) Handles dvOANotice.DataBound
        If dvOANotice.DataItemCount > 0 AndAlso dvOANotice.CurrentMode = DetailsViewMode.ReadOnly Then
            Dim lbtnSend As LinkButton = dvOANotice.FindControl("lbtnSend")
            Dim lblSentDT As Label = dvOANotice.FindControl("lblSentDT")
            If lblSentDT.Text = "" Then
                lbtnSend.Visible = True
            Else
                lbtnSend.Visible = False
            End If
        End If
    End Sub

    Private Sub dvOANotice_ItemCommand(sender As Object, e As DetailsViewCommandEventArgs) Handles dvOANotice.ItemCommand
        If e.CommandName = "Send" Then
            Try
                Dim intReleaseNoticeId As Integer = Integer.Parse(RouteData.Values("ReleaseNoticeId"))

                Dim bl As New PTBL
                Dim repo As New PTRepository
                Dim oan As OrgAdminNotice = repo.GetOrgAdminNoticeById(intReleaseNoticeId)
                'Dim el1 As EmailLog = bl.GetEmailLogById(intEmailLogID)
                If bl.SendNoticeToOrgAdmin(oan) Then
                    dvOANotice.DataBind()
                    lblInfoOAN.Visible = True
                    lblInfoOAN.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(1041)) ' E-Mail message has been sent.
                End If

            Catch ex As Exception
                lblErrorOAN.Visible = True
                lblErrorOAN.Text = WebUtil.ProcessException(ex)

            End Try
        End If
    End Sub
End Class