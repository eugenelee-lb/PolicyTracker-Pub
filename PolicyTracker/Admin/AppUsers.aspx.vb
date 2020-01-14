Imports PolicyTracker.Lib

Public Class AppUsers
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Users - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Settings"
        Master.ActiveSubMenu = "AppUsers"
    End Sub

    Private Sub gvUsers_DataBound(sender As Object, e As System.EventArgs) Handles gvUsers.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        gv.SelectedIndex = -1
        dvUser.ChangeMode(DetailsViewMode.Insert)
        dvUser_ModeChanged(Nothing, Nothing)

    End Sub

    Private Sub gvUsers_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles gvUsers.SelectedIndexChanged
        Try
            dvUser.ChangeMode(DetailsViewMode.ReadOnly)
            dvUser.Caption = "User Details"

            WebUtil.SetGridRowStyle(gvUsers)

            ' GroupMember
            'If gvUsers.SelectedIndex > -1 Then
            '    BindGroups()
            'Else
            '    panGroups.Visible = False
            'End If
        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub

    Private Sub dvUser_ItemDeleted(sender As Object, e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvUser.ItemDeleted
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
                    dvUser.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                dvUser.DataBind()
            Else
                ' Success
                gvUsers.SelectedIndex = -1
                gvUsers.DataBind()
                dvUser.ChangeMode(DetailsViewMode.Insert)

                lblInfo.Visible = True
                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Protected Sub dvUser_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvUser.ItemInserted
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
                ' Keep the row in insert mode
                e.KeepInInsertMode = True
            Else
                ' Select the record inserted
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("UserId", e.Values("UserId").ToString)
                WebUtil.SelectGridRowByKeyValue(gvUsers, colKeyValues)
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

    Private Sub dvUser_ItemUpdated(sender As Object, e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvUser.ItemUpdated
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
                    dvUser.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInEditMode = True
            Else
                ' Select the record updated
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("UserId", e.Keys.Item("UserId").ToString)
                WebUtil.SelectGridRowByKeyValue(gvUsers, colKeyValues)
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

    Private Sub dvUser_ModeChanged(sender As Object, e As System.EventArgs) Handles dvUser.ModeChanged
        Select Case dvUser.CurrentMode
            Case DetailsViewMode.Insert
                If gvUsers.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
                    dvUser.ChangeMode(DetailsViewMode.ReadOnly)
                    dvUser.Caption = "User Details"
                Else
                    gvUsers.SelectedIndex = -1
                    WebUtil.SetGridRowStyle(gvUsers)
                    dvUser.Caption = "Add New User"

                    ' hide controls
                    'panGroups.Visible = False
                End If
            Case DetailsViewMode.Edit
                dvUser.Caption = "Edit User"
            Case DetailsViewMode.ReadOnly
                dvUser.Caption = "User Details"
        End Select

    End Sub

    Private Sub odsUserDetails_Inserting(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsUserDetails.Inserting
        Dim appUser As AppUser = e.InputParameters("appUser")
        appUser.CreateUser = CType(Context.User, CustomPrincipal).UserId
        appUser.CreateDT = Now()
        appUser.LastUpdateUser = CType(Context.User, CustomPrincipal).UserId
        appUser.LastUpdateDT = Now()
    End Sub

    Private Sub odsUserDetails_Updating(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsUserDetails.Updating
        Dim obj As AppUser = e.InputParameters("appUser")
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()
    End Sub

    Private Sub odsUsersGrid_Selected(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles odsUsersGrid.Selected
        ViewState("gvRecordCount") = ""
        If e.ReturnValue IsNot Nothing Then
            If CType(e.ReturnValue, IEnumerable(Of Object)) IsNot Nothing Then
                Dim totalCount As Integer = CType(e.ReturnValue, IEnumerable(Of Object)).Count
                ViewState("gvRecordCount") = "Total rows: " & totalCount.ToString()
            End If
        End If
    End Sub
End Class