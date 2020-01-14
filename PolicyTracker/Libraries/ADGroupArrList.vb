Imports Microsoft.VisualBasic
Imports System.Collections
Imports ADWrapper

Public Class ADGroupArrList
    Inherits ArrayList

    Public Enum GroupFields
        InitValue
        Name
        DisplayName
        DistinguishedName
        Description
    End Enum 'GroupFields

    Public Overloads Sub Sort(ByVal sortField As GroupFields, ByVal isAscending As Boolean)
        Select Case sortField
            Case GroupFields.Name
                Me.Sort(New NameComparer)
            Case GroupFields.DisplayName
                Me.Sort(New DisplayNameComparer)
            Case GroupFields.DistinguishedName
                Me.Sort(New DistinguishedNameComparer)
            Case GroupFields.Description
                Me.Sort(New DescriptionComparer)
        End Select
        If Not isAscending Then
            Me.Reverse()
        End If
    End Sub 'Sort

    Public NotInheritable Class NameComparer
        Implements IComparer

        Public Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim first As ADGroup = CType(x, ADGroup)
            Dim second As ADGroup = CType(y, ADGroup)
            Return first.Name.CompareTo(second.Name)
        End Function 'Compare
    End Class

    Public NotInheritable Class DisplayNameComparer
        Implements IComparer

        Public Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim first As ADGroup = CType(x, ADGroup)
            Dim second As ADGroup = CType(y, ADGroup)
            Return first.DisplayName.CompareTo(second.DisplayName)
        End Function 'Compare
    End Class

    Private NotInheritable Class DescriptionComparer
        Implements IComparer

        Public Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim first As ADGroup = CType(x, ADGroup)
            Dim second As ADGroup = CType(y, ADGroup)
            Return first.Description.CompareTo(second.Description)
        End Function 'Compare
    End Class

    Private NotInheritable Class DistinguishedNameComparer
        Implements IComparer

        Public Function [Compare](ByVal x As Object, ByVal y As Object) As Integer Implements IComparer.Compare
            Dim first As ADGroup = CType(x, ADGroup)
            Dim second As ADGroup = CType(y, ADGroup)
            Return first.DistinguishedName.CompareTo(second.DistinguishedName)
        End Function 'Compare
    End Class

End Class
