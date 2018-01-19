#region Class

namespace Class
{
    public class PublicType
    {
    }

    internal class InternalType
    {
    }

    public partial class ParentType
    {
        private class PrivateType
        {
        }

        protected class ProtectedType
        {
        }

        protected internal class ProtectedInternalType
        {
        }

        internal protected class InternalProtectedType
        {
        }
    }
}

#endregion

#region Struct

namespace Struct
{
    public struct PublicType
    {
    }

    internal struct InternalType
    {
    }

    public partial struct ParentType
    {
        private struct PrivateType
        {
        }

        protected struct ProtectedType
        {
        }

        protected internal struct ProtectedInternalType
        {
        }

        internal protected struct InternalProtectedType
        {
        }
    }
}

#endregion

#region Interface

namespace Interface
{
    public interface PublicType
    {
    }

    internal interface InternalType
    {
    }

    public partial interface ParentType
    {
        private interface PrivateType
        {
        }

        protected interface ProtectedType
        {
        }

        protected internal interface ProtectedInternalType
        {
        }

        internal protected interface InternalProtectedType
        {
        }
    }
}

#endregion

#region Enum

namespace Enum
{
    public enum PublicType
    {
    }

    internal enum InternalType
    {
    }

    public partial class ParentType
    {
        private enum PrivateType
        {
        }

        protected enum ProtectedType
        {
        }

        protected internal enum ProtectedInternalType
        {
        }

        internal protected enum InternalProtectedType
        {
        }
    }
}

#endregion

#region Delegate

namespace Delegate
{
    public delegate void PublicType();

    internal delegate void InternalType();

    public partial class ParentType
    {
        private delegate void PrivateType();

        protected delegate void ProtectedType();

        protected internal delegate void ProtectedInternalType();

        internal protected delegate void InternalProtectedType();
    }
}

#endregion