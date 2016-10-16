Imports System
Imports System.Collections.Generic

Public Interface ImplementInterfaceType
    Inherits IDisposable
End Interface

Public Interface ImplementGenericInterfaceType
    Inherits IList(Of String)
End Interface