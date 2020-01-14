Imports PolicyTracker.Lib

Public Class CommonCodes
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Common Codes - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Settings"
        Master.ActiveSubMenu = "CommonCodes"
    End Sub

    Private Sub gvCommonCodes_DataBound(sender As Object, e As System.EventArgs) Handles gvCommonCodes.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        gv.SelectedIndex = -1
        dvCommonCode.ChangeMode(DetailsViewMode.Insert)
        dvCommonCode_ModeChanged(Nothing, Nothing)

    End Sub

    Protected Sub gvCommonCodes_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvCommonCodes.SelectedIndexChanged
        dvCommonCode.ChangeMode(DetailsViewMode.ReadOnly)
        dvCommonCode_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvCommonCodes)
    End Sub

    Private Sub dvCommonCode_DataBound(sender As Object, e As System.EventArgs) Handles dvCommonCode.DataBound
        Dim ddlCMCatg As DropDownList = dvCommonCode.FindControl("ddlCMCatg")
        ddlCMCatg.SelectedValue = dropCatg.SelectedValue
    End Sub

    Protected Sub dvCommonCode_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvCommonCode.ItemDeleted
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
                    dvCommonCode.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                dvCommonCode.DataBind()
            Else
                ' Refresh categories
                If dropCatg.SelectedValue = "00" Then dropCatg.DataBind()

                gvCommonCodes.SelectedIndex = -1
                gvCommonCodes.DataBind()
                dvCommonCode.ChangeMode(DetailsViewMode.Insert)

                lblInfo.Visible = True
                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Protected Sub dvCommonCode_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvCommonCode.ItemInserted
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
                'colKeyValues.Add("CMId", e.Values("CMId").ToString)
                'WebUtil.SelectGridRowByKeyValue(gvCommonCodes, colKeyValues)
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

    Private Sub dvCommonCode_ItemInserting(sender As Object, e As System.Web.UI.WebControls.DetailsViewInsertEventArgs) Handles dvCommonCode.ItemInserting
        e.Values("LastUpdateDT") = Now()
        e.Values("LastUpdateUser") = CType(User, CustomPrincipal).UserId
    End Sub

    Protected Sub dvCommonCode_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvCommonCode.ItemUpdated
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
                    dvCommonCode.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInEditMode = True
            Else
                ' Refresh categories
                If dropCatg.SelectedValue = "00" Then dropCatg.DataBind()

                ' Select the record updated
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("CmId", e.Keys.Item("CmId").ToString)
                WebUtil.SelectGridRowByKeyValue(gvCommonCodes, colKeyValues)
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

    Protected Sub dvCommonCode_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dvCommonCode.ModeChanged
        Select Case dvCommonCode.CurrentMode
            Case DetailsViewMode.Insert
                If gvCommonCodes.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
                    dvCommonCode.ChangeMode(DetailsViewMode.ReadOnly)
                    dvCommonCode.Caption = "Common Code Details"
                Else
                    gvCommonCodes.SelectedIndex = -1
                    WebUtil.SetGridRowStyle(gvCommonCodes)
                    dvCommonCode.Caption = "Add New Common Code"

                    Dim ddlCMCatg As DropDownList = dvCommonCode.FindControl("ddlCMCatg")
                    ddlCMCatg.SelectedValue = dropCatg.SelectedValue
                End If
            Case DetailsViewMode.Edit
                dvCommonCode.Caption = "Edit Common Code"
            Case DetailsViewMode.ReadOnly
                dvCommonCode.Caption = "Common Code Details"
        End Select

    End Sub

    Private Sub odsCommonCodeDetails_Inserted(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceStatusEventArgs) Handles odsCommonCodeDetails.Inserted
        If e.Exception Is Nothing Then
            ' Refresh categories
            If dropCatg.SelectedValue = "00" Then dropCatg.DataBind()

            ' Select the record inserted
            Dim colKeyValues = New Collections.Specialized.NameValueCollection
            colKeyValues.Add("NewCmId", e.ReturnValue)
            WebUtil.SelectGridRowByKeyValue(gvCommonCodes, colKeyValues)
        End If

    End Sub

    Protected Sub odsCommonCodeDetails_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsCommonCodeDetails.Inserting
        Dim obj As CommonCode = e.InputParameters("commonCode")
        obj.CreateUser = CType(User, CustomPrincipal).UserId
        obj.CreateDT = Now()
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()
        obj.Disabled = 0 'default value
    End Sub

    Protected Sub odsCommonCodeDetails_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsCommonCodeDetails.Updating
        Dim obj As CommonCode = e.InputParameters("commonCode")
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()
    End Sub

    Private Sub dropCatg_DataBound(sender As Object, e As System.EventArgs) Handles dropCatg.DataBound
        dropCatg.Items.Insert(0, New ListItem("-- Root Category --", "00"))
    End Sub

    Private Sub dropCatg_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles dropCatg.SelectedIndexChanged
        gvCommonCodes.PageIndex = 0
        gvCommonCodes.SelectedIndex = -1
        gvCommonCodes.DataBind()
        dvCommonCode.ChangeMode(DetailsViewMode.Insert)
        dvCommonCode.Caption = "Add New Common Code"
    End Sub

    Private Sub odsCommonCodeList_Selected(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles odsCommonCodeList.Selected
        ViewState("gvRecordCount") = ""
        If e.ReturnValue IsNot Nothing Then
            If CType(e.ReturnValue, IEnumerable(Of Object)) IsNot Nothing Then
                Dim totalCount As Integer = CType(e.ReturnValue, IEnumerable(Of Object)).Count
                ViewState("gvRecordCount") = "Total rows: " & totalCount.ToString()
            End If
        End If
    End Sub

End Class