'Imports Microsoft.VisualBasic
'Imports System.Collections.Generic
Imports System.Data.SqlTypes
Imports System.IO
'Imports System.Linq
Imports System.Text
'Imports System.Web

Namespace Util

    ''' <summary>
    ''' Simple CSV export
    ''' Example:
    '''   CsvExport myExport = new CsvExport();
    '''
    '''   myExport.AddRow();
    '''   myExport["Region"] = "New York, USA";
    '''   myExport["Sales"] = 100000;
    '''   myExport["Date Opened"] = new DateTime(2003, 12, 31);
    '''
    '''   myExport.AddRow();
    '''   myExport["Region"] = "Sydney \"in\" Australia";
    '''   myExport["Sales"] = 50000;
    '''   myExport["Date Opened"] = new DateTime(2005, 1, 1, 9, 30, 0);
    '''
    ''' Then you can do any of the following three output options:
    '''   string myCsv = myExport.Export();
    '''   myExport.ExportToFile("Somefile.csv");
    '''   byte[] myCsvData = myExport.ExportToBytes();
    ''' </summary>
    Public Class CsvExport
        ''' <summary>
        ''' To keep the ordered list of column names
        ''' </summary>
        Private fields As New List(Of String)()

        ''' <summary>
        ''' The list of rows
        ''' </summary>
        Private rows As New List(Of Dictionary(Of String, Object))()

        ''' <summary>
        ''' The current row
        ''' </summary>
        Private ReadOnly Property currentRow() As Dictionary(Of String, Object)
            Get
                Return rows(rows.Count - 1)
            End Get
        End Property

        ''' <summary>
        ''' Set a value on this column
        ''' </summary>
        Default Public WriteOnly Property Item(field As String) As Object
            Set
                ' Keep track of the field names, because the dictionary loses the ordering
                If Not fields.Contains(field) Then
                    fields.Add(field)
                End If
                currentRow(field) = Value
            End Set
        End Property

        ''' <summary>
        ''' Call this before setting any fields on a row
        ''' </summary>
        Public Sub AddRow()
            rows.Add(New Dictionary(Of String, Object)())
        End Sub

        ''' <summary>
        ''' Add a list of typed objects, maps object properties to CsvFields
        ''' </summary>
        Public Sub AddRows(Of T)(list As IEnumerable(Of T))
            If list.Any() Then
                For Each obj In list
                    AddRow()
                    Dim values = obj.[GetType]().GetProperties()
                    For Each value In values
                        Me(value.Name) = value.GetValue(obj, Nothing)
                    Next
                Next
            End If
        End Sub

        ''' <summary>
        ''' Converts a value to how it should output in a csv file
        ''' If it has a comma, it needs surrounding with double quotes
        ''' Eg Sydney, Australia -> "Sydney, Australia"
        ''' Also if it contains any double quotes ("), then they need to be replaced with quad quotes[sic] ("")
        ''' Eg "Dangerous Dan" McGrew -> """Dangerous Dan"" McGrew"
        ''' </summary>
        Public Shared Function MakeValueCsvFriendly(value As Object) As String
            If value Is Nothing Then
                Return ""
            End If
            If TypeOf value Is INullable AndAlso DirectCast(value, INullable).IsNull Then
                Return ""
            End If
            If TypeOf value Is DateTime Then
                If DirectCast(value, DateTime).TimeOfDay.TotalSeconds = 0 Then
                    Return DirectCast(value, DateTime).ToString("yyyy-MM-dd")
                End If
                Return DirectCast(value, DateTime).ToString("yyyy-MM-dd HH:mm:ss")
            End If
            Dim output As String = value.ToString().Trim()
            If output.Contains(",") OrElse output.Contains("""") OrElse output.Contains(vbLf) OrElse output.Contains(vbCr) Then
                output = """"c + output.Replace("""", """""") + """"c
            End If

            If output.Length > 30000 Then
                'cropping value for stupid Excel
                If output.EndsWith("""") Then
                    output = output.Substring(0, 30000)
                    If output.EndsWith("""") AndAlso Not output.EndsWith("""""") Then
                        'rare situation when cropped line ends with a '"'
                        output += """"
                    End If
                    'add another '"' to escape it
                    output += """"
                Else
                    output = output.Substring(0, 30000)
                End If
            End If
            Return output
        End Function

        ''' <summary>
        ''' Outputs all rows as a CSV, returning one string at a time
        ''' </summary>
        Private Iterator Function ExportToLines() As IEnumerable(Of String)
            Yield "sep=,"

            ' The header
            Yield String.Join(",", fields)

            ' The rows
            For Each row As Dictionary(Of String, Object) In rows
                For Each k As String In fields.Where(Function(f) Not row.ContainsKey(f))
                    row(k) = Nothing
                Next
                Yield String.Join(",", fields.[Select](Function(field) MakeValueCsvFriendly(row(field))))
            Next
        End Function

        ''' <summary>
        ''' Output all rows as a CSV returning a string
        ''' </summary>
        Public Function Export() As String
            Dim sb As New StringBuilder()

            For Each line As String In ExportToLines()
                sb.AppendLine(line)
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' Exports to a file
        ''' </summary>
        Public Sub ExportToFile(path As String)
            File.WriteAllLines(path, ExportToLines(), Encoding.UTF8)
        End Sub

        ''' <summary>
        ''' Exports as raw UTF8 bytes
        ''' </summary>
        Public Function ExportToBytes() As Byte()
            Dim data = Encoding.UTF8.GetBytes(Export())
            Return Encoding.UTF8.GetPreamble().Concat(data).ToArray()
        End Function
    End Class

End Namespace
