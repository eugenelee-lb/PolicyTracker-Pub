Public Class DivisionDTO
    Public Property DivCode() As String
        Get
            Return m_DivCode
        End Get
        Set(value As String)
            m_DivCode = value
        End Set
    End Property
    Private m_DivCode As String
    Public Property DivDesc() As String
        Get
            Return m_DivDesc
        End Get
        Set(value As String)
            m_DivDesc = value
        End Set
    End Property
    Private m_DivDesc As String
    Public Property CMDept() As String
        Get
            Return m_CMDept
        End Get
        Set(value As String)
            m_CMDept = value
        End Set
    End Property
    Private m_CMDept As String
    Public Property PersUnitCode() As String
        Get
            Return m_PersUnitCode
        End Get
        Set(value As String)
            m_PersUnitCode = value
        End Set
    End Property
    Private m_PersUnitCode As String
    Public Property FamisOrg() As String
        Get
            Return m_FamisOrg
        End Get
        Set(value As String)
            m_FamisOrg = value
        End Set
    End Property
    Private m_FamisOrg As String
    Public Property FamisDefaultIndex() As String
        Get
            Return m_FamisDefaultIndex
        End Get
        Set(value As String)
            m_FamisDefaultIndex = value
        End Set
    End Property
    Private m_FamisDefaultIndex As String
End Class
