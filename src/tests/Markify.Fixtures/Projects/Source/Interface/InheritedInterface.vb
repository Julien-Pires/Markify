Imports System
Imports System.Collections.Generic

Public Interface IImplementIDisposable
    Inherits IDisposable
End Interface

Public Interface IImplementGenericInterface
    Inherits IList(Of String), IReadOnlyCollection(Of String)
End Interface