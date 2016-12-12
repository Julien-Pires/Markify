using System;
using System.Collections;

public partial struct FooType : IDisposable
{
    private int _fieldOne;

    private int PropertyOne { get; set; }

    public event EventHandler Done;

    partial void PartialMethod();

    public void MethodOne(int foo);
}

public partial struct FooType : IEnumerable
{
    private float _fieldTwo;

    private int PropertyTwo { get; set; }

    public event EventHandler Started;

    partial void PartialMethod() { }

    partial void PartialMethod(int foo);

    public void MethodTwo(int foo, int bar) { }
}