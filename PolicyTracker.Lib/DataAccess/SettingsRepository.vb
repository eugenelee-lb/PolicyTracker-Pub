Imports System.Data.Objects
Imports System.Linq.Dynamic

Public Class SettingsRepository
    Implements IDisposable, ISettingsRepository

    Private context As New PTEntities()

    Public Sub New()
        'context.CommonCodes.MergeOption = MergeOption.NoTracking
        'context.Preferences.MergeOption = MergeOption.NoTracking
        'context.Messages.MergeOption = MergeOption.NoTracking
        'context.Departments.MergeOption = MergeOption.NoTracking
        'context.Configurations.MergeOption = MergeOption.NoTracking
        'context.USStates.MergeOption = MergeOption.NoTracking
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

#Region "CommonCode methods"
    Public Function GetCommonCodes() As System.Collections.Generic.IEnumerable(Of CommonCode) Implements ISettingsRepository.GetCommonCodes
        Return context.CommonCodes.ToList()
    End Function

    Public Function GetCommonCodes(sortExpression As String) As System.Collections.Generic.IEnumerable(Of CommonCode) Implements ISettingsRepository.GetCommonCodes
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "CmCatg"
        End If
        Return context.CommonCodes.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetCommonCodesByCatg(sortExpression As String, catg As String) As System.Collections.Generic.IEnumerable(Of CommonCode) Implements ISettingsRepository.GetCommonCodesByCatg
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "DispOrder"
        End If
        If String.IsNullOrWhiteSpace(catg) Then
            catg = ""
        End If
        Return context.CommonCodes.OrderBy("it." & sortExpression).Where(Function(a) a.CmCatg.Equals(catg)).ToList()

    End Function

    Public Function GetCommonCodesByCatgDesc(sortExpression As String, catg As String, desc As String, stat As String) As System.Collections.Generic.IEnumerable(Of CommonCode) Implements ISettingsRepository.GetCommonCodesByCatgDesc
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "DispOrder"
        End If

        Dim qry As IQueryable(Of CommonCode) = context.CommonCodes.OrderBy("it." & sortExpression).AsQueryable

        If Not String.IsNullOrWhiteSpace(catg) Then
            qry = qry.Where(Function(c) c.CmCatg.Equals(catg))
        End If
        If Not String.IsNullOrWhiteSpace(desc) Then
            qry = qry.Where(Function(c) c.CmCodeDesc.ToUpper.Contains(desc.ToUpper))
        End If

        If stat = "A" Then
            qry = qry.Where(Function(a) a.Disabled = False)
        ElseIf stat = "D" Then
            qry = qry.Where(Function(a) a.Disabled = True)
        End If

        Return qry.ToList()

    End Function

    Public Function GetCommonCodesByCatgStatus(sortExpression As String, catg As String, stat As Boolean) As System.Collections.Generic.IEnumerable(Of CommonCode) Implements ISettingsRepository.GetCommonCodesByCatgStatus
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "DispOrder"
        End If
        If String.IsNullOrWhiteSpace(catg) Then
            catg = ""
        End If
        stat = Not stat
        Return context.CommonCodes.OrderBy("it." & sortExpression).Where(Function(a) a.CmCatg.Equals(catg) And a.Disabled.Equals(stat)).ToList()

    End Function

    Public Function GetCommonCodesByCatgStatusWithCodeDesc(catg As String, stat As Boolean) As System.Collections.Generic.IEnumerable(Of Object)
        If String.IsNullOrWhiteSpace(catg) Then
            catg = ""
        End If
        stat = Not stat

        Dim qry = (From a In context.CommonCodes
                   Where a.CmCatg = catg And a.Disabled = stat
                   Select a.CmCatg, a.CmCode, CmCodeDesc = a.CmCode & " - " & a.CmCodeDesc, a.DispOrder
                   Order By DispOrder)
        Return qry.ToList()
    End Function

    Public Function GetCommonCodeByCMId(cmId As Integer) As CommonCode Implements ISettingsRepository.GetCommonCodeByCMId
        Return context.CommonCodes.Where(Function(a) a.CmId.Equals(cmId)).FirstOrDefault()
    End Function

    Public Function GetCommonCodeByCatgCode(catg As String, code As String) As CommonCode Implements ISettingsRepository.GetCommonCodeByCatgCode
        Return context.CommonCodes.Where(Function(a) a.CmCatg.Equals(catg) And a.CmCode.Equals(code)).FirstOrDefault()
    End Function

    Public Function InsertCommonCode(commonCode As CommonCode) As Integer Implements ISettingsRepository.InsertCommonCode
        context.CommonCodes.Add(commonCode)
        context.SaveChanges()
        Return commonCode.CmId
    End Function

    Public Sub DeleteCommonCode(commonCode As CommonCode) Implements ISettingsRepository.DeleteCommonCode
        context.CommonCodes.Attach(commonCode)
        context.CommonCodes.Remove(commonCode)
        SaveChanges()
    End Sub

    Public Sub UpdateCommonCode(commonCode As CommonCode, origCommonCode As CommonCode) Implements ISettingsRepository.UpdateCommonCode
        context.CommonCodes.Attach(origCommonCode)
        'context.ApplyCurrentValues("CommonCodes", commonCode)
        context.Entry(origCommonCode).CurrentValues.SetValues(commonCode)
        SaveChanges()
    End Sub

