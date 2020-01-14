Imports PolicyTracker.Lib

Public Class Classifications
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Classifications - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Settings"
        Master.ActiveSubMenu = "Classifications"
    End Sub

    Private Sub gvClassifications_DataBound(sender As Object, e As System.EventArgs) Handles gvClassifications.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        gv.SelectedIndex = -1
        'dvClassification.ChangeMode(DetailsViewMode.Insert)
        'dvClassification_ModeChanged(Nothing, Nothing)

    End Sub

    Protected Sub gvClassifications_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvClassifications.SelectedIndexChanged
        'dvClassification.ChangeMode(DetailsViewMode.ReadOnly)
        'dvClassification_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvClassifications)
    End Sub

    Private Sub odsClassifications_Selected(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles odsClassifications.Selected
        ViewState("gvRecordCount") = ""
        If e.ReturnValue IsNot Nothing Then
            If CType(e.ReturnValue, IEnumerable(Of Object)) IsNot Nothing Then
                Dim totalCount As Integer = CType(e.ReturnValue, IEnumerable(Of Object)).Count
                ViewState("gvRecordCount") = "Total rows: " & totalCount.ToString()
            End If
        End If
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        gvClassifications.PageIndex = 0
        gvClassifications.SelectedIndex = -1
        gvClassifications.DataBind()
        'dvClassification.ChangeMode(DetailsViewMode.Insert)
        'dvClassification.Caption = "Add New Classification"
        ''dvClassification.DataBind()
    End Sub

    'Protected Sub dvClassification_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvClassification.ItemDeleted
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
    '                dvClassification.DataBind()
    '            End If

    '            ' Indicate that the exception has been handled
    '            e.ExceptionHandled = True
    '            dvClassification.DataBind()
    '        Else
    '            ' Success
    '            gvClassifications.SelectedIndex = -1
    '            gvClassifications.DataBind()
    '            dvClassification.ChangeMode(DetailsViewMode.Insert)

    '            lblInfo.Visible = True
    '            lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
    '        End If

    '    Catch ex As Exception
    '        lblError.Visible = True
    '        lblError.Text = WebUtil.ProcessException(ex)
    '        e.ExceptionHandled = True
    '    End Try

    'End Sub

    'Protected Sub dvClassification_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvClassification.ItemInserted
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
    '            colKeyValues.Add("ClassCode", e.Values("ClassCode").ToString)
    '            WebUtil.SelectGridRowByKeyValue(gvClassifications, colKeyValues)
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

    'Private Sub dvClassification_ItemInserting(sender As Object, e As System.Web.UI.WebControls.DetailsViewInsertEventArgs) Handles dvClassification.ItemInserting
    '    e.Values("LastUpdateDT") = Now()
    '    e.Values("LastUpdateUser") = CType(User, CustomPrincipal).UserId
    'End Sub

    'Protected Sub dvClassification_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvClassification.ItemUpdated
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
    '                dvClassification.DataBind()
    '            End If

    '            ' Indicate that the exception has been handled
    '            e.ExceptionHandled = True
    '            ' Keep the row in edit mode
    '            e.KeepInEditMode = True
    '        Else
    '            ' Select the record updated
    '            Dim colKeyValues = New Collections.Specialized.NameValueCollection
    '            colKeyValues.Add("ClassCode", e.Keys.Item("ClassCode").ToString)
    '            WebUtil.SelectGridRowByKeyValue(gvClassifications, colKeyValues)
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

    'Protected Sub dvClassification_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dvClassification.ModeChanged
    '    Select Case dvClassification.CurrentMode
    '        Case DetailsViewMode.Insert
    '            If gvClassifications.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
    '                dvClassification.ChangeMode(DetailsViewMode.ReadOnly)
    '                dvClassification.Caption = "Classification Details"
    '            Else
    '                gvClassifications.SelectedIndex = -1
    '                WebUtil.SetGridRowStyle(gvClassifications)
    '                dvClassification.Caption = "Add New Classification"
    '            End If
    '        Case DetailsViewMode.Edit
    '            dvClassification.Caption = "Edit Classification"
    '        Case DetailsViewMode.ReadOnly
    '            dvClassification.Caption = "Classification Details"
    '    End Select

    'End Sub

    'Protected Sub odsClassification_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsClassification.Inserting
    '    Dim obj As Classification = e.InputParameters("cls")
    '    obj.CreateUser = CType(User, CustomPrincipal).UserId
    '    obj.CreateDT = Now()
    '    obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
    '    obj.LastUpdateDT = Now()
    '    obj.Disabled = False  'default value
    'End Sub

    'Protected Sub odsClassification_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsClassification.Updating
    '    Dim obj As Classification = e.InputParameters("cls")
    '    obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
    '    obj.LastUpdateDT = Now()
    'End Sub

End Class