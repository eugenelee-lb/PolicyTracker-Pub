Imports PolicyTracker.Lib
Imports System.IO

Public Class WebUtil
    Public Shared Function ProcessException(ByVal ex As Exception) As String

        If ex.GetType.Equals(GetType(System.Threading.ThreadAbortException)) Then
            'strLog &= vbCrLf & "**** ThreadAbortException"
            Return ""
        End If

        Dim strLog As String = String.Empty
        Dim strMessage As String = ""

        Dim cpUser As CustomPrincipal
        Try
            cpUser = CType(HttpContext.Current.User, CustomPrincipal)
            strLog &= "Domain\User, Role: " & cpUser.Domain & "\" & cpUser.UserId & " " & cpUser.UserName & ", " & cpUser.Role & vbCrLf
        Catch ex1 As Exception
        End Try

        Dim session As System.Web.SessionState.HttpSessionState = HttpContext.Current.Session
        If session IsNot Nothing Then
            strLog &= "Session Values" & vbCrLf
            For ii As Integer = 0 To session.Count - 1
                If Not (session.Item(ii) Is Nothing) Then
                    strLog &= session.Keys(ii).ToString & ":" & session.Item(ii).ToString & vbCrLf
                End If
            Next
        End If

        Dim request As System.Web.HttpRequest = HttpContext.Current.Request
        strLog &= "PhysicalPath:" & request.PhysicalPath.ToString & vbCrLf _
            & "QueryString:" & request.QueryString.ToString & vbCrLf _
            & "UserHostAddress:" & request.UserHostAddress.ToString & vbCrLf _
            & "UserAgent: " & request.UserAgent & vbCrLf _
            & "ex.Source:" & ex.Source & vbCrLf _
            & "ex.Message:" & vbCrLf & ex.Message & vbCrLf _
            & "ex.StackTrace:" & ex.StackTrace & vbCrLf _
            & "ex.GetType:" & ex.GetType.ToString()

        ' System.Data.Entity.Validation.DbEntityValidationException
        If TypeOf ex Is System.Data.Entity.Validation.DbEntityValidationException Then
            Dim valEx As System.Data.Entity.Validation.DbEntityValidationException = ex
            For Each valErrors In valEx.EntityValidationErrors
                For Each valError In valErrors.ValidationErrors
                    strLog &= vbCrLf _
                        & String.Format("Class: {0}, Property: {1}, Error: {2}", valErrors.Entry.Entity.GetType().FullName, valError.PropertyName, valError.ErrorMessage)
                Next
            Next
        End If

        ' File Log
        Dim ERR_LOG_DIR As String = request.PhysicalApplicationPath & "\App_Data"

        If Not Directory.Exists(ERR_LOG_DIR) Then
            Directory.CreateDirectory(ERR_LOG_DIR)
        End If
        Dim strFilePath As String = ERR_LOG_DIR & "\ERR_" & Now.ToString("yyyyMM") & ".txt"
        Dim sw As StreamWriter = File.AppendText(strFilePath)
        Try
            sw.Write(vbCrLf & DateTime.Now.ToString("G") & vbCrLf)
            sw.Write(strLog & vbCrLf)
        Finally
            ' Close the stream to the file.
            sw.Close()
        End Try

        ' handle InnerException
        If ex.InnerException IsNot Nothing Then
            strMessage = ProcessException(ex.InnerException)
        Else
            ' Error Message
            If TypeOf ex Is System.Data.SqlClient.SqlException AndAlso CType(ex, System.Data.SqlClient.SqlException).Number = 2627 Then
                strMessage = HtmlMsgEncode(SettingsBL.GetMessageText(-9106)) 'Violation of PRIMARY KEY constraint
            ElseIf TypeOf ex Is System.Data.SqlClient.SqlException AndAlso CType(ex, System.Data.SqlClient.SqlException).Number = 547 Then
                strMessage = HtmlMsgEncode(SettingsBL.GetMessageText(-9107)) ' The DELETE statement conflicted with the REFERENCE constraint "FK_
            ElseIf TypeOf ex Is System.Data.Entity.Core.OptimisticConcurrencyException Then
                strMessage = HtmlMsgEncode(SettingsBL.GetMessageText(-9100)) ' Concurrency Error - Update
            ElseIf TypeOf ex Is System.Data.NoNullAllowedException Then
                strMessage = HtmlMsgEncode(SettingsBL.GetMessageText(-9104)) '"There are one or more required fields that are missing."
            ElseIf TypeOf ex Is ArgumentException Then
                strMessage = HtmlMsgEncode(SettingsBL.GetMessageText(-9105, CType(ex, ArgumentException).ParamName)) 'The {paramName} value is illegal.
            ElseIf TypeOf ex Is ApplicationException Then
                strMessage = ex.Message
            Else
                strMessage = HtmlMsgEncode(SettingsBL.GetMessageText(-9999)) '"System error. Please try again later."
            End If
        End If

        Return strMessage
    End Function

    Public Shared Sub ExceptionHandle(ByVal ex As Exception)

        If ex.GetType.Equals(GetType(System.Threading.ThreadAbortException)) Then
            'strLog &= vbCrLf & "**** ThreadAbortException"
            Exit Sub
        End If

        Dim strLog As String = String.Empty

        Dim cpUser As CustomPrincipal
        Try
            cpUser = CType(HttpContext.Current.User, CustomPrincipal)
            strLog &= "Domain\User, Role: " & cpUser.Domain & "\" & cpUser.UserId & " " & cpUser.UserName & ", " & cpUser.Role & vbCrLf
        Catch ex1 As Exception
        End Try

        Dim session As System.Web.SessionState.HttpSessionState = HttpContext.Current.Session
        If session IsNot Nothing Then
            strLog &= "Session Values" & vbCrLf
            For ii As Integer = 0 To session.Count - 1
                If Not (session.Item(ii) Is Nothing) Then
                    strLog &= session.Keys(ii).ToString & ":" & session.Item(ii).ToString & vbCrLf
                End If
            Next
        End If

        Dim request As System.Web.HttpRequest = HttpContext.Current.Request
        strLog &= "PhysicalPath:" & request.PhysicalPath.ToString & vbCrLf _
            & "QueryString:" & request.QueryString.ToString & vbCrLf _
            & "UserHostAddress:" & request.UserHostAddress.ToString & vbCrLf _
            & "UserAgent: " & request.UserAgent & vbCrLf
        strLog &= "ex.Source:" & ex.Source & vbCrLf _
            & "ex.Message:" & vbCrLf & ex.Message & vbCrLf _
            & "ex.StackTrace:" & ex.StackTrace & vbCrLf _
            & "ex.GetType:" & ex.GetType.ToString()

        ' File Log
        Dim ERR_LOG_DIR As String = request.PhysicalApplicationPath & "\App_Data"

        If Not Directory.Exists(ERR_LOG_DIR) Then
            Directory.CreateDirectory(ERR_LOG_DIR)
        End If
        Dim strFilePath As String = ERR_LOG_DIR & "\ERR_" & Now.ToString("yyyyMM") & ".txt"
        Dim sw As StreamWriter = File.AppendText(strFilePath)
        Try
            sw.Write(vbCrLf & DateTime.Now.ToString("G") & vbCrLf)
            sw.Write(strLog & vbCrLf)
        Finally
            ' Close the stream to the file.
            sw.Close()
        End Try

        ' handle InnerException
        If ex.InnerException IsNot Nothing Then
            ExceptionHandle(ex.InnerException)
        End If

    End Sub

    Public Shared Sub WriteLog(ByVal logText As String)
        ' File Log
        Dim strLog As String = String.Empty

        Dim cpUser As CustomPrincipal
        Try
            cpUser = CType(HttpContext.Current.User, CustomPrincipal)
            strLog &= "Domain\User, Role: " & cpUser.Domain & "\" & cpUser.UserId & " " & cpUser.UserName & ", " & cpUser.Role & vbCrLf
        Catch ex1 As Exception
        End Try

        Dim session As System.Web.SessionState.HttpSessionState = HttpContext.Current.Session
        strLog &= "Session Values" & vbCrLf
        For ii As Integer = 0 To session.Count - 1
            If Not (session.Item(ii) Is Nothing) Then
                strLog &= session.Keys(ii).ToString & ":" & session.Item(ii).ToString & vbCrLf
            End If
        Next
        Dim request As System.Web.HttpRequest = HttpContext.Current.Request
        strLog &= "PhysicalPath:" & request.PhysicalPath.ToString & vbCrLf _
            & "QueryString:" & request.QueryString.ToString & vbCrLf _
            & "UserHostAddress:" & request.UserHostAddress.ToString & vbCrLf _
            & "UserAgent: " & request.UserAgent & vbCrLf

        strLog &= logText

        Dim ERR_LOG_DIR As String = request.PhysicalApplicationPath & "\App_Data"

        If Not Directory.Exists(ERR_LOG_DIR) Then
            Directory.CreateDirectory(ERR_LOG_DIR)
        End If
        Dim strFilePath As String = ERR_LOG_DIR & "\LOG_" & Now.ToString("yyyyMM") & ".txt"
        Dim sw As StreamWriter = File.AppendText(strFilePath)
        Try
            sw.Write(vbCrLf & DateTime.Now.ToString("G") & vbCrLf)
            sw.Write(strLog & vbCrLf)
        Finally
            ' Close the stream to the file.
            sw.Close()
        End Try

    End Sub

    ' Solve a problem with read only text boxes failing to be post back
    Public Shared Sub SetReadonlyTextBoxes(ByVal Page As Control)
        For Each ctrl As Control In Page.Controls
            If TypeOf ctrl Is TextBox Then
                If CType(ctrl, TextBox).ReadOnly Then
                    CType(ctrl, TextBox).Text = HttpContext.Current.Request.Params(CType(ctrl, TextBox).UniqueID)
                End If
            Else
                If ctrl.Controls.Count > 0 Then
                    SetReadonlyTextBoxes(ctrl)
                End If
            End If
        Next
    End Sub

    ' Set Grid's Row Styles
    Public Shared Sub SetGridRowStyle(ByRef gv As GridView)
        Dim onMouseOutStyle As String = "this.className = '@ClassName'"
        Dim className As String = String.Empty

        For Each row As GridViewRow In gv.Rows
            If row.RowType = DataControlRowType.DataRow Then
                If row.RowState = DataControlRowState.Alternate Then
                    className = "AlternatingRowStyle"
                Else
                    className = "RowStyle"
                End If
                If row.RowIndex = gv.SelectedIndex Then
                    className = "SelectedRowStyle"
                End If

                row.Attributes.Add("onmouseover", "this.className='SelectedRowStyle'")
                row.Attributes.Add("onmouseout", onMouseOutStyle.Replace("@ClassName", className))
            End If
        Next

    End Sub

    ' Convert message text to display on web pages: HtmlEncode and replace VbCrLf with <br /> 
    Public Shared Function HtmlMsgEncode(ByVal msgText As String) As String
        ' Encode the string input
        Dim sb As StringBuilder = New StringBuilder(HttpUtility.HtmlEncode(msgText))

        ' Selectively allow  <b> and <i>
        sb.Replace("&lt;b&gt;", "<b>")
        sb.Replace("&lt;/b&gt;", "</b>")
        sb.Replace("&lt;i&gt;", "<i>")
        sb.Replace("&lt;/i&gt;", "</i>")

        ' Replace CRLF with <br />
        sb.Replace(vbCrLf, "<br />")
        sb.Replace(vbLf, "<br />")
        sb.Replace(vbCr, "<br />")

        Return sb.ToString()
    End Function

    Public Shared Function SelectGridRowByKeyValue(ByRef gv As GridView, ByVal keyValues As Collections.Specialized.NameValueCollection) As Boolean
        ' * You must have the DataKeyNames set on the GridView for this to work

        Dim intSelectedIndex As Integer = -1 ' by default no row is selected (if the row with keyValue is not found)
        Dim intPageIndex As Integer = 0 ' default page is the first page

        gv.DataBind()
        Dim intGridViewPages As Integer = gv.PageCount

        Dim boolFound As Boolean = False
        ' Loop thru each page in the GridView
        For intPage As Integer = 0 To intGridViewPages - 1
            If boolFound Then Exit For

            If gv.PageIndex <> intPage Then
                ' Set the current GridView page
                gv.PageIndex = intPage
                ' Bind the GridView to the current page
                gv.DataBind()
            End If

            ' Loop thru each DataKey(row) in the current page of GridView
            For i As Integer = 0 To gv.DataKeys.Count - 1
                Dim boolEqual As Boolean = True
                ' Loop thru each Key columns
                For j As Integer = 0 To keyValues.Count - 1
                    If keyValues(j).ToString <> gv.DataKeys(i).Values(j).ToString Then boolEqual = False
                Next

                If boolEqual Then
                    ' If it is a match set the variables and exit
                    intSelectedIndex = i
                    intPageIndex = intPage
                    boolFound = True
                    Exit For
                End If
            Next
        Next

        ' Set the GridView to the values found
        If gv.PageIndex <> intPageIndex Then gv.PageIndex = intPageIndex
        gv.SelectedIndex = intSelectedIndex
        If gv.PageIndex <> intPageIndex Then gv.DataBind()

        SetGridRowStyle(gv)

        Return boolFound
    End Function

    'In JavaScript you can add special characters to a text string by using the backslash sign.
    'Insert Special Characters
    'The backslash (\) is used to insert apostrophes, new lines, quotes, and other special characters into a text string.
    Public Shared Function JSStringEncode(ByVal orgText As String) As String
        Return orgText.Replace("\", "\\").Replace("'", "\'").Replace("""", "\""").Replace("&", "\&").Replace(vbNewLine, "\n").Replace(vbCr, "\r").Replace(vbTab, "\t").Replace(vbBack, "\b").Replace(vbFormFeed, "\f")
    End Function

    ''' <summary>
    ''' Gets the MIME type of the file name specified based on the file name's
    ''' extension.  If the file's extension is unknown, returns "octet-stream"
    ''' generic for streaming file bytes.
    ''' </summary>
    ''' <param name="sFileName">The name of the file for which the MIME type
    ''' refers to.</param>
    Public Shared Function GetMimeTypeByFileName(ByVal sFileName As String) As String
        Dim sMime As String = "application/octet-stream"

        Dim sExtension As String = System.IO.Path.GetExtension(sFileName)
        If Not String.IsNullOrEmpty(sExtension) Then
            sExtension = sExtension.Replace(".", "")
            sExtension = sExtension.ToLower()

            If sExtension = "xls" OrElse sExtension = "xlsx" Then
                sMime = "application/ms-excel"
            ElseIf sExtension = "doc" OrElse sExtension = "docx" Then
                sMime = "application/msword"
            ElseIf sExtension = "ppt" OrElse sExtension = "pptx" Then
                sMime = "application/ms-powerpoint"
            ElseIf sExtension = "rtf" Then
                sMime = "application/rtf"
            ElseIf sExtension = "zip" Then
                sMime = "application/zip"
            ElseIf sExtension = "mp3" Then
                sMime = "audio/mpeg"
            ElseIf sExtension = "bmp" Then
                sMime = "image/bmp"
            ElseIf sExtension = "gif" Then
                sMime = "image/gif"
            ElseIf sExtension = "jpg" OrElse sExtension = "jpeg" Then
                sMime = "image/jpeg"
            ElseIf sExtension = "png" Then
                sMime = "image/png"
            ElseIf sExtension = "tiff" OrElse sExtension = "tif" Then
                sMime = "image/tiff"
            ElseIf sExtension = "txt" Then
                sMime = "text/plain"
            End If
        End If

        Return sMime
    End Function

    ''' <summary>
    ''' Streams the bytes specified as a file with the name specified using HTTP to the 
    ''' calling browser.
    ''' </summary>
    ''' <param name="tFileName">The name of the file as it will apear when the user
    ''' clicks either open or save as in their browser to accept the file
    ''' download.</param>
    ''' <param name="fileBytes">The file as a byte array to be streamed.</param>
    Public Shared Sub StreamFileToBrowser(ByVal contentType As String, ByVal tFileName As String, ByVal fileBytes As Byte())
        Dim context As System.Web.HttpContext = System.Web.HttpContext.Current
        context.Response.Clear()
        context.Response.ClearHeaders()
        context.Response.ClearContent()
        context.Response.AppendHeader("content-length", fileBytes.Length.ToString())
        context.Response.ContentType = contentType
        context.Response.AppendHeader("content-disposition", "attachment; filename=" & tFileName)
        context.Response.BinaryWrite(fileBytes)

        ' use this instead of response.end to avoid thread aborted exception (known issue):
        ' http://support.microsoft.com/kb/312629/EN-US
        context.ApplicationInstance.CompleteRequest()
    End Sub

    Public Shared Function EncodeAsFileName(ByVal fileName As String, ByVal replaceWith As String) As String
        Return Regex.Replace(fileName, "[" + Regex.Escape(New String(Path.GetInvalidFileNameChars())) + "]", replaceWith)
    End Function
End Class
