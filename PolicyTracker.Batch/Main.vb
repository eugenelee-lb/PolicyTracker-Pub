Option Explicit On
Option Strict On
Imports Microsoft.VisualBasic
Imports System.Data
Imports System.IO
Imports System.Configuration
Imports System.Globalization
Imports PolicyTracker.Lib

Module Main

    Private LOG_DIR As String = ConfigurationManager.AppSettings("LOG_DIR") '".\LOG"
    Private LOG_FILE As String

    Sub Main()
        ' Evaluate arguments
        Dim args As String() = Environment.GetCommandLineArgs()
        Dim strOption As String = ""
        Dim blnSyntaxError As Boolean = False

        If args.Length = 2 Then
            strOption = args(1).ToUpper.Trim
            If strOption <> "/HR" And strOption <> "/PT" And strOption <> "/HRAD" _
                Then blnSyntaxError = True
        Else
            blnSyntaxError = True
        End If

        If blnSyntaxError Then
            Console.WriteLine("=== Syntax Error ===")
            Console.WriteLine("Usage: " & args(0) & " /Switch")
            Console.WriteLine("Switches:")
            Console.WriteLine("  HR: HR data update")
            Console.WriteLine("  PT: PT craete data & send notices")
            Console.WriteLine("  HRAD: HR/AD compare")
            Exit Sub
            'strOption = "/HR"
        End If

        ' Log and Data file creation
        Dim swLog As StreamWriter
        Dim ifp As IFormatProvider = Globalization.DateTimeFormatInfo.InvariantInfo

        LOG_FILE = LOG_DIR & "\BATCH_" & Now.ToString("yyyyMMdd") & ".txt"

        If Not Directory.Exists(LOG_DIR) Then
            Directory.CreateDirectory(LOG_DIR)
        End If

        If Not File.Exists(LOG_FILE) Then
            swLog = File.CreateText(LOG_FILE)
        Else
            swLog = File.AppendText(LOG_FILE)
        End If

        Try
            Dim batch As PolicyTracker.Lib.Batch = New PolicyTracker.Lib.Batch

            swLog.WriteLine("")

            ' HR data update
            If strOption = "/HR" Then
                swLog.WriteLine(Now.ToString("G", ifp) & " ===== Start HR Data Update =====")
                batch.HRDataUpdate(swLog)
                swLog.WriteLine(Now.ToString("G", ifp) & " ===== End HR Data Update =====")
            End If

            If strOption = "/PT" Then
                swLog.WriteLine(Now.ToString("G", ifp) & " ===== Start PT Create Data & Send Notices =====")
                batch.PTCreateSend(swLog)
                swLog.WriteLine(Now.ToString("G", ifp) & " ===== End PT Create Data & Send Notices =====")
            End If

            If strOption = "/HRAD" Then
                swLog.WriteLine(Now.ToString("G", ifp) & " ===== Start HR/AD compare =====")
                batch.EmpHRADCompare(swLog)
                swLog.WriteLine(Now.ToString("G", ifp) & " ===== End HR/AD compare =====")
            End If

        Catch ex As Exception
            swLog.WriteLine(Now.ToString("G", ifp) & " ===== Exception =====")
            LogEx(swLog, ex)
            System.Environment.ExitCode = 1

        Finally
            swLog.Close()

        End Try
    End Sub

    Private Sub LogEx(ByRef swLog As StreamWriter, ByRef ex As Exception)
        swLog.Write(ex.Source & vbCrLf & ex.Message & vbCrLf & ex.StackTrace & vbCrLf & "ex.GetType:" & ex.GetType.ToString() & vbCrLf)
        ' System.Data.Entity.Validation.DbEntityValidationException
        If TypeOf ex Is System.Data.Entity.Validation.DbEntityValidationException Then
            Dim valEx = CType(ex, System.Data.Entity.Validation.DbEntityValidationException)
            For Each valErrors In valEx.EntityValidationErrors
                For Each valError In valErrors.ValidationErrors
                    swLog.Write(String.Format("Class: {0}, Property: {1}, Error: {2}", valErrors.Entry.Entity.GetType().FullName, valError.PropertyName, valError.ErrorMessage) & vbCrLf)
                Next
            Next
        End If
        ' inner
        If ex.InnerException IsNot Nothing Then
            LogEx(swLog, ex.InnerException)
        End If
    End Sub

End Module
