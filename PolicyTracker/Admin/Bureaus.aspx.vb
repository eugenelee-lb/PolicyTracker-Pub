Imports PolicyTracker.Lib

Public Class Bureaus
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Bureaus - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Settings"
        Master.ActiveSubMenu = "Bureaus"
    End Sub

    Private Sub gvBureaus_DataBound(sender As Object, e As System.EventArgs) Handles gvBureaus.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        gv.SelectedIndex = -1
        'dvBureau.ChangeMode(DetailsViewMode.Insert)
        'dvBureau_ModeChanged(Nothing, Nothing)

    End Sub

    Protected Sub gvBureaus_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvBureaus.SelectedIndexChanged
        'dvBureau.ChangeMode(DetailsViewMode.ReadOnly)
        'dvBureau_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvBureaus)
    End Sub

    Private Sub odsBureaus_Selected(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles odsBureaus.Selected
        ViewState("gvRecordCount") = ""
        If e.ReturnValue IsNot Nothing Then
            If CType(e.ReturnValue, IEnumerable(Of Object)) IsNot Nothing Then
                Dim totalCount As Integer = CType(e.ReturnValue, IEnumerable(Of Object)).Count
                ViewState("gvRecordCount") = "Total rows: " & totalCount.ToString()
            End If
        End If
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        gvBureaus.PageIndex = 0
        gvBureaus.SelectedIndex = -1
        gvBureaus.DataBind()
        'dvBureau.ChangeMode(DetailsViewMode.Insert)
        'dvBureau.Caption = "Add New Bureau"
        ''dvBureau.DataBind()
    End Sub

    'Protected Sub dvBureau_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvBureau.ItemDeleted
    '    Try
    '        If e.Exception IsNot Nothing Then
    '            Dim strMessage As String = WebUtil.ProcessException(e.Exception)

    '            ' Display a user-friendly message
    '            lblError.Visible = True
    '            lblError.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9112)) '"There was a problem deleting the message. "

    '            Dim _customValidator = New CustomValidator()
    '            _customValidator.IsValid = False
    '            _customValidator.ErrorMessage = strMessage
    '            _customValidator.ValidationGroup = "vgDetails"
    '            Page.Validators.Add(_customValidator)

    '            Dim inner As Exception = e.Exception
    '            If inner.InnerException IsNot Nothing Then inner = inner.InnerException
    '            If inner.InnerException IsNot Nothing Then inner = inner.InnerException
    '            If TypeOf inner Is System.Data.OptimisticConcurrencyException Then
    '                dvBureau.DataBind()
    '            End If

    '            ' Indicate that the exception has been handled
    '            e.ExceptionHandled = True
    '            dvBureau.DataBind()
    '        Else
    '            ' Success
    '            gvBureaus.SelectedIndex = -1
    '            gvBureaus.DataBind()
    '            dvBureau.ChangeMode(DetailsViewMode.Insert)

    '            lblInfo.Visible = True
    '            lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
    '        End If

    '    Catch ex As Exception
    '        lblError.Visible = True
    '        lblError.Text = WebUtil.ProcessException(ex)
    '        e.ExceptionHandled = True
    '    End Try

    'End Sub

    'Protected Sub dvBureau_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvBureau.ItemInserted
    '    Try
    '        If e.Exception IsNot Nothing Then
    '            Dim strMessage As String = WebUtil.ProcessException(e.Exception)

    '            ' Display a user-friendly message
    '            lblError.Visible = True
    '            lblError.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9113)) '"There was a problem adding a new record. "

    '            Dim _customValidator = New CustomValidator()
    '            _customValidator.IsValid = False
    '            _customValidator.ErrorMessage = strMessage
    '            _customValidator.ValidationGroup = "vgDetails"
    '            Page.Validators.Add(_customValidator)

    '            ' Indicate that the exception has been handled
    '            e.ExceptionHandled = True
    '            ' Keep the row in edit mode
    '            e.KeepInInsertMode = True
    '        Else
    '            ' Select the record inserted
    '            Dim colKeyValues = New Collections.Specialized.NameValueCollection
    '            colKeyValues.Add("BurCode", e.Values("BurCode").ToString)
    '            WebUtil.SelectGridRowByKeyValue(gvBureaus, colKeyValues)
    '            lblDetailsViewMode.Text = "ReadOnly"

    '            lblInfo.Visible = True
    '            lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(11)) ' Record Inserted.
    '        End If

    '    Catch ex As Exception
    '        lblError.Visible = True
    '        lblError.Text = WebUtil.ProcessException(ex)
    '        e.ExceptionHandled = True
    '    End Try

    'End Sub

    'Private Sub dvBureau_ItemInserting(sender As Object, e As System.Web.UI.WebControls.DetailsViewInsertEventArgs) Handles dvBureau.ItemInserting
    '    e.Values("LastUpdateDT") = Now()
    '    e.Values("LastUpdateUser") = CType(User, CustomPrincipal).UserId
    'End Sub

    'Protected Sub dvBureau_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvBureau.ItemUpdated
    '    Try
    '        If e.Exception IsNot Nothing Then
    '            Dim strMessage As String = WebUtil.ProcessException(e.Exception)

    '            ' Display a user-friendly message
    '            lblError.Visible = True
    '            lblError.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(-9111)) '"There was a problem updating the record. "

    '            Dim _customValidator = New CustomValidator()
    '            _customValidator.IsValid = False
    '            _customValidator.ErrorMessage = strMessage
    '            _customValidator.ValidationGroup = "vgDetails"
    '            Page.Validators.Add(_customValidator)

    '            Dim inner As Exception = e.Exception
    '            If inner.InnerException IsNot Nothing Then inner = inner.InnerException
    '            If inner.InnerException IsNot Nothing Then inner = inner.InnerException
    '            If TypeOf inner Is System.Data.OptimisticConcurrencyException Then
    '                dvBureau.DataBind()
    '            End If

    '            ' Indicate that the exception has been handled
    '            e.ExceptionHandled = True
    '            ' Keep the row in edit mode
    '            e.KeepInEditMode = True
    '        Else
    '            ' Select the record updated
    '            Dim colKeyValues = New Collections.Specialized.NameValueCollection
    '            colKeyValues.Add("BurCode", e.Keys.Item("BurCode").ToString)
    '            WebUtil.SelectGridRowByKeyValue(gvBureaus, colKeyValues)
    '            lblDetailsViewMode.Text = "ReadOnly"

    '            lblInfo.Visible = True
    '            lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(12)) ' Record Updated
    '        End If

    '    Catch ex As Exception
    '        lblError.Visible = True
    '        lblError.Text = WebUtil.ProcessException(ex)
    '        e.ExceptionHandled = True
    '    End Try

    'End Sub

    'Protected Sub dvBureau_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dvBureau.ModeChanged
    '    Select Case dvBureau.CurrentMode
    '        Case DetailsViewMode.Insert
    '            If gvBureaus.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
    '                dvBureau.ChangeMode(DetailsViewMode.ReadOnly)
    '                dvBureau.Caption = "Bureau Details"
    '            Else
    '                gvBureaus.SelectedIndex = -1
    '                WebUtil.SetGridRowStyle(gvBureaus)
    '                dvBureau.Caption = "Add New Bureau"
    '            End If
    '        Case DetailsViewMode.Edit
    '            dvBureau.Caption = "Edit Bureau"
    '        Case DetailsViewMode.ReadOnly
    '            dvBureau.Caption = "Bureau Details"
    '    End Select

    'End Sub

    'Protected Sub odsBureau_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsBureau.Inserting
    '    Dim obj As Bureau = e.InputParameters("Bureau")
    '    obj.CreateUser = CType(User, CustomPrincipal).UserId
    '    obj.CreateDT = Now()
    '    obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
    '    obj.LastUpdateDT = Now()
    '    obj.Disabled = False  'default value
    'End Sub

    'Protected Sub odsBureau_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsBureau.Updating
    '    Dim obj As Bureau = e.InputParameters("Bureau")
    '    obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
    '    obj.LastUpdateDT = Now()
    'End Sub

End Class