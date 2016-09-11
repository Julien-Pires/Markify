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
        public void GetProjectPath_ShouldThrow_WhenSolutionNameIsNull(VisualStudioEnvironment sut)
        {
            Check.ThatCode(() => sut.GetProjectPath(null, "Bar")).Throws<ArgumentNullException>();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0)]
        public void GetProjectPath_ShouldThrow_WhenProjectNameIsNull(VisualStudioEnvironment sut)
        {
            Check.ThatCode(() => sut.GetProjectPath("Foo", null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0, "Bar")]
        public void GetProjectPath_ShouldReturnNull_WhenSolutionIsNotCurrent(string solution, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetProjectPath(solution, "Foo")).IsNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0, "Foo")]
        [VisualStudioEnvironmentData("Foo", 10, 0, "Foo")]
        public void GetProjectPath_ShouldReturnNull_WhenProjectDoesNotExists(string solution, string project,
            VisualStudioEnvironment sut)
        {
            Check.That(sut.GetProjectPath(solution, project)).IsNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 4, 0, new object[] { "Foo" })]
        [VisualStudioEnvironmentData("Foo", 4, 2, new object[] { "Foo" })]
        public void GetProjectPath_ShouldReturnPath_WhenProjectExists(string solution, VisualStudioEnvironment sut)
        {
            var project = GetRandom(sut, solution);

            var actual = sut.GetProjectPath(solution, project);
            Check.That(actual).IsNotNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 2, 0, new object[] { "Foo" })]
        public void GetProjectPath_ShouldReturnAbsolutePath_WhenProjectExists(string solution,
            VisualStudioEnvironment sut)
        {
            var project = GetRandom(sut, solution);

            var actual = sut.GetProjectPath(solution, project);
            Check.That(actual.IsAbsoluteUri).IsTrue();
        }
    }
}