#End Region

#Region "Preference methods"
    Public Function GetPreferences(sortExpression As String) As System.Collections.Generic.IEnumerable(Of Preference) Implements ISettingsRepository.GetPreferences
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "Catg"
        End If
        Return context.Preferences.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetPreferenceByCatgUserId(catg As String, userId As String) As Preference Implements ISettingsRepository.GetPreferenceByCatgUserId
        Return context.Preferences.Where(Function(a) a.Catg.Equals(catg) And a.UserId.Equals(userId)).FirstOrDefault()
    End Function

    Public Function GetPreferenceValue(catg As String, userId As String) As String Implements ISettingsRepository.GetPreferenceValue
        Dim pf = (From p In context.Preferences
                Where p.Catg.Equals(catg) And p.UserId.Equals(userId)
                Select p).FirstOrDefault()
        If pf Is Nothing Then
            Return ""
        Else
            If String.IsNullOrEmpty(pf.Val) Then
                Return ""
            Else
                Return pf.Val
            End If
        End If
    End Function

    Public Sub InsertPreference(preference As Preference) Implements ISettingsRepository.InsertPreference
        context.Preferences.Add(preference)
        context.SaveChanges()
    End Sub

    Public Sub UpdatePreference(preference As Preference, origPreference As Preference) Implements ISettingsRepository.UpdatePreference
        context.Preferences.Attach(origPreference)
        'context.ApplyCurrentValues("Preferences", preference)
        context.Entry(origPreference).CurrentValues.SetValues(preference)
        SaveChanges()
    End Sub

    Public Sub DeletePreference(preference As Preference) Implements ISettingsRepository.DeletePreference
        context.Preferences.Attach(preference)
        context.Preferences.Remove(preference)
        SaveChanges()
    End Sub

#End Region

#Region "Message methods"
    Public Function GetMessageByMsgNo(msgNo As Integer) As Message Implements ISettingsRepository.GetMessageByMsgNo
        Return context.Messages.Where(Function(a) a.MsgNo.Equals(msgNo)).FirstOrDefault()
    End Function

    Public Function GetMessages(sortExpression As String) As System.Collections.Generic.IEnumerable(Of Message) Implements ISettingsRepository.GetMessages
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "MsgNo"
        End If
        Return context.Messages.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetMessagesByTextAndTitle(sortExpression As String, msgText As String, msgTitle As String) As System.Collections.Generic.IEnumerable(Of Message) Implements ISettingsRepository.GetMessagesByTextAndTitle
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "MsgNo"
        End If

        Dim qryMessage As IQueryable(Of Message) = context.Messages.OrderBy("it." & sortExpression).AsQueryable

        If Not String.IsNullOrEmpty(msgText) Then
            qryMessage = qryMessage.Where(Function(c) c.MsgText.ToUpper.Contains(msgText.ToUpper))
        End If
        If Not String.IsNullOrEmpty(msgTitle) Then
            qryMessage = qryMessage.Where(Function(c) c.MsgTitle.ToUpper.Contains(msgTitle.ToUpper))
        End If
        Return qryMessage.ToList()
    End Function

    Public Sub DeleteMessage(message As Message) Implements ISettingsRepository.DeleteMessage
        context.Messages.Attach(message)
        context.Messages.Remove(message)
        SaveChanges()
    End Sub

    Public Sub InsertMessage(message As Message) Implements ISettingsRepository.InsertMessage
        context.Messages.Add(message)
        context.SaveChanges()
    End Sub

    Public Sub UpdateMassage(message As Message, origMessage As Message) Implements ISettingsRepository.UpdateMassage
        context.Messages.Attach(origMessage)
        'context.ApplyCurrentValues("Messages", message)
        context.Entry(origMessage).CurrentValues.SetValues(message)
        SaveChanges()
    End Sub

