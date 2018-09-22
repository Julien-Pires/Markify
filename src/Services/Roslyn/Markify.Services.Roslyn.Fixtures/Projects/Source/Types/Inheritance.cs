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
}

#endregion

#region Interface

namespace Interface
{
    public interface ImplementInterfaceType : IDisposable
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