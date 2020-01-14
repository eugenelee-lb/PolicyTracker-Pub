Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports PolicyTracker.Lib.PTBL

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class PacketService
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function UpdateRecipientView(relId As Integer, recipId As String, clientIP As String) As PacketUpdateResult
        Dim bl = New PolicyTracker.Lib.PTBL()
        Return bl.UpdateRecipientView(relId, recipId, clientIP)
    End Function

End Class