public interface IPublicInterface { }

internal interface IInternalInterface { }

public interface IPrivateFoo
{
    private interface IPrivateInterface { }
}

public interface IProtectedFoo
{
    protected interface IProtectedInterface { }
}

public interface IProtectedInternalFoo
{
    protected internal interface IProtectedInternalInterface { }
}