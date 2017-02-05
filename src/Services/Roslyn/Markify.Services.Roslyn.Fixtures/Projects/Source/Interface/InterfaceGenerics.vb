Imports System.Collections.Generic

Public Interface SingleGenericType(Of T)
End Interface

Public Interface MultipleGenericType(Of T As {Class, IList(Of String)}, Y As Structure)
End Interface

Public Interface CovariantGenericType(Of In T)
End Interface

Public Interface ContravariantGenericType(Of Out T)
End Interface