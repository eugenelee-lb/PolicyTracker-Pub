Public Class UserInfo
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "User Info - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = ""
        Master.ActiveSubMenu = ""

        Try
            If Not Page.IsPostBack Then
                Dim cpUser As CustomPrincipal = CType(Context.User, CustomPrincipal)
                Dim strHostName As String = ""
                Try
                    strHostName = System.Net.Dns.GetHostEntry(Request.ServerVariables("REMOTE_HOST")).HostName
                Catch ex As Exception
                    WebUtil.ProcessException(ex)
                End Try

                Dim strUserInfo As String = "Domain: " & cpUser.Domain & "<br />" _
                    & "User ID: " & cpUser.UserId & "<br />" _
                    & "User Name: " & cpUser.UserName & "<br />" _
                    & "Org Code: " & cpUser.OrgCode & "<br />" _
                    & "Role: " & cpUser.Role & "<br />" _
                    & "Auth Type: " & cpUser.AuthType & "<br /><br />" _
                    & "Request.UserHostAddress: " & Request.UserHostAddress & "<br />" _
                    & "Request.ServerVariables('REMOTE_HOST'): " & Request.ServerVariables("REMOTE_HOST") & "<br />" _
                    & "GetHostEntry: " & strHostName

                WebUtil.WriteLog(strUserInfo.Replace("<br />", vbCrLf))
                lblUserInfo.Text = strUserInfo

                If Not CType(User, CustomPrincipal).IsInRole("SA") Then
                    divGetHostname.Visible = False
                End If
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub

    Protected Sub btnGetHostName_Click(sender As Object, e As EventArgs)
        Try
            Dim strHostName As String = ""
            strHostName = System.Net.Dns.GetHostEntry(txtIPAddress.Text).HostName

            lblHostname.Text = strHostName

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub
End Class