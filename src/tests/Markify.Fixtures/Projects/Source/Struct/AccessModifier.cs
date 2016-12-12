public struct PublicType { }

internal struct InternalType { }

public partial struct ParentType
{
    private struct PrivateType { }

    protected struct ProtectedType { }

    protected internal struct ProtectedInternalType { }

    internal protected struct InternalProtectedType { }
}