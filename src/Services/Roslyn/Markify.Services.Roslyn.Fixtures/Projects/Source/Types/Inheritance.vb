Imports System
Imports System.Collections.Generic

#Region "Class"

Namespace Class
    Public Class InheritType
        Inherits Exception
    End Class

    Public Class ImplementInterfaceType
        Implements IDisposable
    End Class

    Public Class MixedInheritanceType
        Inherits Exception
        Implements IDisposable
    End Class
End Namespace

#End Region

#Region "Struct"

Namespace Struct
    Public Structure ImplementInterfaceType
        Implements IDisposable
    End Structure
End Namespace

#End Region

#Region "Interface"

Namespace Interface
    Public Interface ImplementInterfaceType
        Inherits IDisposable
    End Interface
End Namespace

#End Region

#Region "Enum"

Namespace Enum
    Public Enum InheritPrimitiveType As Int32
    End Enum
End Namespace

#End Region