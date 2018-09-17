Imports System.Collections

#Region "Class"

Namespace Class
    Public Class NoGenericType
    End Class

    Public Class SingleGenericType(Of T)
    End Class

    Public Class MultipleGenericType(Of T As Structure, Y As { IEnumerable, Class, New })
    End Class
End Namespace

#End Region

#Region "Struct"

Namespace Struct
    Public Structure NoGenericType
    End Structure

    Public Structure SingleGenericType(Of T)
    End Structure

    Public Structure MultipleGenericType(Of T As Structure, Y As { IEnumerable, Class, New })
    End Structure
End Namespace

#End Region

#Region "Interface"

Namespace Interface
    Public Interface NoGenericType
    End Interface

    Public Interface SingleGenericType(Of T)
    End Interface

    Public Interface MultipleGenericType(Of T As Structure, Y As { IEnumerable, Class, New })
    End Interface

    Public Interface CovariantGenericType(Of In T)
    End Interface

    Public Interface ContravariantGenericType(Of Out T)
    End Interface
End Namespace

#End Region

#Region "Delegate"

Namespace Delegate
    Public Delegate Sub NoGenericType()

    Public Delegate Sub SingleGenericType(Of T)()

    Public Delegate Sub MultipleGenericType(Of T As Structure, Y As { IEnumerable, Class, New })()

    Public Delegate Sub CovariantGenericType(Of In T)()

    Public Delegate Sub ContravariantGenericType(Of Out T)()
End Namespace

#End Region