#End Region

#Region "Department methods"
    Public Function GetDepartments(sortExpression As String) As System.Collections.Generic.IEnumerable(Of Department) Implements ISettingsRepository.GetDepartments
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "DeptCode"
        End If
        Return context.Departments.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetDepartmentsByStat(sortExpression As String, stat As String) As IEnumerable(Of Department) Implements ISettingsRepository.GetDepartmentsByStat
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "DeptCode"
        End If

        Dim qry As IQueryable(Of Department) = context.Departments.OrderBy("it." & sortExpression).AsQueryable

        If stat = "A" Then
            qry = qry.Where(Function(a) a.Disabled = False)
        ElseIf stat = "D" Then
            qry = qry.Where(Function(a) a.Disabled = True)
        End If

        Return qry.ToList()
    End Function

    Public Function GetDeptByDeptCode(deptCode As String) As Department Implements ISettingsRepository.GetDeptByDeptCode
        Return context.Departments.Where(Function(a) a.DeptCode.Equals(deptCode)).FirstOrDefault()
    End Function

    Public Sub InsertDepartment(department As Department) Implements ISettingsRepository.InsertDepartment
        context.Departments.Add(department)
        context.SaveChanges()
    End Sub

    Public Sub UpdateDepartment(department As Department, origDepartment As Department) Implements ISettingsRepository.UpdateDepartment
        context.Departments.Attach(origDepartment)
        'context.ApplyCurrentValues("Departments", department)
        context.Entry(origDepartment).CurrentValues.SetValues(department)
        SaveChanges()
    End Sub

    Public Sub DeleteDepartment(department As Department) Implements ISettingsRepository.DeleteDepartment
        context.Departments.Attach(department)
        context.Departments.Remove(department)
        SaveChanges()
    End Sub

#End Region

#Region "Configuration"
    Public Function GetConfigurations(sortExpression As String) As System.Collections.Generic.IEnumerable(Of Configuration) Implements ISettingsRepository.GetConfigurations
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "ConfigKey"
        End If
        Return context.Configurations.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetConfigurationsByKeyDesc(sortExpression As String, key As String, desc As String) As System.Collections.Generic.IEnumerable(Of Configuration) Implements ISettingsRepository.GetConfigurationsByKeyDesc
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "ConfigKey"
        End If

        Dim qry As IQueryable(Of Configuration) = context.Configurations.OrderBy("it." & sortExpression).AsQueryable

        If Not String.IsNullOrEmpty(key) Then
            qry = qry.Where(Function(c) c.ConfigKey.ToUpper.Contains(key.ToUpper))
        End If
        If Not String.IsNullOrEmpty(desc) Then
            qry = qry.Where(Function(c) c.ConfigDesc.ToUpper.Contains(desc.ToUpper))
        End If
        Return qry.ToList()
    End Function

    Public Function GetConfigurationByKey(key As String) As Configuration Implements ISettingsRepository.GetConfigurationByKey
        Return context.Configurations.Where(Function(a) a.ConfigKey.ToUpper.Equals(key.ToUpper)).FirstOrDefault()
    End Function

    Public Sub InsertConfiguration(configuration As Configuration) Implements ISettingsRepository.InsertConfiguration
        context.Configurations.Add(configuration)
        context.SaveChanges()
    End Sub

    Public Sub UpdateConfiguration(configuration As Configuration, origConfiguration As Configuration) Implements ISettingsRepository.UpdateConfiguration
        context.Configurations.Attach(origConfiguration)
        'context.ApplyCurrentValues("Configurations", configuration)
        context.Entry(origConfiguration).CurrentValues.SetValues(configuration)
        SaveChanges()
    End Sub

    Public Sub DeleteConfiguration(configuration As Configuration) Implements ISettingsRepository.DeleteConfiguration
        context.Configurations.Attach(configuration)
        context.Configurations.Remove(configuration)
        SaveChanges()
    End Sub

