public delegate void WithoutParameters();

public delegate void WithOneParameter(Int32 foo);

public delegate void WithMultipleParameters(Int32 foo, Single bar);

public delegate void WithDefaultParameters(Int32 foo = 1);

public delegate void WithParametersModifiers(ref Int32 foo, out Int32 bar);

public delegate void WithGenericParameters<T>(T foo, T[] bar);

public delegate Int32 WithReturnType();

public delegate T WithGenericReturnType<T>();

public delegate void SingleGenericType<T>();

public delegate void MultipleGenericType<T, Y>()
    where T : class, IList<string>
    where Y : struct;

public delegate void CovariantGenericType<in T>();

public delegate void ContravariantGenericType<out T>();