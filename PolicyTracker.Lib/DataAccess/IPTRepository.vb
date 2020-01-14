Public Interface IPTRepository
    Inherits IDisposable
    ' Schedule
    Function GetSchedules(sortExpression As String) As IEnumerable(Of Schedule)
    Function GetSchedulesByNameDescStat(sortExpression As String, scheduleName As String, scheduleDesc As String, stat As String) As IEnumerable(Of Schedule)
    Function GetSchedulesByPolicy(ByVal sortExpression As String, ByVal policyId As Integer, ByVal isMember As Boolean) As IEnumerable(Of Schedule)
    Function GetScheduleById(ByVal scheduleId As Integer) As Schedule
    Function InsertSchedule(ByVal schedule As Schedule) As Integer
    Sub UpdateSchedule(ByVal schedule As Schedule, ByVal origSchedule As Schedule)
    Sub DeleteSchedule(ByVal schedule As Schedule)

    ' Policy
    Function GetPolicies(sortExpression As String) As IEnumerable(Of Policy)
    Function GetPoliciesByNameDesc(sortExpression As String, name As String, desc As String) As IEnumerable(Of Policy)
    Function GetPoliciesByNameDescStat(sortExpression As String, name As String, desc As String, stat As String) As IEnumerable(Of Policy)
    Function GetPoliciesBySchedule(ByVal sortExpression As String, ByVal scheduleId As Integer, ByVal isMember As Boolean) As IEnumerable(Of Policy)
    Function GetPolicyById(ByVal policyId As Integer) As Policy
    Function InsertPolicy(ByVal policy As Policy) As Integer
    Sub UpdatePolicy(ByVal policy As Policy, ByVal origPolicy As Policy)
    Sub DeletePolicy(ByVal policy As Policy)

    'Recipient Groups
    Function GetRecipGroups(sortExpression As String) As IEnumerable(Of RecipGroup)
    Function GetRecipGroupByID(recipgroupId As Integer) As RecipGroup
    Function InsertRecipGroup(RecipGroup As RecipGroup) As Integer
    Sub UpdateRecipGroup(recipgroup As RecipGroup, origRecipGroup As RecipGroup)
    Sub DeleteRecipGroup(recipgroup As RecipGroup)

    ' UploadFiles
    Function GetUploadFileById(ByVal fileId As String) As UploadFile
    Function GetUploadFilesByPolicyId(ByVal sortExpression As String,
                                   ByVal policyId As Integer) As IEnumerable(Of UploadFile)
    Sub InsertUploadFile(ByVal uploadFile As UploadFile)
    Sub DeleteUploadFile(ByVal uploadFile As UploadFile)

    ' ReleaseRecipient
    Function GetReleaseRecipientById(ByVal releaseId As Integer, recipientId As String) As ReleaseRecipient
    Sub UpdateReleaseRecipient(ByVal releaseRecipient As ReleaseRecipient, ByVal origReleaseRecipient As ReleaseRecipient)

    ' ReleaseNotice
    Function GetReleaseNoticeById(ByVal releaseNoticeId As Integer) As ReleaseNotice
    Function InsertReleaseNotice(ByVal releaseNotice As ReleaseNotice) As Integer
    Sub UpdateReleaseNotice(ByVal releaseNotice As ReleaseNotice, ByVal origReleaseNotice As ReleaseNotice)
    Sub DeleteReleaseNotice(ByVal releaseNotice As ReleaseNotice)

    ' RecipientNotice
    Function GetRecipientNoticeById(ByVal releaseNoticeId As Integer, recipientId As String) As RecipientNotice
    Sub UpdateRecipientNotice(ByVal recipientNotice As RecipientNotice, ByVal origRecipientNotice As RecipientNotice)

    ' OrgAdminNotice
    Function GetOrgAdminNoticeById(ByVal releaseNoticeId As Integer) As OrgAdminNotice

End Interface