#End Region

#Region "USStates"
    Public Function GetUSStatesByCount(sortExpression As String, count As Integer) As System.Collections.Generic.IEnumerable(Of USState) Implements ISettingsRepository.GetUSStatesByCount
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "Id"
        End If
        Return context.USStates.OrderBy("it." & sortExpression).Where(Function(a) a.Id <= count).ToList()
    End Function
#End Region

#Region "Notices"
    Public Function GetNotices(sortExpression As String) As System.Collections.Generic.IEnumerable(Of Notice) Implements ISettingsRepository.GetNotices
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "NoticeId desc"
        End If
        Return context.Notices.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetNoticesActive() As IEnumerable(Of Notice) Implements ISettingsRepository.GetNoticesActive
        Dim notices = (From n In context.Notices
                       Where n.Disabled = False And n.StartDate < Now() And n.EndDate > Now()
                       Select n
                       Order By n.DispOrder).ToList()
        Return notices
    End Function

    Public Function GetNoticeById(id As Integer) As Notice Implements ISettingsRepository.GetNoticeById
        Return context.Notices.Where(Function(a) a.NoticeId.Equals(id)).FirstOrDefault()
    End Function

    Public Function InsertNotice(notice As Notice) As Integer Implements ISettingsRepository.InsertNotice
        context.Notices.Add(notice)
        context.SaveChanges()
        Return notice.NoticeId
    End Function

    Public Sub UpdateNotice(notice As Notice, origNotice As Notice) Implements ISettingsRepository.UpdateNotice
        context.Notices.Attach(origNotice)
        'context.ApplyCurrentValues("Notices", notice)
        context.Entry(origNotice).CurrentValues.SetValues(notice)
        SaveChanges()
    End Sub

    Public Sub DeleteNotice(notice As Notice) Implements ISettingsRepository.DeleteNotice
        context.Notices.Attach(notice)
        context.Notices.Remove(notice)
        SaveChanges()
    End Sub
#End Region

#Region "AppUser methods"

    Public Function GetAppUsers(sortExpression As String) As System.Collections.Generic.IEnumerable(Of AppUser) Implements ISettingsRepository.GetAppUsers
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "UserId"
        End If
        Return context.AppUsers.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function SearchAppUsersByRoleName(sortExpression As String, role As String, name As String) As System.Collections.Generic.IEnumerable(Of AppUser) Implements ISettingsRepository.SearchAppUsersByRoleName
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "UserId"
        End If

        Dim qry As IQueryable(Of AppUser) = context.AppUsers.OrderBy("it." & sortExpression).AsQueryable

        If Not (role = "ALL") Then
            qry = qry.Where(Function(c) c.UserRole.Equals(role))
        End If
        If Not String.IsNullOrWhiteSpace(name) Then
            qry = qry.Where(Function(c) c.UserName.ToUpper.Contains(name.ToUpper))
        End If
        Return qry.ToList()
    End Function

    Public Function GetAppUserByID(userId As String) As AppUser Implements ISettingsRepository.GetAppUserByID
        Return context.AppUsers.Where(Function(a) a.UserId.Equals(userId)).FirstOrDefault()
    End Function

    Public Sub InsertAppUser(appUser As AppUser) Implements ISettingsRepository.InsertAppUser
        context.AppUsers.Add(appUser)
        context.SaveChanges()
    End Sub

    Public Sub UpdateAppUser(appUser As AppUser, origAppUser As AppUser) Implements ISettingsRepository.UpdateAppUser
        context.AppUsers.Attach(origAppUser)
        'context.ApplyCurrentValues("AppUsers", appUser)
        context.Entry(origAppUser).CurrentValues.SetValues(appUser)
        SaveChanges()
    End Sub

    Public Sub DeleteAppUser(appUser As AppUser) Implements ISettingsRepository.DeleteAppUser
        context.AppUsers.Attach(appUser)
        context.AppUsers.Remove(appUser)
        SaveChanges()
    End Sub

#End Region

