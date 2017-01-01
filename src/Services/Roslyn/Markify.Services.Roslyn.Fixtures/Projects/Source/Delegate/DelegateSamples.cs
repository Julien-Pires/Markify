public delegate void FooType();

public partial class ParentType
{
    public delegate void NestedType();
}

public partial class ParentType
{
    public partial class AnotherNestedType
    {
        public delegate void DeeperNestedType();
    }
}

namespace FooNamespace
{
    public delegate void InNamespaceType();
}

namespace FooNamespace.BarNamespace
{
    public partial class ParentType
    {
        public delegate void NestedType();
    }
}