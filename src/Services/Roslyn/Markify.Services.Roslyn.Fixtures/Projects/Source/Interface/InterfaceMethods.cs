public interface FooType
{
    void Method();

    int IntMethod();

    void WithParametersMethod(int foo, ref int bar, out int foobar);

    void WithNoNameParameterMethod(__arglist)

    T SingleGenericMethod<T>(T foo = default(T));

    T MultiGenericMethod<T, Y>(T foo, Y bar)
        where T : IList
        where Y : IDisposable, IEnumerable;
}