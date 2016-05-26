public delegate void PublicDelegate();

internal delegate void InternalDelegate();

public class PrivateFoo
{
    private delegate void PrivateDelegate();
}

public class ProtectedFoo
{
    protected delegate void ProtectedDelegate();
}

public class ProtectedInternalFoo
{
    protected internal delegate void ProtectedInternalDelegate();
}