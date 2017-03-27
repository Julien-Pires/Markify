using System.Linq;
using Markify.Domain.Ide;
using Markify.Services.VisualStudio.Tests.Attributes;
using NFluent;
using Xunit;

using static EnvDTE.CodeModelLanguageConstants;
using static Markify.Domain.Ide.ProjectLanguage;

namespace Markify.Services.VisualStudio.Tests.VisualStudioEnvironment
{
    public sealed partial class VisualStudioEnvironmentTests
    {
        [Theory]
        [VisualStudioEnvironmentData]
        [VisualStudioEnvironmentData(project: 0, solutionFolder: 10)]
        public void Projects_ShouldReturnZeroProjects_WhenSolutionDoesNotHaveProjects(VisualStudio.VisualStudioEnvironment sut)
        {
            var actual = sut.CurrentSolution.Value;

            Check.That(actual.Projects).HasSize(0);
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 10, values: new object[]{ 10 })]
        [VisualStudioEnvironmentData(project: 10, solutionFolder: 2, values: new object[]{ 30 })]
        public void Projects_ShouldReturnCorrectProjectCounts_WhenSolutionHasSome(int expected, VisualStudio.VisualStudioEnvironment sut)
        {
            var actual = sut.CurrentSolution.Value;

            Check.That(actual.Projects).HasSize(expected);
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 10, language: vsCMLanguageCSharp, supportedLanguages: new object[0], values: new object[] { 10 })]
        [VisualStudioEnvironmentData(project: 10, language: vsCMLanguageCSharp, supportedLanguages: new object[]{ CSharp }, values: new object[] { 10 })]
        [VisualStudioEnvironmentData(project: 10, language: vsCMLanguageVB, supportedLanguages: new object[]{ CSharp }, values: new object[] { 0 })]
        [VisualStudioEnvironmentData(project: 10, solutionFolder: 1, language: vsCMLanguageCSharp, supportedLanguages: new object[]{ CSharp }, values: new object[] { 20 })]
        public void Projects_ShouldReturnCorrectCount_WhenProjectsAreFiltered(int expected, VisualStudio.VisualStudioEnvironment sut)
        {
            var actual = sut.CurrentSolution.Value;

            Check.That(actual.Projects).HasSize(expected);
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 1)]
        public void ProjectPath_ShouldNotBeNull(VisualStudio.VisualStudioEnvironment sut)
        {
            var solution = sut.CurrentSolution.Value;
            var actual = solution.Projects;

            Check.That(actual.All(c => c.Path != null)).IsTrue();
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 1)]
        public void ProjectName_ShouldNotBeNullOrEmpty(VisualStudio.VisualStudioEnvironment sut)
        {
            var solution = sut.CurrentSolution.Value;
            var actual = solution.Projects;

            Check.That(actual.All(c => !string.IsNullOrWhiteSpace(c.Name))).IsTrue();
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 1, language: vsCMLanguageVC, values: new object[] { Unsupported })]
        [VisualStudioEnvironmentData(project: 1, language: vsCMLanguageCSharp, values: new object[]{ CSharp })]
        [VisualStudioEnvironmentData(project: 1, language: vsCMLanguageVB, values: new object[] { VisualBasic })]
        public void Projects_ShouldHaveExpectedLanguage(ProjectLanguage expected, VisualStudio.VisualStudioEnvironment sut)
        {
            var solution = sut.CurrentSolution.Value;
            var actual = solution.Projects;

            Check.That(actual.Select(c => c.Language)).Contains(expected);
        }
    }
}