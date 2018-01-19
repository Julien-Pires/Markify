using System;
using System.Collections.Generic;

#region Class

namespace Class
{
    public class InheritType : Exception
    {
    }

    public class ImplementInterfaceType : IDisposable
    {
    }

    public class ImplementGenericInterfaceType : IList<String>
    {
    }

    public class MixedInheritanceType : Exception, IDisposable
    {
    }
}

#endregion

#region Struct

namespace Struct
{
    public struct ImplementInterfaceType : IDisposable
    {
    }

    public struct ImplementGenericInterfaceType : IList<String>
    {
    }
}

#endregion

#region Interface

namespace Interface
{
    public interface ImplementInterfaceType : IDisposable
    {
    }

    public interface ImplementGenericInterfaceType : IList<String>
    {
    }
}

#endregion

#region Enum

namespace Enum
{
    public enum InheritPrimitiveType : Int32
    {
    }
}

#endregion