Imports PolicyTracker.lib

Public Class GridViewPager
    Inherits System.Web.UI.UserControl

    Private _gridView As GridView

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim c As Control = Parent

        While (Not (c) Is Nothing)
            If (TypeOf c Is GridView) Then
                _gridView = CType(c, GridView)
                ' Read preference
                Dim _settingsBL As SettingsBL = New SettingsBL
                Dim _preference As Preference = _settingsBL.GetPreferencesByGatgUserId("RESULTS_PER_PAGE" & Me.Parent.Page.AppRelativeVirtualPath & ":" & _gridView.ID, CType(Context.User, CustomPrincipal).UserId)
                If _preference IsNot Nothing Then
                    Try
                        _gridView.PageSize = Integer.Parse(_preference.Val)
                    Catch ex As Exception
                    End Try
                End If
                ' End of read preference
                Exit While
            End If
            c = c.Parent
        End While
    End Sub

    Protected Sub TextBoxPage_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles TextBoxPage.TextChanged
        If (_gridView Is Nothing) Then
            Return
        End If
        Dim page As Integer
        If Integer.TryParse(TextBoxPage.Text.Trim, page) Then
            If (page <= 0) Then
                page = 1
            End If
            If (page > _gridView.PageCount) Then
                page = _gridView.PageCount
            End If
            _gridView.PageIndex = (page - 1)
        End If
        TextBoxPage.Text = (_gridView.PageIndex + 1).ToString
    End Sub

    Protected Sub DropDownListPageSize_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If (_gridView Is Nothing) Then
                Return
            End If
            Dim dropdownlistpagersize As DropDownList = CType(sender, DropDownList)
            Try
                _gridView.PageSize = Integer.Parse(dropdownlistpagersize.SelectedValue)
            Catch ex As Exception
            End Try

            ' Write preference
            Dim bllPreferences As New SettingsBL
            bllPreferences.WritePreference("RESULTS_PER_PAGE" & Me.Parent.Page.AppRelativeVirtualPath & ":" & _gridView.ID, CType(Context.User, CustomPrincipal).UserId, dropdownlistpagersize.SelectedValue, CType(Context.User, CustomPrincipal).UserId)
            ' End of write preference

            Dim pageindex As Integer = _gridView.PageIndex
            _gridView.DataBind()
            If (_gridView.PageIndex <> pageindex) Then
                'if page index changed it means the previous page was not valid and was adjusted. Rebind to fill control with adjusted page
                _gridView.DataBind()
            End If

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRender
        If (Not (_gridView) Is Nothing) Then
            LabelNumberOfPages.Text = _gridView.PageCount.ToString
            TextBoxPage.Text = (_gridView.PageIndex + 1).ToString
            DropDownListPageSize.SelectedValue = _gridView.PageSize.ToString
        End If
    End Sub

    Public Sub SetTotalRecordCount(ByVal totalCount As String)
        LabelTotalRecordCount.Text = totalCount
    End Sub

End Class