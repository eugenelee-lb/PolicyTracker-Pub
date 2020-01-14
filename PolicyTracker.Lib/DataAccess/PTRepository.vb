Imports System.Data.Objects
Imports System.Linq.Dynamic
Imports PolicyTracker.Lib

Public Class PTRepository
    Implements IDisposable, IPTRepository

    Private context As New PTEntities()

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

    Public Sub SaveChanges()
        Try
            context.SaveChanges()
        Catch ocex As OptimisticConcurrencyException
            Dim ctx = CType(context, Entity.Infrastructure.IObjectContextAdapter).ObjectContext
            ctx.Refresh(Entity.Core.Objects.RefreshMode.StoreWins, ocex.StateEntries(0).Entity)
            Throw ocex
        End Try
    End Sub


#Region "Schedule methods"
    Public Function GetSchedules(sortExpression As String) As IEnumerable(Of Schedule) Implements IPTRepository.GetSchedules
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "LastUpdateDT desc"
        End If
        Return context.Schedules.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetSchedulesByNameDescStat(sortExpression As String, scheduleName As String, scheduleDesc As String, stat As String) As IEnumerable(Of Schedule) Implements IPTRepository.GetSchedulesByNameDescStat
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "LastUpdateDT desc"
        End If

        Dim qry As IQueryable(Of Schedule) = context.Schedules.OrderBy("it." & sortExpression).AsQueryable

        If Not String.IsNullOrEmpty(scheduleName) Then
            qry = qry.Where(Function(a) a.ScheduleName.ToUpper.Contains(scheduleName.ToUpper))
        End If

        If Not String.IsNullOrWhiteSpace(scheduleDesc) Then
            qry = qry.Where(Function(a) a.ScheduleDesc.ToUpper.Contains(scheduleDesc.ToUpper))
        End If

        If stat = "A" Then
            qry = qry.Where(Function(a) a.Disabled = False)
        ElseIf stat = "D" Then
            qry = qry.Where(Function(a) a.Disabled = True)
        End If

        Return qry.ToList()
    End Function

    Public Function GetSchedulesByPolicy(sortExpression As String, policyId As Integer, isMember As Boolean) As IEnumerable(Of Schedule) Implements IPTRepository.GetSchedulesByPolicy
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "ScheduleName"
        End If
        'Return context.Schedules.OrderBy("it." & sortExpression).Where(Function(s) s.Policies.Contains(GetPolicyById(policyId)) = isMember).ToList()
        Dim policy = context.Policies.Where(Function(p) p.PolicyId.Equals(policyId)).FirstOrDefault()
        If policy Is Nothing Then Return Nothing

        Dim assignedSchedules = policy.Schedules.ToList()
        If isMember = True Then Return assignedSchedules

        Dim allSchedules = GetSchedules("")
        Dim unassignedSchedules = allSchedules.Except(assignedSchedules.AsEnumerable()).ToList()
        Return unassignedSchedules

    End Function

    Public Function GetScheduleById(scheduleId As Integer) As Schedule Implements IPTRepository.GetScheduleById
        Return context.Schedules.Where(Function(s) s.ScheduleId.Equals(scheduleId)).FirstOrDefault()
    End Function

    Public Function InsertSchedule(schedule As Schedule) As Integer Implements IPTRepository.InsertSchedule
        Dim newOwner = New ScheduleOwner
        newOwner.UserId = schedule.CreateUser
        schedule.ScheduleOwners.Add(newOwner)

        context.Schedules.Add(schedule)
        context.SaveChanges()
        Return schedule.ScheduleId
    End Function

    Public Sub UpdateSchedule(schedule As Schedule, origSchedule As Schedule) Implements IPTRepository.UpdateSchedule
        Dim ow = (From a In context.ScheduleOwners
                  Where a.ScheduleId = schedule.ScheduleId And a.UserId = schedule.LastUpdateUser
                  Select a).FirstOrDefault()
        If ow Is Nothing Then
            Throw New ApplicationException("You cannot edit this schedule because you are not an owner.")
        End If

        context.Schedules.Attach(origSchedule)
        context.Entry(origSchedule).CurrentValues.SetValues(schedule)
        SaveChanges()
    End Sub

    Public Sub DeleteSchedule(schedule As Schedule) Implements IPTRepository.DeleteSchedule
        Dim ow = (From a In context.ScheduleOwners
                  Where a.ScheduleId = schedule.ScheduleId And a.UserId = schedule.LastUpdateUser
                  Select a).FirstOrDefault()
        If ow Is Nothing Then
            Throw New ApplicationException("You cannot delete this schedule because you are not an owner.")
        End If

        schedule.ScheduleOwners.Remove(ow)
        context.Schedules.Attach(schedule)
        context.Schedules.Remove(schedule)
        SaveChanges()
    End Sub
