using System;

public interface FooType
{
    event EventHandler PrivateEvent;

    event EventHandler<EventArgs> GenericEvent;
}