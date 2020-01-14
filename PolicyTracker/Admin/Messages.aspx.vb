Imports PolicyTracker.Lib

Public Class Messages
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Messages - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Settings"
        Master.ActiveSubMenu = "Messages"
    End Sub

    Private Sub gvMessages_DataBound(sender As Object, e As System.EventArgs) Handles gvMessages.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        gv.SelectedIndex = -1
        dvMessage.ChangeMode(DetailsViewMode.Insert)
        dvMessage_ModeChanged(Nothing, Nothing)

    End Sub

    Protected Sub gvMessages_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvMessages.SelectedIndexChanged
        dvMessage.ChangeMode(DetailsViewMode.ReadOnly)
        dvMessage.DataBind()
        dvMessage_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvMessages)
    End Sub

    Protected Sub dvMessage_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvMessage.ItemDeleted
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
                    dvMessage.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                dvMessage.DataBind()
            Else
                ' Success
                gvMessages.SelectedIndex = -1
                gvMessages.DataBind()
                dvMessage.ChangeMode(DetailsViewMode.Insert)

                lblInfo.Visible = True
                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Protected Sub dvMessage_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvMessage.ItemInserted
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
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("MsgNo", e.Values("MsgNo").ToString)
                WebUtil.SelectGridRowByKeyValue(gvMessages, colKeyValues)
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

    Private Sub dvMessage_ItemInserting(sender As Object, e As System.Web.UI.WebControls.DetailsViewInsertEventArgs) Handles dvMessage.ItemInserting
        e.Values("LastUpdateDT") = Now()
        e.Values("LastUpdateUser") = CType(User, CustomPrincipal).UserId
    End Sub

    Protected Sub dvMessage_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvMessage.ItemUpdated
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
                    dvMessage.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInEditMode = True
            Else
                ' Select the record updated
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("MsgNo", e.Keys.Item("MsgNo").ToString)
                WebUtil.SelectGridRowByKeyValue(gvMessages, colKeyValues)
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

    Protected Sub dvMessage_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dvMessage.ModeChanged
        Select Case dvMessage.CurrentMode
            Case DetailsViewMode.Insert
                If gvMessages.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
                    dvMessage.ChangeMode(DetailsViewMode.ReadOnly)
                    dvMessage.Caption = "Message Details"
                Else
                    gvMessages.SelectedIndex = -1
                    WebUtil.SetGridRowStyle(gvMessages)
                    dvMessage.Caption = "Add New Message"
                End If
            Case DetailsViewMode.Edit
                dvMessage.Caption = "Edit Message"
            Case DetailsViewMode.ReadOnly
                dvMessage.Caption = "Message Details"
        End Select

    End Sub

    Protected Sub odsMessageDetails_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsMessageDetails.Inserting
        Dim msg As Message = e.InputParameters("message")
        msg.CreateUser = CType(User, CustomPrincipal).UserId
        msg.CreateDT = Now()
        msg.LastUpdateUser = CType(User, CustomPrincipal).UserId
        msg.LastUpdateDT = Now()
    End Sub

    Protected Sub odsMessageDetails_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsMessageDetails.Updating
        Dim msg As Message = e.InputParameters("message")
        msg.LastUpdateUser = CType(User, CustomPrincipal).UserId
        msg.LastUpdateDT = Now()
    End Sub

    Protected Function EvalMsgText() As String
        Return WebUtil.HtmlMsgEncode(Eval("MsgText"))
    End Function

    Private Sub btnSearch_Click(sender As Object, e As System.EventArgs) Handles btnSearch.Click
        gvMessages.PageIndex = 0
        gvMessages.SelectedIndex = -1
        gvMessages.DataBind()
        dvMessage.ChangeMode(DetailsViewMode.Insert)
        dvMessage.Caption = "Add New Message"
    End Sub

    Private Sub odsMessages_Selected(sender As Object, e As Web.UI.WebControls.ObjectDataSourceStatusEventArgs) Handles odsMessages.Selected
        ViewState("gvRecordCount") = ""
        If e.ReturnValue IsNot Nothing Then
            If CType(e.ReturnValue, IEnumerable(Of Object)) IsNot Nothing Then
                Dim totalCount As Integer = CType(e.ReturnValue, IEnumerable(Of Object)).Count
                ViewState("gvRecordCount") = "Total rows: " & totalCount.ToString()
            End If
        End If
    End Sub

    ' Model binding methods
    Public Function GetMessageByNo() As Message
        Using db = New PolicyTracker.Lib.PTEntities()
            Dim item = db.Messages.Find(Integer.Parse(gvMessages.SelectedValue.ToString()))
            Return item
        End Using
    End Function

    Public Sub UpdateMessage(msgNo As Integer)
        Using db = New PolicyTracker.Lib.PTEntities()
            Dim item As Message = Nothing
            item = db.Messages.Find(msgNo)
            If item Is Nothing Then
                ModelState.AddModelError("", String.Format("Message with number {0} was not found", msgNo))
                Exit Sub
            End If

            TryUpdateModel(item)
            If ModelState.IsValid Then
                item.LastUpdateDT = Now()
                item.LastUpdateUser = CType(User, CustomPrincipal).UserId
                db.SaveChanges()
            Else
                Throw New ApplicationException("Input data is not valid.")
            End If
        End Using
    End Sub

    Public Sub DeleteMessage(msgNo As Integer)
        Using db = New PolicyTracker.Lib.PTEntities()
            Dim item As Message = Nothing
            item = db.Messages.Find(msgNo)
            If item Is Nothing Then
                ModelState.AddModelError("", String.Format("Message with number {0} was not found", msgNo))
                Exit Sub
            End If

            db.Entry(item).State = EntityState.Deleted
            Try
                db.SaveChanges()
            Catch ex As DBConcurrencyException
                ModelState.AddModelError("", String.Format("Message with number {0} no longer exists in the database", msgNo))
            End Try
        End Using
    End Sub

    Public Sub InsertMessage()
        Dim item As Message = New Message()

        TryUpdateModel(item)
        If ModelState.IsValid Then
            Using db = New PolicyTracker.Lib.PTEntities()
                item.CreateDT = Now()
                item.CreateUser = CType(User, CustomPrincipal).UserId
                item.LastUpdateDT = Now()
                item.LastUpdateUser = CType(User, CustomPrincipal).UserId
                db.Messages.Add(item)
                db.SaveChanges()
            End Using
        Else
            Throw New ApplicationException("Input data is not valid.")
        End If
    End Sub

End Class