#End Region

#Region "Policy methods"
    Public Function GetPolicies(sortExpression As String) As IEnumerable(Of Policy) Implements IPTRepository.GetPolicies
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "LastUpdateDT desc"
        End If
        Return context.Policies.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetPoliciesByNameDesc(sortExpression As String, name As String, desc As String) As IEnumerable(Of Policy) Implements IPTRepository.GetPoliciesByNameDesc
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "LastUpdateDT desc"
        End If

        Dim qryPolicy As IQueryable(Of Policy) = context.Policies.OrderBy("it." & sortExpression).AsQueryable

        If Not String.IsNullOrEmpty(name) Then
            qryPolicy = qryPolicy.Where(Function(c) c.PolicyName.Contains(name))
        End If
        If Not String.IsNullOrEmpty(desc) Then
            qryPolicy = qryPolicy.Where(Function(c) c.PolicyDesc.Contains(desc))
        End If
        Return qryPolicy.ToList()
    End Function

    Public Function GetPoliciesByNameDescStat(sortExpression As String, name As String, desc As String, stat As String) As IEnumerable(Of Policy) Implements IPTRepository.GetPoliciesByNameDescStat
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "LastUpdateDT desc"
        End If

        Dim qry As IQueryable(Of Policy) = context.Policies.OrderBy("it." & sortExpression).AsQueryable

        If Not String.IsNullOrEmpty(name) Then
            qry = qry.Where(Function(c) c.PolicyName.ToUpper.Contains(name.ToUpper))
        End If
        If Not String.IsNullOrEmpty(desc) Then
            qry = qry.Where(Function(c) c.PolicyDesc.ToUpper.Contains(desc.ToUpper))
        End If
        If stat = "A" Then
            qry = qry.Where(Function(a) a.Disabled = False)
        ElseIf stat = "D" Then
            qry = qry.Where(Function(a) a.Disabled = True)
        End If
        Return qry.ToList()
    End Function

    Public Function GetPoliciesBySchedule(sortExpression As String, scheduleId As Integer, isMember As Boolean) As IEnumerable(Of Policy) Implements IPTRepository.GetPoliciesBySchedule
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "PolicyName"
        End If
        Return context.Policies.OrderBy("it." & sortExpression).Where(Function(p) p.Schedules.Contains(GetScheduleById(scheduleId)) = isMember).ToList()
    End Function

    Public Function GetPolicyById(policyId As Integer) As Policy Implements IPTRepository.GetPolicyById
        Return context.Policies.Where(Function(p) p.PolicyId.Equals(policyId)).FirstOrDefault()
    End Function

    Public Function InsertPolicy(policy As Policy) As Integer Implements IPTRepository.InsertPolicy
        Dim newOwner = New PolicyOwner
        newOwner.UserId = policy.CreateUser
        policy.PolicyOwners.Add(newOwner)

        context.Policies.Add(policy)
        context.SaveChanges()
        Return policy.PolicyId
    End Function

    Public Sub UpdatePolicy(policy As Policy, origPolicy As Policy) Implements IPTRepository.UpdatePolicy
        Dim ow = (From a In context.PolicyOwners
                  Where a.PolicyId = policy.PolicyId And a.UserId = policy.LastUpdateUser
                  Select a).FirstOrDefault()
        If ow Is Nothing Then
            Throw New ApplicationException("You cannot update this policy because you are not an owner.")
        End If

        context.Policies.Attach(origPolicy)
        context.Entry(origPolicy).CurrentValues.SetValues(policy)
        SaveChanges()
    End Sub

    Public Sub DeletePolicy(policy As Policy) Implements IPTRepository.DeletePolicy
        Dim ow = (From a In context.PolicyOwners
                  Where a.PolicyId = policy.PolicyId And a.UserId = policy.LastUpdateUser
                  Select a).FirstOrDefault()
        If ow Is Nothing Then
            Throw New ApplicationException("You cannot delete this policy because you are not an owner.")
        End If

        policy.PolicyOwners.Remove(ow)
        context.Policies.Attach(policy)
        context.Policies.Remove(policy)
        SaveChanges()
    End Sub
