Imports PolicyTracker.Lib
Imports System.Net.Mail

Public Class EmpInfo
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private _empId As String

    ' The id parameter should match the DataKeyNames value set on the control
    ' or be decorated with a value provider attribute, e.g. <QueryString>ByVal id as Integer
    Public Function fvEmployee_GetItem() As Object

        Using ctx = New PolicyTracker.Lib.PTEntities
            Dim em = (From emp In ctx.vEmployees
                        Where emp.EmpId = _empId
                        Select emp).FirstOrDefault()
            If em Is Nothing Then
                Throw New ApplicationException("Employee [" & _empId & "] is not found.")
            End If
            Return em
        End Using

    End Function

    Public Sub GetEmpInfo(empId As String)
        _empId = empId
        fvEmployee.Visible = False
        fvEmployee.DataBind()
        fvEmployee.Visible = True
    End Sub

    Protected Function EvalEmail() As String
        Dim svcCLBEmail As New CLBEmailService.CLBEmail()
        Dim strEmpId As String = Eval("EmpId")
        Dim strMailTo As String = ""
        Try
            strMailTo = svcCLBEmail.LookupAddress(strEmpId)
        Catch ex As Exception
            GlobalLib.WriteLog("CLBEmail.LookupAddress returned exception for: " & strEmpId)
        End Try
        Dim message As New MailMessage()
        Dim isValidEmail As Boolean = False
        Try
            message.To.Add(strMailTo)
            isValidEmail = True
        Catch ex As Exception
            ' email address invalid
            GlobalLib.WriteLog("CLBEmail.LookupAddress returned invalid email for: " & strEmpId & vbCrLf & _
                               "Return value: " & strMailTo & vbCrLf & _
                               "Exception: " & ex.Message)
            'strMailTo = ""
        End Try
        Return strMailTo
    End Function
End Class