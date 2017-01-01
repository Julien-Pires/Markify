using Markify.Services.VisualStudio.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Services.VisualStudio.Tests.VisualStudioEnvironment
{
    public sealed partial class VisualStudioEnvironmentTests
    {
        [Theory]
        [VisualStudioEnvironmentData("Foo", values: new object[] { "Foo" })]
        [VisualStudioEnvironmentData("Foo.Bar.FooBar", values: new object[] { "Foo.Bar.FooBar" })]
        public void Name_ShouldReturnCorrectSolutionName(string expected, VisualStudio.VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentSolution.Value.Name).IsEqualTo(expected);
        }
    }
}