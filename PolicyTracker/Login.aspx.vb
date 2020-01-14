Imports PolicyTracker.Lib

Public Class Login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Master.TabName = ""
        Page.Title = "Log In - " & ConfigurationManager.AppSettings("ApplName")
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Try
            ' Additional Userdata
            Dim strAuthInfo As String = UserAuth.Authenticate(txtUserID.Text, txtPassword.Text)

            If Request.QueryString("ReturnUrl") IsNot Nothing AndAlso Request.QueryString("ReturnUrl").StartsWith("/SignUp") Then
                FormsAuthentication.SetAuthCookie(strAuthInfo, False)
                Response.Redirect("~/Default", True)
            Else
                FormsAuthentication.RedirectFromLoginPage(strAuthInfo, False)
            End If

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub
End Class