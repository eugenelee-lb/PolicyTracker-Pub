Imports PolicyTracker.Lib
Imports System.Web.ModelBinding

Public Class Preferences
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Preferences - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Preferences"
        Master.ActiveSubMenu = ""

        If Not Page.IsPostBack Then
            Try
                Dim blSettings As New SettingsBL
                Dim blnDoNotSendNoti As Boolean = False
                Dim strVal As String = blSettings.GetPreferenceValue(SettingsBL.PREF_DONOT_SEND_NOTI, CType(User, CustomPrincipal).UserId)
                If Not String.IsNullOrEmpty(strVal) Then
                    blnDoNotSendNoti = Boolean.Parse(strVal)
                End If

                chbPrefNoti.Checked = blnDoNotSendNoti

            Catch ex As Exception
                lblError.Visible = True
                lblError.Text = WebUtil.ProcessException(ex)
            End Try
        End If

    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        Try
            Dim blSettings As New SettingsBL
            blSettings.WritePreference(SettingsBL.PREF_DONOT_SEND_NOTI, CType(User, CustomPrincipal).UserId, chbPrefNoti.Checked.ToString, CType(User, CustomPrincipal).UserId)

            lblInfo.Visible = True
            lblInfo.Text = WebUtil.HtmlMsgEncode(SettingsBL.GetMessageText(12)) ' Record Updated

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)
        End Try
    End Sub
End Class