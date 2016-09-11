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
        public void GetProjectFiles_ShouldThrow_WhenSolutionIsNull(VisualStudioEnvironment sut)
        {
            Check.ThatCode(() => sut.GetProjectFiles(null, "Foo")).Throws<ArgumentNullException>();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0)]
        public void GetProjectFiles_ShouldThrow_WhenProjectIsNull(VisualStudioEnvironment sut)
        {
            Check.ThatCode(() => sut.GetProjectFiles("Foo", null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0, "Bar")]
        public void GetProjectFiles_ShouldReturnEmpty_WhenSolutionIsNotCurrent(string solution, string project, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetProjectFiles(solution, project)).IsEmpty();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0, "Foo", "Bar")]
        [VisualStudioEnvironmentData("Foo", 10, 0, "Foo", "Bar")]
        public void GetProjectFiles_ShouldReturnEmpty_WhenProjectDoesNotExists(string solution, string project, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetProjectFiles(solution, project)).IsEmpty();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 1, 0, 1, 0, "Foo", 1)]
        [VisualStudioEnvironmentData("Foo", 1, 1, 1, 0, "Foo", 1)]
        [VisualStudioEnvironmentData("Foo", 1, 0, 1, 1, "Foo", 2)]
        [VisualStudioEnvironmentData("Foo", 1, 0, 100, 1, "Foo", 200)]
        [VisualStudioEnvironmentData("Foo", 1, 0, 10, 10, "Foo", 110)]
        public void GetProjectFiles_ShouldReturnAllProjectFiles_WhenProjectExists(string solution, int expected, VisualStudioEnvironment sut)
        {
            var project = GetRandom(sut, solution);

            Check.That(sut.GetProjectFiles(solution, project)).HasSize(expected);
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 1, 0, 1, 0, "Foo")]
        [VisualStudioEnvironmentData("Foo", 1, 0, 100, 0, "Foo")]
        [VisualStudioEnvironmentData("Foo", 1, 0, 10, 10, "Foo")]
        public void GetProjectFiles_ShouldReturnOnlyFiles_WhenProjectExists(string solution, VisualStudioEnvironment sut)
        {
            var project = GetRandom(sut, solution);

            Check.That(sut.GetProjectFiles(solution, project).All(c => c.IsFile)).IsTrue();
        }
    }
}