#Region "UserOrg methods"
    Public Function GetUserOrg(userID As String, orgCode As String) As UserOrg Implements ISettingsRepository.GetUserOrg
        Return context.UserOrgs.Where(Function(a) a.UserId.ToUpper.Equals(userID.ToUpper) And a.OrgCode.Equals(orgCode)).FirstOrDefault()
    End Function

    Public Sub InsertUserOrg(userOrg As UserOrg) Implements ISettingsRepository.InsertUserOrg
        Dim strUserId As String = userOrg.UserId.Split("|")(0)
        Dim strUserName As String = userOrg.UserId.Split("|")(1)
        userOrg.UserId = strUserId

        ' add AppUser when it's new
        Dim usr = (From u In context.AppUsers
                   Where u.UserId = userOrg.UserId
                   Select u).FirstOrDefault()
        If usr Is Nothing Then
            usr = New AppUser
            With usr
                .UserId = strUserId
                .UserName = strUserName
                .UserRole = userOrg.AccessLevel
                .CreateUser = userOrg.CreateUser
                .CreateDT = Now()
                .LastUpdateUser = userOrg.LastUpdateUser
                .LastUpdateDT = Now()
            End With
            context.AppUsers.Add(usr)
        End If
        context.UserOrgs.Add(userOrg)
        context.SaveChanges()
    End Sub

    Public Sub UpdateUserOrg(userOrg As UserOrg, origUserOrg As UserOrg) Implements ISettingsRepository.UpdateUserOrg
        context.UserOrgs.Attach(origUserOrg)
        context.Entry(origUserOrg).CurrentValues.SetValues(userOrg)
        SaveChanges()
    End Sub

    Public Sub DeleteUserOrg(userOrg As UserOrg) Implements ISettingsRepository.DeleteUserOrg
        context.UserOrgs.Attach(userOrg)
        context.UserOrgs.Remove(userOrg)

        ' delete AppUser with DA role
        Dim usr = (From u In context.AppUsers
                   Where u.UserId = userOrg.UserId
                   Select u).FirstOrDefault()
        If usr Is Nothing Then
            Throw New ApplicationException("Cound not find User [" & userOrg.UserId & "].")
        End If
        If usr.UserOrgs.Count = 0 And usr.UserRole = userOrg.AccessLevel Then
            context.AppUsers.Attach(usr)
            context.AppUsers.Remove(usr)
        End If

        SaveChanges()
    End Sub
#End Region

#Region "Bureau methods"
    Public Function GetBureaus(sortExpression As String) As IEnumerable(Of Bureau) Implements ISettingsRepository.GetBureaus
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "BurCode"
        End If
        Return context.Bureaus.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetBureausByDeptDescStat(sortExpression As String, deptCode As String, burDesc As String, stat As String) As IEnumerable(Of Bureau) Implements ISettingsRepository.GetBureausByDeptDescStat
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "BurCode"
        End If

        Dim qry As IQueryable(Of Bureau) = context.Bureaus.OrderBy("it." & sortExpression).AsQueryable

        If Not String.IsNullOrWhiteSpace(deptCode) Then
            qry = qry.Where(Function(a) a.DeptCode.Equals(deptCode))
        End If
        If Not String.IsNullOrWhiteSpace(burDesc) Then
            qry = qry.Where(Function(a) a.BurDesc.ToUpper.Contains(burDesc.ToUpper))
        End If

        If stat = "A" Then
            qry = qry.Where(Function(a) a.Disabled = False)
        ElseIf stat = "D" Then
            qry = qry.Where(Function(a) a.Disabled = True)
        End If

        Return qry.ToList()
    End Function

    Public Function GetBureauByBurCode(burCode As String) As Bureau Implements ISettingsRepository.GetBureauByBurCode
        Return context.Bureaus.Where(Function(a) a.BurCode.Equals(burCode)).FirstOrDefault()
    End Function

    Public Sub InsertBureau(bureau As Bureau) Implements ISettingsRepository.InsertBureau
        context.Bureaus.Add(bureau)
        context.SaveChanges()
    End Sub

    Public Sub UpdateBureau(bureau As Bureau, origBureau As Bureau) Implements ISettingsRepository.UpdateBureau
        context.Bureaus.Attach(origBureau)
        context.Entry(origBureau).CurrentValues.SetValues(bureau)
        SaveChanges()
    End Sub

    Public Sub DeleteBureau(bureau As Bureau) Implements ISettingsRepository.DeleteBureau
        context.Bureaus.Attach(bureau)
        context.Bureaus.Remove(bureau)
        SaveChanges()
    End Sub
#End Region

