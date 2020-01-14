Imports PolicyTracker.Lib

Public Class RecipGroupMembers
    Inherits System.Web.UI.UserControl
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Public Sub GetRecipGroupMembers(RecipGroupId As Integer)
        gvEmployees.Visible = False
        ViewState("RecipGroupId") = RecipGroupId.ToString
        gvEmployees.DataBind()
        gvEmployees.Visible = True
    End Sub

    ' The return type can be changed to IEnumerable, however to support
    ' paging and sorting, the following parameters must be added:
    '     ByVal maximumRows as Integer
    '     ByVal startRowIndex as Integer
    '     ByRef totalRowCount as Integer
    '     ByVal sortByExpression as String
    Public Function gvEmployees_GetData() As IQueryable(Of PolicyTracker.Lib.vEmployee)
                                                     
        Dim db = New PTEntities
        Dim intRecipGroupId = Integer.Parse(ViewState("RecipGroupId"))
        Dim eg = (From e In db.RecipGroups
                    Where e.RecipGroupId = intRecipGroupId
                    Select e).FirstOrDefault()
        If eg Is Nothing Then
            Throw New ApplicationException("Recipient Group [" & intRecipGroupId.ToString & "] is not found.")
        End If
        'If eg.RecipGroupType <> "ATTRIB" Then
        '    Throw New ApplicationException("Recipient Group [" & intRecipGroupId.ToString & "] is not Attribute Type.")
        'End If

        Dim qry = (From e In db.vEmployees
                   Where e.Disabled = False
                   Select e)
        If eg.RecipGroupType = "ATTRIB" Then
            Dim aOrgs = (From org In eg.RecipGroupOrgs, div In db.Divisions
                         Where div.DivCode.StartsWith(org.OrgCode)
                         Select div.DivCode).ToList()
            Dim aClasses = (From cls In eg.Classifications
                            Select cls.ClassCode).ToList()

            If aOrgs.Count = 0 And aClasses.Count = 0 Then
                ' No Org/Class
                qry = qry.Where(Function(e) 1 = 0)
            End If
            If aOrgs.Count > 0 Then
                qry = qry.Where(Function(e) aOrgs.Contains(e.OrgCode))
            End If
            If aClasses.Count > 0 Then
                qry = qry.Where(Function(e) aClasses.Contains(e.ClassCode))
            End If

        ElseIf eg.RecipGroupType = "IND" Then
            Dim aMembers = (From e In eg.Employees Select e.EmpId).ToList()
            qry = qry.Where(Function(e) aMembers.Contains(e.EmpId))
        End If

        ViewState("gvRecordCount") = "Total rows: " & qry.Count.ToString()
        ViewState("gvCaption") = eg.GroupName
        Return qry

    End Function

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

        gv.Caption = ViewState("gvCaption")
    End Sub
End Class