#region Class

using System;

namespace Class
{
    public class TypeWithNoEvent
    {
    }

    public abstract class AbstractTypeWithEvents
    {
        public abstract event EventHandler AbstractEvent;
    }

    public class TypeWithEvents : AbstractTypeWithEvents
    {
        event EventHandler Event;

        private event EventHandler PrivateEvent;

        internal event EventHandler InternalEvent;

        protected internal event EventHandler ProtectedInternalEvent;

        protected event EventHandler ProtectedEvent;

        public static event EventHandler StaticEvent;

        public virtual event EventHandler VirtualEvent;

        public sealed override event EventHandler SealedEvent;

        public event EventHandler<EventArgs> GenericEvent;

        public event EventHandler ExplicitEvent
        {
            add { }
            remove { }
        }
    }
}

#endregion

#region Interface

namespace Interface
{
    public interface TypeWithNoEvent
    {
    }

    public interface TypeWithEvents
    {
        event EventHandler Event;

        event EventHandler<EventArgs> GenericEvent;
    }
}

#endregion

#region Struct

namespace Struct
{
    public struct TypeWithNoEvent
    {
    }

    public struct TypeWithEvents
    {
        event EventHandler Event;

        private event EventHandler PrivateEvent;

        internal event EventHandler InternalEvent;

        public static event EventHandler StaticEvent;

        public event EventHandler<EventArgs> GenericEvent;

        public event EventHandler ExplicitEvent
        {
            add { }
            remove { }
        }
    }
}

#endregion