public delegate void PublicType();

internal delegate void InternalType();

public partial class ParentType
{
    private delegate void PrivateType();

    protected delegate void ProtectedType();

    protected internal delegate void ProtectedInternalType();

    internal protected delegate void InternalProtectedType();
}