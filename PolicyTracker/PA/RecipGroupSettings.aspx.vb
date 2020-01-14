Imports PolicyTracker.Lib
Imports System.IO

Public Class RecipGroupSettings
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Recipient Group Settings - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "PolicyAdmin"
        Master.ActiveSubMenu = "RecipGroups"

        If Not Page.IsPostBack Then
            ' save ownership
            ViewState("IsOwner") = False
            Dim intRecipGroupId As Integer = Integer.Parse(RouteData.Values("RecipGroupId"))
            Dim bl = New PTBL
            If bl.IsRecipGroupOwner(intRecipGroupId, CType(User, CustomPrincipal).UserId) Then ViewState("IsOwner") = True
        End If
    End Sub

    Private Sub dvRecipGroup_DataBound(sender As Object, e As EventArgs) Handles dvRecipGroup.DataBound
        Dim ddlRecipGroupType As DropDownList = dvRecipGroup.FindControl("ddlRecipGroupType")
        Select Case ddlRecipGroupType.SelectedValue
            Case "IND"
                BindMembers()
                panIND.Visible = True

            Case "ATTRIB"
                BindAttributes()
                panOrgs.Visible = True
        End Select
    End Sub

    Private Sub BindMembers()
        ' only owners can add/remove
        If ViewState("IsOwner") Then
            gvMembers.Columns(0).Visible = True
            panAddMember.Visible = True
        Else
            gvMembers.Columns(0).Visible = False
            panAddMember.Visible = False
        End If

        ' clear add controls
        txtEmpId.Text = ""
        txtEmpName.Text = ""

        Dim intRecipGroupId As Integer = Integer.Parse(RouteData.Values("RecipGroupId"))
        Using ctx = New PolicyTracker.Lib.PTEntities

            Dim assignedMembers = (From g In ctx.RecipGroups, e In g.Employees
                                   Where g.RecipGroupId = intRecipGroupId
                                   Select e.EmpId, Name = e.FirstName & " " & e.LastName).ToList()

            gvMembers.DataSource = assignedMembers
            gvMembers.DataBind()

        End Using
    End Sub

    Private Sub btnAddMember_Click(sender As Object, e As EventArgs) Handles btnAddMember.Click
        Try
            Dim intRecipGroupId As Integer = Integer.Parse(RouteData.Values("RecipGroupId"))
            Dim intReturn As Integer

            Using ctx = New PolicyTracker.Lib.PTEntities
                Dim newMember = (From emp In ctx.Employees
                                 Where emp.EmpId = txtEmpId.Text
                                 Select emp).FirstOrDefault()
                If newMember Is Nothing Then
                    Throw New ApplicationException("Employee [" & txtEmpId.Text & "] is not found.")
                End If
                If newMember.Disabled Then
                    Throw New ApplicationException("Employee [" & txtEmpId.Text & "] is disabled.")
                End If

                Dim grp = (From g In ctx.RecipGroups
                           Where g.RecipGroupId = intRecipGroupId
                           Select g).FirstOrDefault()
                If grp Is Nothing Then
                    Throw New ApplicationException("Employee Group [" & intRecipGroupId.ToString & "] is not found.")
                End If

                grp.Employees.Add(newMember)
                intReturn = ctx.SaveChanges()
            End Using

            If intReturn = 1 Then
                lblInfoMembers.Text = "New Member [" & txtEmpId.Text & "] has been added."
                lblInfoMembers.Visible = True
                BindMembers()
            End If

        Catch ex As Exception
            lblErrorMembers.Visible = True
            lblErrorMembers.Text = WebUtil.ProcessException(ex)

        End Try
    End Sub

    Private Sub gvMembers_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvMembers.RowCommand
        Try
            If e.CommandName = "DeleteMember" Then
                Dim intRecipGroupId As Integer = Integer.Parse(RouteData.Values("RecipGroupId"))
                Dim strEmpId As String = e.CommandArgument.ToString
                Dim intReturn As Integer

                Using ctx = New PolicyTracker.Lib.PTEntities
                    Dim member = (From emp In ctx.Employees
                                     Where emp.EmpId = strEmpId
                                     Select emp).FirstOrDefault()
                    If member Is Nothing Then
                        Throw New ApplicationException("Employee [" & e.CommandArgument & "] is not found.")
                    End If

                    Dim grp = (From g In ctx.RecipGroups
                               Where g.RecipGroupId = intRecipGroupId
                               Select g).FirstOrDefault()
                    If grp Is Nothing Then
                        Throw New ApplicationException("Employee Group [" & intRecipGroupId.ToString & "] is not found.")
                    End If

                    grp.Employees.Remove(member)
                    intReturn = ctx.SaveChanges()
                End Using

                If intReturn = 1 Then
                    BindMembers()
                    lblInfoMembers.Text = "Member [" & strEmpId & "] has been removed."
                    lblInfoMembers.Visible = True
                End If

            ElseIf e.CommandName = "EmpInfo" Then
                EmpInfo.GetEmpInfo(e.CommandArgument.ToString)

            End If

        Catch ex As Exception
            lblErrorMembers.Visible = True
            lblErrorMembers.Text = WebUtil.ProcessException(ex)

        End Try
    End Sub

    Private Sub BindAttributes()
        ' only owners can add/remove
        If ViewState("IsOwner") Then
            gvOrgs.Columns(0).Visible = True
            gvClasses.Columns(0).Visible = True
            panAddOrg.Visible = True
            panAddClass.Visible = True
        Else
            gvOrgs.Columns(0).Visible = False
            gvClasses.Columns(0).Visible = False
            panAddOrg.Visible = False
            panAddClass.Visible = False
        End If

        Dim intRecipGroupId As Integer = Integer.Parse(RouteData.Values("RecipGroupId"))
        Using ctx = New PolicyTracker.Lib.PTEntities
            ' Org
            Dim aOrgs = (From eg In ctx.RecipGroups, org In eg.RecipGroupOrgs, vo In ctx.vOrganizations
                         Where eg.RecipGroupId = intRecipGroupId And org.OrgCode = vo.OrgCode
                         Select vo).ToList()

            gvOrgs.DataSource = aOrgs
            gvOrgs.DataBind()

            Dim allOrgs = (From vo In ctx.vOrganizations
                           Where vo.Disabled = False
                           Select vo).ToList()

            Dim uOrgs = allOrgs.Except(aOrgs.AsEnumerable()).ToList()
            ddlOrgToAdd.DataSource = uOrgs
            ddlOrgToAdd.DataBind()
            ddlOrgToAdd.Items.Insert(0, New ListItem("-- Select --", ""))

            ' Class
            Dim aClasses = (From eg In ctx.RecipGroups, cls In eg.Classifications
                            Where eg.RecipGroupId = intRecipGroupId
                            Select cls.ClassCode, ClassDesc = cls.ClassCode & " - " & cls.ClassDesc).ToList()

            gvClasses.DataSource = aClasses
            gvClasses.DataBind()

            Dim allClasses = (From cls In ctx.Classifications
                                Where cls.Disabled = False
                                Select cls.ClassCode, ClassDesc = cls.ClassCode & " - " & cls.ClassDesc).ToList()

            Dim uClasses = (From c In allClasses.Except(aClasses.AsEnumerable())
                            Select c.ClassCode, c.ClassDesc).ToList()
            ddlClassToAdd.DataSource = uClasses
            ddlClassToAdd.DataBind()
            ddlClassToAdd.Items.Insert(0, New ListItem("-- Select --", ""))

            ' Formula
            Dim strForOrg As String = ""
            For Each o In aOrgs
                If strForOrg <> "" Then strForOrg &= ", "
                strForOrg &= "'" & o.OrgCode & "'"
            Next
            Dim strForClass As String = ""
            For Each c In aClasses
                If strForClass <> "" Then strForClass &= ", "
                strForClass &= "'" & c.ClassCode & "'"
            Next

            Dim strFormula As String = ""
            If strForOrg <> "" Then
                strFormula &= "Organization IN (" & strForOrg & ")"
            End If
            If strForClass <> "" Then
                If strFormula <> "" Then strFormula &= " and "
                strFormula &= "Classification IN (" & strForClass & ")"
            End If

            If strFormula <> "" Then
                litFormula.Text = strFormula
                divFormula.Visible = True
            Else
                divFormula.Visible = False
            End If

        End Using
    End Sub

    Private Sub btnAddOrg_Click(sender As Object, e As EventArgs) Handles btnAddOrg.Click
        Try
            Dim intRecipGroupId As Integer = Integer.Parse(RouteData.Values("RecipGroupId"))
            Dim intReturn As Integer

            Using ctx = New PolicyTracker.Lib.PTEntities
                ' search existing orgs
                Dim ego = (From a In ctx.RecipGroupOrgs
                           Where a.RecipGroupId = intRecipGroupId And ddlOrgToAdd.SelectedValue.StartsWith(a.OrgCode)
                           Select a).FirstOrDefault()
                If ego IsNot Nothing Then
                    Throw New ApplicationException("Organization [" & ddlOrgToAdd.SelectedValue & "] is already in this group.")
                End If

                Dim newOrg = (From o In ctx.vOrganizations
                                 Where o.OrgCode = ddlOrgToAdd.SelectedValue
                                 Select o).FirstOrDefault()
                If newOrg Is Nothing Then
                    Throw New ApplicationException("Organization [" & ddlOrgToAdd.SelectedValue & "] is not found.")
                End If
                If newOrg.Disabled Then
                    Throw New ApplicationException("Organization [" & ddlOrgToAdd.SelectedValue & "] is disabled.")
                End If

                Dim grp = (From g In ctx.RecipGroups
                           Where g.RecipGroupId = intRecipGroupId
                           Select g).FirstOrDefault()
                If grp Is Nothing Then
                    Throw New ApplicationException("Employee Group [" & intRecipGroupId.ToString & "] is not found.")
                End If

                Dim newEGO As New RecipGroupOrg
                With newEGO
                    .RecipGroupId = intRecipGroupId
                    .OrgCode = newOrg.OrgCode
                End With
                grp.RecipGroupOrgs.Add(newEGO)
                intReturn = ctx.SaveChanges()
            End Using

            If intReturn = 1 Then
                Dim strOrgDesc As String = ddlOrgToAdd.SelectedItem.Text
                BindAttributes()
                lblInfoOrgs.Text = "Organization [" & strOrgDesc & "] has been added."
                lblInfoOrgs.Visible = True
            End If

        Catch ex As Exception
            lblErrorOrgs.Visible = True
            lblErrorOrgs.Text = WebUtil.ProcessException(ex)

        End Try
    End Sub

    Private Sub gvOrgs_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvOrgs.RowCommand
        Try
            If e.CommandName = "Remove" Then
                Dim intRecipGroupId As Integer = Integer.Parse(RouteData.Values("RecipGroupId"))
                Dim strOrgCode As String = e.CommandArgument.ToString
                Dim intReturn As Integer

                Using ctx = New PolicyTracker.Lib.PTEntities
                    Dim member = (From a In ctx.RecipGroupOrgs
                                     Where a.RecipGroupId = intRecipGroupId And a.OrgCode = strOrgCode
                                     Select a).FirstOrDefault()
                    If member Is Nothing Then
                        Throw New ApplicationException("Assigned Organization [" & strOrgCode & "] is not found.")
                    End If

                    Dim grp = (From g In ctx.RecipGroups
                               Where g.RecipGroupId = intRecipGroupId
                               Select g).FirstOrDefault()
                    If grp Is Nothing Then
                        Throw New ApplicationException("Employee Group [" & intRecipGroupId.ToString & "] is not found.")
                    End If

                    grp.RecipGroupOrgs.Remove(member)
                    intReturn = ctx.SaveChanges()
                End Using

                If intReturn = 1 Then
                    BindAttributes()
                    lblInfoOrgs.Text = "Assigned Organization [" & strOrgCode & "] has been removed."
                    lblInfoOrgs.Visible = True
                End If

            End If

        Catch ex As Exception
            lblErrorOrgs.Visible = True
            lblErrorOrgs.Text = WebUtil.ProcessException(ex)

        End Try
    End Sub

    Private Sub btnAddClass_Click(sender As Object, e As EventArgs) Handles btnAddClass.Click
        Try
            Dim intRecipGroupId As Integer = Integer.Parse(RouteData.Values("RecipGroupId"))
            Dim intReturn As Integer

            Using ctx = New PolicyTracker.Lib.PTEntities
                ' search existing classes
                Dim cls = (From eg In ctx.RecipGroups, c In eg.Classifications
                           Where eg.RecipGroupId = intRecipGroupId And c.ClassCode = ddlClassToAdd.SelectedValue
                           Select c).FirstOrDefault()
                If cls IsNot Nothing Then
                    Throw New ApplicationException("Classification [" & ddlClassToAdd.SelectedValue & "] is already in this group.")
                End If

                Dim newCls = (From c In ctx.Classifications
                                 Where c.ClassCode = ddlClassToAdd.SelectedValue
                                 Select c).FirstOrDefault()
                If newCls Is Nothing Then
                    Throw New ApplicationException("Classification [" & ddlClassToAdd.SelectedValue & "] is not found.")
                End If
                If newCls.Disabled Then
                    Throw New ApplicationException("Classification [" & ddlClassToAdd.SelectedValue & "] is disabled.")
                End If

                Dim grp = (From g In ctx.RecipGroups
                           Where g.RecipGroupId = intRecipGroupId
                           Select g).FirstOrDefault()
                If grp Is Nothing Then
                    Throw New ApplicationException("Employee Group [" & intRecipGroupId.ToString & "] is not found.")
                End If

                grp.Classifications.Add(newCls)
                intReturn = ctx.SaveChanges()
            End Using

            If intReturn = 1 Then
                Dim strClassDesc As String = ddlClassToAdd.SelectedItem.Text
                BindAttributes()
                lblInfoClasses.Text = "Classification [" & strClassDesc & "] has been added."
                lblInfoClasses.Visible = True
            End If

        Catch ex As Exception
            lblErrorClasses.Visible = True
            lblErrorClasses.Text = WebUtil.ProcessException(ex)

        End Try
    End Sub

    Private Sub gvClasses_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvClasses.RowCommand
        Try
            If e.CommandName = "Remove" Then
                Dim intRecipGroupId As Integer = Integer.Parse(RouteData.Values("RecipGroupId"))
                Dim strClassCode As String = e.CommandArgument.ToString
                Dim intReturn As Integer

                Using ctx = New PolicyTracker.Lib.PTEntities
                    Dim member = (From g In ctx.RecipGroups, c In g.Classifications
                                     Where g.RecipGroupId = intRecipGroupId And c.ClassCode = strClassCode
                                     Select c).FirstOrDefault()
                    If member Is Nothing Then
                        Throw New ApplicationException("Assigned Classification [" & strClassCode & "] is not found.")
                    End If

                    Dim grp = (From g In ctx.RecipGroups
                               Where g.RecipGroupId = intRecipGroupId
                               Select g).FirstOrDefault()
                    If grp Is Nothing Then
                        Throw New ApplicationException("Employee Group [" & intRecipGroupId.ToString & "] is not found.")
                    End If

                    grp.Classifications.Remove(member)
                    intReturn = ctx.SaveChanges()
                End Using

                If intReturn = 1 Then
                    BindAttributes()
                    lblInfoClasses.Text = "Assigned Classification [" & strClassCode & "] has been removed."
                    lblInfoClasses.Visible = True
                End If

            End If

        Catch ex As Exception
            lblErrorClasses.Visible = True
            lblErrorClasses.Text = WebUtil.ProcessException(ex)

        End Try
    End Sub

    Private Sub lbtnReviewGroupMembers_Command(sender As Object, e As CommandEventArgs) Handles lbtnReviewGroupMembers.Command
        Dim intRecipGroupId As Integer = Integer.Parse(RouteData.Values("RecipGroupId").ToString)
        RecipGroupMembers.GetRecipGroupMembers(intRecipGroupId)
    End Sub

    Private Sub btnDownload_ServerClick(sender As Object, e As EventArgs) Handles btnDownload.ServerClick
        Dim lblRecipGroupName As Label = CType(dvRecipGroup.FindControl("lblRecipGroupName"), Label)
        Dim strFolder As String = Request.PhysicalApplicationPath & "Export"
        Dim strFileNm As String = "Group_" & WebUtil.EncodeAsFileName(lblRecipGroupName.Text, "_").Replace(" ", "_") & "_" & Today.ToString("yyyyMMdd") & ".csv"

        Try
            Dim intRecipGroupId As Integer = Integer.Parse(RouteData.Values("RecipGroupId").ToString)
            Using ctx = New PolicyTracker.Lib.PTEntities
                Dim assignedMembers = (From g In ctx.RecipGroups, emp In g.Employees
                                       Where g.RecipGroupId = intRecipGroupId
                                       Select emp.EmpId, Name = emp.FirstName & " " & emp.LastName).ToList()

                ' create folder
                If Not Directory.Exists(strFolder) Then
                    Directory.CreateDirectory(strFolder)
                End If

                Dim myExport As New Util.CsvExport

                For Each ro In assignedMembers
                    myExport.AddRow()
                    myExport("Emp ID") = ro.EmpId
                    myExport("Name") = ro.Name
                Next

                myExport.ExportToFile(strFolder & "\" & strFileNm)
            End Using

            ' get the file bytes to download to the browser
            Dim fileBytes As Byte()
            fileBytes = System.IO.File.ReadAllBytes(strFolder & "\" & strFileNm)

            ' download this file to the browser
            WebUtil.StreamFileToBrowser(WebUtil.GetMimeTypeByFileName(strFileNm), strFileNm, fileBytes)

        Catch ex As Exception
            lblErrorInd.Visible = True
            lblErrorInd.Text = WebUtil.ProcessException(ex)
        Finally
            If File.Exists(strFolder & "\" & strFileNm) Then File.Delete(strFolder & "\" & strFileNm)
        End Try
    End Sub
End Class