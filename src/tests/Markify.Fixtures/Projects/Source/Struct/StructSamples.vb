Public Structure FooType
End Structure

Public Partial Structure ParentType
    Public Partial Structure NestedType
    End Structure
End Structure

Public Partial Structure ParentType
    Public Partial Structure AnotherNestedType
        Public Structure DeeperNestedType
        End Structure
    End Structure
End Structure

Namespace FooNamespace
    Public Structure InNamespaceType
    End Structure
End Namespace

Namespace FooNamespace.BarNamespace
    Public Partial Structure ParentType
        Public Structure NestedType
        End Structure
    End Structure
End Namespace