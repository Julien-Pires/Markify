Public Delegate Sub FooType()

Public Partial Class ParentType
    Public Delegate Sub NestedType()
End Class

Public Partial Class ParentType
    Public Partial Class AnotherNestedType
        Public Delegate Sub DeeperNestedType()
    End Class
End Class

Namespace FooNamespace
    Public Delegate Sub InNamespaceType()
End Namespace

Namespace FooNamespace.BarNamespace
    Public Partial Class ParentType
        Public Delegate Sub NestedType()
    End Class
End Namespace