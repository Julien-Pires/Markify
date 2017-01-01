public interface PublicType { }

internal interface InternalType { }

public partial interface ParentType
{
    private interface PrivateType { }

    protected interface ProtectedType { }

    protected internal interface ProtectedInternalType { }

    internal protected interface InternalProtectedType { }
}