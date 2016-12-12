using System.Collections.Generic;

public delegate void SingleGenericType<T>();

public delegate void MultipleGenericType<T, Y>()
    where T : class, IList<string>
    where Y : struct;

public delegate void CovariantGenericType<in T>();

public delegate void ContravariantGenericType<out T>();