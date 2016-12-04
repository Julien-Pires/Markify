public abstract class AbstractFooType
{
    public virtual int SealedProperty { get; set; }
}

public class FooType : AbstractFooType
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

    public int WithInitialValueProperty { get; } = 1;

    protected int WithModifierProperty { get; set; }

    int WithNoModifierProperty { get; set; }

    protected internal int WithMultipleModifiersProperty { get; set; }

    public static int StaticProperty { get; set; }

    public virtual int VirtualProperty { get; set; }
    
    public sealed override int SealedProperty { get; set; }
}