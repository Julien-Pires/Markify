using System;

public struct FooType
{
    event EventHandler PrivateEvent;

    internal event EventHandler InternalEvent;

    public static event EventHandler StaticEvent;

    public event EventHandler<EventArgs> GenericEvent;

    public event EventHandler ExplicitEvent
    {
        add { }
        remove { }
    }
}