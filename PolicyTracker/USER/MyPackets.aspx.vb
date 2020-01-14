Imports PolicyTracker.Lib
Imports System.Web.ModelBinding

Public Class MyPackets
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Page.Title = "My Packets - " & ConfigurationManager.AppSettings("ApplName")
        Master.ActiveMenu = "MyPackets"
        Master.ActiveSubMenu = ""

        If Not Page.IsPostBack Then
            gvPackets.Sort("ReleaseDate", SortDirection.Descending)
        End If
    End Sub

    Public Function gvPackets_GetData(<Control> ddlFilterStat As Boolean) As IQueryable(Of PolicyTracker.Lib.vPacket)
        Dim db = New PTEntities
        Dim qry = (From a In db.vPackets
                    Where a.RecipientId.ToUpper = CType(User, CustomPrincipal).UserId.ToUpper _
                    And a.AckDT.HasValue = Not ddlFilterStat
                    Select a)

        Dim cnt = qry.Count()
        ViewState("gvRecordCount") = "Total rows: " & cnt.ToString()

        Return qry
    End Function

    Private Sub gvPackets_DataBound(sender As Object, e As EventArgs) Handles gvPackets.DataBound
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

End Class