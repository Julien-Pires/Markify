using System;
using Markify.Core.IDE.VisualStudio;
using Markify.Core.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Core.Tests.IDE
{
    public sealed partial class VisualStudioEnvironment_Tests
    {
        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0)]
        public void GetSolutionPath_ShouldThrow_WhenNameIsNull(VisualStudioEnvironment sut)
        {
            Check.ThatCode(() => sut.GetSolutionPath(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0, "Bar")]
        public void GetSolutionPath_ShouldReturnNull_WhenSolutionIsNotCurrent(string solution, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetSolutionPath(solution)).IsNull();
        }

        [Theory]
        [VisualStudioEnvironmentData(null, 0, 0)]
        public void GetSolutionPath_ShouldReturnNull_WhenCurrentSolutionIsNull(string solution, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetSolutionPath(solution)).IsNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 1, 1, new object[] { "Foo" })]
        public void GetSolutionPath_ShouldReturnPath(string solution, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetSolutionPath(solution)).IsNotNull();
        }
    }
}
