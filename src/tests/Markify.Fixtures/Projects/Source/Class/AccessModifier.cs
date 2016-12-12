public class PublicType { }

internal class InternalType { }

public partial class ParentType
{
    private class PrivateType { }

    protected class ProtectedType { }

    protected internal class ProtectedInternalType { }

    internal protected class InternalProtectedType { }
}