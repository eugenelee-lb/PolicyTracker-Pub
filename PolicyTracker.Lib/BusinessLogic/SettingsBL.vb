<System.ComponentModel.DataObject()> _
Public Class SettingsBL
    Private repo As ISettingsRepository

    Public Sub New()
        Me.repo = New SettingsRepository()
    End Sub

    Public Sub New(ByVal repo As ISettingsRepository)
        Me.repo = repo
    End Sub

    Private disposedValue As Boolean = False

    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                repo.Dispose()
            End If
        End If
        Me.disposedValue = True
    End Sub

    Public Sub Dispose()
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub


#Region "Common Code"

    Public Function GetCommonCodes() As IEnumerable(Of CommonCode)
        Return repo.GetCommonCodes()
    End Function

    Public Function GetCommonCodes(ByVal sortExpression As String) As IEnumerable(Of CommonCode)
        Return repo.GetCommonCodes(sortExpression)
    End Function

    Public Function GetCommonCodesByCatg(ByVal sortExpression As String, ByVal catg As String) As IEnumerable(Of CommonCode)
        Return repo.GetCommonCodesByCatg(sortExpression, catg)
    End Function

    Public Function GetCommonCodesByCatgDesc(ByVal sortExpression As String, ByVal catg As String, desc As String, stat As String) As IEnumerable(Of CommonCode)
        Return repo.GetCommonCodesByCatgDesc(sortExpression, catg, desc, stat)
    End Function

    Public Function GetCommonCodesByCatgStatus(ByVal sortExpression As String, ByVal catg As String, stat As Boolean) As IEnumerable(Of CommonCode)
        Return repo.GetCommonCodesByCatgStatus(sortExpression, catg, stat)
    End Function

    Public Function GetCommonCodeByCMId(ByVal cmId As Integer) As CommonCode
        Return repo.GetCommonCodeByCMId(cmId)
    End Function

    Public Function GetCommonCodeByCatgCode(ByVal catg As String, ByVal code As String) As CommonCode
        Return repo.GetCommonCodeByCatgCode(catg, code)
    End Function

    Public Function AddCommonCode(ByVal commonCode As CommonCode) As Integer
        Try
            Return repo.InsertCommonCode(commonCode)
        Catch ex As Exception
            'Include catch blocks for specific exceptions first, 
            'and handle or log the error as appropriate in each. 
            'Include a generic catch block like this one last. 
            Throw ex
        End Try
    End Function

    Public Sub DeleteCommonCode(ByVal commonCode As CommonCode)
        Try
            repo.DeleteCommonCode(commonCode)
        Catch ex As Exception
            'Include catch blocks for specific exceptions first, 
            'and handle or log the error as appropriate in each. 
            'Include a generic catch block like this one last. 
            Throw ex
        End Try
    End Sub

    Public Sub UpdateCommonCode(ByVal commonCode As CommonCode, ByVal origCommonCode As CommonCode)
        Try
            repo.UpdateCommonCode(commonCode, origCommonCode)
        Catch ex As Exception
            'Include catch blocks for specific exceptions first, 
            'and handle or log the error as appropriate in each. 
            'Include a generic catch block like this one last. 
            Throw ex
        End Try

    End Sub

#End Region

