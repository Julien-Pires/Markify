public delegate void SingleDelegate();

public class ParentClass
{
    public delegate void NestedDelegate();
}

namespace FooSpace
{
    public delegate void InNamespaceDelegate();
}