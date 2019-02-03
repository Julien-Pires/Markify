#region Class

namespace Class
{
    public class TypeWithNoProperties
    {
    }

    public class TypeWithPrivateDefaultModifier
    {
        Int32 WithNoModifierProperty { get; set; }
    }

    public class TypeWithProperties
    {
        public Int32 AutoProperty { get; set; }

        public Int32 ReadOnlyProperty { get; }

        public Int32 WriteOnlyProperty
        {
            set { }
        }

        public Int32 WithExpressionBody => 1;

        public Int32 WithGetterModifierProperty
        {
            internal get { return 1; }
            set { }
        }

        public Int32 WithSetterModifierProperty
        {
            get { return 1; }
            internal set { }
        }

        public Int32 WithInitialValueProperty { get; } = 1;

        private Int32 PrivateProperty { get; set; }

        internal Int32 InternalProperty { get; set; }

        public static Int32 StaticProperty { get; set; }

        protected Int32 ProtectedProperty { get; set; }

        protected internal Int32 ProtectedInternalProperty { get; set; }

        public virtual Int32 VirtualProperty { get; set; }

        public sealed override Int32 SealedProperty { get; set; }
    }
}

#endregion

#region Interface

namespace Interface
{
    public interface TypeWithNoProperties
    {
    }

    public interface TypeWithPublicDefaultModifier
    {
        Int32 WithNoModifierProperty { get; set; }
    }

    public interface TypeWithProperties
    {
        Int32 AutoProperty { get; set; }

        Int32 ReadOnlyProperty { get; }

        Int32 WriteOnlyProperty { set; }
    }
}
#endregion

#region Struct

namespace Struct
{
    public struct TypeWithNoProperties
    {
    }

    public struct TypeWithPrivateDefaultModifier
    {
        Int32 WithNoModifierProperty { get; set; }
    }

    public struct TypeWithProperties
    {
        public Int32 AutoProperty { get; set; }

        public Int32 ReadOnlyProperty { get; }

        public Int32 WriteOnlyProperty
        {
            set { }
        }

        public Int32 WithExpressionBody => 1;

        public Int32 WithGetterModifierProperty
        {
            internal get { return 1; }
            set { }
        }

        public Int32 WithSetterModifierProperty
        {
            get { return 1; }
            internal set { }
        }

        public static Int32 WithInitialValueProperty { get; } = 1;

        private Int32 PrivateProperty { get; set; }

        internal Int32 InternalProperty { get; set; }

        public static Int32 StaticProperty { get; set; }
    }
}

#endregion