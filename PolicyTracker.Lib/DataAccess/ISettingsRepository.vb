Public Interface ISettingsRepository
    Inherits IDisposable

    ' Common Code
    Function GetCommonCodes() As IEnumerable(Of CommonCode)
    Function GetCommonCodes(sortExpression As String) As IEnumerable(Of CommonCode)
    Function GetCommonCodesByCatg(sortExpression As String, catg As String) As IEnumerable(Of CommonCode)
    Function GetCommonCodesByCatgStatus(sortExpression As String, catg As String, stat As Boolean) As IEnumerable(Of CommonCode)
    Function GetCommonCodesByCatgDesc(sortExpression As String, catg As String, desc As String, stat As String) As IEnumerable(Of CommonCode)
    Function GetCommonCodeByCMId(ByVal cmId As Integer) As CommonCode
    Function GetCommonCodeByCatgCode(ByVal catg As String, ByVal code As String) As CommonCode
    Function InsertCommonCode(ByVal commonCode As CommonCode) As Integer
    Sub DeleteCommonCode(ByVal commonCode As CommonCode)
    Sub UpdateCommonCode(ByVal commonCode As CommonCode, ByVal origCommonCode As CommonCode)

    ' Preference
    Function GetPreferences(ByVal sortExpression As String) As IEnumerable(Of Preference)
    Function GetPreferenceByCatgUserId(catg As String, userId As String) As Preference
    Function GetPreferenceValue(catg As String, userId As String) As String
    Sub InsertPreference(ByVal preference As Preference)
    Sub DeletePreference(ByVal preference As Preference)
    Sub UpdatePreference(ByVal preference As Preference, ByVal origPreference As Preference)

    ' Message
    Function GetMessages(ByVal sortExpression As String) As IEnumerable(Of Message)
    Function GetMessagesByTextAndTitle(ByVal sortExpression As String, ByVal msgText As String, ByVal msgTitle As String) As IEnumerable(Of Message)
    Function GetMessageByMsgNo(ByVal msgNo As Integer) As Message
    Sub InsertMessage(ByVal message As Message)
    Sub DeleteMessage(ByVal message As Message)
    Sub UpdateMassage(ByVal message As Message, ByVal origMessage As Message)

    ' Department
    Function GetDepartments(ByVal sortExpression As String) As IEnumerable(Of Department)
    Function GetDepartmentsByStat(ByVal sortExpression As String, ByVal stat As String) As IEnumerable(Of Department)
    Function GetDeptByDeptCode(ByVal deptCode As String) As Department
    Sub InsertDepartment(ByVal department As Department)
    Sub DeleteDepartment(ByVal department As Department)
    Sub UpdateDepartment(ByVal department As Department, ByVal origDepartment As Department)

    ' Configuration
    Function GetConfigurations(ByVal sortExpression As String) As IEnumerable(Of Configuration)
    Function GetConfigurationsByKeyDesc(ByVal sortExpression As String, ByVal key As String, ByVal desc As String) As IEnumerable(Of Configuration)
    Function GetConfigurationByKey(ByVal key As String) As Configuration
    Sub InsertConfiguration(ByVal configuration As Configuration)
    Sub DeleteConfiguration(ByVal configuration As Configuration)
    Sub UpdateConfiguration(ByVal configuration As Configuration, ByVal origConfiguration As Configuration)

    ' USStates
    Function GetUSStatesByCount(sortExpression As String, count As Integer) As IEnumerable(Of USState)

    ' Notices
    Function GetNotices(ByVal sortExpression As String) As IEnumerable(Of Notice)
    Function GetNoticesActive() As IEnumerable(Of Notice)
    Function GetNoticeById(ByVal id As Integer) As Notice
    Function InsertNotice(ByVal notice As Notice) As Integer
    Sub DeleteNotice(ByVal notice As Notice)
    Sub UpdateNotice(ByVal notice As Notice, ByVal origNotice As Notice)

    ' AppUser
    Function GetAppUsers(sortExpression As String) As IEnumerable(Of AppUser)
    Function SearchAppUsersByRoleName(sortExpression As String, role As String, name As String) As IEnumerable(Of AppUser)
    Function GetAppUserByID(userID As String) As AppUser
    Sub InsertAppUser(ByVal appUser As AppUser)
    Sub DeleteAppUser(ByVal appUser As AppUser)
    Sub UpdateAppUser(ByVal appUser As AppUser, ByVal origAppUser As AppUser)

    ' UserOrg
    Function GetUserOrg(userID As String, orgCode As String) As UserOrg
    Sub InsertUserOrg(ByVal userOrg As UserOrg)
    Sub DeleteUserOrg(ByVal userOrg As UserOrg)
    Sub UpdateUserOrg(ByVal userOrg As UserOrg, ByVal origUserOrg As UserOrg)

    ' Bureau
    Function GetBureaus(ByVal sortExpression As String) As IEnumerable(Of Bureau)
    Function GetBureausByDeptDescStat(ByVal sortExpression As String, ByVal deptCode As String, ByVal burDesc As String, ByVal stat As String) As IEnumerable(Of Bureau)
    Function GetBureauByBurCode(ByVal burCode As String) As Bureau
    Sub InsertBureau(ByVal bureau As Bureau)
    Sub DeleteBureau(ByVal bureau As Bureau)
    Sub UpdateBureau(ByVal bureau As Bureau, ByVal origBureau As Bureau)

    ' Division
    Function GetDivisions(ByVal sortExpression As String) As IEnumerable(Of Division)
    Function GetDivisionsByDeptBurDescStat(ByVal sortExpression As String, ByVal deptCode As String, ByVal burCode As String, ByVal divDesc As String, ByVal stat As String) As IEnumerable(Of Division)
    Function GetDivisionByDivCode(ByVal divCode As String) As Division
    Sub InsertDivision(ByVal division As Division)
    Sub DeleteDivision(ByVal division As Division)
    Sub UpdateDivision(ByVal division As Division, ByVal origDivision As Division)

    Function GetOrgsByCodeDescStat(ByVal sortExpression As String, ByVal orgCode As String, ByVal orgDesc As String, ByVal stat As String) As IEnumerable(Of vOrganization)

    ' Classification
    Function GetClassifications(ByVal sortExpression As String) As IEnumerable(Of Classification)
    Function GetClassificationsByCodeDescTypeStat(ByVal sortExpression As String, ByVal classCode As String, ByVal classDesc As String, ByVal classType As String, ByVal stat As String) As IEnumerable(Of Classification)
    Function GetClassificationsByStatWithCodeDesc(ByVal sortExpression As String, ByVal stat As String) As IEnumerable(Of Object)
    Function GetClassificationByClassCode(ByVal classCode As String) As Classification
    Sub InsertClassification(ByVal classification As Classification)
    Sub DeleteClassification(ByVal classification As Classification)
    Sub UpdateClassification(ByVal classification As Classification, ByVal origClassification As Classification)
    Function GetClassTypes() As IEnumerable(Of Object)

    ' Employee
    Function GetEmployees(ByVal sortExpression As String) As IEnumerable(Of Employee)
    Function GetVEmployeesByNameOrgClassStat(ByVal sortExpression As String, ByVal name As String, ByVal orgCode As String, ByVal classCode As String, ByVal stat As String) As IEnumerable(Of vEmployee)
    Function GetEmployeeByEmpId(ByVal empId As String) As Employee

End Interface
