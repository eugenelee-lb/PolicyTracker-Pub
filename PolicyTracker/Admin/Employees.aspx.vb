Imports PolicyTracker.Lib
Imports System.Web.ModelBinding

Public Class Employees
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Employees - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Settings"
        Master.ActiveSubMenu = "Employees"
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
        'dvEmployee.ChangeMode(DetailsViewMode.Insert)
        'dvEmployee_ModeChanged(Nothing, Nothing)

    End Sub

    Protected Sub gvEmployees_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles gvEmployees.SelectedIndexChanged
        'dvEmployee.ChangeMode(DetailsViewMode.ReadOnly)
        'dvEmployee_ModeChanged(Nothing, Nothing)

        WebUtil.SetGridRowStyle(gvEmployees)
    End Sub

    'Private Sub odsEmployees_Selected(sender As Object, e As ObjectDataSourceStatusEventArgs) Handles odsEmployees.Selected
    '    ViewState("gvRecordCount") = ""
    '    If e.ReturnValue IsNot Nothing Then
    '        If CType(e.ReturnValue, IEnumerable(Of Object)) IsNot Nothing Then
    '            Dim totalCount As Integer = CType(e.ReturnValue, IEnumerable(Of Object)).Count
    '            ViewState("gvRecordCount") = "Total rows: " & totalCount.ToString()
    '        End If
    '    End If
    'End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        gvEmployees.DataBind()
        gvEmployees.Visible = True

    End Sub

    Public Function gvEmployees_GetData(<Control> txtFilterName As String,
                                        <Control> ddlFilterStat As String,
                                        <Control("lstFilterOrg", "Items")> filterOrgItems As ListItemCollection,
                                        <Control("lstFilterClass", "Items")> filterClassItems As ListItemCollection) As IQueryable(Of PolicyTracker.Lib.vEmployee)
        Dim db = New PolicyTracker.Lib.PTEntities
        Dim qry = (From e In db.vEmployees
                   Select e
                   Order By e.EmpId).AsQueryable
        ' filter name
        If Not String.IsNullOrWhiteSpace(txtFilterName) Then
            qry = qry.Where(Function(a) a.FirstName.Contains(txtFilterName) Or a.LastName.Contains(txtFilterName) Or a.MiddleName.Contains(txtFilterName))
        End If

        ' filter org
        Dim lstOrg As New List(Of String)
        For Each li As ListItem In filterOrgItems
            If li.Selected Then lstOrg.Add(li.Value)
        Next
        Dim aOrgs = (From org In lstOrg, div In db.Employees
                     Where div.OrgCode.StartsWith(org)
                     Select div.OrgCode).ToList().Distinct()
        If aOrgs.Count > 0 Then qry = qry.Where(Function(a) (aOrgs.Contains(a.OrgCode)))

        ' filter class
        Dim lstClass As New List(Of String)
        For Each li As ListItem In filterClassItems
            If li.Selected Then lstClass.Add(li.Value)
        Next
        Dim aClasses = (From cls In lstClass, [class] In db.Employees
                        Where [class].ClassCode.StartsWith(cls)
                        Select [class].ClassCode).ToList().Distinct()
        If aClasses.Count > 0 Then qry = qry.Where(Function(a) (aClasses.Contains(a.ClassCode)))

        ' status
        If ddlFilterStat = "A" Then
            qry = qry.Where(Function(a) a.Disabled = False)
        ElseIf ddlFilterStat = "D" Then
            qry = qry.Where(Function(a) a.Disabled = True)
        End If

        ViewState("gvRecordCount") = "Total rows: " & qry.Count.ToString()

        Return qry
    End Function
End Class