public enum PublicType { }

internal enum InternalType { }

public partial class ParentType
{
    private enum PrivateType { }

    protected enum ProtectedType { }

    protected internal enum ProtectedInternalType { }

    internal protected enum InternalProtectedType { }
}