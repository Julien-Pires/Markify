Imports System
Imports System.Collections.Generic

Public Structure ImplementInterfaceType
    Implements IDisposable
End Structure

Public Structure ImplementGenericInterfaceType
    Implements IList(Of String)
End Structure