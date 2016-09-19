using System;
using Markify.Models.IDE;
using Markify.Core.IDE.VisualStudio;
using Markify.Core.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Core.Tests.IDE
{
    public sealed partial class VisualStudioEnvironmentTests
    {
        [Theory]
        [VisualStudioEnvironmentData(solution: "Foo", values: new object[] { "c:/Foo/" })]
        [VisualStudioEnvironmentData(solution: "Foo", folder: "c:/Bar", values: new object[] { "c:/Bar/Foo/" })]
        [VisualStudioEnvironmentData(solution: "Foo.Bar.FooBar", values: new object[] { "c:/Foo.Bar.FooBar/" })]
        [VisualStudioEnvironmentData(solution: "Foo.Bar.FooBar", folder: "c:/Bar", values: new object[] { "c:/Bar/Foo.Bar.FooBar/" })]
        public void Path_ShouldReturnCorrectSolutionPath(string expected, VisualStudioEnvironment sut)
        {
            var solution = sut.CurrentSolution.ValueOr(default(Solution));

            Check.That(solution.Path).IsEqualTo(new Uri(expected));
        }
    }
}