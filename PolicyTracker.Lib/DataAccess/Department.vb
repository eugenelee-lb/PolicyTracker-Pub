'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class Department
    Public Property DeptCode As String
    Public Property DeptDesc As String
    Public Property FamisOrgCode As String
    Public Property ADOU As String
    Public Property DefaultUserGroup As String
    Public Property Disabled As Boolean
    Public Property CreateDT As Date
    Public Property CreateUser As String
    Public Property LastUpdateDT As Date
    Public Property LastUpdateUser As String

    Public Overridable Property Bureaus As ICollection(Of Bureau) = New HashSet(Of Bureau)

End Class
