Imports System
Imports System.Collections.Generic

Public Structure ImplementIDisposable
    Inherits IDisposable
End Structure

Public Structure ImplementGenericInterface
    Inherits IList(Of String), IReadOnlyCollection(Of String)
End Structure