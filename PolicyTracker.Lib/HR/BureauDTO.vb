Public Class BureauDTO
    Public Property BurCode() As String
        Get
            Return m_BurCode
        End Get
        Set(value As String)
            m_BurCode = value
        End Set
    End Property
    Private m_BurCode As String
    Public Property BurDesc() As String
        Get
            Return m_BurDesc
        End Get
        Set(value As String)
            m_BurDesc = value
        End Set
    End Property
    Private m_BurDesc As String
End Class
