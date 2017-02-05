public struct FooType
{
    public int AutoProperty { get; set; }

    public int ReadOnlyProperty { get; }

    public int WithExpressionBody => 1;

    public int WriteOnlyProperty
    {
        set { }
    }

    public int WithGetterModifierProperty
    {
        internal get { return 1; }
        set { }
    }

    public int WithSetterModifierProperty
    {
        get { return 1; }
        internal set { }
    }

    public static int WithInitialValueProperty { get; } = 1;

    protected int WithModifierProperty { get; set; }

    int WithNoModifierProperty { get; set; }

    public static int StaticProperty { get; set; }
}