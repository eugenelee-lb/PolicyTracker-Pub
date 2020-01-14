Imports Microsoft.VisualBasic
Imports System.Collections
Imports ADWrapper

Public Class ADUserArrList
    Inherits ArrayList

    Public Enum UserFields
        InitValue
        UserName
        FirstName
        MiddleInitial
        LastName
        DisplayName
        Title
        Email
        DistinguishedName
        IsAccountActive
    End Enum 'UserFields

    Public Overloads Sub Sort(ByVal sortField As UserFields, ByVal isAscending As Boolean)
        Select Case sortField
            Case UserFields.UserName
                Me.Sort(New UserNameComparer)
            Case UserFields.FirstName
                Me.Sort(New FirstNameComparer)
            Case UserFields.LastName
                Me.Sort(New LastNameComparer)
            Case UserFields.DisplayName
                Me.Sort(New DisplayNameComparer)
            Case UserFields.Email
                Me.Sort(New EmailComparer)
            Case UserFields.IsAccountActive
                Me.Sort(New IsAccountActiveComparer)
        End Select
        If Not isAscending Then
            Me.Reverse()
        End If
    End Sub 'Sort

    Public NotInheritable Class UserNameComparer
        Implements IComparer

        Public Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim first As ADUser = CType(x, ADUser)
            Dim second As ADUser = CType(y, ADUser)
            Return first.UserName.CompareTo(second.UserName)
        End Function 'Compare
    End Class

    Private NotInheritable Class FirstNameComparer
        Implements IComparer

        Public Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim first As ADUser = CType(x, ADUser)
            Dim second As ADUser = CType(y, ADUser)
            Return first.FirstName.CompareTo(second.FirstName)
        End Function 'Compare
    End Class

    Private NotInheritable Class LastNameComparer
        Implements IComparer

        Public Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim first As ADUser = CType(x, ADUser)
            Dim second As ADUser = CType(y, ADUser)
            Return first.LastName.CompareTo(second.LastName)
        End Function 'Compare
    End Class

    Public NotInheritable Class DisplayNameComparer
        Implements IComparer

        Public Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim first As ADUser = CType(x, ADUser)
            Dim second As ADUser = CType(y, ADUser)
            Return first.DisplayName.CompareTo(second.DisplayName)
        End Function 'Compare
    End Class

    Private NotInheritable Class EmailComparer
        Implements IComparer

        Public Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim first As ADUser = CType(x, ADUser)
            Dim second As ADUser = CType(y, ADUser)
            Return first.Email.CompareTo(second.Email)
        End Function 'Compare
    End Class

    Private NotInheritable Class IsAccountActiveComparer
        Implements IComparer

        Public Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim first As ADUser = CType(x, ADUser)
            Dim second As ADUser = CType(y, ADUser)
            Return first.IsAccountActive.CompareTo(second.IsAccountActive)
        End Function 'Compare
    End Class

End Class