#End Region

#Region "UploadFile methods"
    Public Function GetUploadFileById(fileId As String) As UploadFile Implements IPTRepository.GetUploadFileById
        Return context.UploadFiles.Where(Function(a) a.FileId.Equals(fileId)).FirstOrDefault()
    End Function

    Public Function GetUploadFilesByPolicyId(sortExpression As String, policyId As Integer) As IEnumerable(Of UploadFile) Implements IPTRepository.GetUploadFilesByPolicyId
        If String.IsNullOrWhiteSpace(sortExpression) Then sortExpression = "CreateDT"

        Dim qry As IQueryable(Of UploadFile) = context.UploadFiles.OrderBy("it." & sortExpression).AsQueryable
        qry = qry.Where(Function(c) (From p In c.Policies Where p.PolicyId.Equals(policyId)).Any())

        Return qry.ToList()
    End Function

    Public Sub InsertUploadFile(uploadFile As UploadFile) Implements IPTRepository.InsertUploadFile
        context.UploadFiles.Add(uploadFile)
        context.SaveChanges()
    End Sub

    Public Sub DeleteUploadFile(uploadFile As UploadFile) Implements IPTRepository.DeleteUploadFile
        context.UploadFiles.Attach(uploadFile)
        context.UploadFiles.Remove(uploadFile)
        SaveChanges()
    End Sub
#End Region

#Region "Recipient Group methods"
    Public Function GetRecipGroup(sortExpression As String) As IEnumerable(Of RecipGroup) Implements IPTRepository.GetRecipGroups
        If String.IsNullOrWhiteSpace(sortExpression) Then
            sortExpression = "LastUpdateDT desc"
        End If
        Return context.RecipGroups.OrderBy("it." & sortExpression).ToList()
    End Function

    Public Function GetRecipGroupById(recipgroupId As Integer) As RecipGroup Implements IPTRepository.GetRecipGroupByID
        Return context.RecipGroups.Where(Function(g) g.RecipGroupId.Equals(recipgroupId)).FirstOrDefault()
    End Function

    Public Function InsertRecipGroup(recipGroup As RecipGroup) As Integer Implements IPTRepository.InsertRecipGroup
        Dim newOwner = New RecipGroupOwner
        newOwner.UserId = recipGroup.CreateUser
        recipGroup.RecipGroupOwners.Add(newOwner)

        context.RecipGroups.Add(recipGroup)
        context.SaveChanges()
        Return recipGroup.RecipGroupId
    End Function

    Public Sub UpdateRecipGroup(recipGroup As RecipGroup, origRecipGroup As RecipGroup) Implements IPTRepository.UpdateRecipGroup
        Dim ow = (From a In context.RecipGroupOwners
                  Where a.RecipGroupId = recipGroup.RecipGroupId And a.UserId = recipGroup.LastUpdateUser
                  Select a).FirstOrDefault()
        If ow Is Nothing Then
            Throw New ApplicationException("You cannot update this recipient group because you are not an owner.")
        End If

        context.RecipGroups.Attach(origRecipGroup)
        context.Entry(origRecipGroup).CurrentValues.SetValues(recipGroup)
        SaveChanges()
    End Sub

    Public Sub DeleteRecipGroup(recipGroup As RecipGroup) Implements IPTRepository.DeleteRecipGroup
        Dim ow = (From a In context.RecipGroupOwners
                  Where a.RecipGroupId = recipGroup.RecipGroupId And a.UserId = recipGroup.LastUpdateUser
                  Select a).FirstOrDefault()
        If ow Is Nothing Then
            Throw New ApplicationException("You cannot delete this recipient group because you are not an owner.")
        End If

        recipGroup.RecipGroupOwners.Remove(ow)
        context.RecipGroups.Attach(recipGroup)
        context.RecipGroups.Remove(recipGroup)
        SaveChanges()
    End Sub
