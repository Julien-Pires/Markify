Public Interface FooType
End Interface

Public Partial Interface ParentType
    Public Partial Interface NestedType
    End Interface
End Interface

Public Partial Interface ParentType
    Public Partial Interface AnotherNestedType
        Public Interface DeeperNestedType
        End Interface
    End Interface
End Interface

Namespace FooNamespace
    Public Interface InNamespaceType
    End Interface
End Namespace

Namespace FooNamespace.BarNamespace
    Public Partial Interface ParentType
        Public Interface NestedType
        End Interface
    End Interface
End Namespace