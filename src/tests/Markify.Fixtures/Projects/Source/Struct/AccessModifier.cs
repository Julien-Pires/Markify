public struct PublicStruct { }

internal struct InternalStruct { }

public struct PrivateFoo
{
    private struct PrivateStruct { }
}

public class ProtectedFoo
{
    protected struct ProtectedStruct { }
}

public class ProtectedInternalFoo
{
    protected internal struct ProtectedInternalStruct { }
}