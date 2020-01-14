Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Configuration

Public Class HRService

    Public Sub GetData(Of T)(tableName As String, ByRef dto As T)
        Using client = New HttpClient()
            Dim baseUrl As String = ConfigurationManager.AppSettings("HRWebAPI_BaseURL") 'Request.Url.Scheme + "://" + Request.Url.Authority & Request.ApplicationPath.TrimEnd("/"c) & "/"
            client.BaseAddress = New Uri(baseUrl)
            client.DefaultRequestHeaders.Accept.Clear()
            client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

            Dim response As HttpResponseMessage = client.GetAsync("api/" & tableName).Result
            If response.IsSuccessStatusCode Then
                dto = response.Content.ReadAsAsync(Of T)().Result
            Else
                'divMessage.InnerHtml = ("Error Code - " + response.StatusCode & " : Message - ") + response.ReasonPhrase
                Throw New ApplicationException("Error Code - " + response.StatusCode.ToString & " : Message - " + response.ReasonPhrase)
            End If
        End Using

    End Sub
End Class
