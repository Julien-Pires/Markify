#Region "Class"

Namespace Class
    Public Class FooType
    End Class

    Partial Public Class ParentType
        Partial Public Class NestedType
        End Class
    End Class

    Partial Public Class ParentType
        Partial Public Class AnotherNestedType
            Public Class DeeperNestedType
            End Class
        End Class
    End Class

    Namespace Nested
        Public Class InNamespaceType
        End Class
    End Namespace
End Namespace

#End Region

#Region "Struct"

Namespace Struct
    Public Structure FooType
    End Structure

    Partial Public Structure ParentType
        Partial Public Structure NestedType
        End Structure
    End Structure

    Partial Public Structure ParentType
        Partial Public Structure AnotherNestedType
            Public Structure DeeperNestedType
            End Structure
        End Structure
    End Structure

    Namespace Nested
        Public Structure InNamespaceType
        End Structure
    End Namespace
End Namespace

#End Region

#Region "Interface"

Namespace Interface
    Public Interface FooType
    End Interface

    Partial Public Interface ParentType
        Partial Public Interface NestedType
        End Interface
    End Interface

    Partial Public Interface ParentType
        Partial Public Interface AnotherNestedType
            Public Interface DeeperNestedType
            End Interface
        End Interface
    End Interface

    Namespace Nested
        Public Interface InNamespaceType
        End Interface
    End Namespace
End Namespace

#End Region

#Region "Enum"

Namespace Enum
    Public Enum FooType
    End Enum

    Partial Public Class ParentType
        Public Enum NestedType
        End Enum
    End Class

    Partial Public Class ParentType
        Partial Public Class AnotherNestedType
            Public Enum DeeperNestedType
            End Enum
        End Class
    End Class

    Namespace Nested
        Public Enum InNamespaceType
        End Enum
    End Namespace
End Namespace

#End Region

#Region "Delegate"

Namespace Delegate
    Public Delegate Sub FooType()

    Partial Public Class ParentType
        Public Delegate Sub NestedType()
    End Class

    Partial Public Class ParentType
        Partial Public Class AnotherNestedType
            Public Delegate Sub DeeperNestedType()
        End Class
    End Class

    Namespace Nested
        Public Delegate Sub InNamespaceType()
    End Namespace
End Namespace

#End Region