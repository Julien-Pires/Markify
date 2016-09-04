using System;
using System.Linq;
using Markify.Core.IDE;
using Markify.Core.IDE.VisualStudio;
using Markify.Core.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Core.Tests.IDE
{
    public class VisualStudioEnvironment_Tests
    {
        #region Helpers

        private static string GetRandom(IIDEEnvironment environment, string solution)
        {
            var allProject = environment.GetProjects(solution).ToArray();

            return allProject.ElementAt(new Random().Next(allProject.Length));
        }

        #endregion

        #region GetSolutionPath

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
        [VisualStudioEnvironmentData("Foo", 1, 1, "Foo")]
        public void GetSolutionPath_ShouldReturnPath(string solution, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetSolutionPath(solution)).IsNotNull();
        }

        #endregion

        #region GetProjects

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
        [VisualStudioEnvironmentData("Foo", 0, 0, "Foo", 0)]
        [VisualStudioEnvironmentData("Foo", 4, 2, "Foo", 12)]
        public void GetProjects_ShouldReturnCorrectProjectCount(string solution, int expected, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetProjects(solution).Count()).IsEqualTo(expected);
        }

        #endregion

        #region GetProjectPath

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
        [VisualStudioEnvironmentData("Foo", 4, 0, "Foo")]
        [VisualStudioEnvironmentData("Foo", 4, 2, "Foo")]
        public void GetProjectPath_ShouldReturnPath_WhenProjectExists(string solution, VisualStudioEnvironment sut)
        {
            var project = GetRandom(sut, solution);

            var actual = sut.GetProjectPath(solution, project);
            Check.That(actual).IsNotNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 2, 0, "Foo")]
        public void GetProjectPath_ShouldReturnAbsolutePath_WhenProjectExists(string solution,
            VisualStudioEnvironment sut)
        {
            var project = GetRandom(sut, solution);

            var actual = sut.GetProjectPath(solution, project);
            Check.That(actual.IsAbsoluteUri).IsTrue();
        }

        #endregion
    }
}