Imports PolicyTracker.Lib
Imports System.Web.ModelBinding

Public Class Releases
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "Releases - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "Releases"
        Master.ActiveSubMenu = ""

        If Not Page.IsPostBack Then
            Me.Form.DefaultButton = btnSearch.UniqueID

            txtFilterRelDateFrom.Text = Today.AddDays(-30).ToString("MM/dd/yyyy")
            ddlFilterOrg.DataBind()

            gvReleases.Sort("ReleaseDate", SortDirection.Descending)
        End If
    End Sub

    Private Sub ddlFilterOrg_DataBound(sender As Object, e As EventArgs) Handles ddlFilterOrg.DataBound
        If CType(User, CustomPrincipal).IsInRole("SA") Then
            ddlFilterOrg.Items.Insert(0, New ListItem("-- All --", ""))
        End If
    End Sub

    Protected Sub odsOrg_Selecting(sender As Object, e As ObjectDataSourceSelectingEventArgs)
        e.InputParameters("userRole") = CType(Page.User, CustomPrincipal).Role
        e.InputParameters("userId") = CType(Page.User, CustomPrincipal).UserId
    End Sub

    Public Function gvReleases_GetData(<Control> txtFilterSubject As String, _
                                       <Control> txtFilterRelDateFrom As String, <Control> txtFilterRelDateTo As String, _
                                       <Control> ddlFilterOrg As String) As IQueryable(Of PolicyTracker.Lib.Release)
        Dim db = New PolicyTracker.Lib.PTEntities
        Dim qry = (From s In db.Releases
                   Select s)
        ' org
        If ddlFilterOrg <> "" Then
            qry = qry.Where(Function(a) (From r In a.ReleaseRecipients
                                         Where r.OrgCode.StartsWith(ddlFilterOrg)).Any())
        End If

        ' filter subject
        If Not String.IsNullOrWhiteSpace(txtFilterSubject) Then
            qry = qry.Where(Function(a) (a.Subject.Contains(txtFilterSubject)))
        End If
        ' filter release date
        Dim datFrom As Date
        If Date.TryParse(txtFilterRelDateFrom, datFrom) Then
            qry = qry.Where(Function(a) (a.ReleaseDate >= datFrom))
        End If
        Dim datTo As Date
        If Date.TryParse(txtFilterRelDateTo, datTo) Then
            qry = qry.Where(Function(a) (a.ReleaseDate <= datTo))
        End If

        ViewState("gvRecordCount") = "Total rows: " & qry.Count.ToString()

        Return qry
    End Function

    Private Sub gvReleases_DataBound(sender As Object, e As EventArgs) Handles gvReleases.DataBound
        Dim gv As WebControls.GridView = sender
        WebUtil.SetGridRowStyle(gv)
        If gv.Rows.Count > 0 Then
            gv.BottomPagerRow.Visible = True
            ' record count
            Dim pager As GridViewPager = gv.BottomPagerRow.FindControl("GridViewPager1")
            pager.SetTotalRecordCount(ViewState("gvRecordCount"))
        End If

        'gv.SelectedIndex = -1
        'dvSchedule.ChangeMode(DetailsViewMode.Insert)
        'dvSchedule_ModeChanged(Nothing, Nothing)
    End Sub

End Class