Imports System.IO
Imports System.Configuration
Imports System.DirectoryServices

Public Class Batch

    Private BATCH_USER As String = ConfigurationManager.AppSettings("BATCH_USER") '"BATCH_PROC"

    Public Sub HRDataUpdate(sw As StreamWriter)
        Dim ifp As IFormatProvider = Globalization.DateTimeFormatInfo.InvariantInfo

        'Dim provider As CultureInfo = CultureInfo.InvariantCulture

        ' disable records
        'sw.WriteLine(Now.ToString("G", ifp) & " == Start Disable Records ==")
        'Using ctx As New PTEntities()
        '    Dim objCtx = CType(ctx, System.Data.Entity.Infrastructure.IObjectContextAdapter).ObjectContext
        '    objCtx.ExecuteStoreCommand("update DBO.Departments set Disabled = 1", Nothing)
        '    objCtx.ExecuteStoreCommand("update DBO.Bureaus set Disabled = 1", Nothing)
        '    objCtx.ExecuteStoreCommand("update DBO.Divisions set Disabled = 1", Nothing)
        '    objCtx.ExecuteStoreCommand("update DBO.Classifications set Disabled = 1", Nothing)
        '    objCtx.ExecuteStoreCommand("update DBO.Employees set Disabled = 1", Nothing)
        'End Using

        Dim svcHR As New PolicyTracker.Lib.HRService  'HRWebService.WebService

        Using ctx As New PTEntities()
            Dim intNew As Integer
            Dim intUpd As Integer

            ' Departments
            sw.WriteLine(Now.ToString("G", ifp) & " == Start Departments Update ==")
            Dim hrDepts As IEnumerable(Of DepartmentDTO) = Nothing
            svcHR.GetData(Of IEnumerable(Of DepartmentDTO))("departments", hrDepts)

            Dim objCtx = CType(ctx, System.Data.Entity.Infrastructure.IObjectContextAdapter).ObjectContext
            objCtx.ExecuteStoreCommand("update DBO.Departments set Disabled = 1", Nothing)

            intNew = 0
            intUpd = 0
            For Each hrDept In hrDepts
                Dim dept = (From a In ctx.Departments
                              Where a.DeptCode = hrDept.DeptCode.Trim
                              Select a).FirstOrDefault()
                If dept Is Nothing Then
                    Dim newDept As New Department
                    With newDept
                        .DeptCode = hrDept.DeptCode.Trim
                        .DeptDesc = hrDept.DeptDesc.Trim
                        .FamisOrgCode = hrDept.FamisOrgCode.Trim
                        .Disabled = False
                        .CreateDT = Now
                        .CreateUser = BATCH_USER
                        .LastUpdateDT = Now
                        .LastUpdateUser = BATCH_USER
                    End With
                    ctx.Departments.Add(newDept)
                    intNew += 1
                Else
                    With dept
                        .Disabled = False
                        If Not (.DeptDesc = hrDept.DeptDesc.Trim And _
                                .FamisOrgCode = hrDept.FamisOrgCode.Trim) Then
                            .DeptDesc = hrDept.DeptDesc.Trim
                            .FamisOrgCode = hrDept.FamisOrgCode.Trim
                            .LastUpdateDT = Now
                            .LastUpdateUser = BATCH_USER
                            intUpd += 1
                        End If
                    End With
                End If
                Try
                    ctx.SaveChanges()
                Catch ex As Exception
                    sw.WriteLine("  ** Error in save Department " & hrDept.DeptCode)
                    Throw ex
                End Try
            Next
            sw.WriteLine("  # of Departments New/Update: " & intNew.ToString & "/" & intUpd.ToString)

            ' Bureaus
            sw.WriteLine(Now.ToString("G", ifp) & " == Start Bureaus Update ==")
            Dim hrBureaus As IEnumerable(Of BureauDTO) = Nothing
            svcHR.GetData(Of IEnumerable(Of BureauDTO))("bureaus", hrBureaus)

            objCtx.ExecuteStoreCommand("update DBO.Bureaus set Disabled = 1", Nothing)

            intNew = 0
            intUpd = 0
            For Each hrBur In hrBureaus.Where(Function(a) a.BurDesc.Trim <> "*" And a.BurDesc.Trim <> "NOT USED")
                Dim bur = (From a In ctx.Bureaus
                              Where a.BurCode = hrBur.BurCode.Trim
                              Select a).FirstOrDefault()
                If bur Is Nothing Then
                    Dim dep = (From d In ctx.Departments
                               Where d.DeptCode = hrBur.BurCode.Substring(0, 2) And d.Disabled = False
                               Select d).FirstOrDefault()
                    If dep IsNot Nothing Then
                        Dim newBur As New Bureau
                        With newBur
                            .BurCode = hrBur.BurCode.Trim
                            .BurDesc = hrBur.BurDesc.Trim
                            .DeptCode = hrBur.BurCode.Substring(0, 2)
                            .Disabled = False
                            .CreateDT = Now
                            .CreateUser = BATCH_USER
                            .LastUpdateDT = Now
                            .LastUpdateUser = BATCH_USER
                        End With
                        ctx.Bureaus.Add(newBur)
                        intNew += 1
                    End If
                Else
                    With bur
                        .Disabled = False
                        If Not (.BurDesc = hrBur.BurDesc.Trim And _
                                .DeptCode = hrBur.BurCode.Substring(0, 2)) Then
                            .BurDesc = hrBur.BurDesc.Trim
                            .DeptCode = hrBur.BurCode.Substring(0, 2)
                            .LastUpdateDT = Now
                            .LastUpdateUser = BATCH_USER
                            intUpd += 1
                        End If
                    End With
                End If
                Try
                    ctx.SaveChanges()
                Catch ex As Exception
                    sw.WriteLine("  ** Error in save Bureau " & hrBur.BurCode)
                    Throw ex
                End Try
            Next
            sw.WriteLine("  # of Bureaus New/Update: " & intNew.ToString & "/" & intUpd.ToString)

            ' Divisions
            sw.WriteLine(Now.ToString("G", ifp) & " == Start Divisions Update ==")
            Dim hrDivisions As IEnumerable(Of DivisionDTO) = Nothing
            svcHR.GetData(Of IEnumerable(Of DivisionDTO))("divisions", hrDivisions)

            objCtx.ExecuteStoreCommand("update DBO.Divisions set Disabled = 1", Nothing)

            intNew = 0
            intUpd = 0
            For Each hrDiv In hrDivisions.Where(Function(a) a.DivDesc.Trim <> "*")
                Dim Div = (From a In ctx.Divisions
                Where a.DivCode = hrDiv.DivCode.Trim
                              Select a).FirstOrDefault()
                If Div Is Nothing Then
                    Dim bur = (From b In ctx.Bureaus
                               Where b.BurCode = hrDiv.DivCode.Substring(0, 3) And b.Disabled = False
                               Select b).FirstOrDefault()
                    If bur IsNot Nothing Then
                        Dim newDiv As New Division
                        With newDiv
                            .DivCode = hrDiv.DivCode.Trim
                            .DivDesc = hrDiv.DivDesc.Trim
                            .BurCode = hrDiv.DivCode.Substring(0, 3)
                            .CMDept = hrDiv.CMDept
                            .PersUnitCode = hrDiv.PersUnitCode
                            .FamisOrg = hrDiv.FamisOrg
                            .FamisDefaultIndex = hrDiv.FamisDefaultIndex
                            .Disabled = False
                            .CreateDT = Now
                            .CreateUser = BATCH_USER
                            .LastUpdateDT = Now
                            .LastUpdateUser = BATCH_USER

                            If .DivDesc.Contains("DO NOT USE") Then
                                .Disabled = True
                            End If
                        End With
                        ctx.Divisions.Add(newDiv)
                        intNew += 1
                    End If
                Else
                    With Div
                        .Disabled = False
                        If .DivDesc.Contains("DO NOT USE") Then
                            .Disabled = True
                        End If
                        If Not (.DivDesc = hrDiv.DivDesc.Trim And _
                                .BurCode = hrDiv.DivCode.Substring(0, 3) And _
                                .CMDept = hrDiv.CMDept And _
                                .PersUnitCode = hrDiv.PersUnitCode And _
                                .FamisOrg = hrDiv.FamisOrg And _
                                .FamisDefaultIndex = hrDiv.FamisDefaultIndex) Then
                            .DivDesc = hrDiv.DivDesc.Trim
                            .BurCode = hrDiv.DivCode.Substring(0, 3)
                            .CMDept = hrDiv.CMDept
                            .PersUnitCode = hrDiv.PersUnitCode
                            .FamisOrg = hrDiv.FamisOrg
                            .FamisDefaultIndex = hrDiv.FamisDefaultIndex
                            .LastUpdateDT = Now
                            .LastUpdateUser = BATCH_USER
                            intUpd += 1
                        End If
                    End With
                End If
                Try
                    ctx.SaveChanges()
                Catch ex As Exception
                    sw.WriteLine("  ** Error in save Division " & hrDiv.DivCode)
                    Throw ex
                End Try
            Next
            sw.WriteLine("  # of Divisions New/Update: " & intNew.ToString & "/" & intUpd.ToString)

            ' Classifications
            sw.WriteLine(Now.ToString("G", ifp) & " == Start Classifications Update ==")
            Dim hrClasses As IEnumerable(Of ClassificationDTO) = Nothing
            svcHR.GetData(Of IEnumerable(Of ClassificationDTO))("classifications", hrClasses)

            objCtx.ExecuteStoreCommand("update DBO.Classifications set Disabled = 1", Nothing)

            intNew = 0
            intUpd = 0
            For Each hrCls In hrClasses
                Dim cls = (From a In ctx.Classifications
                              Where a.ClassCode = hrCls.ClassCode.Trim
                              Select a).FirstOrDefault()
                If cls Is Nothing Then
                    Dim newCls As New Classification
                    With newCls
                        .ClassCode = hrCls.ClassCode.Trim
                        .ClassDesc = hrCls.Title.Trim
                        .ClassType = hrCls.ClassType.Trim
                        .Disabled = False
                        .CreateDT = Now
                        .CreateUser = BATCH_USER
                        .LastUpdateDT = Now
                        .LastUpdateUser = BATCH_USER
                    End With
                    ctx.Classifications.Add(newCls)
                    intNew += 1
                Else
                    With cls
                        .Disabled = False
                        If Not (.ClassDesc = hrCls.Title.Trim And _
                                .ClassType = hrCls.ClassType.Trim) Then
                            .ClassDesc = hrCls.Title.Trim
                            .ClassType = hrCls.ClassType.Trim
                            .LastUpdateDT = Now
                            .LastUpdateUser = BATCH_USER
                            intUpd += 1
                        End If
                    End With
                End If
                Try
                    ctx.SaveChanges()
                Catch ex As Exception
                    sw.WriteLine("  ** Error in save Classification " & hrCls.ClassCode)
                    Throw ex
                End Try
            Next
            sw.WriteLine("  # of Classifications New/Update: " & intNew.ToString & "/" & intUpd.ToString)

            ' Employees
            sw.WriteLine(Now.ToString("G", ifp) & " == Start Employees Update ==")
            Dim hrEmployees As IEnumerable(Of EmployeeDTO) = Nothing
            svcHR.GetData(Of IEnumerable(Of EmployeeDTO))("employees", hrEmployees)

            objCtx.ExecuteStoreCommand("update DBO.Employees set Disabled = 1", Nothing)

            intNew = 0
            intUpd = 0
            For Each hrEmp In hrEmployees
                Dim emp = (From a In ctx.Employees
                            Where a.EmpId = hrEmp.EmpId.Trim
                            Select a).FirstOrDefault()
                If emp Is Nothing Then
                    Dim newEmp As New Employee
                    With newEmp
                        .EmpId = hrEmp.EmpId.Trim
                        .PIN = GlobalLib.TrimOrNull(hrEmp.Pin)
                        .Status = hrEmp.Stat.Trim
                        .FirstName = hrEmp.FName.Trim
                        .MiddleName = GlobalLib.TrimOrNull(hrEmp.MName)
                        .LastName = hrEmp.LName.Trim
                        .Suffix = GlobalLib.TrimOrNull(hrEmp.Suffix)
                        .OrgCode = hrEmp.OrgCode
                        .ClassCode = hrEmp.ClassCode
                        .HireDate = hrEmp.HireDate
                        .Disabled = False
                        .CreateDT = Now
                        .LastUpdateDT = Now
                    End With
                    ctx.Employees.Add(newEmp)
                    intNew += 1
                Else
                    With emp
                        .Disabled = False
                        If Not (.PIN = GlobalLib.TrimOrNull(hrEmp.Pin) And _
                                .Status = hrEmp.Stat.Trim And _
                                .FirstName = hrEmp.FName.Trim And _
                                .MiddleName = GlobalLib.TrimOrNull(hrEmp.MName) And _
                                .LastName = hrEmp.LName.Trim And _
                                .Suffix = GlobalLib.TrimOrNull(hrEmp.Suffix) And _
                                .OrgCode = hrEmp.OrgCode And _
                                .ClassCode = hrEmp.ClassCode And _
                                .HireDate = hrEmp.HireDate) Then
                            .PIN = GlobalLib.TrimOrNull(hrEmp.Pin)
                            .Status = hrEmp.Stat.Trim
                            .FirstName = hrEmp.FName.Trim
                            .MiddleName = GlobalLib.TrimOrNull(hrEmp.MName)
                            .LastName = hrEmp.LName.Trim
                            .Suffix = GlobalLib.TrimOrNull(hrEmp.Suffix)
                            .OrgCode = hrEmp.OrgCode
                            .ClassCode = hrEmp.ClassCode
                            .HireDate = hrEmp.HireDate
                            .LastUpdateDT = Now
                            intUpd += 1
                        End If
                    End With
                End If
                Try
                    ctx.SaveChanges()
                Catch ex As Exception
                    sw.WriteLine("  ** Error in save Employee " & hrEmp.EmpId)
                    Throw ex
                End Try
            Next
            sw.WriteLine("  # of Employees New/Update: " & intNew.ToString & "/" & intUpd.ToString)

        End Using
    End Sub

    Public Sub PTCreateSend(sw As StreamWriter)
        Dim ifp As IFormatProvider = Globalization.DateTimeFormatInfo.InvariantInfo

        'Dim provider As CultureInfo = CultureInfo.InvariantCulture

        Dim bll As PTBL = New PTBL

        Using ctx As New PTEntities()
            sw.WriteLine(Now.ToString("G", ifp) & " == Start Create Release ==")
            Dim intCreateRH As Integer
            Dim intLeadDaysToCreateRelease As Integer = Integer.Parse(ConfigurationManager.AppSettings("LeadDaysToCreateRelease"))
            Dim datFrom As Date = Today.AddDays(-1)
            Dim datTo As Date = Today.AddDays(intLeadDaysToCreateRelease)
            For Each sch As Schedule In ctx.Schedules.Where(
                    Function(s) s.Disabled = False And s.Policies.Count > 0 And s.RecipGroups.Count > 0 _
                        And (s.Frequency <> "ONE_TIME" Or (s.FixedReleaseDate > datFrom And s.FixedReleaseDate <= datTo)))
                Try
                    sw.WriteLine(" CreateRelease for schedule (" & sch.ScheduleId.ToString & ")")
                    intCreateRH = bll.CreateRelease(sch.ScheduleId, BATCH_USER, intLeadDaysToCreateRelease)
                    sw.WriteLine("  affected rows: " & intCreateRH.ToString)

                Catch ex As Exception
                    sw.WriteLine(Now.ToString("G", ifp) & " == Exception ==")
                    WriteExceptions(sw, ex)
                End Try
            Next
            sw.WriteLine(Now.ToString("G", ifp) & " == End Create Release ==")
            ctx.SaveChanges()


            sw.WriteLine(Now.ToString("G", ifp) & " == Start Create Policies/Recipients/Notices ==")
            Dim intCreateRP As Integer
            Dim intCreateNO As Integer
            Dim intLeadDaysToCreateRecipientPolicy As Integer = Integer.Parse(ConfigurationManager.AppSettings("LeadDaysToCreateRecipientPolicy"))
            datFrom = Today.AddDays(-1)
            datTo = Today.AddDays(intLeadDaysToCreateRecipientPolicy)
            For Each rh As Release In ctx.Releases.Where(
                    Function(h) (Not h.ReleaseRecipients.Any()) _
                        And h.ReleaseDate > datFrom _
                        And h.ReleaseDate <= datTo)
                Try
                    sw.WriteLine(" CreateReleasePolicyRecip for Release (" & rh.ReleaseId.ToString & ")")
                    intCreateRP = bll.CreateReleasePolicyRecip(rh.ReleaseId, BATCH_USER)
                    sw.WriteLine("  affected rows: " & intCreateRP.ToString)

                    sw.WriteLine(" CreateReleaseNotices for RelHist (" & rh.ReleaseId.ToString & ")")
                    intCreateNO = bll.CreateReleaseNotices(rh.ReleaseId, BATCH_USER)
                    sw.WriteLine("  affected rows: " & intCreateNO.ToString)

                Catch ex As Exception
                    sw.WriteLine(Now.ToString("G", ifp) & " == Exception ==")
                    WriteExceptions(sw, ex)
                End Try
            Next
            sw.WriteLine(Now.ToString("G", ifp) & " == End Create Policies/Recipients/Notices ==")
            ctx.SaveChanges()


            sw.WriteLine(Now.ToString("G", ifp) & " == Start Create Email Notices ==")
            Dim intCreateEN As Integer
            Dim intLeadDaysToCreateNotice As Integer = Integer.Parse(ConfigurationManager.AppSettings("LeadDaysToCreateNotice"))
            datFrom = Now.AddDays(-1)
            datTo = Now.AddDays(intLeadDaysToCreateNotice)
            For Each rhn As ReleaseNotice In ctx.ReleaseNotices.Where(
                    Function(n) (Not n.RecipientNotices.Any()) _
                        And n.NoticeDate > datFrom _
                        And n.NoticeDate < datTo)
                Try
                    sw.WriteLine(" CreateEmailNotices for ReleaseNotice (" & rhn.ReleaseNoticeId.ToString & ")")
                    intCreateEN = bll.CreateEmailNotices(rhn.ReleaseNoticeId)
                    sw.WriteLine("  affected rows: " & intCreateEN.ToString)

                Catch ex As Exception
                    sw.WriteLine(Now.ToString("G", ifp) & " == Exception ==")
                    WriteExceptions(sw, ex)
                End Try
            Next
            sw.WriteLine(Now.ToString("G", ifp) & " == End Create Email Notices ==")
            ctx.SaveChanges()


            sw.WriteLine(Now.ToString("G", ifp) & " == Start Query/Sending E-Mail notifications ==")
            ' ReleaseNotices
            Dim intRetryDaysToSendNotice As Integer = Integer.Parse(ConfigurationManager.AppSettings("RetryDaysToSendNotice"))
            datFrom = Today.AddDays(-1 * intRetryDaysToSendNotice)
            datTo = Today
            For Each rhn As ReleaseNotice In ctx.ReleaseNotices.Where( _
                        Function(n) n.NoticeDate >= datFrom And n.NoticeDate <= datTo And n.CompleteDT Is Nothing)

                sw.WriteLine(" ReleaseNotice(" & rhn.ReleaseNoticeId.ToString & ") " & "Type:" & rhn.NoticeType & " Date:" & rhn.NoticeDate.ToString("M/d/yyyy"))
                ' set StartDT
                If rhn.StartDT Is Nothing Then
                    rhn.StartDT = Now()
                End If

                Dim intSuccess As Integer = 0
                Dim intFail As Integer = 0

                Dim intRhnID As Integer = rhn.ReleaseNoticeId
                For Each en As RecipientNotice In ctx.RecipientNotices.Where( _
                    Function(e) e.ReleaseNoticeId = intRhnID And e.SentDT Is Nothing)

                    Try
                        sw.WriteLine("  RecipientNotice(" & en.ReleaseNoticeId.ToString & ") To:" & en.RecipientId & "/" & en.To)
                        If bll.SendNoticeToRecip(en) Then
                            intSuccess += 1
                        Else
                            intFail += 1
                        End If
                    Catch ex As Exception
                        intFail += 1
                    End Try
                Next
                For Each en As OrgAdminNotice In ctx.OrgAdminNotices.Where( _
                    Function(e) e.ReleaseNoticeId = intRhnID And e.SentDT Is Nothing)

                    Try
                        sw.WriteLine("  OrgAdminNotice(" & en.ReleaseNoticeId.ToString & ") To:" & en.To)
                        If bll.SendNoticeToOrgAdmin(en) Then
                            intSuccess += 1
                        Else
                            intFail += 1
                        End If
                    Catch ex As Exception
                        intFail += 1
                    End Try
                Next
                If intFail = 0 And intSuccess > 0 Then
                    rhn.CompleteDT = Now()
                End If

                sw.WriteLine(" Sent (" & intSuccess.ToString & ") message(s)")
                sw.WriteLine(" Failed (" & intFail.ToString & ") message(s)")
            Next
            sw.WriteLine(Now.ToString("G", ifp) & " == End Query/Sending E-Mail notifications ==")
            ctx.SaveChanges()

        End Using

    End Sub

    Private Sub WriteExceptions(ByRef swLog As StreamWriter, ex As Exception)
        swLog.Write(ex.Source & vbCrLf & ex.Message & vbCrLf & ex.StackTrace & vbCrLf)
        If ex.InnerException IsNot Nothing Then
            WriteExceptions(swLog, ex.InnerException)
        End If
    End Sub

    Public Sub EmpHRADCompare(sw As StreamWriter)
        Dim _OrgCode As String = "33" ' 83 HARBOR, 85 WATER, 41: DP, 70: PR, 17: TS, 33: Library, 23: Fire, 15: Financial Management
        Dim _ADPath As String = "LDAP://CI.LONG-BEACH.CA.US"  '"LDAP://wd.lbwater.net" ''"LDAP://LBPLDC2.lbpl.local" "LDAP://PLB065.polb.local" 
        Dim _RunLF1 As Boolean = False

        sw.WriteLine("  OrgCode: " & _OrgCode & ", AD Path: " & _ADPath)

        Dim ifp As IFormatProvider = Globalization.DateTimeFormatInfo.InvariantInfo

        Dim strFolder As String = "C:\LOG\PolicyTracker"
        Dim strFileNm As String = "EmpHRADCompare_" & _OrgCode & "_" & Today.ToString("yyyyMMdd") & ".csv"

        Using ctx As New PTEntities()

            Dim emps = (From e In ctx.vEmployees
                        Where e.OrgCode.StartsWith(_OrgCode) And e.Disabled = False
                        Select e Order By e.EmpId).ToList()

            Dim de = New DirectoryEntry(_ADPath)
            'Dim de = New DirectoryEntry(_ADPath, "COLB-LDAP", "COLB-LDAP!", AuthenticationTypes.Secure)

            Dim filter As String
            Dim propertiesToLoad As String() = New String(9) {"sAMAccountName", "cn", "givenName", "sn", "initials", "mail", "userPrincipalName", "department", "title", "employeeType"}
            Dim sr As SearchResult

            Dim intWith As Integer = 0
            Dim intWithOut As Integer = 0

            ' create folder
            If Not Directory.Exists(strFolder) Then
                Directory.CreateDirectory(strFolder)
            End If

            Dim myExport As New Util.CsvExport

            For Each emp In emps
                myExport.AddRow()
                myExport("Emp ID") = emp.EmpId
                myExport("HR FN") = emp.FirstName
                myExport("HR MN") = emp.MiddleName
                myExport("HR LN") = emp.LastName
                myExport("Hire Date") = emp.HireDate
                myExport("Class") = emp.ClassCode
                myExport("ClassDesc") = emp.ClassDesc

                Dim adFound As Boolean = False
                Dim _UserId As String = ""
                Dim _FirstName As String = ""
                Dim _LastName As String = ""
                Dim _MI As String = ""
                Dim _cn As String = ""
                Dim _userPrincipalName As String = ""

                filter = "(&(objectCategory=person)(objectClass=user)(sAMAccountName=" & emp.EmpId & "))"
                'filter = "(&(objectCategory=person)(objectClass=user)(employeeID=" & emp.EmpId.Trim & "*))" ' POLB
                Using deSearch As New DirectorySearcher(filter, propertiesToLoad)
                    deSearch.SearchRoot = de
                    deSearch.SearchScope = SearchScope.Subtree
                    sr = deSearch.FindOne()

                    If sr IsNot Nothing Then
                        Using deUser As New DirectoryEntry(sr.Path)
                            intWith += 1 : adFound = True
                            myExport("sAMAccountName") = ADHelper.GetProperty(deUser, "sAMAccountName")
                            myExport("givenName") = ADHelper.GetProperty(deUser, "givenName")
                            myExport("initials") = ADHelper.GetProperty(deUser, "initials")
                            myExport("sn") = ADHelper.GetProperty(deUser, "sn")
                            myExport("cn") = ADHelper.GetProperty(deUser, "cn")
                            myExport("userPrincipalName") = ADHelper.GetProperty(deUser, "userPrincipalName")
                            myExport("mail") = ADHelper.GetProperty(deUser, "mail")
                            myExport("description") = ADHelper.GetProperty(deUser, "description")
                            myExport("department") = ADHelper.GetProperty(deUser, "department")
                            myExport("title") = ADHelper.GetProperty(deUser, "title")
                            myExport("employeeType") = ADHelper.GetProperty(deUser, "employeeType")
                            myExport("distinguishedName") = ADHelper.GetProperty(deUser, "distinguishedName")
                            myExport("disabled") = ADHelper.IsDisabled(deUser.Properties("userAccountControl").Value).ToString
                            myExport("Mapping") = "ID"
                        End Using
                    End If
                End Using

                If adFound = False Then
                    filter = "(&(objectCategory=person)(objectClass=user)(sn=" & emp.LastName & ")(givenName=" & emp.FirstName & "))"
                    Using deSearch As New DirectorySearcher(filter, propertiesToLoad)
                        deSearch.SearchRoot = de
                        deSearch.SearchScope = SearchScope.Subtree
                        sr = deSearch.FindOne()

                        If sr IsNot Nothing Then
                            Using deUser As New DirectoryEntry(sr.Path)
                                intWith += 1 : adFound = True
                                myExport("sAMAccountName") = ADHelper.GetProperty(deUser, "sAMAccountName")
                                myExport("givenName") = ADHelper.GetProperty(deUser, "givenName")
                                myExport("initials") = ADHelper.GetProperty(deUser, "initials")
                                myExport("sn") = ADHelper.GetProperty(deUser, "sn")
                                myExport("cn") = ADHelper.GetProperty(deUser, "cn")
                                myExport("userPrincipalName") = ADHelper.GetProperty(deUser, "userPrincipalName")
                                myExport("mail") = ADHelper.GetProperty(deUser, "mail")
                                myExport("description") = ADHelper.GetProperty(deUser, "description")
                                myExport("department") = ADHelper.GetProperty(deUser, "department")
                                myExport("title") = ADHelper.GetProperty(deUser, "title")
                                myExport("employeeType") = ADHelper.GetProperty(deUser, "employeeType")
                                myExport("distinguishedName") = ADHelper.GetProperty(deUser, "distinguishedName")
                                myExport("disabled") = ADHelper.IsDisabled(deUser.Properties("userAccountControl").Value).ToString
                                myExport("Mapping") = "LF"
                            End Using
                        End If
                    End Using
                End If

                If adFound = False And _RunLF1 Then
                    filter = "(&(objectCategory=person)(objectClass=user)(sn=" & emp.LastName & ")(givenName=" & emp.FirstName.Substring(0, 1) & "*))"
                    Using deSearch As New DirectorySearcher(filter, propertiesToLoad)
                        deSearch.SearchRoot = de
                        deSearch.SearchScope = SearchScope.Subtree
                        sr = deSearch.FindOne()

                        If sr IsNot Nothing Then
                            Using deUser As New DirectoryEntry(sr.Path)
                                intWith += 1 : adFound = True
                                myExport("sAMAccountName") = ADHelper.GetProperty(deUser, "sAMAccountName")
                                myExport("givenName") = ADHelper.GetProperty(deUser, "givenName")
                                myExport("initials") = ADHelper.GetProperty(deUser, "initials")
                                myExport("sn") = ADHelper.GetProperty(deUser, "sn")
                                myExport("cn") = ADHelper.GetProperty(deUser, "cn")
                                myExport("userPrincipalName") = ADHelper.GetProperty(deUser, "userPrincipalName")
                                myExport("mail") = ADHelper.GetProperty(deUser, "mail")
                                myExport("description") = ADHelper.GetProperty(deUser, "description")
                                myExport("department") = ADHelper.GetProperty(deUser, "department")
                                myExport("title") = ADHelper.GetProperty(deUser, "title")
                                myExport("employeeType") = ADHelper.GetProperty(deUser, "employeeType")
                                myExport("distinguishedName") = ADHelper.GetProperty(deUser, "distinguishedName")
                                myExport("disabled") = ADHelper.IsDisabled(deUser.Properties("userAccountControl").Value).ToString
                                myExport("Mapping") = "LF1"
                            End Using
                        End If
                    End Using
                End If

                If adFound = False Then intWithOut += 1
            Next

            myExport.ExportToFile(strFolder & "\" & strFileNm)

            sw.WriteLine("  # of Employees With AD/Without AD/Total: " & intWith.ToString & "/" & intWithOut.ToString & "/" & emps.Count.ToString)

        End Using
    End Sub

End Class
