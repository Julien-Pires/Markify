public interface FooType { }

public partial interface ParentType
{
    public partial interface NestedType { }
}

public partial interface ParentType
{
    public partial interface AnotherNestedType
    {
        public interface DeeperNestedType { }
    }
}

namespace FooNamespace
{
    public interface InNamespaceType { }
}

namespace FooNamespace.BarNamespace
{
    public partial interface ParentType
    {
        public interface NestedType { }
    }
}