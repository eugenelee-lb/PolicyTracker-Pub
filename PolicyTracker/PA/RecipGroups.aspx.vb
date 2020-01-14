Imports PolicyTracker.Lib
Imports System.Web.ModelBinding

Public Class RecipGroups
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Recipient Groups - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "PolicyAdmin"
        Master.ActiveSubMenu = "RecipGroups"

        If Not Page.IsPostBack Then
            ddlFilterStat.DataBind()
            If CType(User, CustomPrincipal).IsInRole("SA") Then
                ddlFilterOwner.SelectedValue = "ALL"
            End If
            Me.Form.DefaultButton = btnSearch.UniqueID

            gvRecipGroups.Sort("LastUpdateDT", SortDirection.Descending)
        End If
    End Sub

    Public Function gvRecipGroups_GetData(<Control> txtFilterName As String, _
                                       <Control> ddlFilterStat As String, _
                                       <Control> ddlFilterOwner As String) As IQueryable(Of PolicyTracker.Lib.RecipGroup)
        Dim ctx = New PolicyTracker.Lib.PTEntities
        Dim qry = (From eg In ctx.RecipGroups Select eg)

        ' filter Owner 
        If ddlFilterOwner = "MY" Then
            ' Owned by user
            qry = qry.Where(Function(a) (From o In a.RecipGroupOwners
                                         Where o.UserId = CType(Page.User, CustomPrincipal).UserId).Any())
        Else
            ' All - shared to user
            Dim blSettings As New SettingsBL
            Dim myDepts = blSettings.GetDepartmentsByAdmin(CType(Page.User, CustomPrincipal).Role, CType(Page.User, CustomPrincipal).UserId)
            Dim listMyDepts = (From d In myDepts Select d.DeptCode)
            qry = qry.Where(Function(a) ((From o In a.RecipGroupOwners
                                          Where o.UserId = CType(Page.User, CustomPrincipal).UserId).Any() _
                                         Or (a.ShareType = "DEPT" And listMyDepts.Contains(a.DeptCode)) _
                                         Or (a.ShareType = "PUB")))
        End If

        ' filter name
        If Not String.IsNullOrWhiteSpace(txtFilterName) Then
            qry = qry.Where(Function(a) (a.GroupName.Contains(txtFilterName)))
        End If
        ' filter status
        If ddlFilterStat = "A" Then
            qry = qry.Where(Function(a) (a.Disabled = False))
        ElseIf ddlFilterStat = "D" Then
            qry = qry.Where(Function(a) (a.Disabled = True))
        End If

        ViewState("gvRecordCount") = "Total rows: " & qry.Count.ToString()

        Return qry
    End Function

    Private Sub gvRecipGroups_DataBound(sender As Object, e As System.EventArgs) Handles gvRecipGroups.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        gv.SelectedIndex = -1
        dvRecipGroup.ChangeMode(DetailsViewMode.Insert)
        dvRecipGroup_ModeChanged(Nothing, Nothing)
    End Sub

    Private Sub gvRecipGroups_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvRecipGroups.RowCommand
        If e.CommandName = "Settings" Then
            Dim intRecipGroupId As Integer = Integer.Parse(e.CommandArgument)
            Response.Redirect("~/PA/RecipGroupSettings/" & intRecipGroupId.ToString(), False)
        End If
    End Sub

    Protected Sub gvRecipGroups_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvRecipGroups.SelectedIndexChanged
        dvRecipGroup.ChangeMode(DetailsViewMode.ReadOnly)
        dvRecipGroup_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvRecipGroups)

    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        'gvRecipGroups.Visible = True
    End Sub

    ' RecipGroup
    Private Sub dvRecipGroup_DataBound(sender As Object, e As EventArgs) Handles dvRecipGroup.DataBound
        Try
            If dvRecipGroup.CurrentMode = DetailsViewMode.ReadOnly And dvRecipGroup.DataKey.Value IsNot Nothing Then
                ' save ownership
                ViewState("IsOwner") = False
                Dim intRecipGroupId As Integer = Integer.Parse(dvRecipGroup.DataKey.Value)
                Dim bl = New PTBL
                If bl.IsRecipGroupOwner(intRecipGroupId, CType(User, CustomPrincipal).UserId) Then ViewState("IsOwner") = True

                BindOwners()
                gvOwners.Visible = True
            End If

            ' only owners can edit/delete
            If dvRecipGroup.CurrentMode = DetailsViewMode.ReadOnly Then
                Dim lbtnEdit As LinkButton = dvRecipGroup.FindControl("lbtnEdit")
                Dim lbtnDelete As LinkButton = dvRecipGroup.FindControl("lbtnDelete")
                lbtnEdit.Visible = False
                lbtnDelete.Visible = False
                If ViewState("IsOwner") Then
                    lbtnEdit.Visible = True
                    lbtnDelete.Visible = True
                End If
            End If

            If dvRecipGroup.CurrentMode = DetailsViewMode.Insert Then
                Dim ddlShareType As DropDownList = dvRecipGroup.FindControl("ddlShareType")
                Dim ddlDeptCode As DropDownList = dvRecipGroup.FindControl("ddlDeptCode")
                ddlShareType.SelectedValue = "DEPT"
                ddlDeptCode.SelectedIndex = 1
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub

    Private Sub dvRecipGroup_ItemDeleted(sender As Object, e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvRecipGroup.ItemDeleted
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
                    dvRecipGroup.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                dvRecipGroup.DataBind()
            Else
                ' Success
                gvRecipGroups.SelectedIndex = -1
                gvRecipGroups.DataBind()
                dvRecipGroup.ChangeMode(DetailsViewMode.Insert)

                lblInfo.Visible = True
                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try
    End Sub

    Protected Sub dvRecipGroup_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvRecipGroup.ItemInserted
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
                'Dim colKeyValues = New Collections.Specialized.NameValueCollection
                'colKeyValues.Add("RecipGroupId", e.Values("RecipGroupId").ToString)
                'WebUtil.SelectGridRowByKeyValue(gvRecipGroups, colKeyValues)
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

    Private Sub dvRecipGroup_ItemUpdated(sender As Object, e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvRecipGroup.ItemUpdated
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
                    dvRecipGroup.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInEditMode = True
            Else
                ' Select the record updated
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("RecipGroupId", e.Keys.Item("RecipGroupId").ToString)
                WebUtil.SelectGridRowByKeyValue(gvRecipGroups, colKeyValues)
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

    Private Sub dvRecipGroup_ItemUpdating(sender As Object, e As DetailsViewUpdateEventArgs) Handles dvRecipGroup.ItemUpdating
        If Not Me.IsValid Then e.Cancel = True
    End Sub

    Private Sub dvRecipGroup_ModeChanged(sender As Object, e As System.EventArgs) Handles dvRecipGroup.ModeChanged
        Select Case dvRecipGroup.CurrentMode
            Case DetailsViewMode.Insert
                If gvRecipGroups.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
                    dvRecipGroup.ChangeMode(DetailsViewMode.ReadOnly)
                    'dvRecipGroup.Caption = "Recipient Group Details"
                    tlkRecipGroup.InnerText = "Recipient Group Details"
                    tliOwners.Visible = True
                Else
                    gvRecipGroups.SelectedIndex = -1
                    WebUtil.SetGridRowStyle(gvRecipGroups)
                    'dvRecipGroup.Caption = "Add New Recipient Group"
                    tlkRecipGroup.InnerText = "Add New Recipient Group"
                    tliOwners.Visible = False
                End If
            Case DetailsViewMode.Edit
                'dvRecipGroup.Caption = "Edit Recipient Group"
                tlkRecipGroup.InnerText = "Edit Recipient Group"
                tliOwners.Visible = True
            Case DetailsViewMode.ReadOnly
                'dvRecipGroup.Caption = "Recipient Group Details"
                tlkRecipGroup.InnerText = "Recipient Group Details"
                tliOwners.Visible = True
        End Select
    End Sub

    Private Sub odsRecipGroup_Inserted(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles odsRecipGroup.Inserted
        If e.Exception Is Nothing Then
            ' Select the record inserted
            Dim colKeyValues = New Collections.Specialized.NameValueCollection
            colKeyValues.Add("NewRecipGroupId", e.ReturnValue)
            If WebUtil.SelectGridRowByKeyValue(gvRecipGroups, colKeyValues) Then
                lblDetailsViewMode.Text = "ReadOnly"

                BindOwners()
                gvOwners.Visible = True
            End If
        End If
    End Sub

    Private Sub odsRecipGroup_Inserting(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsRecipGroup.Inserting
        Dim RecipGroup As RecipGroup = e.InputParameters("RecipGroup")
        RecipGroup.CreateUser = CType(Context.User, CustomPrincipal).UserId
        RecipGroup.CreateDT = Now()
        RecipGroup.LastUpdateUser = CType(Context.User, CustomPrincipal).UserId
        RecipGroup.LastUpdateDT = Now()
    End Sub

    Private Sub odsRecipGroup_Updating(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsRecipGroup.Updating
        Dim obj As RecipGroup = e.InputParameters("RecipGroup")
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()
    End Sub

    Private Sub odsRecipGroup_Deleting(sender As Object, e As Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsRecipGroup.Deleting
        Dim obj As RecipGroup = e.InputParameters("RecipGroup")
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        'obj.LastUpdateDT = Now()
    End Sub

    ' Owners
    Private Sub BindOwners()
        ' only owners can add/remove
        If ViewState("IsOwner") Then
            gvOwners.Columns(0).Visible = True
            panAddOwner.Visible = True
        Else
            gvOwners.Columns(0).Visible = False
            panAddOwner.Visible = False
        End If

        Dim intRecipGroupId As Integer
        If Not Integer.TryParse(gvRecipGroups.SelectedValue, intRecipGroupId) Then Exit Sub

        Using ctx = New PolicyTracker.Lib.PTEntities

            Dim aOwners = (From a In ctx.RecipGroupOwners, u In ctx.AppUsers
                           Where a.RecipGroupId = intRecipGroupId And a.UserId = u.UserId
                           Select Owner = a.UserId, OwnerName = u.UserName
                           Order By Owner).ToList()

            gvOwners.DataSource = aOwners
            gvOwners.DataBind()
            spnOwnCnt.InnerText = aOwners.Count.ToString

            Dim userRoles = {"PA", "SA"}
            Dim allOwners = (From au In ctx.AppUsers
                             Where userRoles.Contains(au.UserRole)
                             Select Owner = au.UserId, OwnerName = au.UserName
                             Order By Owner).ToList()

            Dim uOwners = allOwners.Except(aOwners.AsEnumerable()).ToList()
            ddlOwnerToAdd.DataSource = uOwners
            ddlOwnerToAdd.DataBind()
            ddlOwnerToAdd.Items.Insert(0, New ListItem("-- Select --", ""))

        End Using
    End Sub

    Private Sub btnAddOwner_Click(sender As Object, e As EventArgs) Handles btnAddOwner.Click
        Try
            Dim intRecipGroupId As Integer = Integer.Parse(gvRecipGroups.SelectedValue)
            Dim intReturn As Integer

            Using ctx = New PolicyTracker.Lib.PTEntities
                ' search existing Owner
                Dim ro = (From a In ctx.RecipGroupOwners
                          Where a.RecipGroupId = intRecipGroupId And ddlOwnerToAdd.SelectedValue.Equals(a.UserId)
                           Select a).FirstOrDefault()
                If ro IsNot Nothing Then
                    Throw New ApplicationException("Recipient Group Owner [" & ddlOwnerToAdd.SelectedValue & "] already exists.")
                End If

                Dim newOwner = (From u In ctx.AppUsers
                                 Where u.UserId = ddlOwnerToAdd.SelectedValue
                                 Select u).FirstOrDefault()
                If newOwner Is Nothing Then
                    Throw New ApplicationException("User [" & ddlOwnerToAdd.SelectedValue & "] is not found.")
                End If

                Dim rg = (From a In ctx.RecipGroups
                          Where a.RecipGroupId = intRecipGroupId
                          Select a).FirstOrDefault()
                If rg Is Nothing Then
                    Throw New ApplicationException("Recipient Group [" & intRecipGroupId.ToString & "] is not found.")
                End If

                Dim newRO As New RecipGroupOwner
                With newRO
                    .RecipGroupId = intRecipGroupId
                    .UserId = newOwner.UserId
                End With
                rg.RecipGroupOwners.Add(newRO)
                intReturn = ctx.SaveChanges()
            End Using

            If intReturn = 1 Then
                Dim strOwnerName As String = ddlOwnerToAdd.SelectedItem.Text
                BindOwners()
                lblInfoOwners.Text = "Recipinet Group Owner [" & strOwnerName & "] has been added."
                lblInfoOwners.Visible = True
            End If

        Catch ex As Exception
            lblErrorOwners.Visible = True
            lblErrorOwners.Text = WebUtil.ProcessException(ex)

        End Try
    End Sub

    Private Sub gvOwners_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvOwners.RowCommand
        Try
            If e.CommandName = "Remove" Then
                Dim intRecipGroupId As Integer = Integer.Parse(gvRecipGroups.SelectedValue)
                Dim strOwner As String = e.CommandArgument.ToString
                Dim intReturn As Integer

                Using ctx = New PolicyTracker.Lib.PTEntities
                    Dim member = (From a In ctx.RecipGroupOwners
                                     Where a.RecipGroupId = intRecipGroupId And a.UserId = strOwner
                                     Select a).FirstOrDefault()
                    If member Is Nothing Then
                        Throw New ApplicationException("Recipient Group Owner [" & strOwner & "] is not found.")
                    End If

                    Dim rg = (From a In ctx.RecipGroups
                              Where a.RecipGroupId = intRecipGroupId
                              Select a).FirstOrDefault()
                    If rg Is Nothing Then
                        Throw New ApplicationException("Recipient Group [" & intRecipGroupId.ToString & "] is not found.")
                    End If

                    ' cannot remove yourself
                    If strOwner = CType(User, CustomPrincipal).UserId Then
                        Throw New ApplicationException("You cannot remove yourself from owner.")
                    End If
                    ' cannot remove creator from owner
                    If rg.CreateUser.ToLower = strOwner.ToLower Then
                        Throw New ApplicationException("You cannot remove Recipient Group creator from owner.")
                    End If

                    rg.RecipGroupOwners.Remove(member)
                    intReturn = ctx.SaveChanges()
                End Using

                If intReturn = 1 Then
                    BindOwners()
                    lblInfoOwners.Text = "Recipient Group Owner [" & strOwner & "] has been removed."
                    lblInfoOwners.Visible = True
                End If

            End If

        Catch ex As Exception
            lblErrorOwners.Visible = True
            lblErrorOwners.Text = WebUtil.ProcessException(ex)

        End Try
    End Sub

    Protected Sub cvDeptCode_ServerValidate(source As Object, args As ServerValidateEventArgs)
        args.IsValid = True
        Dim ddlShareType As DropDownList = dvRecipGroup.FindControl("ddlShareType")
        Dim ddlDeptCode As DropDownList = dvRecipGroup.FindControl("ddlDeptCode")
        If ddlShareType.SelectedValue = "DEPT" And ddlDeptCode.SelectedValue = "" Then
            args.IsValid = False
        End If
    End Sub

    ' user permitted departments
    Protected Sub odsDeptCode_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs)
        e.InputParameters("userRole") = CType(Page.User, CustomPrincipal).Role
        e.InputParameters("userId") = CType(Page.User, CustomPrincipal).UserId
    End Sub
End Class