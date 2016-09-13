using System.Linq;
using Markify.Core.IDE.VisualStudio;
using Markify.Core.Tests.Attributes;
using NFluent;
using Xunit;

using Solution = Markify.Models.IDE.Solution;

using static EnvDTE.CodeModelLanguageConstants;
using static Markify.Models.IDE.ProjectLanguage;

namespace Markify.Core.Tests.IDE
{
    public sealed partial class VisualStudioEnvironment_Tests
    {
        [Theory]
        [VisualStudioEnvironmentData]
        [VisualStudioEnvironmentData(project: 0, solutionFolder: 10)]
        public void Projects_ShouldReturnZeroProjects_WhenSolutionDoesNotHaveProjects(VisualStudioEnvironment sut)
        {
            var actual = sut.CurrentSolution.ValueOr(default(Solution));

            Check.That(actual.Projects).HasSize(0);
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 10, values: new object[]{ 10 })]
        [VisualStudioEnvironmentData(project: 10, solutionFolder: 2, values: new object[]{ 30 })]
        public void Projects_ShouldReturnCorrectProjectCounts_WhenSolutionHasSome(int expected, VisualStudioEnvironment sut)
        {
            var actual = sut.CurrentSolution.ValueOr(default(Solution));

            Check.That(actual.Projects).HasSize(expected);
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 10, language: vsCMLanguageCSharp, supportedLanguages: new object[0], values: new object[] { 10 })]
        [VisualStudioEnvironmentData(project: 10, language: vsCMLanguageCSharp, supportedLanguages: new object[]{ CSharp }, values: new object[] { 10 })]
        [VisualStudioEnvironmentData(project: 10, language: vsCMLanguageVB, supportedLanguages: new object[]{ CSharp }, values: new object[] { 0 })]
        [VisualStudioEnvironmentData(project: 10, solutionFolder: 1, language: vsCMLanguageCSharp, supportedLanguages: new object[]{ CSharp }, values: new object[] { 20 })]
        public void Projects_ShouldReturnCorrectCount_WhenProjectsAreFiltered(int expected, VisualStudioEnvironment sut)
        {
            var actual = sut.CurrentSolution.ValueOr(default(Solution));

            Check.That(actual.Projects).HasSize(expected);
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 1)]
        public void ProjectPath_ShouldNotBeNull(VisualStudioEnvironment sut)
        {
            var solution = sut.CurrentSolution.ValueOr(default(Solution));
            var actual = solution.Projects;

            Check.That(actual.All(c => c.Path != null)).IsTrue();
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 1)]
        public void ProjectName_ShouldNotBeNullOrEmpty(VisualStudioEnvironment sut)
        {
            var solution = sut.CurrentSolution.ValueOr(default(Solution));
            var actual = solution.Projects;

            Check.That(actual.All(c => !string.IsNullOrWhiteSpace(c.Name))).IsTrue();
        }
    }
}