#region Class

namespace Class
{
    public class TypeWithNoMethods
    {
    }

    public partial class TypeWithMethods
    {
        void WithNoModifierMethod()
        {
        }

        public void WithoutParameters()
        {
        }

        public void WithOneParameter(Int32 foo)
        {
        }

        public void WithMultipleParameters(Int32 foo, Single bar)
        {
        }

        public void WithDefaultParameters(Int32 foo = 1)
        {
        }

        public void WithParametersModifiers(ref Int32 foo, out Int32 bar)
        {
        }

        public void WithGenericParameters<T>(T foo, T[] bar)
        {
        }

        internal void InternalMethod()
        {
        }

        private void PrivateMethod()
        {
        }

        protected void ProtectedMethod()
        {
        }

        protected internal void ProtectedInternalMethod()
        {
        }

        public Int32 WithReturnType()
        {
        }

        public T WithGenericReturnType<T>()
        {
        }

        public void SingleGenericType<T>()
        {
        }

        public void MultipleGenericType<T, Y>()
            where T : Int32, IList<string>
            where Y : Int32
        {
        }

        public void CovariantGenericType<in T>()
        {
        }

        public void ContravariantGenericType<out T>()
        {
        }

        public static void StaticMethod()
        {
        }

        public virtual void VirtualMethod()
        {
        }

        public sealed override SealedMethod()
        {
        }

        public partial void PartialMethod();
    }
}

#endregion

#region Struct

namespace Struct
{
    public struct TypeWithNoMethods
    {
    }

    public struct TypeWithMethods
    {
        void WithNoModifierMethod()
        {
        }

        public void WithoutParameters()
        {
        }

        public void WithOneParameter(Int32 foo)
        {
        }

        public void WithMultipleParameters(Int32 foo, Single bar)
        {
        }

        public void WithDefaultParameters(Int32 foo = 1)
        {
        }

        public void WithParametersModifiers(ref Int32 foo, out Int32 bar)
        {
        }

        public void WithGenericParameters<T>(T foo, T[] bar)
        {
        }

        internal void InternalMethod()
        {
        }

        private void PrivateMethod()
        {
        }

        public Int32 WithReturnType()
        {
        }

        public T WithGenericReturnType<T>()
        {
        }

        public void SingleGenericType<T>()
        {
        }

        public static void StaticMethod()
        {
        }

        public partial void PartialMethod();

        public void MultipleGenericType<T, Y>()
            where T : class, IList<string>
            where Y : struct
        {
        }

        public void CovariantGenericType<in T>()
        {
        }

        public void ContravariantGenericType<out T>()
        {
        }
    }
}

#endregion

#region Interface

namespace Interface
{
    public interface TypeWithNoMethods
    {
    }

    public interface TypeWithMethods
    {
        void WithNoModifierMethod();

        void WithoutParameters();

        void WithOneParameter(Int32 foo);

        void WithMultipleParameters(Int32 foo, Single bar);

        void WithDefaultParameters(Int32 foo = 1);

        void WithParametersModifiers(ref Int32 foo, out Int32 bar);

        void WithGenericParameters<T>(T foo, T[] bar);

        Int32 WithReturnType();

        T WithGenericReturnType<T>();

        void SingleGenericType<T>();

        void MultipleGenericType<T, Y>()
            where T : class, IList<string>
            where Y : struct;

        void CovariantGenericType<in T>();

        void ContravariantGenericType<out T>();
    }
}

#endregion