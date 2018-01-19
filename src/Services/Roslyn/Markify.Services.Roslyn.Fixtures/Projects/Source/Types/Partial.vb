Imports System
Imports System.Collections

#Region "Class"

Namespace Class
    Partial Public NotInheritable Class FooType
        Implements IDisposable
        Private _fieldOne As Integer

        Private Property PropertyOne As Integer

        Public Event Done As EventHandler

        Partial Sub PartialMethod()
        End Sub

        Public Sub MethodOne(foo As Integer)
        End Sub
    End Class

    Partial Public Class FooType
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
    End Class
End Namespace

#End Region

#Region "Interface"

Namespace Interface
    Partial Public Interface FooType
        Inherits IDisposable
        Property PropertyOne As Integer

        Event Done As EventHandler

        Sub MethodOne(foo As Integer)

        Sub MethodFour(foo As String)
    End Interface

    Partial Public Interface FooType
        Inherits IEnumerable
        Property PropertyTwo As Integer

        Event Started As EventHandler

        Sub MethodTwo(foo As Integer, bar As Integer)

        Sub MethodThree(foo As Double)
    End Interface
End Namespace

#End Region

#Region "Struct"

Namespace Struct
    Partial Public Structure FooType
        Implements IDisposable
        Private _fieldOne As Integer

        Private Property PropertyOne As Integer

        Public Event Done As EventHandler

        Partial Sub PartialMethod()
        End Sub

        Public Sub MethodOne(foo As Integer)
        End Sub
    End Structure

    Partial Public Structure FooType
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
End Namespace

#End Region