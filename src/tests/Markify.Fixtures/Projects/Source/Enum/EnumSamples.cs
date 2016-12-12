public enum FooType { }

public partial class ParentType
{
    public enum NestedType { }
}

public partial class ParentType
{
    public partial class AnotherNestedType
    {
        public enum DeeperNestedType { }
    }
}

namespace FooNamespace
{
    public enum InNamespaceType { }
}

namespace FooNamespace.BarNamespace
{
    public partial class ParentType
    {
        public enum NestedType { }
    }
}