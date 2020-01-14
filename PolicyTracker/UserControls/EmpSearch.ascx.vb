Public Class EmpSearch
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            If CType(Page.User, CustomPrincipal).IsInRole("SA") Then
                ddlFilterOrg.Items.Insert(0, New ListItem("-- All --", ""))
            End If
        End If
    End Sub

    Protected Sub odsOrg_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs)
        e.InputParameters("userRole") = CType(Page.User, CustomPrincipal).Role
        e.InputParameters("userId") = CType(Page.User, CustomPrincipal).UserId
    End Sub

    Private Sub gvEmployees_DataBound(sender As Object, e As System.EventArgs) Handles gvEmployees.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        gv.SelectedIndex = -1
    End Sub

    Protected Function GetAnchor() As String
        Dim strEmpId As String = Eval("EmpId").ToString.Trim
        Dim strFName As String = Eval("FirstName").ToString.Trim
        Dim strMI As String = Eval("MiddleName")
        Dim strLName As String = Eval("LastName").ToString.Trim
        Dim strEmpName As String = strFName
        'If Not String.IsNullOrEmpty(strMI) Then strEmpName &= " " & strMI
        If Not String.IsNullOrEmpty(strLName) Then strEmpName &= " " & strLName
        strEmpName = WebUtil.JSStringEncode(strEmpName)

        Dim strRtn As String
        strRtn = "<a href=""javascript:SelectEmp('" & strEmpId & "','" & strEmpName & "')"">" & strEmpId & "</a>"

        Return strRtn
    End Function

    Private Sub odsEmployees_Selected(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles odsEmployees.Selected
        ViewState("gvRecordCount") = ""
        If e.ReturnValue IsNot Nothing Then
            If CType(e.ReturnValue, IEnumerable(Of Object)) IsNot Nothing Then
                Dim totalCount As Integer = CType(e.ReturnValue, IEnumerable(Of Object)).Count
                ViewState("gvRecordCount") = "Total rows: " & totalCount.ToString()
            End If
        End If
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        gvEmployees.Visible = True
    End Sub

End Class