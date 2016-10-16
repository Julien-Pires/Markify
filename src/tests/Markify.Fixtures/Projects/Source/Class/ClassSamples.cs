public class FooType { }

public partial class ParentType
{
    public partial class NestedType { }
}

public partial class ParentType
{
    public partial class AnotherNestedType
    {
        public class DeeperNestedType { }
    }
}

namespace FooNamespace
{
    public class InNamespaceType { }
}

namespace FooNamespace.BarNamespace
{
    public partial class ParentType
    {
        public class NestedType { }
    }
}