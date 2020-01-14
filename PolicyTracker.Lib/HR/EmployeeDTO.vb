Public Class EmployeeDTO
    Public Property EmpId() As String
        Get
            Return m_EmpId
        End Get
        Set(value As String)
            m_EmpId = Value
        End Set
    End Property
    Private m_EmpId As String
    Public Property Pin() As String
        Get
            Return m_Pin
        End Get
        Set(value As String)
            m_Pin = Value
        End Set
    End Property
    Private m_Pin As String
    Public Property Stat() As String
        Get
            Return m_Stat
        End Get
        Set(value As String)
            m_Stat = Value
        End Set
    End Property
    Private m_Stat As String
    Public Property FName() As String
        Get
            Return m_FName
        End Get
        Set(value As String)
            m_FName = Value
        End Set
    End Property
    Private m_FName As String
    Public Property MName() As String
        Get
            Return m_MName
        End Get
        Set(value As String)
            m_MName = Value
        End Set
    End Property
    Private m_MName As String
    Public Property LName() As String
        Get
            Return m_LName
        End Get
        Set(value As String)
            m_LName = Value
        End Set
    End Property
    Private m_LName As String
    Public Property Suffix() As String
        Get
            Return m_Suffix
        End Get
        Set(value As String)
            m_Suffix = Value
        End Set
    End Property
    Private m_Suffix As String
    Public Property OrgCode() As String
        Get
            Return m_OrgCode
        End Get
        Set(value As String)
            m_OrgCode = Value
        End Set
    End Property
    Private m_OrgCode As String
    Public Property ClassCode() As String
        Get
            Return m_ClassCode
        End Get
        Set(value As String)
            m_ClassCode = Value
        End Set
    End Property
    Private m_ClassCode As String
    Public Property HireDate() As System.Nullable(Of DateTime)
        Get
            Return m_HireDate
        End Get
        Set(value As System.Nullable(Of DateTime))
            m_HireDate = Value
        End Set
    End Property
    Private m_HireDate As System.Nullable(Of DateTime)
End Class
