Imports System
Imports System.Collections.Generic

Public Class InheritClass
    Inherits Exception
End Class

Public Class ImplementInterfaceClass
    Inherits IDisposable
End Class

Public Class ImplementGenInterfaceClass
    Inherits IList(Of String), IReadOnlyCollection(Of String)
End Class

Public Class MixedInheritanceClass
    Inherits Exception, IList(Of String), IDisposable
End Class