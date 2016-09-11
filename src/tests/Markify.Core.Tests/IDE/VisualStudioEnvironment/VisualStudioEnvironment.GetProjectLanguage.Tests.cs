using System;
using EnvDTE;
using Markify.Models.IDE;
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
        [VisualStudioEnvironmentData("Foo", 0, 0, new object[] { "Bar" })]
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
    }
}
