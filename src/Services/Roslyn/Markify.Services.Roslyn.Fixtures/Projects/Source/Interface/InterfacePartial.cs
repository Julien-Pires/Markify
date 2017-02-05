using System;
using System.Collections;

public partial interface FooType : IDisposable
{
    int PropertyOne { get; set; }

    event EventHandler Done;

    void MethodOne(int foo);
}

public partial interface FooType : IEnumerable
{
    int PropertyTwo { get; set; }

    event EventHandler Started;

    void MethodTwo(int foo, int bar);
}