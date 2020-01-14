Imports Microsoft.VisualBasic
Imports System.Security.Principal
Imports ADWrapper
Imports PolicyTracker.Lib

'*********************************************************************
'
' CustomPrincipal Class
'
' The CustomPrincipal class implements the IPrincipal interface so it
' can be used in place of the GenericPrincipal object.  Requirements for
' implementing the IPrincipal interface include implementing the
' IIdentity interface and an implementation for IsInRole.  The custom
' principal is attached to the current request in Global.asax in the
' Authenticate_Request event handler.  The user's role is stored in the
' custom principal object in the Global_AcquireRequestState event handler.
'
'*********************************************************************
Public Class CustomPrincipal
    Implements IPrincipal

    ' Required to implement the IPrincipal interface.
    Protected _Identity As IIdentity
    Protected _Role As String
    Protected _Domain As String
    Protected _AuthType As String
    Protected _UserId As String
    Protected _UserName As String
    Protected _FirstName As String
    Protected _LastName As String
    Protected _MI As String
    Protected _OrgCode As String

    'Protected _DID As String

    Public Sub New(ByVal ticketUserInfo As String)
        Dim arrUserInfo() As String = ticketUserInfo.Split("|")
        _Domain = arrUserInfo(0)
        _AuthType = arrUserInfo(1)
        _UserId = arrUserInfo(2)
        _UserName = arrUserInfo(3)
        _FirstName = arrUserInfo(4)
        _LastName = arrUserInfo(5)
        _MI = arrUserInfo(6)

        _Identity = New GenericIdentity(_UserId)

        _Role = "USER" ' default 

    End Sub

    Public Sub New(ByVal identity As IIdentity)
        _AuthType = "Windows"
        _Identity = identity
        _Role = "" ' default to empty

        Dim strIdentityName As String = _Identity.Name

        ' Change User for Development/Debug
        If ConfigurationManager.AppSettings("DevUserID").ToLower.IndexOf(strIdentityName.ToLower) >= 0 Then
            If ConfigurationManager.AppSettings("DevUserIDChange") <> "" Then
                strIdentityName = ConfigurationManager.AppSettings("DevUserIDChange")
            Else
                _Role = "SA" ' System Admin
            End If
        End If

        _Domain = strIdentityName.Substring(0, strIdentityName.LastIndexOf("\"))
        _UserId = strIdentityName.Substring(strIdentityName.LastIndexOf("\") + 1, strIdentityName.Length - strIdentityName.LastIndexOf("\") - 1)

        Dim bllSettings As New SettingsBL
        ' SA/PA/OA
        If _Role = "" Then
            Dim appUser = bllSettings.GetAppUserByID(_UserId)
            If appUser IsNot Nothing Then
                _Role = appUser.UserRole
            End If
        End If
        ' OA --> PA
        If _Role = "OA" Then
            Dim orgs = bllSettings.GetOrgsForPA("PA", _UserId)
            If orgs IsNot Nothing AndAlso orgs.Count > 0 Then _Role = "PA"
        End If

        ' HR active employee
        If ConfigurationManager.AppSettings("MonitorUserID").ToLower.IndexOf(strIdentityName.ToLower) >= 0 Then
            _UserName = _UserId
            _FirstName = ""
            _LastName = _UserId
            _MI = ""
        Else
            Dim daSettings As New SettingsRepository
            Dim emp As Employee = daSettings.GetEmployeeByEmpId(_UserId)

            If emp Is Nothing Then
                Throw New ApplicationException(SettingsBL.GetMessageText(-7, _UserId) & " [UNEHR]") 'User Not Employee in HR - you are not permitted to log in the system. Please contact system administrator.
            End If
            If emp.Disabled Then
                Throw New ApplicationException(SettingsBL.GetMessageText(-7, _UserId) & " [UDHR]") ' User Disabled in HR - you are not permitted to log in the system. Please contact system administrator.
            End If

            _FirstName = emp.FirstName
            _LastName = emp.LastName
            _MI = emp.MiddleName
            If String.IsNullOrWhiteSpace(_MI) Then
                _UserName = emp.FirstName + " " + emp.LastName
            Else
                _UserName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName
            End If
            _OrgCode = emp.OrgCode
        End If

        ' AD account
        If _Domain.ToUpper = "CLB" Then
            'Dim adUser As ADUser
            'Dim adMan As ADManager
            'adMan = ADManager.Instance()
            'If ADManager.UserExists(_UserId) Then
            '    adUser = adMan.LoadUser(_UserId)
            '    '_UserName = adUser.FirstName + " " + adUser.LastName
            '    '_FirstName = adUser.FirstName
            '    '_LastName = adUser.LastName
            '    '_MI = adUser.MiddleInitial
            '    If Not adUser.IsAccountActive Then
            '        Throw New ApplicationException(SettingsBL.GetMessageText(-7, _UserId) & " [DAD]") ' User Disabled in AD - you are not permitted to log in the system. Please contact system administrator.
            '    End If
            'End If
            If _Role = "" Then _Role = "USER"

        ElseIf _Domain.ToUpper = "POLB" Then ' HARBOR
            If _Role = "" Then _Role = "USER"

        ElseIf _Domain.ToUpper = "LBPL" Then ' LIBRARY
            If _Role = "" Then _Role = "USER"

        ElseIf _Domain.ToUpper = "WD" Then ' WATER
            If _Role = "" Then _Role = "USER"

        End If

    End Sub

    Public Sub New(ByVal domain As String, ByVal username As String)
        _Domain = domain
        _UserId = username
        _UserName = username
        _Role = ""
        _FirstName = ""
        _LastName = ""
        _MI = ""
        '_DID = ""
    End Sub

    ' IIdentity property used to retrieve the Identity object attached to this principal.
    Public ReadOnly Property Identity() As IIdentity Implements IPrincipal.Identity
        Get
            Return _Identity
        End Get
    End Property

    Public ReadOnly Property Domain() As String
        Get
            Return _Domain
        End Get
    End Property

    Public ReadOnly Property UserId() As String
        Get
            Return _UserId
        End Get
    End Property

    Public ReadOnly Property UserName() As String
        Get
            Return _UserName
        End Get
    End Property

    Public ReadOnly Property FirstName() As String
        Get
            Return _FirstName
        End Get
    End Property

    Public ReadOnly Property LastName() As String
        Get
            Return _LastName
        End Get
    End Property

    Public ReadOnly Property MI() As String
        Get
            Return _MI
        End Get
    End Property

    Public ReadOnly Property Role() As String
        Get
            Return _Role
        End Get
    End Property

    Public ReadOnly Property AuthType() As String
        Get
            Return _AuthType
        End Get
    End Property

    Public ReadOnly Property OrgCode() As String
        Get
            Return _OrgCode
        End Get
    End Property

    '*********************************************************************
    ' Checks to see if the current user is a member of AT LEAST ONE of
    ' the roles in the role string.  Returns true if found, otherwise false.
    ' role is a comma-delimited list of role IDs.
    '*********************************************************************
    Public Function IsInRole(ByVal role As String) As Boolean Implements IPrincipal.IsInRole
        Dim roleArray As String() = role.Split(New Char() {","c})

        Dim r As String
        For Each r In roleArray
            If _Role = r Then
                Return True
            End If
        Next r
        Return False
    End Function 'IsInRole

End Class