#Region "Division methods"
    Public Function GetDivisions(sortExpression As String) As IEnumerable(Of Division) Implements ISettingsRepository.GetDivisions
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "DivCode"
        End If
        Return context.Divisions.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetDivisionsByDeptBurDescStat(sortExpression As String, deptCode As String, burCode As String, divDesc As String, stat As String) As IEnumerable(Of Division) Implements ISettingsRepository.GetDivisionsByDeptBurDescStat
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "DivCode"
        End If

        Dim qry As IQueryable(Of Division) = context.Divisions.OrderBy("it." & sortExpression).AsQueryable

        If Not String.IsNullOrWhiteSpace(deptCode) Then
            qry = qry.Where(Function(a) a.DivCode.Substring(0, 2).Equals(deptCode))
        End If
        If Not String.IsNullOrWhiteSpace(burCode) Then
            qry = qry.Where(Function(a) a.BurCode.Equals(burCode))
        End If
        If Not String.IsNullOrWhiteSpace(divDesc) Then
            qry = qry.Where(Function(a) a.DivDesc.ToUpper.Contains(divDesc.ToUpper))
        End If

        If stat = "A" Then
            qry = qry.Where(Function(a) a.Disabled = False)
        ElseIf stat = "D" Then
            qry = qry.Where(Function(a) a.Disabled = True)
        End If

        Return qry.ToList()
    End Function

    Public Function GetDivisionByDivCode(divCode As String) As Division Implements ISettingsRepository.GetDivisionByDivCode
        Return context.Divisions.Where(Function(a) a.DivCode.Equals(divCode)).FirstOrDefault()
    End Function

    Public Sub InsertDivision(division As Division) Implements ISettingsRepository.InsertDivision
        context.Divisions.Add(division)
        context.SaveChanges()
    End Sub

    Public Sub UpdateDivision(division As Division, origDivision As Division) Implements ISettingsRepository.UpdateDivision
        context.Divisions.Attach(origDivision)
        context.Entry(origDivision).CurrentValues.SetValues(division)
        SaveChanges()
    End Sub

    Public Sub DeleteDivision(division As Division) Implements ISettingsRepository.DeleteDivision
        context.Divisions.Attach(division)
        context.Divisions.Remove(division)
        SaveChanges()
    End Sub

    Public Function GetOrgsByCodeDescStat(sortExpression As String, orgCode As String, orgDesc As String, stat As String) As IEnumerable(Of vOrganization) Implements ISettingsRepository.GetOrgsByCodeDescStat
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "OrgCode"
        End If

        Dim qry As IQueryable(Of vOrganization) = context.vOrganizations.OrderBy("it." & sortExpression).AsQueryable

        If Not String.IsNullOrWhiteSpace(orgCode) Then
            qry = qry.Where(Function(a) a.OrgCode.StartsWith(orgCode))
        End If
        If Not String.IsNullOrWhiteSpace(orgDesc) Then
            qry = qry.Where(Function(a) a.OrgDesc.ToUpper.Contains(orgDesc.ToUpper))
        End If

        If stat = "A" Then
            qry = qry.Where(Function(a) a.Disabled = False)
        ElseIf stat = "D" Then
            qry = qry.Where(Function(a) a.Disabled = True)
        End If

        Return qry.ToList()
    End Function
#End Region


