Imports PolicyTracker.Lib
Imports System.Web.ModelBinding
Imports System.IO

Public Class Policies
    Inherits System.Web.UI.Page

    Private Sub Policies_Init(sender As Object, e As EventArgs) Handles Me.Init
        'Page.Form.Attributes.Add("enctype", "multipart/form-data")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Policies - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "PolicyAdmin"
        Master.ActiveSubMenu = "Policies"

        If Not Page.IsPostBack Then
            If RouteData.Values("PolicyId") IsNot Nothing Then
                divSearchFilters.Visible = False
                spnHeader.InnerText = "Policy"
                dvPolicy.DefaultMode = DetailsViewMode.ReadOnly
            End If
            ddlFilterStat.DataBind()
            If CType(User, CustomPrincipal).IsInRole("SA") Then
                ddlFilterOwner.SelectedValue = "ALL"
            End If
            Me.Form.DefaultButton = btnSearch.UniqueID

            gvPolicies.Sort("LastUpdateDT", SortDirection.Descending)
        End If
    End Sub

    Public Function gvPolicies_GetData(<Control> txtFilterName As String, <Control> txtFilterDesc As String,
                                       <Control> ddlFilterStat As String,
                                       <Control> ddlFilterOwner As String,
                                       <RouteData> PolicyId As Object) As IQueryable(Of PolicyTracker.Lib.Policy)
        Dim ctx = New PolicyTracker.Lib.PTEntities
        Dim qry = (From p In ctx.Policies Select p)

        ' policy Id 
        If PolicyId IsNot Nothing Then
            ddlFilterOwner = "ALL"
            txtFilterName = ""
            txtFilterDesc = ""
            ddlFilterStat = ""
            Dim intPolicyId As Integer = Integer.Parse(PolicyId.ToString)
            qry = qry.Where(Function(a) a.PolicyId = intPolicyId)
        End If

        ' filter Owner 
        If ddlFilterOwner = "MY" Then
            ' Owned by user
            qry = qry.Where(Function(a) (From o In a.PolicyOwners
                                         Where o.UserId = CType(Page.User, CustomPrincipal).UserId).Any())
        Else
            ' All - shared to user
            Dim blSettings As New SettingsBL
            Dim myDepts = blSettings.GetDepartmentsByAdmin(CType(Page.User, CustomPrincipal).Role, CType(Page.User, CustomPrincipal).UserId)
            Dim listMyDepts = (From d In myDepts Select d.DeptCode)
            qry = qry.Where(Function(a) ((From o In a.PolicyOwners
                                          Where o.UserId = CType(Page.User, CustomPrincipal).UserId).Any() _
                                         Or (a.ShareType = "DEPT" And listMyDepts.Contains(a.DeptCode)) _
                                         Or (a.ShareType = "PUB")))
        End If

        ' filter name
        If Not String.IsNullOrWhiteSpace(txtFilterName) Then
            qry = qry.Where(Function(a) (a.PolicyName.Contains(txtFilterName)))
        End If
        ' filter desc
        If Not String.IsNullOrWhiteSpace(txtFilterDesc) Then
            qry = qry.Where(Function(a) (a.PolicyDesc.Contains(txtFilterDesc)))
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

    Private Sub gvPolicies_DataBound(sender As Object, e As System.EventArgs) Handles gvPolicies.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        If RouteData.Values("PolicyId") IsNot Nothing Then
            If gv.Rows.Count = 1 Then
                gv.SelectRow(0)
                dvPolicy.DataBind()

                'BindFiles()
                'BindSchedules()
                'BindOwners()
            Else
                Throw New ApplicationException("You do not have permission to this policy.")
            End If
        Else
            gv.SelectedIndex = -1
            dvPolicy.ChangeMode(DetailsViewMode.Insert)
            dvPolicy_ModeChanged(Nothing, Nothing)
        End If
    End Sub

    Protected Sub gvPolicies_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvPolicies.SelectedIndexChanged
        dvPolicy.ChangeMode(DetailsViewMode.ReadOnly)
        dvPolicy_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvPolicies)

    End Sub

    Private Sub btnSearch_Click(sender As Object, e As System.EventArgs) Handles btnSearch.Click
        'gvPolicies.DataBind()
    End Sub

    Protected Function EvalHtmlEncode(colname As String) As String
        'Return Eval(colname)
        Return WebUtil.HtmlMsgEncode(Eval(colname))
    End Function

    Private Sub dvPolicy_DataBound(sender As Object, e As EventArgs) Handles dvPolicy.DataBound
        Try
            If dvPolicy.CurrentMode = DetailsViewMode.ReadOnly And dvPolicy.DataKey.Value IsNot Nothing Then
                ' save ownership
                ViewState("IsOwner") = False
                Dim intPolicyId As Integer = Integer.Parse(gvPolicies.SelectedDataKey("PolicyId"))
                Dim bl = New PTBL
                If bl.IsPolicyOwner(intPolicyId, CType(User, CustomPrincipal).UserId) Then ViewState("IsOwner") = True

                BindFiles()
                BindSchedules()
                BindOwners()
            End If

            ' only owners can edit/delete
            If dvPolicy.CurrentMode = DetailsViewMode.ReadOnly Then
                Dim lbtnEdit As LinkButton = dvPolicy.FindControl("lbtnEdit")
                Dim lbtnDelete As LinkButton = dvPolicy.FindControl("lbtnDelete")
                If ViewState("IsOwner") Then
                    lbtnEdit.Visible = True
                    lbtnDelete.Visible = True
                Else
                    lbtnEdit.Visible = False
                    lbtnDelete.Visible = False
                End If

                If RouteData.Values("PolicyId") IsNot Nothing Then
                    Dim lbtnNew As LinkButton = dvPolicy.FindControl("lbtnNew")
                    lbtnNew.Visible = False
                End If
            End If

            If dvPolicy.CurrentMode = DetailsViewMode.Insert Then
                Dim ddlShareType As DropDownList = dvPolicy.FindControl("ddlShareType")
                Dim ddlDeptCode As DropDownList = dvPolicy.FindControl("ddlDeptCode")
                ddlShareType.SelectedValue = "DEPT"
                ddlDeptCode.SelectedIndex = 1
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub

    ' Policy
    Protected Sub dvPolicy_ItemDeleted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewDeletedEventArgs) Handles dvPolicy.ItemDeleted
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
                    dvPolicy.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                dvPolicy.DataBind()
            Else
                ' Success
                gvPolicies.SelectedIndex = -1
                gvPolicies.DataBind()
                dvPolicy.ChangeMode(DetailsViewMode.Insert)

                lblInfo.Visible = True
                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(13)) ' Record Deleted
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
            e.ExceptionHandled = True
        End Try
    End Sub

    Protected Sub dvPolicy_ItemInserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewInsertedEventArgs) Handles dvPolicy.ItemInserted
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

    Private Sub dvPolicy_ItemInserting(sender As Object, e As System.Web.UI.WebControls.DetailsViewInsertEventArgs) Handles dvPolicy.ItemInserting
        e.Values("LastUpdateDT") = Now()
        e.Values("LastUpdateUser") = CType(User, CustomPrincipal).UserId
    End Sub

    Protected Sub dvPolicy_ItemUpdated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DetailsViewUpdatedEventArgs) Handles dvPolicy.ItemUpdated
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
                    dvPolicy.DataBind()
                End If

                ' Indicate that the exception has been handled
                e.ExceptionHandled = True
                ' Keep the row in edit mode
                e.KeepInEditMode = True
            Else
                ' Select the record inserted
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("PolicyId", e.Keys.Item("PolicyId").ToString)
                WebUtil.SelectGridRowByKeyValue(gvPolicies, colKeyValues)
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

    Protected Sub dvPolicy_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dvPolicy.ModeChanged
        Select Case dvPolicy.CurrentMode
            Case DetailsViewMode.Insert
                If gvPolicies.SelectedIndex > -1 And lblDetailsViewMode.Text = "ReadOnly" Then
                    dvPolicy.ChangeMode(DetailsViewMode.ReadOnly)
                    'dvPolicy.Caption = "Policy Details"
                    tlkPolicy.InnerText = "Policy Details"
                    tliSchedules.Visible = True
                    tliFiles.Visible = True
                    tliOwners.Visible = True
                Else
                    gvPolicies.SelectedIndex = -1
                    WebUtil.SetGridRowStyle(gvPolicies)
                    'dvPolicy.Caption = "Add New Policy"
                    tlkPolicy.InnerText = "Add New Policy"
                    tliSchedules.Visible = False
                    tliFiles.Visible = False
                    tliOwners.Visible = False
                End If
            Case DetailsViewMode.Edit
                'dvPolicy.Caption = "Edit Policy"
                tlkPolicy.InnerText = "Edit Policy"
                tliSchedules.Visible = True
                tliFiles.Visible = True
                tliOwners.Visible = True
            Case DetailsViewMode.ReadOnly
                'dvPolicy.Caption = "Policy Details"
                tlkPolicy.InnerText = "Policy Details"
                tliSchedules.Visible = True
                tliFiles.Visible = True
                tliOwners.Visible = True
        End Select

    End Sub

    Private Sub odsPolicyDetails_Inserted(sender As Object, e As System.Web.UI.WebControls.ObjectDataSourceStatusEventArgs) Handles odsPolicyDetails.Inserted
        If e.Exception Is Nothing Then
            ' Select the record inserted
            Dim colKeyValues = New Collections.Specialized.NameValueCollection
            colKeyValues.Add("NewPolicyID", e.ReturnValue)
            If WebUtil.SelectGridRowByKeyValue(gvPolicies, colKeyValues) Then
                lblDetailsViewMode.Text = "ReadOnly"

                BindFiles()
                BindSchedules()
                BindOwners()

                gvFiles.DataBind()
                gvFiles.Visible = True
            End If
        End If
    End Sub

    Protected Sub odsPolicyDetails_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsPolicyDetails.Inserting
        Dim obj As Policy = e.InputParameters("policy")

        If obj.ShareType = "PRI" And obj.DeptCode IsNot Nothing Then
            obj.DeptCode = Nothing
        End If
        obj.CreateUser = CType(User, CustomPrincipal).UserId
        obj.CreateDT = Now()
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()
    End Sub

    Protected Sub odsPolicyDetails_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsPolicyDetails.Updating
        Dim obj As Policy = e.InputParameters("policy")
        obj.LastUpdateUser = CType(User, CustomPrincipal).UserId
        obj.LastUpdateDT = Now()
    End Sub

    Private Sub odsPolicyDetails_Deleting(sender As Object, e As Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles odsPolicyDetails.Deleting
        Dim obj As Policy = e.InputParameters("policy")
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

        Dim intPolicyId As Integer
        If Not Integer.TryParse(gvPolicies.SelectedValue, intPolicyId) Then Exit Sub

        Using ctx = New PolicyTracker.Lib.PTEntities

            Dim aOwners = (From a In ctx.PolicyOwners, u In ctx.AppUsers
                           Where a.PolicyId = intPolicyId And a.UserId = u.UserId
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
            Dim intPolicyId As Integer = Integer.Parse(gvPolicies.SelectedValue)
            Dim intReturn As Integer

            Using ctx = New PolicyTracker.Lib.PTEntities
                ' search existing Owner
                Dim po = (From a In ctx.PolicyOwners
                          Where a.PolicyId = intPolicyId And ddlOwnerToAdd.SelectedValue.Equals(a.UserId)
                           Select a).FirstOrDefault()
                If po IsNot Nothing Then
                    Throw New ApplicationException("Policy Owner [" & ddlOwnerToAdd.SelectedValue & "] already exists.")
                End If

                Dim newOwner = (From u In ctx.AppUsers
                                 Where u.UserId = ddlOwnerToAdd.SelectedValue
                                 Select u).FirstOrDefault()
                If newOwner Is Nothing Then
                    Throw New ApplicationException("User [" & ddlOwnerToAdd.SelectedValue & "] is not found.")
                End If

                Dim pol = (From p In ctx.Policies
                           Where p.PolicyId = intPolicyId
                           Select p).FirstOrDefault()
                If pol Is Nothing Then
                    Throw New ApplicationException("Policy [" & intPolicyId.ToString & "] is not found.")
                End If

                Dim newPO As New PolicyOwner
                With newPO
                    .PolicyId = intPolicyId
                    .UserId = newOwner.UserId
                End With
                pol.PolicyOwners.Add(newPO)
                intReturn = ctx.SaveChanges()
            End Using

            If intReturn = 1 Then
                Dim strOwnerName As String = ddlOwnerToAdd.SelectedItem.Text
                BindOwners()
                lblInfoOwners.Text = "Policy Owner [" & strOwnerName & "] has been added."
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
                Dim intPolicyId As Integer = Integer.Parse(gvPolicies.SelectedValue)
                Dim strOwner As String = e.CommandArgument.ToString
                Dim intReturn As Integer

                Using ctx = New PolicyTracker.Lib.PTEntities
                    Dim member = (From a In ctx.PolicyOwners
                                     Where a.PolicyId = intPolicyId And a.UserId = strOwner
                                     Select a).FirstOrDefault()
                    If member Is Nothing Then
                        Throw New ApplicationException("Policy Owner [" & strOwner & "] is not found.")
                    End If

                    Dim pol = (From p In ctx.Policies
                               Where p.PolicyId = intPolicyId
                               Select p).FirstOrDefault()
                    If pol Is Nothing Then
                        Throw New ApplicationException("Policy [" & intPolicyId.ToString & "] is not found.")
                    End If

                    ' cannot remove yourself
                    If strOwner = CType(User, CustomPrincipal).UserId Then
                        Throw New ApplicationException("You cannot remove yourself from owner.")
                    End If
                    ' cannot remove creator from owner
                    If pol.CreateUser.ToLower = strOwner.ToLower Then
                        Throw New ApplicationException("You cannot remove policy creator from owner.")
                    End If

                    pol.PolicyOwners.Remove(member)
                    intReturn = ctx.SaveChanges()
                End Using

                If intReturn = 1 Then
                    BindOwners()
                    lblInfoOwners.Text = "Policy Owner [" & strOwner & "] has been removed."
                    lblInfoOwners.Visible = True
                End If

            End If

        Catch ex As Exception
            lblErrorOwners.Visible = True
            lblErrorOwners.Text = WebUtil.ProcessException(ex)

        End Try
    End Sub

    ' Schedules
    Private Sub BindSchedules()
        Dim intPolicyId As Integer
        If Not Integer.TryParse(gvPolicies.SelectedValue, intPolicyId) Then Exit Sub

        Using context = New PTEntities()
            Dim policy = (From p In context.Policies
                          Where p.PolicyId = intPolicyId
                          Select p).First()
            Dim assignedSchedules = policy.Schedules.ToList()

            gvSchedules.DataSource = assignedSchedules
            gvSchedules.DataBind()
            spnSchCnt.InnerText = assignedSchedules.Count.ToString

        End Using
    End Sub

    ' Upload Files
    Private Sub BindFiles()
        ' only owners can add/remove
        If ViewState("IsOwner") Then
            gvFiles.Columns(1).Visible = True
            fileUpload.Visible = True
            btnUpload.Visible = True
        Else
            gvFiles.Columns(1).Visible = False
            fileUpload.Visible = False
            btnUpload.Visible = False
        End If

        Dim intPolicyId As Integer
        If Not Integer.TryParse(gvPolicies.SelectedValue, intPolicyId) Then Exit Sub

        Using db = New PolicyTracker.Lib.PTEntities
            Dim uFiles = (From uf In db.UploadFiles
                          Where uf.Policies.Where(Function(a) a.PolicyId = intPolicyId).Any()
                          Select uf Order By uf.OriginalName).ToList()
            spnFileCnt.InnerText = uFiles.Count.ToString
            gvFiles.DataSource = uFiles
            gvFiles.DataBind()

            ' tooltip show/hide
            'Dim lblDisclaimerEval As Label = dvPolicy.FindControl("lblDisclaimerEval")
            Dim pol = (From p In db.Policies
                       Where p.PolicyId = intPolicyId
                       Select p).FirstOrDefault()
            Dim hasURL As Boolean = False
            If pol IsNot Nothing AndAlso pol.ShowDisclaimer Then
                Dim patternURL As String = "https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}"
                hasURL = Regex.IsMatch(pol.Disclaimer, patternURL)
            End If

            If uFiles.Count = 0 And hasURL = False Then
                tliFiles.Attributes.Add("class", "showTooltip")
            Else
                tliFiles.Attributes.Item("class") = tliFiles.Attributes.Item("class").Replace("showTooltip", "")
            End If
        End Using

    End Sub

    Private Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        Try
            If fileUpload.HasFile Then
                Dim fileId As String = Guid.NewGuid().ToString
                Dim contentType As String = fileUpload.PostedFile.ContentType
                Dim fileStoreLoc As String = ConfigurationManager.AppSettings("FILE_STORE_LOC")

                Dim uploadPath As String = Now.ToString("yyyyMM")
                Dim _fileData As Byte() = New Byte(fileUpload.PostedFile.InputStream.Length) {}
                If fileStoreLoc = "FS" Then ' store to File System
                    Dim uploadRoot As String = ConfigurationManager.AppSettings("UPLOAD_FOLDER") & "\"
                    If Not Directory.Exists(uploadRoot & uploadPath) Then
                        Directory.CreateDirectory(uploadRoot & uploadPath)
                    End If
                    uploadPath &= "\" & fileId
                    fileUpload.SaveAs(uploadRoot & uploadPath)
                Else ' store to Database
                    fileUpload.PostedFile.InputStream.Read(_fileData, 0, _fileData.Length)
                End If

                ' save meta data
                Dim newFile As New UploadFile
                With newFile
                    .FileId = fileId
                    If fileStoreLoc = "FS" Then
                        .FileUrl = uploadPath
                    Else
                        .FileData = _fileData
                    End If
                    .OriginalName = fileUpload.FileName
                    .Length = fileUpload.PostedFile.InputStream.Length
                    .ContentType = contentType
                    .CreateDT = Now
                    .CreateUser = CType(User, CustomPrincipal).UserId
                End With

                Dim bl As New PTBL
                bl.AddPolicyFile(gvPolicies.SelectedValue, newFile)
                BindFiles()

                lblInfoUpload.Text = "Your file was uploaded." ' as " & uploadPath & "," & contentType
                lblInfoUpload.Visible = True

                ' Select the record updated
                Dim colKeyValues = New Collections.Specialized.NameValueCollection
                colKeyValues.Add("FileId", fileId)
                WebUtil.SelectGridRowByKeyValue(gvFiles, colKeyValues)
            Else
                lblErrorUpload.Text = "Please choose a file to upload."
                lblErrorUpload.Visible = True
            End If

        Catch ex As Exception
            lblErrorUpload.Visible = True
            lblErrorUpload.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub

    Private Sub gvFiles_DataBound(sender As Object, e As EventArgs) Handles gvFiles.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        'If gv.Rows.Count > 0 Then
        '    gv.BottomPagerRow.Visible = True
        '    ' record count
        '    Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
        '    pager.SetTotalRecordCount(ViewState("gvFilesRecordCount"))
        'End If

        gv.SelectedIndex = -1

    End Sub

    Private Sub gvFiles_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvFiles.RowCommand
        Try
            If e.CommandName = "Download" Then
                Dim repo As New PTRepository
                Dim uploadFile = repo.GetUploadFileById(e.CommandArgument)
                If uploadFile Is Nothing Then
                    Throw New ApplicationException("Could not find upload file record.")
                End If

                Dim fi As FileInfo = Nothing
                If Not String.IsNullOrWhiteSpace(uploadFile.FileUrl) Then
                    Dim uploadRoot As String = ConfigurationManager.AppSettings("UPLOAD_FOLDER") & "\"
                    If Not File.Exists(uploadRoot & uploadFile.FileUrl) Then
                        ' file not exists
                        Throw New ApplicationException("Could not find file in repository.")
                    End If
                    fi = New FileInfo(uploadRoot & uploadFile.FileUrl)
                End If

                Response.Clear()
                Response.ClearHeaders()
                Response.ClearContent()
                Response.AddHeader("Content-Disposition", "attachment; filename=""" + uploadFile.OriginalName + """")
                Response.AddHeader("Content-Length", uploadFile.Length.ToString())
                Response.ContentType = uploadFile.ContentType
                Response.Flush()
                If fi IsNot Nothing Then
                    Response.TransmitFile(fi.FullName)
                Else
                    Response.BinaryWrite(uploadFile.FileData)
                End If
                Response.End()

            ElseIf e.CommandName = "DeleteFile" Then
                Dim bl As New PTBL
                Dim repo As New PTRepository
                Dim uploadFile = repo.GetUploadFileById(e.CommandArgument)
                If uploadFile Is Nothing Then
                    Throw New ApplicationException("Could not find upload file record.")
                End If

                ' delete file
                Dim uploadRoot As String = ConfigurationManager.AppSettings("UPLOAD_FOLDER") & "\"
                If File.Exists(uploadRoot & uploadFile.FileUrl) Then
                    File.Delete(uploadRoot & uploadFile.FileUrl)
                Else
                    ' file not exists
                    'Throw New ApplicationException("Could not find file in repository.")
                End If

                ' delete record
                bl.DeletePolicyFile(gvPolicies.SelectedValue, uploadFile.FileId)
                BindFiles()

                lblInfoUpload.Text = "A file was deleted."
                lblInfoUpload.Visible = True

            End If

        Catch ex As Exception
            lblErrorUpload.Visible = True
            lblErrorUpload.Text = WebUtil.ProcessException(ex)

        Finally
            gvFiles.DataBind()

        End Try
    End Sub

    Private Sub gvFiles_RowDataBound(sender As Object, e As Web.UI.WebControls.GridViewRowEventArgs) Handles gvFiles.RowDataBound
        Dim row As GridViewRow = e.Row
        Dim btnDownload As LinkButton = row.FindControl("lbtnDownload")
        If btnDownload IsNot Nothing Then
            'lblInfoUpload.Text &= lnkDownload.UniqueID.Substring(lnkDownload.UniqueID.IndexOf("atcBiz$")) & "<br/>"
            Dim pbt As New PostBackTrigger
            pbt.ControlID = btnDownload.UniqueID.Substring(btnDownload.UniqueID.IndexOf("gvFiles$"))
            UpdatePanel1.Triggers.Add(pbt)
        End If
        'lblInfoUpload.Visible = True
    End Sub

    Protected Sub cvDeptCode_ServerValidate(source As Object, args As ServerValidateEventArgs)
        args.IsValid = True
        Dim ddlShareType As DropDownList = dvPolicy.FindControl("ddlShareType")
        Dim ddlDeptCode As DropDownList = dvPolicy.FindControl("ddlDeptCode")
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