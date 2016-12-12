public interface FooType
{
    int AutoProperty { get; set; }

    int ReadOnlyProperty { get; }

    int WriteOnlyProperty { set; }

    int WithNoModifierProperty { get; set; }
}