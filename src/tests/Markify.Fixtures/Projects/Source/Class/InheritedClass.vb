Imports System
Imports System.Collections.Generic

Namespace FooSpace.InnerSpace
    Public Class InheritClass
        Inherits Exception
    End Class
End Namespace

Public Class ImplementInterfaceClass
    Inherits IDisposable
End Class

Public Class ImplementGenInterfaceClass
    Inherits IList(Of String), IReadOnlyCollection(Of String)
End Class

Public Class MixedInheritanceClass
    Inherits Exception, IList(Of String), IDisposable
End Class