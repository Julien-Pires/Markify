Imports System
Imports System.Collections

Public Partial Structure FooType
    Implements IDisposable
    Private _fieldOne As Integer

    Private Property PropertyOne As Integer

    Public Event Done As EventHandler

    Partial Sub PartialMethod()
    End Sub

    Public Sub MethodOne(foo As Integer)
    End Sub
End Structure

Public Partial Structure FooType
    Implements IEnumerable
    Private _fieldTwo As Decimal

    Private Property PropertyTwo As Integer

    Public Event Started As EventHandler

    Partial Sub PartialMethod()
    End Sub

    Partial Sub PartialMethod(foo As Integer)
    End Sub

    Public Sub MethodTwo(foo As Integer, bar As Integer)
    End Sub
End Structure