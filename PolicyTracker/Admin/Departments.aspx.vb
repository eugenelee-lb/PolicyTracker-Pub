Imports PolicyTracker.Lib

Public Class Departments
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Departments - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Settings"
        Master.ActiveSubMenu = "Departments"
    End Sub

    Private Sub gvDepartments_DataBound(sender As Object, e As System.EventArgs) Handles gvDepartments.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        gv.SelectedIndex = -1
        dvDepartment.ChangeMode(DetailsViewMode.Insert)
        dvDepartment_ModeChanged(Nothing, Nothing)

    End Sub

    Protected Sub gvDepartments_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvDepartments.SelectedIndexChanged
        dvDepartment.ChangeMode(DetailsViewMode.ReadOnly)
        dvDepartment_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvDepartments)
    End Sub

    Protected Sub dvDepartment_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvDepartment.ItemDeleted
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
                    dvDepartment.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                dvDepartment.DataBind()
            Else
                ' Success
                gvDepartments.SelectedIndex = -1
                gvDepartments.DataBind()
                dvDepartment.ChangeMode(DetailsViewMode.Insert)

                lblInfo.Visible = True
                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Protected Sub dvDepartment_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvDepartment.ItemInserted
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
                colKeyValues.Add("DeptCode", e.Values("DeptCode").ToString)
                WebUtil.SelectGridRowByKeyValue(gvDepartments, colKeyValues)
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

    Private Sub dvDepartment_ItemInserting(sender As Object, e As System.Web.UI.WebControls.DetailsViewInsertEventArgs) Handles dvDepartment.ItemInserting
        e.Values("LastUpdateDT") = Now()
        e.Values("LastUpdateUser") = CType(User, CustomPrincipal).UserId
    End Sub

    Protected Sub dvDepartment_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvDepartment.ItemUpdated
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
                    dvDepartment.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInEditMode = True
            Else
                ' Select the record updated
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("DeptCode", e.Keys.Item("DeptCode").ToString)
                WebUtil.SelectGridRowByKeyValue(gvDepartments, colKeyValues)
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

    Protected Sub dvDepartment_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dvDepartment.ModeChanged
        Select Case dvDepartment.CurrentMode
            Case DetailsViewMode.Insert
                If gvDepartments.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
                    dvDepartment.ChangeMode(DetailsViewMode.ReadOnly)
                    dvDepartment.Caption = "Department Details"
                Else
                    gvDepartments.SelectedIndex = -1
                    WebUtil.SetGridRowStyle(gvDepartments)
                    dvDepartment.Caption = "Add New Department"
                End If
            Case DetailsViewMode.Edit
                dvDepartment.Caption = "Edit Department"
            Case DetailsViewMode.ReadOnly
                dvDepartment.Caption = "Department Details"
        End Select

    End Sub

    Protected Sub odsDepartmentDetails_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsDepartmentDetails.Inserting
        Dim dept As Department = e.InputParameters("department")
        dept.CreateUser = CType(User, CustomPrincipal).UserId
        dept.CreateDT = Now()
        dept.LastUpdateUser = CType(User, CustomPrincipal).UserId
        dept.LastUpdateDT = Now()
        dept.Disabled = False  'default value
    End Sub

    Protected Sub odsDepartmentDetails_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsDepartmentDetails.Updating
        Dim dept As Department = e.InputParameters("department")
        dept.LastUpdateUser = CType(User, CustomPrincipal).UserId
        dept.LastUpdateDT = Now()
    End Sub

    Private Sub odsDepartmentsGrid_Selected(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles odsDepartmentsGrid.Selected
        ViewState("gvRecordCount") = ""
        If e.ReturnValue IsNot Nothing Then
            If CType(e.ReturnValue, IEnumerable(Of Object)) IsNot Nothing Then
                Dim totalCount As Integer = CType(e.ReturnValue, IEnumerable(Of Object)).Count
                ViewState("gvRecordCount") = "Total rows: " & totalCount.ToString()
            End If
        End If
    End Sub
End Class