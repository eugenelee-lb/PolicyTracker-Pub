Imports System.IO
Imports System.Configuration
Imports System.Globalization
Imports System.Linq.Dynamic
Imports System.Text
Imports System.Net.Mail

<System.ComponentModel.DataObject()> _
Public Class PTBL

    ' Schedule - Policy
    Public Function AddSchedulePolicy(scheduleID As Integer, policyID As Integer) As Boolean
        Dim intAffectedRows As Integer = 0
        Using db As New PTEntities()
            Dim policy = (
                From p In db.Policies
                Where p.PolicyId = policyID
                Select p).First()

            If policy Is Nothing Then Return False
            Dim schedule = (
                From s In db.Schedules
                Where s.ScheduleId = scheduleID
                Select s).First()
            policy.Schedules.Add(schedule)

            intAffectedRows = db.SaveChanges()
        End Using
        Return intAffectedRows = 1
    End Function

    Public Function DeleteSchedulePolicy(scheduleID As Integer, policyID As Integer) As Boolean
        Dim intAffectedRows As Integer = 0
        Using db As New PTEntities()
            Dim policy = (
                From p In db.Policies
                Where p.PolicyId = policyID
                Select p).First()

            If policy Is Nothing Then
                Throw New ApplicationException("Exception occurred in DeleteSchedulePolicy. Invalid PolicyID [" & policyID.ToString & "] ")
            End If
            Dim schedule = (
                From s In db.Schedules
                Where s.ScheduleId = scheduleID
                Select s).First()
            If schedule Is Nothing Then
                Throw New ApplicationException("Exception occurred in DeleteSchedulePolicy. Invalid ScheduleID [" & scheduleID.ToString & "] ")
            End If
            policy.Schedules.Remove(schedule)

            intAffectedRows = db.SaveChanges()
        End Using
        Return intAffectedRows = 1
    End Function

    ' Policy File
    Public Sub AddPolicyFile(policyId As Integer, docFile As UploadFile)
        Using db As New PTEntities()
            Dim pol = (From a In db.Policies
                           Where a.PolicyId = policyId
                           Select a).FirstOrDefault()
            If pol Is Nothing Then
                Throw New ApplicationException("Policy ID is not valid [" & policyId.ToString & "]")
            End If

            pol.UploadFiles.Add(docFile)
            db.SaveChanges()
        End Using
    End Sub

    Public Sub DeletePolicyFile(policyId As Integer, fileId As String)
        Using db As New PTEntities()
            Dim pol = (From a In db.Policies
                           Where a.PolicyId = policyId
                           Select a).FirstOrDefault()
            If pol Is Nothing Then
                Throw New ApplicationException("Policy ID is not valid [" & policyId.ToString & "]")
            End If

            Dim uf = (From a In db.UploadFiles
                        Where a.FileId = fileId
                        Select a).FirstOrDefault()
            If uf Is Nothing Then
                Throw New ApplicationException("File ID is not valid [" & fileId & "]")
            End If

            pol.UploadFiles.Remove(uf)
            If uf.Policies.Count = 1 And uf.ReleasePolicies.Count = 0 Then
                db.UploadFiles.Remove(uf)
            End If
            db.SaveChanges()
        End Using
    End Sub

    ' PacketAckFile
    Public Sub AddPacketAckFile(releaseId As Integer, recipientId As String, docFile As UploadFile)
        Using db As New PTEntities()
            Dim pac = (From a In db.ReleaseRecipients
                           Where a.ReleaseId And a.RecipientId = recipientId
                           Select a).FirstOrDefault()
            If pac Is Nothing Then
                Throw New ApplicationException("Packet is not found [" & releaseId.ToString & "-" & recipientId & "]")
            End If

            Dim newAckFile As New PacketAckFile
            With newAckFile
                .ReleaseId = releaseId
                .RecipientId = recipientId
                .Disabled = False
            End With
            docFile.PacketAckFiles.Add(newAckFile)
            db.UploadFiles.Add(docFile)
            db.SaveChanges()
        End Using
    End Sub

    Public Sub DeletePacketAckFile(releaseId As Integer, recipientId As String, fileId As String)
        Using db As New PTEntities()
            Dim pac = (From a In db.ReleaseRecipients
                           Where a.ReleaseId And a.RecipientId = recipientId
                           Select a).FirstOrDefault()
            If pac Is Nothing Then
                Throw New ApplicationException("Packet is not found [" & releaseId.ToString & "-" & recipientId & "]")
            End If

            Dim af = (From a In db.PacketAckFiles
                        Where a.ReleaseId = releaseId And a.RecipientId = recipientId And a.FileId = fileId
                        Select a).FirstOrDefault()
            If af Is Nothing Then
                Throw New ApplicationException("Packet Ack File is not found [" & releaseId.ToString & "-" & recipientId & "-" & fileId & "]")
            End If

            af.Disabled = True
            db.SaveChanges()
        End Using
    End Sub

    ' Check owner of schedule/policy/recip group
    Public Function IsScheduleOwner(scheduleId As Integer, userId As String) As Boolean
        Dim isOwner As Boolean = False
        Using db As New PTEntities()
            isOwner = (From a In db.ScheduleOwners
                       Where a.ScheduleId = scheduleId And a.UserId = userId).Any()
        End Using
        Return isOwner
    End Function

    Public Function IsPolicyOwner(policyId As Integer, userId As String) As Boolean
        Dim isOwner As Boolean = False
        Using db As New PTEntities()
            isOwner = (From a In db.PolicyOwners
                       Where a.PolicyId = policyId And a.UserId = userId).Any()
        End Using
        Return isOwner
    End Function

    Public Function IsRecipGroupOwner(recipGroupId As Integer, userId As String) As Boolean
        Dim isOwner As Boolean = False
        Using db As New PTEntities()
            isOwner = (From a In db.RecipGroupOwners
                       Where a.RecipGroupId = recipGroupId And a.UserId = userId).Any()
        End Using
        Return isOwner
    End Function

    ' check if release/packet under org by release/admin
    Public Shared Function IsReleaseForAdmin(releaseId As Integer, adminId As String) As Boolean
        Dim isAdmin As Boolean = False
        Using db As New PTEntities()
            isAdmin = (From r In db.ReleaseRecipients, o In db.UserOrgs
                       Where r.ReleaseId = releaseId And o.UserId.ToUpper = adminId.ToUpper _
                       And r.OrgCode.StartsWith(o.OrgCode)).Any()
        End Using
        Return isAdmin
    End Function

    Public Shared Function IsPacketForAdmin(releaseId As Integer, recipId As String, adminId As String) As Boolean
        Dim isAdmin As Boolean = False
        Using db As New PTEntities()
            isAdmin = (From r In db.ReleaseRecipients, o In db.UserOrgs
                       Where r.ReleaseId = releaseId And r.RecipientId.ToUpper = recipId.ToUpper And o.UserId.ToUpper = adminId.ToUpper _
                       And r.OrgCode.StartsWith(o.OrgCode)).Any()
        End Using
        Return isAdmin
    End Function

    Public Shared Function KioskOnlyOrg(userOrg As String) As Boolean
        Dim strOrgsAllKiosk As String = ConfigurationManager.AppSettings.Get("OrgsKioskOnly")
        Dim listOrgs As String() = strOrgsAllKiosk.Split(",")
        For Each org In listOrgs
            If userOrg.StartsWith(org) Then Return True
        Next
        Return False
    End Function

    ' Batch Procedures
    Public Function CreateRelease(ByVal scheduleId As Integer, createUser As String, leadDays As Integer) As Integer
        Dim intReturn As Integer = -1

        Using db As New PTEntities()
            ' Get Schedule
            Dim sch = (From s In db.Schedules
                       Where s.ScheduleId = scheduleId
                       Select s).First()
            If sch Is Nothing Then
                Throw New ApplicationException("Exception occurred in CreateRelease. Invalid Schedule ID [" & scheduleId.ToString() & "].")
            End If

            Dim newRelease = New Release
            With newRelease
                .ScheduleId = scheduleId
                .To = sch.To
                .From = sch.From
                .Subject = sch.Subject
                .Disclaimer = sch.Disclaimer
                .CreateDT = Now()
                .CreateUser = createUser
                .LastUpdateDT = Now()
                .LastUpdateUser = createUser
            End With

            Select Case sch.Frequency
                Case "ONE_TIME"
                    If sch.FixedReleaseDate.HasValue Then
                        If sch.FixedReleaseDate.Value > Now.AddDays(leadDays) Then Return 0
                        ' already exists?
                        Dim rh = (From r In db.Releases
                                   Where r.ScheduleId = scheduleId And r.ReleaseDate = sch.FixedReleaseDate
                                   Select r).FirstOrDefault()
                        If rh Is Nothing Then
                            With newRelease
                                .ReleaseDate = sch.FixedReleaseDate
                                .DeadlineDate = sch.FixedReleaseDate.Value.AddDays(sch.DaysToDeadline)
                            End With
                            db.Releases.Add(newRelease)
                        End If
                    End If

                Case "ANNUAL"
                    If sch.RepeatMonth.HasValue AndAlso sch.RepeatDay.HasValue Then
                        Dim datRelease As Date = New Date(Now.Year, sch.RepeatMonth, sch.RepeatDay)
                        If datRelease < Now.AddDays(-1) Then
                            datRelease = datRelease.AddYears(1)
                        End If
                        If datRelease > Now.AddDays(leadDays) Then Return 0
                        ' already exists?
                        Dim rh = (From r In db.Releases
                                   Where r.ScheduleId = scheduleId And r.ReleaseDate = datRelease
                                   Select r).FirstOrDefault()
                        If rh Is Nothing Then
                            With newRelease
                                .ReleaseDate = datRelease
                                .DeadlineDate = datRelease.AddDays(sch.DaysToDeadline)
                            End With
                            db.Releases.Add(newRelease)
                        End If
                    End If

                Case "SEMI_ANNUAL"
                    If sch.RepeatMonth.HasValue AndAlso sch.RepeatDay.HasValue Then
                        Dim intRepeatMonth As Integer = sch.RepeatMonth Mod 6
                        If intRepeatMonth = 0 Then intRepeatMonth = 6
                        Dim datRelease As Date = New Date(Now.Year, intRepeatMonth, sch.RepeatDay)
                        For ii As Integer = 0 To 2
                            If datRelease > Now.AddDays(-1) Then Exit For
                            datRelease = datRelease.AddMonths(6)
                        Next
                        If datRelease > Now.AddDays(leadDays) Then Return 0
                        ' already exists?
                        Dim rh = (From r In db.Releases
                                   Where r.ScheduleId = scheduleId And r.ReleaseDate = datRelease
                                   Select r).FirstOrDefault()
                        If rh Is Nothing Then
                            With newRelease
                                .ReleaseDate = datRelease
                                .DeadlineDate = datRelease.AddDays(sch.DaysToDeadline)
                            End With
                            db.Releases.Add(newRelease)
                        End If
                    End If

                Case "QUARTERLY"
                    If sch.RepeatMonth.HasValue AndAlso sch.RepeatDay.HasValue Then
                        Dim intRepeatMonth As Integer = sch.RepeatMonth Mod 3
                        If intRepeatMonth = 0 Then intRepeatMonth = 3

                        Dim datRelease As Date = New Date(Now.Year, intRepeatMonth, sch.RepeatDay)
                        For ii As Integer = 0 To 4
                            If datRelease > Now.AddDays(-1) Then Exit For
                            datRelease = datRelease.AddMonths(3)
                        Next
                        If datRelease > Now.AddDays(leadDays) Then Return 0
                        ' already exists?
                        Dim rh = (From r In db.Releases
                                   Where r.ScheduleId = scheduleId And r.ReleaseDate = datRelease
                                   Select r).FirstOrDefault()
                        If rh Is Nothing Then
                            With newRelease
                                .ReleaseDate = datRelease
                                .DeadlineDate = datRelease.AddDays(sch.DaysToDeadline)
                            End With
                            db.Releases.Add(newRelease)
                        End If
                    End If
            End Select

            intReturn = db.SaveChanges()
        End Using

        Return intReturn
    End Function

    Public Function CreateReleasePolicyRecip(ByVal intReleaseId As Integer, createUser As String) As Integer
        Dim intAffectedRows As Integer = 0

        Using db As New PTEntities()
            ' Get Release
            Dim rel = (From rh In db.Releases
                       Where rh.ReleaseId = intReleaseId
                       Select rh).First()
            If rel Is Nothing Then
                Throw New ApplicationException("Exception occurred in CreateReleasePolicyRecip. Invalid Release ID [" & intReleaseId.ToString() & "].")
            End If
            ' Get Schedule
            Dim sch = (From s In db.Schedules.Include("Policies")
                       Where s.ScheduleId = rel.ScheduleId
                       Select s).First()
            If sch Is Nothing Then
                Throw New ApplicationException("Exception occurred in CreateReleasePolicyRecip. Invalid Schedule ID [" & rel.ScheduleId.ToString() & "] for rel [" & intReleaseId.ToString() & "].")
            End If

            ' Delete existing records
            ' If there are any acknowledged records, cannot proceed.
            Dim ackedRelRecip = (From relRecip In db.ReleaseRecipients
                            Where relRecip.ReleaseId = intReleaseId And relRecip.AckDT IsNot Nothing
                            Select relRecip).FirstOrDefault()
            If ackedRelRecip IsNot Nothing Then
                Throw New ApplicationException("There are acknowledged records. You cannot delete/recreate them.")
            End If

            For Each relRecip As ReleaseRecipient In db.ReleaseRecipients.Where( _
                        Function(r) r.ReleaseId = intReleaseId)
                db.ReleaseRecipients.Remove(relRecip)
            Next

            For Each rhp As ReleasePolicy In db.ReleasePolicies.Where( _
                        Function(p) p.ReleaseId = intReleaseId)
                'For Each rf In rhp.UploadFiles
                '    rhp.UploadFiles.Remove(rf)
                'Next
                rhp.UploadFiles.Clear()
                db.ReleasePolicies.Remove(rhp)
            Next

            ' delete unsent notices
            For Each relN As ReleaseNotice In db.ReleaseNotices.Where( _
                        Function(n) n.ReleaseId = intReleaseId)
                Dim nSent As Boolean = False
                For Each rn In relN.RecipientNotices.ToList()
                    If rn.SentDT.HasValue Then
                        nSent = True
                    Else
                        db.RecipientNotices.Remove(rn)
                    End If
                Next
                'relN.RecipientNotices.Clear()
                If relN.OrgAdminNotice IsNot Nothing Then
                    If relN.OrgAdminNotice.SentDT.HasValue Then
                        nSent = True
                    Else
                        db.OrgAdminNotices.Remove(relN.OrgAdminNotice)
                    End If
                End If
                If Not nSent Then db.ReleaseNotices.Remove(relN)
            Next

            db.SaveChanges()

            ' Add ReleasePolicy
            For Each pol As Policy In sch.Policies.Where(Function(p) p.Disabled = False)
                'If pol.Disabled Then Continue For ' skip disabled

                Dim intPolicyId As Integer = pol.PolicyId

                Dim newReleasePolicy = New ReleasePolicy
                With newReleasePolicy
                    .ReleaseId = intReleaseId
                    .PolicyId = intPolicyId
                    .PolicyName = pol.PolicyName
                    .PolicyDesc = pol.PolicyDesc
                    .ShowDisclaimer = pol.ShowDisclaimer
                    .Disclaimer = pol.Disclaimer
                    For Each uf In pol.UploadFiles
                        .UploadFiles.Add(uf)
                    Next
                    db.ReleasePolicies.Add(newReleasePolicy)
                End With
            Next

            ' Add ReleaseRecipients
            Dim lstRecip As New List(Of ReleaseRecipient) ' Temporary list of records to insert

            For Each recipGroup As RecipGroup In sch.RecipGroups
                Dim intRecipGroupId As Integer = recipGroup.RecipGroupId

                Select Case recipGroup.RecipGroupType
                    Case "IND"
                        For Each emp As Employee In recipGroup.Employees
                            ' Add ReleaseRecipients if not exist
                            Dim recip = (From r In lstRecip _
                                         Where r.ReleaseId = intReleaseId _
                                         AndAlso r.RecipientId = emp.EmpId _
                                         Select r).FirstOrDefault()
                            If recip Is Nothing Then
                                Dim newRecip As New ReleaseRecipient
                                With newRecip
                                    .ReleaseId = intReleaseId
                                    .RecipientId = emp.EmpId
                                    .RecipientName = GlobalLib.GetFullName(emp.FirstName, emp.MiddleName, emp.LastName)
                                    .OrgCode = emp.OrgCode
                                    .CreateDT = Now
                                    .CreateUser = createUser
                                    .LastUpdateDT = Now
                                    .LastUpdateUser = createUser
                                    .RecipientEmail = GlobalLib.GetEmailAddress(emp.EmpId)
                                End With
                                lstRecip.Add(newRecip)
                            End If
                        Next

                    Case "ATTRIB"
                        Dim eg = (From e In db.RecipGroups
                                    Where e.RecipGroupId = intRecipGroupId
                                    Select e).FirstOrDefault()
                        Dim aOrgs = (From org In eg.RecipGroupOrgs, div In db.Divisions
                                     Where div.DivCode.StartsWith(org.OrgCode)
                                     Select div.DivCode).ToList()
                        Dim aClasses = (From cls In eg.Classifications
                                        Select cls.ClassCode).ToList()

                        Dim qry = (From e In db.Employees
                                   Where e.Disabled = False
                                   Select e)
                        If aOrgs.Count = 0 And aClasses.Count = 0 Then
                            ' No Org/Class
                            qry = qry.Where(Function(e) 1 = 0)
                        End If
                        If aOrgs.Count > 0 Then
                            qry = qry.Where(Function(e) aOrgs.Contains(e.OrgCode))
                        End If
                        If aClasses.Count > 0 Then
                            qry = qry.Where(Function(e) aClasses.Contains(e.ClassCode))
                        End If

                        Dim lstEmp = qry.ToList()
                        For Each emp In lstEmp
                            Dim recip = (From r In lstRecip _
                                         Where r.ReleaseId = intReleaseId _
                                         AndAlso r.RecipientId = emp.EmpId _
                                         Select r).FirstOrDefault()
                            If recip Is Nothing Then
                                Dim newRecip As New ReleaseRecipient
                                With newRecip
                                    .ReleaseId = intReleaseId
                                    .RecipientId = emp.EmpId
                                    .RecipientName = GlobalLib.GetFullName(emp.FirstName, emp.MiddleName, emp.LastName)
                                    .OrgCode = emp.OrgCode
                                    .CreateDT = Now
                                    .CreateUser = createUser
                                    .LastUpdateDT = Now
                                    .LastUpdateUser = createUser
                                    .RecipientEmail = GlobalLib.GetEmailAddress(emp.EmpId)
                                End With
                                lstRecip.Add(newRecip)
                            End If
                        Next
                End Select
            Next

            For Each recip In lstRecip
                db.ReleaseRecipients.Add(recip)
            Next
            intAffectedRows = db.SaveChanges()

        End Using

        Return intAffectedRows
    End Function

    Public Const NOTI_TYPE_RELE As String = "RELEASE"
    Public Const NOTI_TYPE_REMI As String = "REMINDER"
    Public Const NOTI_TYPE_DEAD As String = "DEADLINE"
    Public Const NOTI_TYPE_OVER As String = "OVERDUE"

    Public Function CreateReleaseNotices(ByVal intReleaseId As Integer, ByVal createUser As String) As Integer
        Dim intAffectedRows As Integer = 0

        Using db As New PTEntities()
            ' Get ReleaseHist
            Dim rel = (From rh In db.Releases
                       Where rh.ReleaseId = intReleaseId
                       Select rh).FirstOrDefault()
            If rel Is Nothing Then
                Throw New ApplicationException("Exception occurred in CreateReleaseNotices. Invalid Release ID [" & intReleaseId.ToString() & "].")
            End If
            ' Get Schedule
            Dim sch = (From s In db.Schedules.Include("Policies")
                       Where s.ScheduleId = rel.ScheduleId
                       Select s).FirstOrDefault()

            ' Delete existing records
            ' If there are any sent email notice, cannot proceed.
            Dim sentNotice As Boolean = False
            For Each relN In rel.ReleaseNotices
                For Each recipN In relN.RecipientNotices.Where(Function(a) a.SentDT.HasValue = True)
                    sentNotice = True
                    Exit For
                Next
                If relN.OrgAdminNotice IsNot Nothing Then
                    Dim oaN = relN.OrgAdminNotice
                    If oaN.SentDT.HasValue Then
                        sentNotice = True
                    End If
                End If
                If sentNotice Then Exit For
            Next

            If sentNotice Then
                Throw New ApplicationException("There is sent email notice. You cannot delete/recreate Release Notices.")
            End If

            ' delete unsent notices
            For Each relN As ReleaseNotice In db.ReleaseNotices.Where(
                        Function(n) n.ReleaseId = intReleaseId)
                Dim nSent As Boolean = False
                For Each rn In relN.RecipientNotices.ToList()
                    If rn.SentDT.HasValue Then
                        nSent = True
                    Else
                        db.RecipientNotices.Remove(rn)
                    End If
                Next
                'relN.RecipientNotices.Clear()
                If relN.OrgAdminNotice IsNot Nothing Then
                    If relN.OrgAdminNotice.SentDT.HasValue Then
                        nSent = True
                    Else
                        db.OrgAdminNotices.Remove(relN.OrgAdminNotice)
                    End If
                End If
                If Not nSent Then db.ReleaseNotices.Remove(relN)
            Next

            db.SaveChanges()

            ' RELEASE
            Dim newRelN As New ReleaseNotice
            With newRelN
                .ReleaseId = intReleaseId
                .NoticeType = NOTI_TYPE_RELE
                .NoticeDate = rel.ReleaseDate
                .CreateDT = Now
                .CreateUser = createUser
                .LastUpdateDT = Now
                .LastUpdateUser = createUser
            End With
            db.ReleaseNotices.Add(newRelN)

            ' DEADLINE
            Dim newRelNDeadline As New ReleaseNotice
            With newRelNDeadline
                .ReleaseId = intReleaseId
                .NoticeType = NOTI_TYPE_DEAD
                .NoticeDate = rel.ReleaseDate.AddDays(sch.DaysToDeadline)
                .CreateDT = Now
                .CreateUser = createUser
                .LastUpdateDT = Now
                .LastUpdateUser = createUser
            End With
            db.ReleaseNotices.Add(newRelNDeadline)

            ' REMINDER
            If Not String.IsNullOrWhiteSpace(sch.DaysToReminder) Then
                Dim strReminderDays() As String = sch.DaysToReminder.Split(",")
                Dim intReminderDay As Integer
                For i As Integer = 0 To strReminderDays.Length - 1
                    If Integer.TryParse(strReminderDays(i), intReminderDay) Then
                        If rel.ReleaseDate.AddDays(intReminderDay) <= rel.ReleaseDate Or rel.ReleaseDate.AddDays(intReminderDay) = rel.DeadlineDate Then
                            Continue For
                        End If
                        Dim newRelNRemind As New ReleaseNotice
                        With newRelNRemind
                            .ReleaseId = intReleaseId
                            .NoticeType = IIf(rel.ReleaseDate.AddDays(intReminderDay) < rel.DeadlineDate, NOTI_TYPE_REMI, NOTI_TYPE_OVER)
                            .NoticeDate = rel.ReleaseDate.AddDays(intReminderDay)
                            .CreateDT = Now
                            .CreateUser = createUser
                            .LastUpdateDT = Now
                            .LastUpdateUser = createUser
                        End With
                        db.ReleaseNotices.Add(newRelNRemind)
                    End If
                Next
            End If

            intAffectedRows = db.SaveChanges()
        End Using

        Return intAffectedRows
    End Function

    Private Sub AddOrgToList(org As String, ByRef lst As List(Of String))
        Dim o = (From a In lst Where a = org).FirstOrDefault()
        If o Is Nothing Then
            lst.Add(org)
        End If
    End Sub

    Public Function CreateEmailNotices(relNoticeId As Integer) As Integer
        Dim intAffectedRows As Integer = 0

        Dim lstDiv As New List(Of String)

        Using db As New PTEntities()

            ' If there are any sent email notice, cannot proceed.
            Dim sentEN = (From en In db.RecipientNotices
                            Where en.ReleaseNoticeId = relNoticeId And en.SentDT IsNot Nothing
                            Select en).FirstOrDefault()
            If sentEN IsNot Nothing Then
                Throw New ApplicationException("There are sent Recipient Notices. You cannot delete/recreate notices.")
            End If
            Dim sentON = (From en In db.OrgAdminNotices
                            Where en.ReleaseNoticeId = relNoticeId And en.SentDT IsNot Nothing
                            Select en).FirstOrDefault()
            If sentON IsNot Nothing Then
                Throw New ApplicationException("There are sent Org Admin Notices. You cannot delete/recreate notices.")
            End If

            ' Delete all unsent notices
            For Each en In db.RecipientNotices.Where( _
                        Function(e) e.ReleaseNoticeId = relNoticeId And e.SentDT Is Nothing)
                db.RecipientNotices.Remove(en)
            Next
            For Each en In db.OrgAdminNotices.Where( _
                        Function(e) e.ReleaseNoticeId = relNoticeId And e.SentDT Is Nothing)
                db.OrgAdminNotices.Remove(en)
            Next

            ' Get ReleaseNotice
            Dim relNotice = (From n In db.ReleaseNotices
                                 Where n.ReleaseNoticeId = relNoticeId
                                 Select n).FirstOrDefault()
            If relNotice Is Nothing Then
                Throw New ApplicationException("Invalid Release Notice ID [" & relNoticeId.ToString & "].")
            End If
            ' Get Release
            Dim rel = (From rh In db.Releases.Include("ReleaseRecipients")
                              Where rh.ReleaseId = relNotice.ReleaseId
                              Select rh).First()
            If rel Is Nothing Then
                Throw New ApplicationException("Invalid Release ID [" & relNotice.ReleaseId.ToString() & "].")
            End If
            ' Get Schedule
            'Dim sch = (From s In db.Schedules.Include("Policies")
            '           Where s.ScheduleId = rel.ScheduleId
            '           Select s).First()
            'If sch Is Nothing Then
            '    Throw New ApplicationException("Invalid ScheduleID [" & rel.ScheduleId.ToString() & "].")
            'End If

            ' Mail To Recipients
            For Each relRecip In rel.ReleaseRecipients '.Where(Function(r) String.IsNullOrEmpty(r.Exception))
                ' skip reminder and deadline for acked
                ' skip acked or exception flagged
                If relNotice.NoticeType <> "RELEASE" And (relRecip.AckDT.HasValue Or Not String.IsNullOrEmpty(relRecip.Exception)) Then
                    Continue For
                End If

                ' list div for OrgAdminNotices
                AddOrgToList(relRecip.OrgCode, lstDiv)

                If Not String.IsNullOrWhiteSpace(relRecip.RecipientEmail) Then
                    Dim enToRecip As New RecipientNotice()
                    With enToRecip
                        .ReleaseNoticeId = relNoticeId
                        .RecipientId = relRecip.RecipientId
                        .To = relRecip.RecipientEmail
                        .From = ConfigurationManager.AppSettings("MailSender")
                        .Subject = rel.Subject & " - " & relNotice.NoticeType 'ConfigurationManager.AppSettings("ApplName") 

                        Dim textFilePath As String = ConfigurationManager.AppSettings("MSG_DIR") & "\Notice.htm"

                        Dim sbBody As New StringBuilder
                        Using sr As StreamReader = New StreamReader(textFilePath)
                            sbBody.Append(sr.ReadToEnd())
                            sr.Close()
                        End Using

                        sbBody.Replace("{Title}", .Subject)
                        sbBody.Replace("{From}", rel.From)
                        sbBody.Replace("{To}", rel.To)
                        sbBody.Replace("{Subject}", rel.Subject)
                        sbBody.Replace("{ReleaseDate}", rel.ReleaseDate.ToString("M/d/yyyy"))
                        sbBody.Replace("{Deadline}", rel.DeadlineDate.ToString("M/d/yyyy"))
                        sbBody.Replace("{Disclaimer}", GlobalLib.HtmlMsgEncode(rel.Disclaimer))
                        sbBody.Replace("{Instruction}", SettingsBL.GetMessageText(1011)) ' Notice Instruction - Recipient
                        ' Kiosk only orgs or URL to packet
                        If PTBL.KioskOnlyOrg(relRecip.OrgCode) Then
                            sbBody.Replace("{URL}", ConfigurationManager.AppSettings("SITE_URL").Replace("PolicyTracker", "PolicyTracker.Kiosk"))
                        Else
                            sbBody.Replace("{URL}", ConfigurationManager.AppSettings("SITE_URL") _
                                       & "USER/Packet/" & relRecip.ReleaseId.ToString & "/" & relRecip.RecipientId)
                        End If
                        .Body = sbBody.ToString()

                        .CreateDT = Now()
                    End With
                    db.RecipientNotices.Add(enToRecip)
                End If
            Next

            ' Mail To Org Admins
            If lstDiv.Count > 0 Then ' only if there is recipient not acked and not flagged exception
                Dim lstOrg As New List(Of String)
                For Each org In lstDiv
                    AddOrgToList(org.Substring(0, 3), lstOrg)
                    AddOrgToList(org.Substring(0, 2), lstOrg)
                Next

                ' Get OrgAdmins
                Dim lstOrgAdmins = (From a In db.vOrgAdmins
                                    Where lstOrg.Contains(a.OrgCode) And a.AccessLevel = "OA" _
                                        And Not (From p In db.Preferences
                                                 Where p.Catg = SettingsBL.PREF_DONOT_SEND_NOTI And p.UserId = a.UserId And p.Val = "True"
                                                 Select p).Any()
                                    Select a).ToList()

                ' Get ScheduleOwners
                Dim schOwners = (From s In db.ScheduleOwners
                                 Where s.ScheduleId = rel.ScheduleId
                                 Select s).ToList()

                Dim enToAdmin As New OrgAdminNotice()
                With enToAdmin
                    .ReleaseNoticeId = relNoticeId
                    .From = ConfigurationManager.AppSettings("MailSender")

                    Dim strEmail As String = ""
                    For Each oa In lstOrgAdmins
                        strEmail &= ";" & oa.UserId
                    Next
                    For Each so In schOwners
                        strEmail &= ";" & so.UserId
                    Next

                    'If strEmail = "" Then
                    '    ' If there is no org admin, schedule owner then send to System Amdins
                    '    GlobalLib.WriteLog("There is no org admin for this release : " & rel.ReleaseId.ToString)
                    '    Dim sAdmins = (From a In db.AppUsers
                    '                   Where a.UserRole = "SA"
                    '                   Select a).ToList()
                    '    If sAdmins IsNot Nothing Then
                    '        For Each sa In sAdmins
                    '            strEmail &= ";" & sa.UserId
                    '        Next
                    '    End If
                    'End If
                    If strEmail.Length > 0 Then strEmail = strEmail.Substring(1)

                    .To = ""
                    Dim arMailTo As String() = strEmail.Split(";")
                    For ii As Integer = 0 To arMailTo.Length - 1
                        Dim em As String = GlobalLib.GetEmailAddress(arMailTo(ii))
                        If Not String.IsNullOrWhiteSpace(em) Then
                            .To &= "; " & em
                        End If
                    Next
                    If .To.Length > 0 Then .To = .To.Substring(2)

                    .Subject = rel.Subject & " - " & relNotice.NoticeType 'ConfigurationManager.AppSettings("ApplName") 

                    Dim textFilePath As String = ConfigurationManager.AppSettings("MSG_DIR") & "\Notice.htm"

                    Dim sbBody As New StringBuilder
                    Using sr As StreamReader = New StreamReader(textFilePath)
                        sbBody.Append(sr.ReadToEnd())
                        sr.Close()
                    End Using

                    sbBody.Replace("{Title}", .Subject)
                    sbBody.Replace("{From}", rel.From)
                    sbBody.Replace("{To}", rel.To)
                    sbBody.Replace("{Subject}", rel.Subject)
                    sbBody.Replace("{ReleaseDate}", rel.ReleaseDate.ToString("M/d/yyyy"))
                    sbBody.Replace("{Deadline}", rel.DeadlineDate.ToString("M/d/yyyy"))
                    sbBody.Replace("{Disclaimer}", rel.Disclaimer)
                    sbBody.Replace("{Instruction}", SettingsBL.GetMessageText(1012)) ' Notice Instruction - Org Admin
                    sbBody.Replace("{URL}", ConfigurationManager.AppSettings("SITE_URL") _
                               & "OA/Release/" & rel.ReleaseId.ToString)
                    .Body = sbBody.ToString()

                    .CreateDT = Now()
                End With
                db.OrgAdminNotices.Add(enToAdmin)
            End If

            intAffectedRows = db.SaveChanges()

        End Using

        Return intAffectedRows
    End Function

    Public Function SendReleaseNotice(ByVal relNoticeId As Integer) As Boolean
        Dim completed As Boolean = False

        Using db As New PTEntities()
            ' Get ReleaseNotice
            Dim relNotice = (From n In db.ReleaseNotices
                                 Where n.ReleaseNoticeId = relNoticeId
                                 Select n).FirstOrDefault()
            If relNotice Is Nothing Then
                Throw New ApplicationException("Invalid Release Notice ID [" & relNoticeId.ToString & "].")
            End If
            If relNotice.StartDT Is Nothing Then
                relNotice.StartDT = Now()
                db.SaveChanges()
            End If

            ' recipient notices
            Dim sendError As Boolean = False
            For Each recipN In relNotice.RecipientNotices.Where(Function(a) a.SentDT Is Nothing)
                If SendNoticeToRecip(recipN) = False Then sendError = True
            Next

            ' org admin notice
            If relNotice.OrgAdminNotice IsNot Nothing AndAlso relNotice.OrgAdminNotice.SentDT Is Nothing Then
                If SendNoticeToOrgAdmin(relNotice.OrgAdminNotice) = False Then sendError = True
            End If
            If sendError = False Then
                relNotice.CompleteDT = Now()
                db.SaveChanges()
                completed = True
            End If
        End Using

        Return completed
    End Function

    ' email methods
    Public Function SendNoticeToRecip(ByVal recipNotice As RecipientNotice) As Boolean
        Dim sent As Boolean = False
        Try
            SMTPSendEmail(recipNotice.From, recipNotice.To, recipNotice.Subject, recipNotice.Body)
            sent = True
        Catch ex As Exception
        End Try

        If sent Then
            UpdateRecipNoticeSent(recipNotice.ReleaseNoticeId, recipNotice.RecipientId)
        End If
        Return sent
    End Function

    Public Sub UpdateRecipNoticeSent(ByVal relNoticeId As Integer, recipId As String)
        Using db As New PTEntities()
            Dim em = (From s In db.RecipientNotices
                      Where s.ReleaseNoticeId = relNoticeId And s.RecipientId = recipId
                      Select s).FirstOrDefault()
            If em Is Nothing Then
                Throw New ApplicationException("Could not find recipient notice [" & relNoticeId.ToString & -recipId & "]")
            End If
            em.SentDT = Now()
            db.SaveChanges()
        End Using
    End Sub

    Public Function SendNoticeToOrgAdmin(ByVal orgAdminNotice As OrgAdminNotice) As Boolean
        Dim sent As Boolean = False
        Try
            SMTPSendEmail(orgAdminNotice.From, orgAdminNotice.To, orgAdminNotice.Subject, orgAdminNotice.Body)
            sent = True
        Catch ex As Exception
        End Try

        If sent Then
            UpdateOrgAdminNoticeSent(orgAdminNotice.ReleaseNoticeId)
        End If
        Return sent
    End Function

    Public Sub UpdateOrgAdminNoticeSent(ByVal relNoticeId As Integer)
        Using db As New PTEntities()
            Dim em = (From s In db.OrgAdminNotices
                      Where s.ReleaseNoticeId = relNoticeId
                      Select s).FirstOrDefault()
            If em Is Nothing Then
                Throw New ApplicationException("Could not find org admin notice [" & relNoticeId.ToString & "]")
            End If
            em.SentDT = Now()
            db.SaveChanges()
        End Using
    End Sub

    ' Send E-mail to SMTP Server
    Private Sub SMTPSendEmail(ByVal sender As String, ByVal recipients As String, ByVal subject As String, ByVal body As String)
        Dim message As New MailMessage()
        message.From = New MailAddress(sender)
        Dim arrRecipients As String() = recipients.Split(CChar(";"))
        For i As Integer = 0 To arrRecipients.Length - 1
            message.To.Add(arrRecipients(i))
        Next
        message.Subject = subject
        message.Body = body
        message.IsBodyHtml = True
        message.Priority = MailPriority.High
        ' attachments
        'If attFiles IsNot Nothing Then
        '    For Each attFile As EmailLogAttachedFile In attFiles
        '        Dim attach As Attachment = New Attachment(attFile.FilePath)
        '        message.Attachments.Add(attach)
        '    Next
        'End If

        Dim client As SmtpClient = New SmtpClient()
        client.UseDefaultCredentials = True
        client.Timeout = 5000 ' timeout 5 seconds
        client.Send(message)
        message.Dispose()
        client.Dispose()
    End Sub

    ' Packet
    Public Class PacketUpdateResult
        Private _ClientIp As String
        Public Property ClientIp() As String
            Get
                Return _ClientIp
            End Get
            Set(ByVal value As String)
                _ClientIp = value
            End Set
        End Property

        Private _DateTime As String
        Public Property DateTime() As String
            Get
                Return _DateTime
            End Get
            Set(ByVal value As String)
                _DateTime = value
            End Set
        End Property
    End Class

    Public Function UpdateRecipientView(relId As Integer, recipId As String, clientIP As String) As PacketUpdateResult
        Dim result As PacketUpdateResult = New PacketUpdateResult()
        Using db As New PTEntities()
            Dim relRecip = (From a In db.ReleaseRecipients
                            Where a.ReleaseId = relId And a.RecipientId = recipId
                            Select a).First()
            If relRecip Is Nothing Then
                Throw New ApplicationException("Exception occurred in UpdateRecipientView. Invalid Release ID, Recipient ID [" & relId.ToString() & "," & recipId & "].")
            End If
            If relRecip.RecipientViewDT.HasValue Then
                Throw New ApplicationException("Exception occurred in UpdateRecipientView. The packet was already viewed [" & relId.ToString() & "," & recipId & "].")
            End If
            relRecip.RecipientViewDT = Now()
            relRecip.RecipientViewClientIP = clientIP

            With result
                .ClientIp = clientIP
                .DateTime = relRecip.RecipientViewDT.ToString()
            End With

            db.SaveChanges()
        End Using
        Return result
    End Function

    Public Function AckPacket(ByVal relId As Integer, ByVal recipId As String, createUser As String, clientIP As String, authType As String) As Boolean
        Using db As New PTEntities()
            ' ReleaseRecipient
            Dim relRecip = (From a In db.ReleaseRecipients
                            Where a.ReleaseId = relId And a.RecipientId = recipId
                            Select a).First()
            If relRecip Is Nothing Then
                Throw New ApplicationException("Exception occurred in AckPacket. Invalid Release ID, Recipient ID [" & relId.ToString() & "," & recipId & "].")
            End If

            If relRecip.AckDT IsNot Nothing Then
                Throw New ApplicationException("Exception occurred in AckPacket. The record [" & relId.ToString() & "," & recipId & "] already has been acknowledged.")
            End If

            With relRecip
                .AckDT = Now
                .AckUserId = createUser
                .AckClientIP = clientIP
                .AckAuthType = authType
            End With

            db.SaveChanges()

        End Using
        Return True
    End Function

End Class
