using System;
using System.Collections;

#region Class

namespace Class
{
    public sealed partial class FooType : IDisposable
    {
        private int _fieldOne;

        public event EventHandler Done;

        private int PropertyOne { get; set; }

        partial void PartialMethod();

        public void MethodOne(int foo)
        {
        }
    }

    public partial class FooType : IEnumerable
    {
        private float _fieldTwo;

        private int PropertyTwo { get; set; }

        public event EventHandler Started;

        partial void PartialMethod()
        {
        }

        partial void PartialMethod(int foo);

        public void MethodTwo(int foo, int bar)
        {
        }
    }
}

#endregion

#region Interface

namespace Interface
{
    public partial interface FooType : IDisposable
    {
        int PropertyOne { get; set; }

        event EventHandler Done;

        void MethodFour(string foo);

        void MethodOne(int foo);
    }

    public partial interface FooType : IEnumerable
    {
        int PropertyTwo { get; set; }

        event EventHandler Started;

        void MethodTwo(int foo, int bar);

        void MethodThree(double foo);
    }
}

#endregion

#region Struct

namespace Struct
{
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
}

#endregion