#End Region

#Region "ReleaseRecipient methods"
    Public Function GetReleaseRecipientById(releaseId As Integer, recipientId As String) As ReleaseRecipient Implements IPTRepository.GetReleaseRecipientById
        Return context.ReleaseRecipients.Where(Function(a) a.ReleaseId.Equals(releaseId) And a.RecipientId.Equals(recipientId)).FirstOrDefault()
    End Function

    Public Sub UpdateReleaseRecipient(releaseRecipient As ReleaseRecipient, origReleaseRecipient As ReleaseRecipient) Implements IPTRepository.UpdateReleaseRecipient
        'Dim ow = (From a In context.PolicyOwners
        '          Where a.PolicyId = Policy.PolicyId And a.UserId = Policy.LastUpdateUser
        '          Select a).FirstOrDefault()
        'If ow Is Nothing Then
        '    Throw New ApplicationException("You don't have permission to update this packet.")
        'End If

        context.ReleaseRecipients.Attach(origReleaseRecipient)
        context.Entry(origReleaseRecipient).CurrentValues.SetValues(releaseRecipient)
        SaveChanges()
    End Sub
#End Region

#Region "ReleaseNotice methods"
    Public Function GetReleaseNoticeById(releaseNoticeId As Integer) As ReleaseNotice Implements IPTRepository.GetReleaseNoticeById
        Return context.ReleaseNotices.Where(Function(a) a.ReleaseNoticeId.Equals(releaseNoticeId)).FirstOrDefault()
    End Function

    Public Function InsertReleaseNotice(releaseNotice As ReleaseNotice) As Integer Implements IPTRepository.InsertReleaseNotice
        context.ReleaseNotices.Add(releaseNotice)
        context.SaveChanges()
        Return releaseNotice.ReleaseNoticeId
    End Function

    Public Sub UpdateReleaseNotice(releaseNotice As ReleaseNotice, origReleaseNotice As ReleaseNotice) Implements IPTRepository.UpdateReleaseNotice
        context.ReleaseNotices.Attach(origReleaseNotice)
        context.Entry(origReleaseNotice).CurrentValues.SetValues(releaseNotice)
        SaveChanges()
    End Sub

    Public Sub DeleteReleaseNotice(releaseNotice As ReleaseNotice) Implements IPTRepository.DeleteReleaseNotice
        context.ReleaseNotices.Attach(releaseNotice)
        context.ReleaseNotices.Remove(releaseNotice)
        SaveChanges()
    End Sub
#End Region

#Region "RecipientNotice methods"
    Public Function GetRecipientNoticeById(releaseNoticeId As Integer, recipientId As String) As RecipientNotice Implements IPTRepository.GetRecipientNoticeById
        Return context.RecipientNotices.Where(Function(a) a.ReleaseNoticeId.Equals(releaseNoticeId) And a.RecipientId.Equals(recipientId)).FirstOrDefault()
    End Function

    Public Sub UpdateRecipientNotice(recipientNotice As RecipientNotice, origRecipientNotice As RecipientNotice) Implements IPTRepository.UpdateRecipientNotice
        context.RecipientNotices.Attach(origRecipientNotice)
        context.Entry(origRecipientNotice).CurrentValues.SetValues(recipientNotice)
        SaveChanges()
    End Sub
#End Region

#Region "OrgAdminNotice methods"
    Public Function GetOrgAdminNoticeById(releaseNoticeId As Integer) As OrgAdminNotice Implements IPTRepository.GetOrgAdminNoticeById
        Return context.OrgAdminNotices.Where(Function(a) a.ReleaseNoticeId.Equals(releaseNoticeId)).FirstOrDefault()
    End Function
#End Region

End Class
