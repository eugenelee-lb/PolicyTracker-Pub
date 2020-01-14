Imports PolicyTracker.Lib
Imports System.Web.ModelBinding

Public Class OrgAdmins
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Org Admins - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "PolicyAdmin"
        Master.ActiveSubMenu = "OrgAdmins"

        If Not Page.IsPostBack Then
            ddlFilterDepartment.DataBind()
            Me.Form.DefaultButton = btnSearch.UniqueID
            'gvOrgAdmins.Sort("OrgCode", SortDirection.Ascending)
        End If
    End Sub

    Private Sub ddlFilterDepartment_DataBound(sender As Object, e As EventArgs) Handles ddlFilterDepartment.DataBound
        If CType(User, CustomPrincipal).IsInRole("SA") Then
            ddlFilterDepartment.Items.Insert(0, New ListItem("-- All --", ""))
        End If
    End Sub

    Protected Sub odsDepartment_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs)
        e.InputParameters("userRole") = CType(Page.User, CustomPrincipal).Role
        e.InputParameters("userId") = CType(Page.User, CustomPrincipal).UserId
    End Sub

    Public Function gvOrgAdmins_GetData(<Control> ddlFilterDepartment As String) As IQueryable(Of PolicyTracker.Lib.vOrgAdmin)
        Dim ctx = New PTEntities
        Dim qry = (From da In ctx.vOrgAdmins
                   Where da.AccessLevel = "OA"
                   Select da
                   Order By da.OrgCode, da.UserId).AsQueryable
        If Not String.IsNullOrEmpty(ddlFilterDepartment) Then
            qry = qry.Where(Function(a) a.OrgCode.StartsWith(ddlFilterDepartment))
        End If

        ViewState("gvRecordCount") = "Total rows: " & qry.Count.ToString()

        Return qry
    End Function

    Private Sub gvOrgAdmins_DataBound(sender As Object, e As System.EventArgs) Handles gvOrgAdmins.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        gv.SelectedIndex = -1
        dvOrgAdmin.ChangeMode(DetailsViewMode.Insert)
        dvOrgAdmin_ModeChanged(Nothing, Nothing)

    End Sub

    Protected Sub gvOrgAdmins_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvOrgAdmins.SelectedIndexChanged
        dvOrgAdmin.ChangeMode(DetailsViewMode.ReadOnly)
        dvOrgAdmin_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvOrgAdmins)
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        gvOrgAdmins.Visible = True

    End Sub

    Private Sub dvOrgAdmin_DataBound(sender As Object, e As EventArgs) Handles dvOrgAdmin.DataBound
        'Try
        '    ' only owners can edit/delete
        '    If dvOrgAdmin.CurrentMode = DetailsViewMode.ReadOnly Then
        '        'Dim lbtnEdit As LinkButton = dvOrgAdmin.FindControl("lbtnEdit")
        '        Dim lbtnDelete As LinkButton = dvOrgAdmin.FindControl("lbtnDelete")
        '        'lbtnEdit.Visible = False
        '        lbtnDelete.Visible = False
        '        If CType(User, CustomPrincipal).IsInRole("SA") Then
        '            'lbtnEdit.Visible = True
        '            lbtnDelete.Visible = True
        '        End If
        '    End If

        'Catch ex As Exception
        '    lblError.Visible = True
        '    lblError.Text = WebUtil.ProcessException(ex)
        'End Try
    End Sub

    Private Sub dvOrgAdmin_ItemDeleted(sender As Object, e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvOrgAdmin.ItemDeleted
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
                    dvOrgAdmin.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                dvOrgAdmin.DataBind()
            Else
                ' Success
                gvOrgAdmins.SelectedIndex = -1
                gvOrgAdmins.DataBind()
                dvOrgAdmin.ChangeMode(DetailsViewMode.Insert)

                lblInfo.Visible = True
                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try

    End Sub

    Protected Sub dvOrgAdmin_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvOrgAdmin.ItemInserted
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
                colKeyValues.Add("OrgCode", e.Values("OrgCode").ToString)
                colKeyValues.Add("UserId", e.Values("UserId").ToString)
                WebUtil.SelectGridRowByKeyValue(gvOrgAdmins, colKeyValues)
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

    Private Sub dvOrgAdmin_ItemUpdated(sender As Object, e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvOrgAdmin.ItemUpdated
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
                    dvOrgAdmin.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInEditMode = True
            Else
                ' Select the record updated
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("UserId", e.Keys.Item("UserId").ToString)
                WebUtil.SelectGridRowByKeyValue(gvOrgAdmins, colKeyValues)
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

    Private Sub dvOrgAdmin_ModeChanged(sender As Object, e As System.EventArgs) Handles dvOrgAdmin.ModeChanged
        Select Case dvOrgAdmin.CurrentMode
            Case DetailsViewMode.Insert
                If gvOrgAdmins.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
                    dvOrgAdmin.ChangeMode(DetailsViewMode.ReadOnly)
                    dvOrgAdmin.Caption = "Organization Admin Details"
                Else
                    gvOrgAdmins.SelectedIndex = -1
                    WebUtil.SetGridRowStyle(gvOrgAdmins)
                    dvOrgAdmin.Caption = "Add New Organization Admin"
                End If
            Case DetailsViewMode.Edit
                dvOrgAdmin.Caption = "Edit Organization Admin"
            Case DetailsViewMode.ReadOnly
                dvOrgAdmin.Caption = "Organization Admin Details"
        End Select

    End Sub

    Private Sub odsOrgAdmin_Inserting(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsOrgAdmin.Inserting
        Dim userOrg As UserOrg = e.InputParameters("userOrg")
        userOrg.CreateUser = CType(Context.User, CustomPrincipal).UserId
        userOrg.CreateDT = Now()
        userOrg.LastUpdateUser = CType(Context.User, CustomPrincipal).UserId
        userOrg.LastUpdateDT = Now()
        userOrg.AccessLevel = "OA"

        Dim txtUserName As TextBox = dvOrgAdmin.FindControl("txtUserName")
        userOrg.UserId &= "|" & txtUserName.Text
    End Sub

    Private Sub odsOrgAdmin_Updating(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsOrgAdmin.Updating
        Dim obj As UserOrg = e.InputParameters("userOrg")
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()
    End Sub

    Protected Function EvalUserName() As String
        Using db As New PTEntities
            Dim userId As String = Eval("UserId").ToString.ToLower
            Dim u = (From a In db.AppUsers Where a.UserId.ToLower = userId
                     Select a).FirstOrDefault()
            If u Is Nothing Then Return ""
            Return u.UserName
        End Using
    End Function

End Class