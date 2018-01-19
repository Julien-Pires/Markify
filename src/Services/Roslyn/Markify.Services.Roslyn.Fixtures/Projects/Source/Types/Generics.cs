using System.Collections.Generic;

#region Class

namespace Class
{
    public class NoGenericType
    {
    }

    public class SingleGenericType<T>
    {
    }

    public class MultipleGenericType<T, Y>
        where T : class, IList<string>
        where Y : struct
    {
    }
}

#endregion

#region Struct

namespace Struct
{
    public struct NoGenericType
    {
    }

    public struct SingleGenericType<T>
    {
    }

    public struct MultipleGenericType<T, Y>
        where T : class, IList<string>
        where Y : struct
    {
    }
}

#endregion

#region Interface

namespace Interface
{
    public interface NoGenericType
    {
    }

    public interface SingleGenericType<T>
    {
    }

    public interface MultipleGenericType<T, Y>
        where T : class, IList<string>
        where Y : struct
    {
    }

    public interface CovariantGenericType<in T>
    {
    }

    public interface ContravariantGenericType<out T>
    {
    }
}

#endregion

#region Delegate

namespace Delegate
{
    public delegate void NoGenericType();

    public delegate void SingleGenericType<T>();

    public delegate void MultipleGenericType<T, Y>()
        where T : class, IList<string>
        where Y : struct;

    public delegate void CovariantGenericType<in T>();

    public delegate void ContravariantGenericType<out T>();
}

#endregion