#Region "Preference"

    Public Const PREF_DONOT_SEND_NOTI As String = "DONOT_SEND_NOTI"

    Public Function GetPreferences(sortExpression As String) As IEnumerable(Of Preference)
        Return repo.GetPreferences(sortExpression)
    End Function

    Public Function GetPreferencesByGatgUserId(catg As String, userId As String) As Preference
        Return repo.GetPreferenceByCatgUserId(catg, userId)
    End Function

    Public Function GetPreferenceValue(catg As String, userId As String) As String
        Return repo.GetPreferenceValue(catg, userId)
    End Function

    Public Sub AddPreference(preference As Preference)
        Try
            repo.InsertPreference(preference)
        Catch ex As Exception
            'Include catch blocks for specific exceptions first, 
            'and handle or log the error as appropriate in each. 
            'Include a generic catch block like this one last. 
            Throw ex
        End Try
    End Sub

    Public Sub UpdatePreference(ByVal preference As Preference, ByVal origPreference As Preference)
        Try
            repo.UpdatePreference(preference, origPreference)
        Catch ex As Exception
            'Include catch blocks for specific exceptions first, 
            'and handle or log the error as appropriate in each. 
            'Include a generic catch block like this one last. 
            Throw ex
        End Try
    End Sub

    Public Sub WritePreference(ByVal catg As String, ByVal userId As String, ByVal val As String, ByVal updateUserID As String)
        Try
            Using context As New PTEntities()
                Dim pref = (From p In context.Preferences
                          Where p.Catg = catg And p.UserId = userId
                          Select p).FirstOrDefault()
                If pref IsNot Nothing Then
                    pref.Val = val
                    pref.LastUpdateUser = updateUserID
                    pref.LastUpdateDT = Now()
                    context.SaveChanges()
                Else
                    pref = New Preference
                    pref.Catg = catg
                    pref.UserId = userId
                    pref.Val = val
                    pref.LastUpdateUser = updateUserID
                    pref.LastUpdateDT = Now()
                    context.Preferences.Add(pref)
                    context.SaveChanges()
                End If
            End Using
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

#End Region

#Region "Message"
    Public Function GetMessages(sortExpression As String) As IEnumerable(Of Message)
        Return repo.GetMessages(sortExpression)
    End Function

    Public Function GetMessageByMsgNo(msgNo As Integer) As Message
        Return repo.GetMessageByMsgNo(msgNo)
    End Function

    Public Function GetMessagesByTextAndTitle(sortExpression As String, msgText As String, msgTitle As String) As IEnumerable(Of Message)
        Return repo.GetMessagesByTextAndTitle(sortExpression, msgText, msgTitle)
    End Function

    Public Sub AddMessage(ByVal message As Message)
        Try
            repo.InsertMessage(message)
        Catch ex As Exception
            'Include catch blocks for specific exceptions first, 
            'and handle or log the error as appropriate in each. 
            'Include a generic catch block like this one last. 
            Throw ex
        End Try
    End Sub

    Public Sub DeleteMessage(ByVal message As Message)
        Try
            repo.DeleteMessage(message)
        Catch ex As Exception
            'Include catch blocks for specific exceptions first, 
            'and handle or log the error as appropriate in each. 
            'Include a generic catch block like this one last. 
            Throw ex
        End Try
    End Sub

    Public Sub UpdateMessage(ByVal message As Message, ByVal origMessage As Message)
        Try
            repo.UpdateMassage(message, origMessage)
        Catch ex As Exception
            'Include catch blocks for specific exceptions first, 
            'and handle or log the error as appropriate in each. 
            'Include a generic catch block like this one last. 
            Throw ex
        End Try
    End Sub


    Public Shared Function GetMessageText(msgNo As Integer) As String
        Using _rep As New SettingsRepository
            Dim _message As Message = _rep.GetMessageByMsgNo(msgNo)
            If _rep Is Nothing Then
                Throw New ApplicationException("Undefined message number:" & msgNo.ToString)
            Else
                Return _message.MsgText
            End If
        End Using
    End Function

    Public Shared Function GetMessageText(msgNo As Integer, arg0 As Object) As String
        Return String.Format(GetMessageText(msgNo), arg0)
    End Function

    Public Shared Function GetMessageText(msgNo As Integer, arg0 As Object, arg1 As Object) As String
        Return String.Format(GetMessageText(msgNo), arg0, arg1)
    End Function

    Public Shared Function GetMessageText(msgNo As Integer, arg0 As Object, arg1 As Object, arg2 As Object) As String
        Return String.Format(GetMessageText(msgNo), arg0, arg1, arg2)
    End Function

#End Region

