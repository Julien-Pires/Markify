public struct FooType { }

public partial struct ParentType
{
    public partial struct NestedType { }
}

public partial struct ParentType
{
    public partial struct AnotherNestedType
    {
        public struct DeeperNestedType { }
    }
}

namespace FooNamespace
{
    public struct InNamespaceType { }
}

namespace FooNamespace.BarNamespace
{
    public partial struct ParentType
    {
        public struct NestedType { }
    }
}