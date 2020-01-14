Public Class ClassificationDTO
    Public Property ClassCode() As String
        Get
            Return m_ClassCode
        End Get
        Set(value As String)
            m_ClassCode = Value
        End Set
    End Property
    Private m_ClassCode As String
    Public Property Title() As String
        Get
            Return m_Title
        End Get
        Set(value As String)
            m_Title = Value
        End Set
    End Property
    Private m_Title As String
    Public Property ClassType() As String
        Get
            Return m_ClassType
        End Get
        Set(value As String)
            m_ClassType = Value
        End Set
    End Property
    Private m_ClassType As String
End Class
