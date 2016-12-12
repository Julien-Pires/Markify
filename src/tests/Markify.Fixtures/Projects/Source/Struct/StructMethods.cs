public partial struct FooType
{
    void Method() { }

    public void PublicMethod() { }

    int IntMethod() { return 1; }

    partial void PartialMethod();

    void WithParametersMethod(int foo, ref int bar, out int foobar) { foobar = 2; }

    void WithNoNameParameterMethod(__arglist) { }

    T SingleGenericMethod<T>(T foo = default(T)) { return default(T); }

    T MultiGenericMethod<T, Y>(T foo, Y bar)
        where T : IList
        where Y : IDisposable, IEnumerable
    {
        return default(T);
    }

    int BodyMethod(int foo) => 1;
}