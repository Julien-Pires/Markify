using System;
using System.Linq;
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
        public void GetProjects_ShouldThrow_WhenNameIsNull(VisualStudioEnvironment sut)
        {
            Check.ThatCode(() => sut.GetProjects(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [VisualStudioEnvironmentData(null, 0, 0, "Foo")]
        public void GetProjects_ShouldReturnNull_WhenCurrentSolutionIsNull(string solution, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetProjects(solution)).IsNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0)]
        public void GetProjects_ShouldReturnNull_WhenSolutionIsNotCurrent(string solution, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetProjects(solution)).IsNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0, new object[] { "Foo", 0 })]
        [VisualStudioEnvironmentData("Foo", 4, 2, new object[] { "Foo", 12 })]
        public void GetProjects_ShouldReturnCorrectProjectCount(string solution, int expected, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetProjects(solution).Count()).IsEqualTo(expected);
        }
    }
}
