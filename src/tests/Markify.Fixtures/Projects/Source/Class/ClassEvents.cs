using System;

public abstract class AbstractFooType
{
    public abstract event EventHandler AbstractEvent;
}

public class FooType : AbstractFooType
{
    event EventHandler PrivateEvent;

    internal event EventHandler InternalEvent;

    protected internal event EventHandler ProtectedInternalEvent;

    public static event EventHandler StaticEvent;

    public virtual event EventHandler VirtualEvent;

    public sealed override event EventHandler AbstractEvent;

    public event EventHandler<EventArgs> GenericEvent;

    public event EventHandler ExplicitEvent
    {
        add { }
        remove { }
    }
}