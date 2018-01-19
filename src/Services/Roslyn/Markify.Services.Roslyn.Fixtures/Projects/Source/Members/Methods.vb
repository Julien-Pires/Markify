#Region "Class"

Namespace Class
    Public Class TypeWithNoMethods
    End Class

    Public Class TypeWithMethods
        Sub WithNoModifierMethod()
        End Sub

        Public Sub WithoutParameters()
        End Sub

        Public Sub WithOneParameter(foo As Int32)
        End Sub

        Public Sub WithMultipleParameters(foo As Int32, bar As Single)
        End Sub

        Public Sub WithDefaultParameters(Optional foo As Int32 = 1)
        End Sub

        Public Sub WithParametersModifiers(ByRef foo As Int32, ByVal bar As Single)
        End Sub

        Public Sub WithGenericParameters(Of T)(foo As T, bar As T())
        End Sub

        Friend Sub InternalMethod()
        End Sub

        Private Sub PrivateMethod()
        End Sub

        Protected Sub ProtectedMethod()
        End Sub

        Protected Friend Sub ProtectedInternalMethod()
        End Sub

        Public Function WithReturnType() As Int32
        End Function

        Public Function WithGenericReturnType(Of T)() As T
        End Function

        Public Sub SingleGenericType(Of T)()
        End Sub

        Public Sub MultipleGenericType(Of T As {Class, IList(Of String)}, Y As Structure)()
        End Sub

        Public Sub CovariantGenericType(Of In T)()
        End Sub

        Public Sub ContravariantGenericType(Of Out T)()
        End Sub

        Public Shared Sub StaticMethod()
        End Sub

        Public Overridable Sub VirtualMethod()
        End Sub

        Public NotOverridable Overrides Sub SealedMethod()
        End Sub

        Partial Public Sub PartialMethod()
        End Sub
    End Class
End Namespace

#End Region

#Region "Struct"

Namespace Struct
    Public Structure TypeWithNoMethods
    End Structure

    Public Structure TypeWithMethods
        Sub WithNoModifierMethod()
        End Sub

        Public Sub WithoutParameters()
        End Sub

        Public Sub WithOneParameter(foo As Int32)
        End Sub

        Public Sub WithMultipleParameters(foo As Int32, bar As Single)
        End Sub

        Public Sub WithDefaultParameters(Optional foo As Int32 = 1)
        End Sub

        Public Sub WithParametersModifiers(ByRef foo As Int32, ByVal bar As Single)
        End Sub

        Public Sub WithGenericParameters(Of T)(foo As T, bar As T())
        End Sub

        Friend Sub InternalMethod()
        End Sub

        Private Sub PrivateMethod()
        End Sub

        Public Function WithReturnType() As Int32
        End Function

        Public Function WithGenericReturnType(Of T)() As T
        End Function

        Public Sub SingleGenericType(Of T)()
        End Sub

        Public Sub MultipleGenericType(Of T As {Class, IList(Of String)}, Y As Structure)()
        End Sub

        Public Sub CovariantGenericType(Of In T)()
        End Sub

        Public Sub ContravariantGenericType(Of Out T)()
        End Sub

        Public Shared Sub StaticMethod()
        End Sub

        Public Overridable Sub VirtualMethod()
        End Sub

        Public NotOverridable Overrides Sub SealedMethod()
        End Sub

        Partial Public Sub PartialMethod()
        End Sub
    End Structure
End Namespace

#End Region

#Region "Interface"

Namespace Interface
    Public Interface TypeWithNoMethods
    End Interface

    Public Interface TypeWithMethods
        Sub WithNoModifierMethod()

        Sub WithoutParameters()

        Sub WithOneParameter(foo As Int32)

        Sub WithMultipleParameters(foo As Int32, bar As Single)

        Sub WithDefaultParameters(Optional foo As Int32 = 1)

        Sub WithParametersModifiers(ByRef foo As Int32, ByVal bar As Single)

        Sub WithGenericParameters(Of T)(foo As T, bar As T())

        Function WithReturnType() As Int32

        Function WithGenericReturnType(Of T)() As T

        Sub SingleGenericType(Of T)()

        Sub MultipleGenericType(Of T As {Class, IList(Of String)}, Y As Structure)()

        Sub CovariantGenericType(Of In T)()

        Sub ContravariantGenericType(Of Out T)()
    End Interface
End Namespace

#End Region