#Region "Classification methods"
    Public Function GetClassifications(sortExpression As String) As IEnumerable(Of Classification) Implements ISettingsRepository.GetClassifications
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "ClassCode"
        End If
        Return context.Classifications.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetClassificationsByCodeDescTypeStat(sortExpression As String, classCode As String, classDesc As String, classType As String, stat As String) As IEnumerable(Of Classification) Implements ISettingsRepository.GetClassificationsByCodeDescTypeStat
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "ClassCode"
        End If

        Dim qry As IQueryable(Of Classification) = context.Classifications.OrderBy("it." & sortExpression).AsQueryable

        If Not String.IsNullOrEmpty(classCode) Then
            qry = qry.Where(Function(a) a.ClassCode.ToUpper.StartsWith(classCode.ToUpper))
        End If

        If Not String.IsNullOrWhiteSpace(classDesc) Then
            qry = qry.Where(Function(a) a.ClassDesc.ToUpper.Contains(classDesc.ToUpper))
        End If

        If Not String.IsNullOrEmpty(classType) Then
            qry = qry.Where(Function(a) a.ClassType = classType)
        End If

        If stat = "A" Then
            qry = qry.Where(Function(a) a.Disabled = False)
        ElseIf stat = "D" Then
            qry = qry.Where(Function(a) a.Disabled = True)
        End If

        Return qry.ToList()
    End Function

    Public Function GetClassificationsByStatWithCodeDesc(sortExpression As String, stat As String) As IEnumerable(Of Object) Implements ISettingsRepository.GetClassificationsByStatWithCodeDesc
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "ClassCode"
        End If

        Dim qry As IQueryable(Of Classification) = context.Classifications.OrderBy("it." & sortExpression).AsQueryable

        If stat = "A" Then
            qry = qry.Where(Function(a) a.Disabled = False)
        ElseIf stat = "D" Then
            qry = qry.Where(Function(a) a.Disabled = True)
        End If

        Dim lst = (From c In qry
                   Select c.ClassCode, c.ClassDesc, ClassCodeDesc = c.ClassCode & " - " & c.ClassDesc).ToList()

        Return lst
    End Function

    Public Function GetClassificationByClassCode(classCode As String) As Classification Implements ISettingsRepository.GetClassificationByClassCode
        Return context.Classifications.Where(Function(a) a.ClassCode.Equals(classCode)).FirstOrDefault()
    End Function

    Public Sub InsertClassification(classification As Classification) Implements ISettingsRepository.InsertClassification
        context.Classifications.Add(classification)
        context.SaveChanges()
    End Sub

    Public Sub UpdateClassification(classification As Classification, origClassification As Classification) Implements ISettingsRepository.UpdateClassification
        context.Classifications.Attach(origClassification)
        context.Entry(origClassification).CurrentValues.SetValues(classification)
        SaveChanges()
    End Sub

    Public Sub DeleteClassification(classification As Classification) Implements ISettingsRepository.DeleteClassification
        context.Classifications.Attach(classification)
        context.Classifications.Remove(classification)
        SaveChanges()
    End Sub

    Public Function GetClassTypes() As IEnumerable(Of Object) Implements ISettingsRepository.GetClassTypes
        Dim qry = From ct In context.Classifications _
                  Group ct By Key = New With {ct.ClassType} _
                  Into ctGroup = Group _
                  Select Key

        Return qry.OrderBy("ClassType").ToList()
    End Function
#End Region

#Region "Employee methods"
    Public Function GetEmployees(sortExpression As String) As IEnumerable(Of Employee) Implements ISettingsRepository.GetEmployees
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "EmpId"
        End If
        Return context.Employees.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetVEmployeesByNameOrgClassStat(sortExpression As String, name As String, orgCode As String, classCode As String, stat As String) As IEnumerable(Of vEmployee) Implements ISettingsRepository.GetVEmployeesByNameOrgClassStat
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "EmpId"
        End If

        Dim qry As IQueryable(Of vEmployee) = context.vEmployees.OrderBy("it." & sortExpression).AsQueryable

        If Not String.IsNullOrWhiteSpace(name) Then
            qry = qry.Where(Function(a) a.FirstName.Contains(name) Or a.LastName.Contains(name) Or a.MiddleName.Contains(name))
        End If
        If Not String.IsNullOrWhiteSpace(orgCode) Then
            qry = qry.Where(Function(a) a.OrgCode.StartsWith(orgCode))
        End If
        If Not String.IsNullOrWhiteSpace(classCode) Then
            qry = qry.Where(Function(a) a.ClassCode.StartsWith(classCode))
        End If

        If stat = "A" Then
            qry = qry.Where(Function(a) a.Disabled = False)
        ElseIf stat = "D" Then
            qry = qry.Where(Function(a) a.Disabled = True)
        End If

        Return qry.ToList()
    End Function

    Public Function GetEmployeeByEmpId(empId As String) As Employee Implements ISettingsRepository.GetEmployeeByEmpId
        Return context.Employees.Where(Function(a) a.EmpId.Equals(empId)).FirstOrDefault()
    End Function
#End Region

    Public Sub SaveChanges()
        Try
            context.SaveChanges()
        Catch ocex As OptimisticConcurrencyException
            Dim ctx = CType(context, Entity.Infrastructure.IObjectContextAdapter).ObjectContext
            ctx.Refresh(Entity.Core.Objects.RefreshMode.StoreWins, ocex.StateEntries(0).Entity)
            Throw ocex
        End Try
    End Sub

End Class
