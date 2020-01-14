Public Class DepartmentDTO
    Public Property DeptCode() As String
        Get
            Return m_DeptCode
        End Get
        Set(value As String)
            m_DeptCode = Value
        End Set
    End Property
    Private m_DeptCode As String
    Public Property DeptDesc() As String
        Get
            Return m_DeptDesc
        End Get
        Set(value As String)
            m_DeptDesc = Value
        End Set
    End Property
    Private m_DeptDesc As String
    Public Property FamisOrgCode() As String
        Get
            Return m_FamisOrgCode
        End Get
        Set(value As String)
            m_FamisOrgCode = Value
        End Set
    End Property
    Private m_FamisOrgCode As String
End Class
