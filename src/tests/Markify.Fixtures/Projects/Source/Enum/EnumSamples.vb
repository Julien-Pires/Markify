Public Enum FooType
End Enum

Public Partial Class ParentType
    Public Enum NestedType
    End Enum
End Class

Public Partial Class ParentType
    Public Partial Class AnotherNestedType
        Public Enum DeeperNestedType
        End Enum
    End Class
End Class

Namespace FooNamespace
    Public Enum InNamespaceType
    End Enum
End Namespace

Namespace FooNamespace.BarNamespace
    Public Partial Class ParentType
        public Enum NestedType
        End Enum
    End Class
End Namespace