Imports PolicyTracker.Lib

Public Class Notices
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Notices - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Settings"
        Master.ActiveSubMenu = "Notices"
    End Sub

    Private Sub gvNotices_DataBound(sender As Object, e As System.EventArgs) Handles gvNotices.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        gv.SelectedIndex = -1
        dvNotice.ChangeMode(DetailsViewMode.Insert)
        dvNotice_ModeChanged(Nothing, Nothing)

    End Sub

    Protected Sub gvNotices_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvNotices.SelectedIndexChanged
        dvNotice.ChangeMode(DetailsViewMode.ReadOnly)
        dvNotice_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvNotices)
    End Sub

    Protected Sub dvNotice_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvNotice.ItemDeleted
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
                If TypeOf inner Is System.Data.OptimisticConcurrencyException Then
                    dvNotice.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                dvNotice.DataBind()
            Else
                ' Success
                gvNotices.SelectedIndex = -1
                gvNotices.DataBind()
                dvNotice.ChangeMode(DetailsViewMode.Insert)

                lblInfo.Visible = True
                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Protected Sub dvNotice_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvNotice.ItemInserted
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

    Private Sub dvNotice_ItemInserting(sender As Object, e As System.Web.UI.WebControls.DetailsViewInsertEventArgs) Handles dvNotice.ItemInserting
        e.Values("LastUpdateDT") = Now()
        e.Values("LastUpdateUser") = CType(User, CustomPrincipal).UserId

        Dim datStart As Date = e.Values("StartDate")
        Dim txtStartTime As TextBox = dvNotice.FindControl("txtStartTime")
        datStart = DateTime.Parse(datStart.ToString("MM/dd/yyyy") & " " & txtStartTime.Text)
        e.Values("StartDate") = datStart
        Dim datEnd As Date = e.Values("EndDate")
        Dim txtEndTime As TextBox = dvNotice.FindControl("txtEndTime")
        datEnd = DateTime.Parse(datEnd.ToString("MM/dd/yyyy") & " " & txtEndTime.Text)
        e.Values("EndDate") = datEnd
    End Sub

    Protected Sub dvNotice_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvNotice.ItemUpdated
        System.Threading.Thread.Sleep(2000)
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
                If TypeOf inner Is System.Data.OptimisticConcurrencyException Then
                    dvNotice.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInEditMode = True
            Else
                ' Select the record updated
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("NoticeId", e.Keys.Item("NoticeId").ToString)
                WebUtil.SelectGridRowByKeyValue(gvNotices, colKeyValues)
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

    Private Sub dvNotice_ItemUpdating(sender As Object, e As System.Web.UI.WebControls.DetailsViewUpdateEventArgs) Handles dvNotice.ItemUpdating
        Dim datStart As Date = e.NewValues("StartDate")
        Dim txtStartTime As TextBox = dvNotice.FindControl("txtStartTime")
        datStart = DateTime.Parse(datStart.ToString("MM/dd/yyyy") & " " & txtStartTime.Text)
        e.NewValues("StartDate") = datStart
        Dim datEnd As Date = e.NewValues("EndDate")
        Dim txtEndTime As TextBox = dvNotice.FindControl("txtEndTime")
        datEnd = DateTime.Parse(datEnd.ToString("MM/dd/yyyy") & " " & txtEndTime.Text)
        e.NewValues("EndDate") = datEnd
    End Sub

    Protected Sub dvNotice_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dvNotice.ModeChanged
        Select Case dvNotice.CurrentMode
            Case DetailsViewMode.Insert
                If gvNotices.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
                    dvNotice.ChangeMode(DetailsViewMode.ReadOnly)
                    dvNotice.Caption = "Notice Details"
                Else
                    gvNotices.SelectedIndex = -1
                    WebUtil.SetGridRowStyle(gvNotices)
                    dvNotice.Caption = "Add New Notice"
                End If
            Case DetailsViewMode.Edit
                dvNotice.Caption = "Edit Notice"
            Case DetailsViewMode.ReadOnly
                dvNotice.Caption = "Notice Details"
        End Select

    End Sub

    Private Sub odsNoticeDetails_Inserted(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceStatusEventArgs) Handles odsNoticeDetails.Inserted
        If e.Exception Is Nothing Then
            ' Select the record inserted
            Dim colKeyValues = New Collections.Specialized.NameValueCollection
            colKeyValues.Add("NewNoticeId", e.ReturnValue)
            WebUtil.SelectGridRowByKeyValue(gvNotices, colKeyValues)
        End If
    End Sub

    Protected Sub odsNoticeDetails_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsNoticeDetails.Inserting
        Dim notice As Notice = e.InputParameters("notice")
        notice.CreateUser = CType(User, CustomPrincipal).UserId
        notice.CreateDT = Now()
        notice.LastUpdateUser = CType(User, CustomPrincipal).UserId
        notice.LastUpdateDT = Now()
    End Sub

    Protected Sub odsNoticeDetails_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsNoticeDetails.Updating
        Dim notice As Notice = e.InputParameters("notice")
        notice.LastUpdateUser = CType(User, CustomPrincipal).UserId
        notice.LastUpdateDT = Now()
    End Sub

    Private Sub odsNotices_Selected(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles odsNotices.Selected
        ViewState("gvRecordCount") = ""
        If e.ReturnValue IsNot Nothing Then
            If CType(e.ReturnValue, IEnumerable(Of Object)) IsNot Nothing Then
                Dim totalCount As Integer = CType(e.ReturnValue, IEnumerable(Of Object)).Count
                ViewState("gvRecordCount") = "Total rows: " & totalCount.ToString()
            End If
        End If
    End Sub
End Class