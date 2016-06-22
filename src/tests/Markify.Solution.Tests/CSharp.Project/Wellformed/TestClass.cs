namespace CSharp.Project.Wellformed
{
    public class TestClass
    {
        protected class InnerTestClass
        {
        }
    }

    public sealed class TestClass1
    {
    }

    internal abstract class TestClass2
    {
    }

    public sealed partial class TestClass3
    {
    }

    public sealed partial class TestClass3
    {
    }

    public abstract partial class TestClass5
    {
    }

    public abstract partial class TestClass5
    {
    }

    internal static class TestClass7
    {
    }
}