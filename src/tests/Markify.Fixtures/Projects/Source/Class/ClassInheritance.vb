Imports System
Imports System.Collections.Generic

Public Class InheritType
    Inherits Exception
End Class

Public Class ImplementInterfaceType
    Implements IDisposable
End Class

Public Class ImplementGenericInterfaceType
    Implements IList(Of String)
End Class

Public Class MixedInheritanceType
    Inherits Exception
    Implements IDisposable
End Class