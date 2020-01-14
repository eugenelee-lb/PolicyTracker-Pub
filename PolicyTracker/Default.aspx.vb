Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If User.IsInRole("USER") Then
            Response.Redirect("~/User/MyPackets", True)
        ElseIf User.IsInRole("PA,OA") Then
            Response.Redirect("~/OA/Releases", True)
        ElseIf User.IsInRole("SA") Then
            Response.Redirect("~/PA/Schedules", True)
        Else
            Response.Redirect("~/Login")
        End If
    End Sub
End Class