Imports System.ComponentModel.DataAnnotations

<MetadataType(GetType(MessageMD))> _
Partial Public Class Message
End Class

Public Class MessageMD
    Private _MsgNo As Integer
    <Key(), Range(-999999999, 999999999), Display(Name:="Message Number")> _
    Public Property MsgNo() As Integer
        Get
            Return _MsgNo
        End Get
        Set(ByVal value As Integer)
            _MsgNo = value
        End Set
    End Property

    Private _MsgText As String
    <Required(), StringLength(1000), Display(Name:="Message Text")> _
    Public Property MsgText() As String
        Get
            Return _MsgText
        End Get
        Set(ByVal value As String)
            _MsgText = value
        End Set
    End Property

    Private _MsgTitle As String
    <Required(), StringLength(50), Display(Name:="Message Title")> _
    Public Property MsgTitle() As String
        Get
            Return _MsgTitle
        End Get
        Set(ByVal value As String)
            _MsgTitle = value
        End Set
    End Property

    Private _CreateDT As DateTime
    Public Property CreateDT() As DateTime
        Get
            Return _CreateDT
        End Get
        Set(ByVal value As DateTime)
            _CreateDT = value
        End Set
    End Property

    Private _CreateUser As String
    Public Property CreateUser() As String
        Get
            Return _CreateUser
        End Get
        Set(ByVal value As String)
            _CreateUser = value
        End Set
    End Property

    <Required(), Display(Name:="Last Update Date/Time"), DisplayFormat(DataFormatString:="{0:M/d/yyyy h:mm:ss.fff tt}")> _
    Private _LastUpdateDT As DateTime
    Public Property LastUpdateDT() As DateTime
        Get
            Return _LastUpdateDT
        End Get
        Set(ByVal value As DateTime)
            _LastUpdateDT = value
        End Set
    End Property

    Private _LastUpdateUser As String
    Public Property LastUpdateUser() As String
        Get
            Return _LastUpdateUser
        End Get
        Set(ByVal value As String)
            _LastUpdateUser = value
        End Set
    End Property

    'Private _NoSuchProperty As Object
    '<Required(), StringLength(30)> _
    'Public Property NoSuchProperty() As Object
    '    Get
    '        Return _NoSuchProperty
    '    End Get
    '    Set(ByVal value As Object)
    '        _NoSuchProperty = value
    '    End Set
    'End Property
End Class
