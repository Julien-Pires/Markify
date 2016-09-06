using System;
using System.Linq;
using EnvDTE;
using Markify.Core.IDE;
using Markify.Models.IDE;
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

        #region Properties

        #region CurrentProject

        [Theory]
        [VisualStudioEnvironmentData("Foo", 2, 0, false)]
        public void CurrentProject_ShouldReturnNull_WhenHasNoActiveProject(VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentProject).IsNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0, true)]
        public void CurrentProject_ShouldReturnNull_WhenSolutionHasNoProject(VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentProject).IsNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 2, 0, true)]
        public void CurrentProject_ShouldReturnProjectName_WhenHasActiveProject(VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentProject).IsNotNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 2, 0, true, "Foo")]
        public void CurrentProject_ShouldReturnProjectNameFromCurrentSolution_WhenHasActiveProject(string solution, VisualStudioEnvironment sut)
        {
            var projects = sut.GetProjects(solution);

            Check.That(projects.Contains(sut.CurrentProject)).IsTrue();
        }

        #endregion

        #endregion

        #region Public Methods

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
        [VisualStudioEnvironmentData("Foo", 1, 1, new object[] { "Foo" })]
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
        [VisualStudioEnvironmentData("Foo", 0, 0, new object[] { "Foo", 0 })]
        [VisualStudioEnvironmentData("Foo", 4, 2, new object[] { "Foo", 12 })]
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

        #endregion

        #region GetProjectFiles

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

        #endregion

        #region GetProjectLanguage

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0)]
        public void GetProjectLanguage_ShouldThrow_WhenSolutionIsNull(VisualStudioEnvironment sut)
        {
            Check.ThatCode(() => sut.GetProjectLanguage(null, "Foo")).Throws<ArgumentNullException>();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0)]
        public void GetProjectLanguage_ShouldThrow_WhenProjectIsNull(VisualStudioEnvironment sut)
        {
            Check.ThatCode(() => sut.GetProjectLanguage("Foo", null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0, new object[]{ "Bar" })]
        public void GetProjectLanguage_ShouldReturnUnsupported_WhenSolutionIsNotCurrent(string solution, string project, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetProjectLanguage(solution, project)).IsEqualTo(ProjectLanguage.Unsupported);
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 1, 0, "C--", "Foo")]
        public void GetProjectLanguage_ShouldReturnUnsupported_WhenLanguageIsNotSupported(string solution, VisualStudioEnvironment sut)
        {
            var project = GetRandom(sut, solution);

            Check.That(sut.GetProjectLanguage(solution, project)).IsEqualTo(ProjectLanguage.Unsupported);
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 1, 0, new object[] { "Foo", "Bar" })]
        public void GetProjectLanguage_ShouldReturnUnsupported_WhenProjectDoesNotExists(string solution, string project, VisualStudioEnvironment sut)
        {
            Check.That(sut.GetProjectLanguage(solution, project)).IsEqualTo(ProjectLanguage.Unsupported);
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 1, 0, CodeModelLanguageConstants.vsCMLanguageCSharp, "Foo", ProjectLanguage.CSharp)]
        [VisualStudioEnvironmentData("Foo", 1, 0, CodeModelLanguageConstants.vsCMLanguageVB, "Foo", ProjectLanguage.VisualBasic)]
        public void GetProjectLanguage_ShouldReturnLanguage_WhenProjectLanguageIsSupported(string solution, ProjectLanguage expected, VisualStudioEnvironment sut)
        {
            var project = GetRandom(sut, solution);

            Check.That(sut.GetProjectLanguage(solution, project)).IsEqualTo(expected);
        }

        #endregion

        #endregion
    }
}