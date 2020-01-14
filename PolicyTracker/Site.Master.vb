Imports PolicyTracker.Lib

Public Class SiteMaster
    Inherits MasterPage

    Public ActiveMenu As String
    Public ActiveSubMenu As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            If Not Page.IsPostBack Then
                lnkAppName.InnerText = ConfigurationManager.AppSettings("ApplName")

                ' login status for Forms Auth
                If Context.User.Identity.IsAuthenticated Then
                    Dim cp As CustomPrincipal = CType(HttpContext.Current.User, CustomPrincipal)
                    userName.InnerText = cp.UserName
                    If cp.AuthType = "Forms" Then
                        lblPipe.Text = "|"
                        LoginStatus1.Visible = True
                    End If
                End If

                ' role base menus
                If Page.User.IsInRole("SA") Then
                    liSettings.Visible = True
                End If
                If Page.User.IsInRole("PA,SA") Then
                    liPolicyAdmin.Visible = True
                End If
                If Page.User.IsInRole("OA,PA,SA") Then
                    liReleases.Visible = True
                End If
                If Page.User.IsInRole("OA,SA") Then
                    liPreferences.Visible = True
                End If
                If Page.User.IsInRole("USER,OA,PA,SA") Then
                    liMyPackets.Visible = True
                End If

                ' active menu
                For Each ctl In ulNavBar.Controls
                    If TypeOf ctl Is HtmlGenericControl Then
                        Dim li As HtmlGenericControl = CType(ctl, HtmlGenericControl)
                        If li.ID = "li" & ActiveMenu Then
                            li.Attributes.Add("class", "active")
                        Else
                            li.Attributes.Remove("class")
                        End If
                        ' active sub menu
                        For Each ctl1 In li.Controls
                            If TypeOf ctl1 Is HtmlGenericControl Then
                                Dim li1 As HtmlGenericControl = CType(ctl1, HtmlGenericControl)
                                If li1.ID = "li" & ActiveSubMenu Then
                                    li1.Attributes.Add("class", "active")
                                Else
                                    li1.Attributes.Remove("class")
                                End If
                            End If
                        Next
                    End If
                Next

                ' Notices
                Dim blSettings As New SettingsBL
                Dim notices = blSettings.GetNoticesActive()
                For Each n In notices
                    noticeMessage.InnerHtml &= "<li>" & n.NoticeText & "</li>"
                Next
                If notices.Count > 0 Then
                    noticeAlert.Visible = True
                    noticeMessage.InnerHtml = "<ul>" & noticeMessage.InnerHtml & "</ul>"
                End If
            End If

        Catch ex As Exception
            WebUtil.ProcessException(ex)
        End Try
    End Sub
End Class