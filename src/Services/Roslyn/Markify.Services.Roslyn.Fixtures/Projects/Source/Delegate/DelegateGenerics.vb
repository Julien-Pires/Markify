Imports System.Collections.Generic

Public Delegate Sub SingleGenericType(Of T)()

Public Delegate Sub MultipleGenericType(Of T As {Class, IList(Of String)}, Y As Structure)()

Public Delegate Sub CovariantGenericType(Of In T)()

Public Delegate Sub ContravariantGenericType(Of Out T)()