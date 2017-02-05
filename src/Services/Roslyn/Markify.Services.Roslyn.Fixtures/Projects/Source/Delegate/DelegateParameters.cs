public delegate void WithoutParameters();

public delegate void WithOneParameter(int foo);

public delegate void WithMultipleParameters(int foo, float bar);

public delegate void WithDefaultParameters(int foo = 1);

public delegate void WithParametersModifiers(ref int foo, out int bar);

public delegate void WithGenericParameters<T>(T foo, T[] bar);