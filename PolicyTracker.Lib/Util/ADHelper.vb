Imports System.DirectoryServices

Public Class ADHelper
    Friend Enum UAC
        ADS_UF_SCRIPT = &H1                             ' The logon script will be executed
        ADS_UF_ACCOUNTDISABLE = &H2                     ' Disable user account
        ADS_UF_HOMEDIR_REQUIRED = &H8                   ' Requires a root directory
        ADS_UF_LOCKOUT = &H10                           ' Account is locked out
        ADS_UF_PASSWD_NOTREQD = &H20                    ' No password is required
        ADS_UF_PASSWD_CANT_CHANGE = &H40                ' The user cannot change the password
        ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = &H80   ' Encrypted password allowed
        ADS_UF_TEMP_DUPLICATE_ACCOUNT = &H100           ' Local user account
        ADS_UF_NORMAL_ACCOUNT = &H200                   ' Typical user account
        'INTERDOMAIN_TRUST_ACCOUNT 0x0800
        'WORKSTATION_TRUST_ACCOUNT 0x1000
        'SERVER_TRUST_ACCOUNT 0x2000
        'DONT_EXPIRE_PASSWORD 0x10000
        'MNS_LOGON_ACCOUNT 0x20000
        'SMARTCARD_REQUIRED 0x40000
        'TRUSTED_FOR_DELEGATION 0x80000
        'NOT_DELEGATED 0x100000
        'USE_DES_KEY_ONLY 0x200000
        'DONT_REQ_PREAUTH 0x400000
        'PASSWORD_EXPIRED 0x800000
        'TRUSTED_TO_AUTH_FOR_DELEGATION 0x1000000
    End Enum

    Public Shared Function GetProperty(ByRef oDE As DirectoryEntry, ByVal PropertyName As String) As String
        If oDE.Properties.Contains(PropertyName) Then
            Select Case oDE.Properties(PropertyName).Value.GetType.FullName
                Case "System.Object[]"
                    Dim strVal As String = ""
                    Dim obj() As System.Object = oDE.Properties(PropertyName).Value
                    For ii As Integer = 0 To obj.Length - 1
                        strVal &= obj(ii).ToString()
                        If ii < obj.Length - 1 Then strVal &= "|"
                    Next
                    Return strVal

                Case "System.__ComObject"
                    'Return oDE.Properties(PropertyName)(0).GetType.FullName
                    Try
                        ' https://www.crammysblog.com/checking-ldap-usnchanged-integer8-value-in-vb-net/
                        Dim largeInt As Object = oDE.Properties(PropertyName).Value
                        Dim highPart As Integer = largeInt.GetType.InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, Nothing, largeInt, Nothing)
                        Dim lowPart As Integer = largeInt.GetType.InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty, Nothing, largeInt, Nothing)
                        'Dim lngVal As Long = highPart * (UInt32.MaxValue + 1) - lowPart ' This throws exception in parsing uSNCreated porperty: Not a valid Win32 FileTime.
                        Dim lngVal As Long = (Convert.ToInt64(highPart) << 32) + Convert.ToInt64(lowPart)
                        Return DateTime.FromFileTime(lngVal).ToString

                        'Dim highpart, lowpart, lngVal As Long
                        'highpart = oDE.Properties(PropertyName)(0).HighPart
                        'lowpart = oDE.Properties(PropertyName)(0).Lowpart
                        'lngVal = (highpart * 2 ^ 32) - lowpart
                        'Return DateTime.FromFileTime(lngVal).ToString
                    Catch ex As Exception
                        'Return "[LongToDate parse error]" 'ex.Message
                        Throw ex
                    End Try

                Case "System.Byte[]"
                    Dim strVal As String = ""
                    Dim byteVal() As Byte = oDE.Properties(PropertyName).Value
                    For ii As Integer = 0 To byteVal.Length - 1
                        strVal &= byteVal(ii).ToString()
                        If ii < byteVal.Length - 1 Then strVal &= "|"
                    Next
                    Return strVal
                Case Else
                    Return oDE.Properties(PropertyName)(0).ToString()
            End Select
            'Return oDE.Properties(PropertyName)(0).ToString()
        Else
            Return String.Empty '"[Property not found]" 
        End If
    End Function

    Public Shared Function IsDisabled(ByVal userAccountControl As Object) As Boolean
        Dim intUAC As Integer = Fix(userAccountControl)
        Return ((intUAC And UAC.ADS_UF_ACCOUNTDISABLE) = UAC.ADS_UF_ACCOUNTDISABLE)
    End Function

End Class
