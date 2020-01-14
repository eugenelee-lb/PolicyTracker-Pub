Imports System.Web.Optimization
Imports System.Security.Principal
Imports System.Threading

Public Class Global_asax
    Inherits HttpApplication

    Sub Application_Start(sender As Object, e As EventArgs)
        ' Fires when the application is started
        BundleConfig.RegisterBundles(BundleTable.Bundles)
        RouteConfig.RegisterRoutes(RouteTable.Routes)
    End Sub

    Private Sub Global_asax_EndRequest(sender As Object, e As EventArgs) Handles Me.EndRequest
        'Dim context As HttpContext = HttpContext.Current
        'If context.Response.Status.Substring(0, 3).Equals("401") Then
        '    context.Response.ClearContent()
        '    context.Response.Write("<script language='javascript'>self.location='" & Server.MapPath("~/Error401.htm") & "';<" & "/script>")
        'End If
    End Sub

    Private Sub Global_asax_Error(sender As Object, e As EventArgs) Handles Me.Error
        ' Fires when an error occurs
        Dim ex As Exception = Server.GetLastError()
        WebUtil.ExceptionHandle(ex)
    End Sub

    Private Sub Global_asax_PostAuthenticateRequest(sender As Object, e As EventArgs) Handles Me.PostAuthenticateRequest
        ' Get a reference to the current User
        Dim usr As IPrincipal = HttpContext.Current.User

        ' If we are dealing with an authenticated forms authentication request
        If usr.Identity.IsAuthenticated Then

            If usr.Identity.AuthenticationType = "Forms" Then
                Dim ticket As FormsAuthenticationTicket = FormsAuthentication.Decrypt(
                Request.Cookies(FormsAuthentication.FormsCookieName).Value)
                ' Create the CustomPrincipal
                Dim cp As New CustomPrincipal(ticket.Name)

                ' Attach the CustomPrincipal to HttpContext.User and Thread.CurrentPrincipal
                HttpContext.Current.User = cp
                Thread.CurrentPrincipal = cp

            Else 'If usr.Identity.AuthenticationType = "Negotiate" Then
                ' Create the CustomPrincipal
                Dim cp As New CustomPrincipal(User.Identity)

                ' Attach the CustomPrincipal to HttpContext.User and Thread.CurrentPrincipal
                HttpContext.Current.User = cp
                Thread.CurrentPrincipal = cp
            End If
        End If
    End Sub
End Class