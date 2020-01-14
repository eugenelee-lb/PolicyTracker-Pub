Imports PolicyTracker.Lib
Imports System.Web.ModelBinding
Imports System.IO

Public Class Packet
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Packet - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = ""
        Master.ActiveSubMenu = ""

        Try
            fvPacket.DataBind()
            If fvPacket.DataItemCount < 1 Then
                panCoverSheet.Visible = False
                gvFiles.Visible = False
                Exit Sub
            End If

            If Page.IsPostBack Then
                hidVerifyIDView.Value = ""
                hidVerifyIDAck.Value = ""
            Else
                Dim intReleaseId As Integer = Integer.Parse(RouteData.Values("ReleaseId").ToString)
                Dim strRecipientId As String = RouteData.Values("RecipientId").ToString

                chbAcknowledge.Text = ""
                chbAcknowledge.Enabled = False
                chbAcknowledge.Visible = False
                btnAck.Visible = False

                ' check permission
                If (Not CType(User, CustomPrincipal).UserId.ToUpper = strRecipientId.ToUpper) _
                    AndAlso (Not CType(User, CustomPrincipal).IsInRole("SA")) Then
                    If Not PTBL.IsPacketForAdmin(intReleaseId, strRecipientId, CType(User, CustomPrincipal).UserId) Then
                        ' redirect to Sign Out
                        WebUtil.WriteLog("You have no permission to this packet.")
                        Response.Redirect("~\SignOut.html")

                        fvPacket.Visible = False
                        panCoverSheet.Visible = False
                        gvFiles.Visible = False
                        panUploadAckFile.Visible = False
                        Throw New ApplicationException("You have no permission to this packet.")
                    End If
                End If

                chbAcknowledge.Visible = True
                lblAckText.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(1001))
                lblPolicyHeader.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(1002))

                ' admins upload/remove ack files
                If CType(User, CustomPrincipal).IsInRole("OA,PA,SA") Then
                    panUploadAckFile.Visible = True
                    'chbAcknowledge.Enabled = True
                    'btnAck.Visible = True
                End If

                ' packet recipient and admins can ack packet
                Dim lblAckDate As Label = fvPacket.FindControl("lblAckDate")
                If lblAckDate IsNot Nothing Then
                    If String.IsNullOrWhiteSpace(lblAckDate.Text) Then
                        lblAckWarning.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(1003)) '"You have not acknowledged yet."
                        lblAckWarning.Visible = True
                        chbAcknowledge.Checked = False
                        chbAcknowledge.Enabled = True
                        btnAck.Visible = True
                    Else
                        lblAckWarning.Visible = False
                        chbAcknowledge.Checked = True
                        chbAcknowledge.Enabled = False
                        btnAck.Visible = False
                    End If
                End If

                ' Recipient = Login User
                If strRecipientId.ToUpper = CType(User, CustomPrincipal).UserId.ToUpper Then
                    ' View
                    Dim lblRecipientViewDT As Label = fvPacket.FindControl("lblRecipientViewDT")
                    If lblRecipientViewDT.Text = "" Then
                        ' need user ID verify?
                        If CType(User, CustomPrincipal).AuthType = "Windows" _
                            AndAlso UserAuth.NeedUserIDVerify(CType(User, CustomPrincipal).OrgCode) Then
                            hidVerifyIDView.Value = "Y"
                            hidUserName.Value = CType(User, CustomPrincipal).UserName
                            hidClientIP.Value = GetHostIPName()
                        Else
                            ' update RecipientViewDT/ClientIP
                            Dim bl As New PTBL
                            bl.UpdateRecipientView(intReleaseId, strRecipientId, GetHostIPName())
                            fvPacket.DataBind()
                        End If
                    End If
                    ' Ack
                    If String.IsNullOrWhiteSpace(lblAckDate.Text) Then
                        ' need user ID verify?
                        If CType(User, CustomPrincipal).AuthType = "Windows" _
                            AndAlso UserAuth.NeedUserIDVerify(CType(User, CustomPrincipal).OrgCode) Then
                            hidVerifyIDAck.Value = "Y"
                            hidUserName.Value = CType(User, CustomPrincipal).UserName
                        End If
                    End If
                End If
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub

    Public Function fvEmployee_GetItem() As Object

        Using ctx = New PolicyTracker.Lib.PTEntities
            Dim em = (From emp In ctx.vEmployees
                      Where emp.EmpId = CType(User, CustomPrincipal).UserId
                      Select emp).FirstOrDefault()
            If em Is Nothing Then
                Throw New ApplicationException("Employee [" & CType(User, CustomPrincipal).UserId & "] is not found.")
            End If
            Return em
        End Using

    End Function

    Public Function fvPacket_GetItem(<RouteData> releaseId As Integer, <RouteData> recipientId As String) As PolicyTracker.Lib.ReleaseRecipient
        Dim db = New PTEntities
        Dim pa = (From a In db.ReleaseRecipients
                  Where a.ReleaseId = releaseId And a.RecipientId = recipientId Select a).FirstOrDefault()

        Return pa
    End Function

    Private Sub fvPacket_DataBound(sender As Object, e As EventArgs) Handles fvPacket.DataBound
    End Sub

    Public Function lvReleasePolicies_GetData(<RouteData> releaseId As Integer) As IQueryable(Of PolicyTracker.Lib.ReleasePolicy)
        Dim db = New PolicyTracker.Lib.PTEntities
        Dim qry = (From a In db.ReleasePolicies Where a.ReleaseId = releaseId Select a)
        Return qry
    End Function

    Protected Function EvalHtmlEncode(colname As String) As String
        Return WebUtil.HtmlMsgEncode(Eval(colname))
    End Function

    Protected Function EvalDesc() As String
        If Eval("PolicyDesc") = "" Then
            Return ""
        Else
            Return WebUtil.HtmlMsgEncode(Eval("PolicyDesc"))
        End If
    End Function

    Protected Function EvalDisclaimer() As String
        If Eval("ShowDisclaimer").ToString = "True" Then
            'Return "<ul><li>" & WebUtil.HtmlMsgEncode(Eval("Disclaimer")) & "</li></ul>"
            Return "<p>" & WebUtil.HtmlMsgEncode(Eval("Disclaimer")) & "</p>"
        Else
            Return ""
        End If
    End Function

    'Protected Function EvalFiles() As String
    '    Dim strFiles As String = ""
    '    Dim fs As HashSet(Of UploadFile) = Eval("UploadFiles")
    '    If fs.Count > 0 Then
    '        strFiles &= "<ul>"
    '        For Each fi In fs
    '            strFiles &= "<li>" & fi.OriginalName & "</li>"
    '        Next
    '        strFiles &= "</ul>"
    '    End If
    '    Return strfiles
    'End Function

    Private Sub btnAck_Click(sender As Object, e As EventArgs) Handles btnAck.Click
        Try
            Dim intReleaseId As Integer = Integer.Parse(RouteData.Values("ReleaseId").ToString)
            Dim strRecipientId As String = RouteData.Values("RecipientId").ToString

            Dim bl As PTBL = New PTBL
            If bl.AckPacket(intReleaseId, strRecipientId, CType(Context.User, CustomPrincipal).UserId,
                        GetHostIPName(),
                        CType(Context.User, CustomPrincipal).AuthType) Then
                fvPacket.DataBind()
                'lvReleasePolicies.DataBind()
                chbAcknowledge.Enabled = False
                btnAck.Visible = False

                lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(1004)) ' Acknowledge success
                lblInfo.Visible = True
                fvPacket.DataBind()
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(1005)) '"There was a problem in acknowledging."

            Dim strMessage As String = WebUtil.ProcessException(ex)
            Dim _customValidator = New CustomValidator()
            _customValidator.IsValid = False
            _customValidator.ErrorMessage = strMessage
            _customValidator.ValidationGroup = "vgMain"
            Page.Validators.Add(_customValidator)
        End Try
    End Sub

    Public Function lvFiles_GetData(<RouteData> releaseId As Integer, <Control> policyId As Integer) As IQueryable(Of PolicyTracker.Lib.UploadFile)
        Dim db = New PolicyTracker.Lib.PTEntities
        Dim qry = (From a In db.UploadFiles
                   Where (From rp In a.ReleasePolicies
                          Where rp.ReleaseId = releaseId And rp.PolicyId = policyId).Any()
                   Select a Order By a.OriginalName)
        Return qry
    End Function

    Protected Sub lvFiles_ItemCommand(sender As Object, e As ListViewCommandEventArgs)
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
            End If
        Catch ex As Exception
            lblErrorDownload.Visible = True
            lblErrorDownload.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub

    Protected Sub lvFiles_ItemDataBound(sender As Object, e As ListViewItemEventArgs)
        Dim row As ListViewItem = e.Item
        Dim btnDownload As LinkButton = row.FindControl("lbtnDownload")
        If btnDownload IsNot Nothing Then
            'lblInfoUpload.Text &= lnkDownload.UniqueID.Substring(lnkDownload.UniqueID.IndexOf("atcBiz$")) & "<br/>"
            Dim pbt As New PostBackTrigger
            pbt.ControlID = btnDownload.UniqueID.Substring(btnDownload.UniqueID.IndexOf("lvReleasePolicies$"))
            UpdatePanel1.Triggers.Add(pbt)
        End If
        'lblInfoUpload.Visible = True
    End Sub

    ' Packet Ack Files
    Public Function gvFiles_GetData(<RouteData> releaseId As Integer, _
                                    <RouteData> recipientId As String) As IQueryable(Of PolicyTracker.Lib.UploadFile)
        Dim db = New PolicyTracker.Lib.PTEntities

        Dim isAdmin As Boolean = CType(User, CustomPrincipal).IsInRole("SA")
        Dim qry = (From uf In db.UploadFiles
                   Where uf.PacketAckFiles.Where(Function(a) a.ReleaseId = releaseId And a.RecipientId = recipientId _
                                                     And (isAdmin = True Or a.Disabled = False)).Any()
                   Select uf Order By uf.OriginalName)
        Return qry
    End Function

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

        ' only admins can add/remove ack file
        If CType(User, CustomPrincipal).IsInRole("OA,PA,SA") Then
            gvFiles.Columns(1).Visible = True
            fileUpload.Visible = True
            btnUpload.Visible = True
        Else
            gvFiles.Columns(1).Visible = False
            fileUpload.Visible = False
            btnUpload.Visible = False
        End If

        ' only SA can see Disabled column
        Dim isAdmin As Boolean = CType(User, CustomPrincipal).IsInRole("SA")
        If Not isAdmin Then
            gvFiles.Columns(5).Visible = False
        End If
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
                'Dim uploadFile = repo.GetUploadFileById(e.CommandArgument)
                'If uploadFile Is Nothing Then
                '    Throw New ApplicationException("Could not find upload file record.")
                'End If

                ' soft delete record
                Dim intReleaseId As Integer = Integer.Parse(RouteData.Values("ReleaseId").ToString)
                Dim strRecipientId As String = RouteData.Values("RecipientId").ToString
                bl.DeletePacketAckFile(intReleaseId, strRecipientId, e.CommandArgument.ToString)

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
        Dim lbtnDelete As LinkButton = row.FindControl("lbtnDelete")
        If lbtnDelete IsNot Nothing Then
            Dim pbt As New PostBackTrigger
            pbt.ControlID = lbtnDelete.UniqueID.Substring(lbtnDelete.UniqueID.IndexOf("gvFiles$"))
            UpdatePanel1.Triggers.Add(pbt)
        End If
        Dim chbDisabled As CheckBox = row.FindControl("chbDisabled")
        If chbDisabled IsNot Nothing Then
            If chbDisabled.Checked Then lbtnDelete.Visible = False
        End If
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

                Dim intReleaseId As Integer = Integer.Parse(RouteData.Values("ReleaseId").ToString)
                Dim strRecipientId As String = RouteData.Values("RecipientId").ToString
                Dim bl As New PTBL
                bl.AddPacketAckFile(intReleaseId, strRecipientId, newFile)

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

            ' bind controls with postbacktrigger
            'lvReleasePolicies.DataBind()
            gvFiles.DataBind()

        Catch ex As Exception
            lblErrorUpload.Visible = True
            lblErrorUpload.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub

    'Protected Sub btnIDVSubmit_ServerClick(sender As Object, e As EventArgs)
    '    If radIDNo.Checked Then
    '        lblInfo.Text &= "[ID:No]"
    '        lblInfo.Visible = True
    '    End If
    '    If radIDYes.Checked Then
    '        lblInfo.Text &= "[ID:Yes]"
    '        lblInfo.Visible = True
    '    End If
    'End Sub

    Private Function GetHostIPName() As String
        Dim strHostName As String = ""
        Try
            strHostName = System.Net.Dns.GetHostEntry(Request.ServerVariables("REMOTE_HOST")).HostName
        Catch ex As Exception
        End Try

        If strHostName = "" Then
            Return Request.UserHostAddress
        Else
            Return Request.UserHostAddress & "/" & strHostName
        End If
    End Function
End Class