#Region "Department"
    Public Function GetDepartments(ByVal sortExpression As String) As IEnumerable(Of Department)
        Return repo.GetDepartments(sortExpression)
    End Function

    Public Function GetDepartmentsByStat(ByVal sortExpression As String, ByVal stat As String) As IEnumerable(Of Department)
        Return repo.GetDepartmentsByStat(sortExpression, stat)
    End Function

    Public Function GetDeptByDeptCode(ByVal deptCode As String) As Department
        Return repo.GetDeptByDeptCode(deptCode)
    End Function

    Public Sub AddDepartment(ByVal department As Department)
        Try
            repo.InsertDepartment(department)
        Catch ex As Exception
            'Include catch blocks for specific exceptions first, 
            'and handle or log the error as appropriate in each. 
            'Include a generic catch block like this one last. 
            Throw ex
        End Try
    End Sub

    Public Sub UpdateDepartment(ByVal department As Department, ByVal origDepartment As Department)
        Try
            repo.UpdateDepartment(department, origDepartment)
        Catch ex As Exception
            'Include catch blocks for specific exceptions first, 
            'and handle or log the error as appropriate in each. 
            'Include a generic catch block like this one last. 
            Throw ex
        End Try
    End Sub

    Public Sub DeleteDepartment(ByVal department As Department)
        Try
            repo.DeleteDepartment(department)
        Catch ex As Exception
            'Include catch blocks for specific exceptions first, 
            'and handle or log the error as appropriate in each. 
            'Include a generic catch block like this one last. 
            Throw ex
        End Try
    End Sub

    Public Function GetDepartmentsByAdmin(ByVal userRole As String, ByVal userId As String) As IEnumerable(Of Department)
        Try
            Using db As New PTEntities()
                Select Case userRole
                    Case "SA"
                        Return GetDepartmentsByStat("", "A")
                    Case "PA"
                        Dim myDepts = (From da In db.vOrgAdmins, d In db.Departments
                                       Where da.OrgCode = d.DeptCode And da.UserId = userId And da.AccessLevel = userRole _
                                       And d.Disabled = False
                                       Select d Order By d.DeptCode).ToList()
                        Return myDepts
                    Case Else
                        Return Nothing
                End Select
            End Using

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetOrgsForPA(ByVal userRole As String, ByVal userId As String) As IEnumerable(Of vOrganization)
        Try
            Using db As New PTEntities()
                Select Case userRole
                    Case "SA"
                        Dim lst = (From o In db.vOrganizations Where o.Disabled = False
                                   Select o).ToList()
                        Return lst
                    Case "PA"
                        Dim lst = (From oa In db.vOrgAdmins, o In db.vOrganizations
                                   Where o.OrgCode.StartsWith(oa.OrgCode) And oa.UserId = userId And oa.AccessLevel = userRole _
                                   And o.Disabled = False
                                   Select o Order By o.OrgCode).ToList()
                        Return lst
                    Case Else
                        Return Nothing
                End Select
            End Using

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Function GetOrgsForOA(ByVal userRole As String, ByVal userId As String) As IEnumerable(Of vOrganization)
        Try
            Using db As New PTEntities()
                Select Case userRole
                    Case "SA"
                        Dim lst = (From o In db.vOrganizations Where o.Disabled = False
                                   Select o).ToList()
                        Return lst
                    Case Else 'PA,OA
                        Dim lst = (From oa In db.vOrgAdmins, o In db.vOrganizations
                                   Where o.OrgCode.StartsWith(oa.OrgCode) And oa.UserId = userId _
                                   And o.Disabled = False
                                   Select o Order By o.OrgCode).ToList()
                        Return lst
                End Select
            End Using

        Catch ex As Exception
            Throw ex
        End Try
    End Function

    Public Shared Function IsUserOrgAdmin(ByVal userId As String, ByVal orgCode As String, ByVal userRole As String) As Boolean
        Using db As New PTEntities()
            Dim da = (From a In db.vOrgAdmins
                      Where a.UserId = userId And a.OrgCode = orgCode And a.AccessLevel = userRole
                      Select a).FirstOrDefault()
            Return da IsNot Nothing

        End Using
    End Function

    Public Shared Function GetOrgAdminsByOrg(ByVal orgCode As String) As List(Of vOrgAdmin)
        Using db As New PTEntities()
            Dim da = (From a In db.vOrgAdmins
                      Where a.OrgCode = orgCode
                      Select a)
            Return da.ToList()
        End Using
    End Function

