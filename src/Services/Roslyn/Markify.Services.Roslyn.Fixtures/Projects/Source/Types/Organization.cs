#region Class

namespace Class
{
    public class FooType
    {
    }

    public partial class ParentType
    {
        public partial class NestedType
        {
        }
    }

    public partial class ParentType
    {
        public partial class AnotherNestedType
        {
            public class DeeperNestedType
            {
            }
        }
    }

    namespace Nested
    {
        public class InNamespaceType
        {
        }
    }
}

#endregion

#region Struct

namespace Struct
{
    public struct FooType
    {
    }

    public partial struct ParentType
    {
        public partial struct NestedType
        {
        }
    }

    public partial struct ParentType
    {
        public partial struct AnotherNestedType
        {
            public struct DeeperNestedType
            {
            }
        }
    }

    namespace Nested
    {
        public struct InNamespaceType
        {
        }
    }
}

#endregion

#region Interface

namespace Interface
{
    public interface FooType
    {
    }

    public partial interface ParentType
    {
        public partial interface NestedType
        {
        }
    }

    public partial interface ParentType
    {
        public partial interface AnotherNestedType
        {
            public interface DeeperNestedType
            {
            }
        }
    }

    namespace Nested
    {
        public interface InNamespaceType
        {
        }
    }
}

#endregion

#region Enum

namespace Enum
{
    public enum FooType
    {
    }

    public partial class ParentType
    {
        public enum NestedType
        {
        }
    }

    public partial class ParentType
    {
        public partial class AnotherNestedType
        {
            public enum DeeperNestedType
            {
            }
        }
    }

    namespace Nested
    {
        public enum InNamespaceType
        {
        }
    }
}

#endregion

#region Delegate

namespace Delegate
{
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

    namespace Nested
    {
        public delegate void InNamespaceType();
    }
}

#endregion