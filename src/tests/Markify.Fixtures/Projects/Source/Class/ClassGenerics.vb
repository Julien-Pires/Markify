Imports System.Collections.Generic

Public Class SingleGenericType(Of T)
End Class

Public Class MultipleGenericType(Of T As {Class, IList(Of String)}, Y As Structure)
End Class