using System;
using Markify.Services.VisualStudio.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Services.VisualStudio.Tests.VisualStudioEnvironment
{
    public sealed partial class VisualStudioEnvironmentTests
    {
        [Theory]
        [VisualStudioEnvironmentData("Foo", values: new object[] { "c:/Foo/" })]
        [VisualStudioEnvironmentData("Foo", "c:/Bar", values: new object[] { "c:/Bar/Foo/" })]
        [VisualStudioEnvironmentData("Foo.Bar.FooBar", values: new object[] { "c:/Foo.Bar.FooBar/" })]
        [VisualStudioEnvironmentData("Foo.Bar.FooBar", "c:/Bar", values: new object[] { "c:/Bar/Foo.Bar.FooBar/" })]
        public void Path_ShouldReturnCorrectSolutionPath(string expected, VisualStudio.VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentSolution.Value.Path).IsEqualTo(new Uri(expected));
        }
    }
}