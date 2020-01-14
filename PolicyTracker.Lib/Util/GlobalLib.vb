Imports Microsoft.VisualBasic
Imports System.Diagnostics
Imports System.IO
Imports System.Configuration
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Net.Mail

'*********************************************************************
'
' GlobalLib Class
'
' Contains static properties which are used globally throughout
' the application.
'
'*********************************************************************

Public Class GlobalLib

    Public Shared Function TrimOrNull(ByVal str As String) As String
        If String.IsNullOrWhiteSpace(str) Then
            Return Nothing
        Else
            Return str.Trim
        End If
    End Function

    ' Convert message text to display on web pages: HtmlEncode and replace VbCrLf with <br /> 
    Public Shared Function HtmlMsgEncode(ByVal msgText As String) As String
        ' Encode the string input
        Dim sb As StringBuilder = New StringBuilder(System.Net.WebUtility.HtmlEncode(msgText))

        ' Selectively allow  <b> and <i>
        sb.Replace("&lt;b&gt;", "<b>")
        sb.Replace("&lt;/b&gt;", "</b>")
        sb.Replace("&lt;i&gt;", "<i>")
        sb.Replace("&lt;/i&gt;", "</i>")

        ' Replace CRLF with <br />
        sb.Replace(vbCrLf, "<br />")
        sb.Replace(vbLf, "<br />")
        sb.Replace(vbCr, "<br />")

        Return sb.ToString()
    End Function

    Public Shared Function ToCapitalizedName(ByVal orgName As String) As String
        Dim strResult As String = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(orgName.ToLower)
        strResult = Regex.Replace(strResult, "\bii\b", "II", RegexOptions.IgnoreCase)
        strResult = Regex.Replace(strResult, "\biii\b", "III", RegexOptions.IgnoreCase)
        strResult = Regex.Replace(strResult, "\biv\b", "IV", RegexOptions.IgnoreCase)

        Return strResult
    End Function

    Public Shared Function GetLastNameFromTiburon(ByVal name As String) As String
        If name.IndexOf(",") > 0 Then
            Return name.Substring(0, name.IndexOf(","))
        Else
            Return name
        End If
    End Function

    Public Shared Function GetFirstNameFromTiburon(ByVal name As String) As String
        If name.IndexOf(",") > 0 Then
            Return name.Substring(name.IndexOf(",") + 1)
        Else
            Return ""
        End If
    End Function

    Public Shared Function GetFullName(ByVal fn As String, mn As String, ln As String) As String
        Dim fullName As String = ""
        If Not String.IsNullOrWhiteSpace(fn) Then
            fullName &= fn.Trim()
        End If
        If Not String.IsNullOrWhiteSpace(mn) Then
            fullName &= " " & mn.Trim()
        End If
        If Not String.IsNullOrWhiteSpace(ln) Then
            fullName &= " " & ln.Trim()
        End If
        Return fullName.Trim()

    End Function

    Public Shared Function GetAge(ByVal Birthdate As System.DateTime, _
        Optional ByVal AsOf As System.DateTime = #1/1/1700#) As String

        'Don't set second parameter if you want Age as of today

        Dim iMonths As Integer
        Dim iYears As Integer
        Dim iDays As Integer

        If AsOf = "#1/1/1700#" Then
            AsOf = DateTime.Now
        End If

        iYears = AsOf.Year - Birthdate.Year
        If (Birthdate.AddYears(iYears) > AsOf) Then iYears = iYears - 1
        If iYears > 0 Then Return iYears.ToString

        iMonths = DateDiff(DateInterval.Month, Birthdate, AsOf)
        If (Birthdate.AddMonths(iMonths) > AsOf) Then iMonths = iMonths - 1
        If iMonths > 1 Then Return iMonths.ToString & " Months"
        If iMonths = 1 Then Return iMonths.ToString & " Month"

        iDays = DateDiff(DateInterval.Day, Birthdate, AsOf)
        If iDays > 1 Then Return iDays.ToString & " Days"
        Return iDays.ToString & " Day"

    End Function

    Public Shared Sub WriteLog(ByVal logText As String)
        ' File Log
        Dim strLog As String = String.Empty

        'Dim cpUser As CustomPrincipal
        'Try
        '    cpUser = CType(HttpContext.Current.User, CustomPrincipal)
        '    strLog &= "Domain\User, Role: " & cpUser.Domain & "\" & cpUser.UserId & " " & cpUser.UserName & ", " & cpUser.Role & vbCrLf
        'Catch ex1 As Exception
        'End Try

        'Dim session As System.Web.SessionState.HttpSessionState = HttpContext.Current.Session
        'strLog &= "Session Values" & vbCrLf
        'For ii As Integer = 0 To session.Count - 1
        '    If Not (session.Item(ii) Is Nothing) Then
        '        strLog &= session.Keys(ii).ToString & ":" & session.Item(ii).ToString & vbCrLf
        '    End If
        'Next
        'Dim request As System.Web.HttpRequest = HttpContext.Current.Request
        'strLog &= "PhysicalPath:" & request.PhysicalPath.ToString & vbCrLf _
        '    & "QueryString:" & request.QueryString.ToString & vbCrLf _
        '    & "UserHostAddress:" & request.UserHostAddress.ToString & vbCrLf _
        '    & "UserAgent: " & request.UserAgent & vbCrLf

        strLog &= logText

        Dim ERR_LOG_DIR As String = ConfigurationManager.AppSettings("LOG_DIR") 'request.PhysicalApplicationPath & "\ERR_LOG"
        'Dim ERR_LOG_DIR As String = request.PhysicalApplicationPath & "\App_Data"

        If Not Directory.Exists(ERR_LOG_DIR) Then
            Directory.CreateDirectory(ERR_LOG_DIR)
        End If
        Dim strFilePath As String = ERR_LOG_DIR & "\LOG_" & Now.ToString("yyyyMM") & ".txt"
        Dim sw As StreamWriter = File.AppendText(strFilePath)
        Try
            sw.Write(vbCrLf & DateTime.Now.ToString("G") & vbCrLf)
            sw.Write(strLog & vbCrLf)
        Finally
            ' Close the stream to the file.
            sw.Close()
        End Try

    End Sub

    Public Shared Function GetEmailAddress(ByVal empId As String) As String
        Dim svcCLBEmail As New CLBEmailService.CLBEmail()
        Dim strEmail As String = ""
        Try
            strEmail = svcCLBEmail.LookupAddress(empId)
        Catch ex As Exception
            'GlobalLib.ExceptionHandle(ex)
            GlobalLib.WriteLog("CLBEmail.LookupAddress returned exception for: " & empId)
            Return ""
        End Try
        Dim message As New MailMessage()
        'Dim isValidEmail As Boolean = False
        Try
            message.To.Add(strEmail)
            'isValidEmail = True
        Catch ex As Exception
            ' email address invalid
            GlobalLib.WriteLog("CLBEmail.LookupAddress returned invalid email for: " & empId & vbCrLf & _
                               "Return value: " & strEmail & vbCrLf & _
                               "Exception: " & ex.Message)
            Return ""
        End Try
        Return strEmail
    End Function
End Class
