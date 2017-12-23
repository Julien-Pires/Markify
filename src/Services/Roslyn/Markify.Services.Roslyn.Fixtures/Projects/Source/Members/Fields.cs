#region Class

namespace Class
{
    public class TypeWithNoField
    {
    }

    public class TypeWithFields
    {
        Int32 Field;

        Int32 FirstField, SecondField;

        static Int32 StaticField;

        const Int32 ConstField = 1;

        readonly Int32 ReadOnlyField;

        static readonly Int32 StaticReadOnlyField;

        public Int32 PublicField;

        private Int32 PrivateField;

        protected Int32 ProtectedField;

        internal Int32 InternalField;

        protected internal Int32 ProtectedInternalField;
    }
}

#endregion

#region Struct

namespace Struct
{
    public struct TypeWithNoField
    {
    }

    public struct TypeWithFields
    {
        Int32 Field;

        Int32 FirstField, SecondField;

        static Int32 StaticField;

        const Int32 ConstField = 1;

        readonly Int32 ReadOnlyField;

        static readonly Int32 StaticReadOnlyField;

        public Int32 PublicField;

        private Int32 PrivateField;

        internal Int32 InternalField;
    }
}

#endregion