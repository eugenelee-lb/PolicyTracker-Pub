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

Partial Public Class RecipGroup
    Public Property RecipGroupId As Integer
    Public Property GroupName As String
    Public Property RecipGroupType As String
    Public Property ShareType As String
    Public Property DeptCode As String
    Public Property Disabled As Boolean
    Public Property CreateDT As Date
    Public Property CreateUser As String
    Public Property LastUpdateDT As Date
    Public Property LastUpdateUser As String

    Public Overridable Property RecipGroupOrgs As ICollection(Of RecipGroupOrg) = New HashSet(Of RecipGroupOrg)
    Public Overridable Property RecipGroupOwners As ICollection(Of RecipGroupOwner) = New HashSet(Of RecipGroupOwner)
    Public Overridable Property Classifications As ICollection(Of Classification) = New HashSet(Of Classification)
    Public Overridable Property Employees As ICollection(Of Employee) = New HashSet(Of Employee)
    Public Overridable Property Schedules As ICollection(Of Schedule) = New HashSet(Of Schedule)

End Class