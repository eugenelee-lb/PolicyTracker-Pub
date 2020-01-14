Imports PolicyTracker.Lib

Public Class Divisions
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Divisions - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Settings"
        Master.ActiveSubMenu = "Divisions"
    End Sub

    Private Sub gvDivisions_DataBound(sender As Object, e As System.EventArgs) Handles gvDivisions.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        gv.SelectedIndex = -1
        'dvDivision.ChangeMode(DetailsViewMode.Insert)
        'dvDivision_ModeChanged(Nothing, Nothing)

    End Sub

    Protected Sub gvDivisions_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvDivisions.SelectedIndexChanged
        'dvDivision.ChangeMode(DetailsViewMode.ReadOnly)
        'dvDivision_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvDivisions)
    End Sub

    Private Sub odsDivisions_Selected(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles odsDivisions.Selected
        ViewState("gvRecordCount") = ""
        If e.ReturnValue IsNot Nothing Then
            If CType(e.ReturnValue, IEnumerable(Of Object)) IsNot Nothing Then
                Dim totalCount As Integer = CType(e.ReturnValue, IEnumerable(Of Object)).Count
                ViewState("gvRecordCount") = "Total rows: " & totalCount.ToString()
            End If
        End If
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        gvDivisions.PageIndex = 0
        gvDivisions.SelectedIndex = -1
        gvDivisions.DataBind()
        'dvDivision.ChangeMode(DetailsViewMode.Insert)
        'dvDivision.Caption = "Add New Division"
        ''dvDivision.DataBind()
    End Sub

    'Protected Sub dvDivision_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvDivision.ItemDeleted
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
    '                dvDivision.DataBind()
    '            End If

    '            ' Indicate that the exception has been handled
    '            e.ExceptionHandled = True
    '            dvDivision.DataBind()
    '        Else
    '            ' Success
    '            gvDivisions.SelectedIndex = -1
    '            gvDivisions.DataBind()
    '            dvDivision.ChangeMode(DetailsViewMode.Insert)

    '            lblInfo.Visible = True
    '            lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
    '        End If

    '    Catch ex As Exception
    '        lblError.Visible = True
    '        lblError.Text = WebUtil.ProcessException(ex)
    '        e.ExceptionHandled = True
    '    End Try

    'End Sub

    'Protected Sub dvDivision_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvDivision.ItemInserted
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
    '            colKeyValues.Add("DivCode", e.Values("DivCode").ToString)
    '            WebUtil.SelectGridRowByKeyValue(gvDivisions, colKeyValues)
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

    'Private Sub dvDivision_ItemInserting(sender As Object, e As System.Web.UI.WebControls.DetailsViewInsertEventArgs) Handles dvDivision.ItemInserting
    '    e.Values("LastUpdateDT") = Now()
    '    e.Values("LastUpdateUser") = CType(User, CustomPrincipal).UserId
    'End Sub

    'Protected Sub dvDivision_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvDivision.ItemUpdated
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
    '                dvDivision.DataBind()
    '            End If

    '            ' Indicate that the exception has been handled
    '            e.ExceptionHandled = True
    '            ' Keep the row in edit mode
    '            e.KeepInEditMode = True
    '        Else
    '            ' Select the record updated
    '            Dim colKeyValues = New Collections.Specialized.NameValueCollection
    '            colKeyValues.Add("DivCode", e.Keys.Item("DivCode").ToString)
    '            WebUtil.SelectGridRowByKeyValue(gvDivisions, colKeyValues)
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

    'Protected Sub dvDivision_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dvDivision.ModeChanged
    '    Select Case dvDivision.CurrentMode
    '        Case DetailsViewMode.Insert
    '            If gvDivisions.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
    '                dvDivision.ChangeMode(DetailsViewMode.ReadOnly)
    '                dvDivision.Caption = "Division Details"
    '            Else
    '                gvDivisions.SelectedIndex = -1
    '                WebUtil.SetGridRowStyle(gvDivisions)
    '                dvDivision.Caption = "Add New Division"
    '            End If
    '        Case DetailsViewMode.Edit
    '            dvDivision.Caption = "Edit Division"
    '        Case DetailsViewMode.ReadOnly
    '            dvDivision.Caption = "Division Details"
    '    End Select

    'End Sub

    'Protected Sub odsDivision_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsDivision.Inserting
    '    Dim obj As Division = e.InputParameters("Division")
    '    obj.CreateUser = CType(User, CustomPrincipal).UserId
    '    obj.CreateDT = Now()
    '    obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
    '    obj.LastUpdateDT = Now()
    '    obj.Disabled = False  'default value
    'End Sub

    'Protected Sub odsDivision_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsDivision.Updating
    '    Dim obj As Division = e.InputParameters("Division")
    '    obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
    '    obj.LastUpdateDT = Now()
    'End Sub

End Class