#End Region

#Region "Configuration"
    Public Function GetConfigurations(sortExpression As String) As IEnumerable(Of Configuration)
        Return repo.GetConfigurations(sortExpression)
    End Function

    Public Function GetConfigurationByKey(key As String) As Configuration
        Return repo.GetConfigurationByKey(key)
    End Function

    Public Function GetConfigurationsByKeyDesc(sortExpression As String, key As String, desc As String) As IEnumerable(Of Configuration)
        Return repo.GetConfigurationsByKeyDesc(sortExpression, key, desc)
    End Function

    Public Sub AddConfiguration(ByVal configuration As Configuration)
        repo.InsertConfiguration(configuration)
    End Sub

    Public Sub DeleteConfiguration(ByVal configuration As Configuration)
        repo.DeleteConfiguration(configuration)
    End Sub

    Public Sub UpdateConfiguration(ByVal configuration As Configuration, ByVal origConfiguration As Configuration)
        repo.UpdateConfiguration(configuration, origConfiguration)
    End Sub

    Public Shared Function GetConfigurationValue(key As String) As String
        Using _rep As New SettingsRepository
            Dim _config As Configuration = _rep.GetConfigurationByKey(key)
            If _rep Is Nothing Then
                Throw New ApplicationException("Undefined configuration key:" & key)
            Else
                Return _config.ConfigValue
            End If
        End Using
    End Function

#End Region

#Region "USState"
    Public Function GetUSStatesByCount(ByVal sortExpression As String, ByVal count As Integer) As IEnumerable(Of USState)
        Return repo.GetUSStatesByCount(sortExpression, count)
    End Function
#End Region

#Region "Notice"
    Public Function GetNotices(sortExpression As String) As IEnumerable(Of Notice)
        Return repo.GetNotices(sortExpression)
    End Function

    Public Function GetNoticesActive() As IEnumerable(Of Notice)
        Return repo.GetNoticesActive()
    End Function

    Public Function GetNoticeById(id As Integer) As Notice
        Return repo.GetNoticeById(id)
    End Function

    Public Function AddNotice(ByVal notice As Notice) As Integer
        Return repo.InsertNotice(notice)
    End Function

    Public Sub DeleteNotice(ByVal notice As Notice)
        repo.DeleteNotice(notice)
    End Sub

    Public Sub UpdateNotice(ByVal notice As Notice, ByVal origNotice As Notice)
        repo.UpdateNotice(notice, origNotice)
    End Sub
#End Region

#Region "AppUser"
    Public Function GetAppUsers(ByVal sortExpression As String) As IEnumerable(Of AppUser)
        Return repo.GetAppUsers(sortExpression)
    End Function

    Public Function SearchAppUsersByRoleName(ByVal sortExpression As String, ByVal role As String, ByVal name As String) As IEnumerable(Of AppUser)
        Return repo.SearchAppUsersByRoleName(sortExpression, role, name)
    End Function

    Public Function GetAppUserByID(ByVal userId As String) As AppUser
        Return repo.GetAppUserByID(userId)
    End Function

    Public Sub AddAppUser(ByVal appUser As AppUser)
        repo.InsertAppUser(appUser)
    End Sub

    Public Sub DeleteAppUser(ByVal appUser As AppUser)
        repo.DeleteAppUser(appUser)
    End Sub

    Public Sub UpdateAppUser(ByVal appUser As AppUser, ByVal origAppUser As AppUser)
        repo.UpdateAppUser(appUser, origAppUser)
    End Sub
#End Region

End Class
