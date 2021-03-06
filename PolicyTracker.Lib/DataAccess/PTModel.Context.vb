﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Public Class PTEntities
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=PTEntities")
    End Sub

    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        Throw New UnintentionalCodeFirstException()
    End Sub

    Public Overridable Property AppUsers() As DbSet(Of AppUser)
    Public Overridable Property CommonCodes() As DbSet(Of CommonCode)
    Public Overridable Property Configurations() As DbSet(Of Configuration)
    Public Overridable Property Messages() As DbSet(Of Message)
    Public Overridable Property Notices() As DbSet(Of Notice)
    Public Overridable Property Preferences() As DbSet(Of Preference)
    Public Overridable Property USStates() As DbSet(Of USState)
    Public Overridable Property Bureaus() As DbSet(Of Bureau)
    Public Overridable Property Classifications() As DbSet(Of Classification)
    Public Overridable Property Departments() As DbSet(Of Department)
    Public Overridable Property Divisions() As DbSet(Of Division)
    Public Overridable Property Employees() As DbSet(Of Employee)
    Public Overridable Property UserOrgs() As DbSet(Of UserOrg)
    Public Overridable Property vEmployees() As DbSet(Of vEmployee)
    Public Overridable Property vOrganizations() As DbSet(Of vOrganization)
    Public Overridable Property OrgAdminNotices() As DbSet(Of OrgAdminNotice)
    Public Overridable Property PolicyOwners() As DbSet(Of PolicyOwner)
    Public Overridable Property RecipGroupOrgs() As DbSet(Of RecipGroupOrg)
    Public Overridable Property RecipGroupOwners() As DbSet(Of RecipGroupOwner)
    Public Overridable Property RecipientNotices() As DbSet(Of RecipientNotice)
    Public Overridable Property ReleasePolicies() As DbSet(Of ReleasePolicy)
    Public Overridable Property Releases() As DbSet(Of Release)
    Public Overridable Property ScheduleOwners() As DbSet(Of ScheduleOwner)
    Public Overridable Property UploadFiles() As DbSet(Of UploadFile)
    Public Overridable Property RecipGroups() As DbSet(Of RecipGroup)
    Public Overridable Property Policies() As DbSet(Of Policy)
    Public Overridable Property Schedules() As DbSet(Of Schedule)
    Public Overridable Property vOrgAdmins() As DbSet(Of vOrgAdmin)
    Public Overridable Property ReleaseRecipients() As DbSet(Of ReleaseRecipient)
    Public Overridable Property PacketAckFiles() As DbSet(Of PacketAckFile)
    Public Overridable Property ReleaseNotices() As DbSet(Of ReleaseNotice)
    Public Overridable Property vPackets() As DbSet(Of vPacket)

End Class
