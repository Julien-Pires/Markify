using System;
using System.Collections;

public sealed partial class FooType : IDisposable
{
    private int _fieldOne;

    public event EventHandler Done;

    private int PropertyOne { get; set; }

    partial void PartialMethod();

    public void MethodOne(int foo) { }
}

public partial class FooType : IEnumerable
{
    private float _fieldTwo;

    private int PropertyTwo { get; set; }

    public event EventHandler Started;

    partial void PartialMethod() { }

    partial void PartialMethod(int foo);

    public void MethodTwo(int foo, int bar) { }
}