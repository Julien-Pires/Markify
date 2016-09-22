using Markify.Core.IDE.VisualStudio;
using Markify.Core.Tests.Attributes;
using Markify.Models.IDE;
using NFluent;
using Xunit;

namespace Markify.Core.Tests.IDE
{
    public sealed partial class VisualStudioEnvironmentTests
    {
        [Theory]
        [VisualStudioEnvironmentData(solution: "Foo", values: new object[] { "Foo" })]
        [VisualStudioEnvironmentData(solution: "Foo.Bar.FooBar", values: new object[] { "Foo.Bar.FooBar" })]
        public void Name_ShouldReturnCorrectSolutionName(string expected, VisualStudioEnvironment sut)
        {
            var solution = sut.CurrentSolution.ValueOr(default(Solution));

            Check.That(solution.Name).IsEqualTo(expected);
        }
    }
}