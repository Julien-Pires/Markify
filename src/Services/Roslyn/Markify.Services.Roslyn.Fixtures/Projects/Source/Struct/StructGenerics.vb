Imports System.Collections.Generic

Public Structure SingleGenericType(Of T)
End Structure

Public Structure MultipleGenericType(Of T As {Class, IList(Of String)}, Y As Structure)
End Structure