Imports PolicyTracker.Lib
Imports System.DirectoryServices
Imports System.Globalization

Public Class UserAuth

    Public Shared Function Authenticate(usr As String, pwd As String) As String
        ' *** Forms auth is for HR employees (1. without AD account, 2. with AD account last logon is over 30 days ago)
        Dim strAuthType As String = "Forms"
        usr = usr.Trim.ToUpper
        Dim strUserName As String = ""
        Dim strLastLogon As String = ""

        ' HR employee active
        Dim daSettings As New SettingsRepository
        Dim emp As Employee = daSettings.GetEmployeeByEmpId(usr)
        If emp Is Nothing Then
            Throw New ApplicationException(SettingsBL.GetMessageText(-7, usr) & " [UNEHR]") 'User Not Employee in HR - you are not permitted to log in the system. Please contact system administrator.
        End If
        If emp.Disabled Then
            Throw New ApplicationException(SettingsBL.GetMessageText(-7, usr) & " [UDHR]") ' User Disabled in HR - you are not permitted to log in the system. Please contact system administrator.
        End If

        If pwd <> emp.PIN.Trim() Then
            Throw New ApplicationException(SettingsBL.GetMessageText(-3) & " [PNM] " & usr) ' Password Not Match - Log in incorrect. [{0}] 
        End If

        ' LI all staff use Kiosk
        If Not PTBL.KioskOnlyOrg(emp.OrgCode) Then
            ' Directory search
            Dim _ADPath As String = ConfigurationManager.AppSettings.Get("ADPath")
            Dim _ADUser As String = ConfigurationManager.AppSettings.Get("ADUser")
            Dim _ADPassword As String = ConfigurationManager.AppSettings.Get("ADPassword")

            Dim de As DirectoryEntry = New DirectoryEntry(_ADPath, _ADUser, _ADPassword, AuthenticationTypes.Secure)
            Dim filter As String = "(sAMAccountName=" & usr & ")"
            Dim propertiesToLoad As String() = New String(7) {
                "sAMAccountName", "distinguishedName", "cn", "sn", "givenName", "initials",
                "employeeNumber", "lastLogon"}
            Dim deSearch As DirectorySearcher = New DirectorySearcher(filter, propertiesToLoad)
            deSearch.SearchRoot = de
            deSearch.SearchScope = SearchScope.Subtree
            deSearch.PageSize = 10
            Dim colSR As SearchResultCollection

            colSR = deSearch.FindAll()
            If colSR.Count > 0 Then ' AD account exists
                Dim sr As SearchResult = colSR.Item(0)

                strLastLogon = GetResultProperty(sr, "lastLogon")
                If strLastLogon <> "" Then
                    Dim datLastLogon As Date
                    Dim culture As CultureInfo
                    Dim styles As DateTimeStyles

                    ' Parse a date and time with no styles.
                    'dateString = "03/01/2009 10:00 AM"
                    culture = CultureInfo.CreateSpecificCulture("en-US")
                    styles = DateTimeStyles.AssumeLocal
                    If Not DateTime.TryParse(strLastLogon, culture, styles, datLastLogon) Then
                        Throw New ApplicationException(SettingsBL.GetMessageText(-5) & " [LLDPS] " & usr) ' Last Logon Date Parse Error - Log in error.
                    End If
                    If datLastLogon > Today.AddDays(-30) Then
                        Throw New ApplicationException(SettingsBL.GetMessageText(-8, usr, ConfigurationManager.AppSettings("SITE_URL").Replace(".Kiosk", "")) & " [LLDIN]") ' Last Logon Date Within 30 days - you are not permitted to log in the site. Please try this URL [].
                    End If
                End If
            End If
        End If

        strUserName = emp.FirstName & " " & emp.LastName

        Return usr & "|" & strAuthType & "|" & emp.EmpId & "|" & strUserName & "|" & emp.FirstName & "|" & emp.LastName & "|" & emp.MiddleName
    End Function

    Public Shared Function GetResultProperty(ByRef res As SearchResult, ByVal PropertyName As String) As String
        If res.Properties(PropertyName).Count < 1 Then Return ""

        Select Case res.Properties(PropertyName)(0).GetType.FullName
            Case "System.Object[]"
                Dim strVal As String = ""
                Dim obj() As System.Object = CType(res.Properties(PropertyName)(0), Object())
                For ii As Integer = 0 To obj.Length - 1
                    strVal &= obj(ii).ToString()
                    If ii < obj.Length - 1 Then strVal &= "|"
                Next
                Return strVal

            Case "System.Int64"
                Try
                    Return DateTime.FromFileTime(res.Properties(PropertyName)(0)).ToString()
                Catch ex As Exception
                    Return "[LongToDate parse error]" 'ex.Message
                End Try

            Case "System.__ComObject"
                Try
                    Dim largeInt As Object = res.Properties(PropertyName)(0)
                    Dim highPart As Integer = CInt(largeInt.GetType.InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, Nothing, largeInt, Nothing))
                    Dim lowPart As Integer = CInt(largeInt.GetType.InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty, Nothing, largeInt, Nothing))
                    Dim lngVal As Long = highPart * (UInt32.MaxValue + 1) - lowPart
                    Return DateTime.FromFileTime(lngVal).ToString()
                Catch ex As Exception
                    Return "[LongToDate parse error]" 'ex.Message
                End Try

            Case "System.Byte[]"
                Dim strVal As String = ""
                Dim byteVal() As Byte = CType(res.Properties(PropertyName)(0), Byte())
                For ii As Integer = 0 To byteVal.Length - 1
                    strVal &= byteVal(ii).ToString()
                    If ii < byteVal.Length - 1 Then strVal &= "|"
                Next
                Return strVal
            Case Else
                Return res.Properties(PropertyName)(0).ToString()
        End Select
    End Function

    Public Shared Function NeedUserIDVerify(userOrg As String) As Boolean
        Dim strOrgsUserIDVerify As String = ConfigurationManager.AppSettings.Get("OrgsUserIDVerify")
        Dim listOrgs As String() = strOrgsUserIDVerify.Split(",")
        For Each org In listOrgs
            If userOrg.StartsWith(org) Then Return True
        Next
        Return False
    End Function

End Class
