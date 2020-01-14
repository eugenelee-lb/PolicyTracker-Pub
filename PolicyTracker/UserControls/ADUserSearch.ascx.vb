Imports ADWrapper

Public Class ADUserSearch
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Function GetAnchor() As String
        Dim strUserId As String = WebUtil.JSStringEncode(Eval("UserName").ToString()).Trim
        Dim strFName As String = WebUtil.JSStringEncode(Eval("FirstName").ToString()).Trim
        Dim strMI As String = WebUtil.JSStringEncode(Eval("MiddleInitial").ToString()).Trim
        Dim strLName As String = WebUtil.JSStringEncode(Eval("LastName").ToString()).Trim
        Dim strTitle As String = WebUtil.JSStringEncode(Eval("Title").ToString())
        Dim strEmail As String = WebUtil.JSStringEncode(Eval("Email").ToString())
        Dim strUserName As String = strFName
        If Not String.IsNullOrEmpty(strMI) Then strUserName &= " " & strMI
        If Not String.IsNullOrEmpty(strLName) Then strUserName &= " " & strLName

        Dim strRtn As String
        strRtn = "<a href=""javascript:SelectUser('" & strUserId & "','" & strUserName & "')"">" & strUserId & "</a>"

        Return strRtn
    End Function

    Private Function GetFilterString() As String
        Dim filter As String = [String].Empty
        If txtLogonName.Text.Trim() <> String.Empty Then
            filter = filter & "(sAMAccountName=*" & txtLogonName.Text.Trim() & "*)"
        End If
        If txtFirstName.Text.Trim() <> String.Empty Then
            filter = filter & "(givenName=*" & txtFirstName.Text.Trim() & "*)"
        End If
        If txtLastName.Text.Trim() <> String.Empty Then
            filter = filter & "(sn=*" & txtLastName.Text.Trim() & "*)"
        End If
        Return filter
    End Function

    Private Sub BindUsers()
        Dim userArr As ADUserArrList = New ADUserArrList
        Dim userList As ArrayList
        Dim adMan As ADManager

        adMan = ADManager.Instance()

        Dim OUPath As String = dropDept.SelectedValue
        If OUPath = "All" Or OUPath = "" Then
            userList = adMan.LoadAllUsers(GetFilterString())
        Else
            OUPath = "/ OU=" & OUPath & "," & ConfigurationManager.AppSettings("ADPathDC")
            userList = adMan.LoadDomainUsers(OUPath, GetFilterString())
        End If

        For ii As Integer = 0 To userList.Count - 1
            userArr.Add(userList.Item(ii))
        Next

        ' Call method to sort the data before databinding
        SortGridData(userArr, SortField, SortAscending)

        gridADUsers.DataSource = userArr
        gridADUsers.DataBind()

    End Sub

    Private Sub SortGridData(ByVal list As ADUserArrList, ByVal sortField As String, ByVal asc As Boolean)
        Dim sortCol As ADUserArrList.UserFields = ADUserArrList.UserFields.InitValue

        Select Case sortField
            Case "UserName"
                sortCol = ADUserArrList.UserFields.UserName
            Case "FirstName"
                sortCol = ADUserArrList.UserFields.FirstName
            Case "LastName"
                sortCol = ADUserArrList.UserFields.LastName
            Case "Email"
                sortCol = ADUserArrList.UserFields.Email
            Case "IsAccountActive"
                sortCol = ADUserArrList.UserFields.IsAccountActive
        End Select

        list.Sort(sortCol, asc)
    End Sub

    Private _UserIdField As String
    Public Property UserIdField() As String
        Get
            Return _UserIdField
        End Get
        Set(ByVal value As String)
            _UserIdField = value
        End Set
    End Property

    Private _UserNameField As String
    Public Property UserNameField() As String
        Get
            Return _UserNameField
        End Get
        Set(ByVal value As String)
            _UserNameField = value
        End Set
    End Property


    Property SortField() As String
        Get
            Dim o As Object = ViewState("SortField")
            If o Is Nothing Then
                Return String.Empty
            End If
            Return CStr(o)
        End Get

        Set(ByVal Value As String)
            If Value = SortField Then
                ' same as current sort file, toggle sort direction
                SortAscending = Not SortAscending
            End If
            ViewState("SortField") = Value
        End Set
    End Property

    Property SortAscending() As Boolean
        Get
            Dim o As Object = ViewState("SortAscending")
            If o Is Nothing Then
                Return True
            End If
            Return CBool(o)
        End Get

        Set(ByVal Value As Boolean)
            ViewState("SortAscending") = Value
        End Set
    End Property

    Protected Sub gridADUsers_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles gridADUsers.DataBound
        WebUtil.SetGridRowStyle(gridADUsers)
    End Sub


    Protected Sub gridUsrs_PageIndexChanging(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewPageEventArgs) Handles gridADUsers.PageIndexChanging
        gridADUsers.PageIndex = e.NewPageIndex
        BindUsers()
    End Sub

    Protected Sub gridUsrs_Sorting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewSortEventArgs) Handles gridADUsers.Sorting
        SortField = e.SortExpression
        BindUsers()
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        Try
            gridADUsers.PageIndex = 0
            BindUsers()

        Catch ex As Exception
            lblError.Visible = True
            lblError.Text = WebUtil.ProcessException(ex)

        End Try
    End Sub
End Class