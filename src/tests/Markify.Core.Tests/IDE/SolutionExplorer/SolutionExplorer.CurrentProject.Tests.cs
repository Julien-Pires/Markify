using Markify.Core.IDE;
using Markify.Models.IDE;
using Markify.Core.Tests.Attributes;
using NFluent;
using Xunit;

using static Markify.Models.IDE.ProjectLanguage;

namespace Markify.Core.Tests.IDE
{
    public partial class SolutionExplorer_Tests
    {
        [Theory]
        [SolutionExplorerInlineAutoData("FooProject", "c:/FooProject", 1, 0, 0, CSharp, new ProjectLanguage[0], new string[0], "Project1")]
        [SolutionExplorerInlineAutoData("BarProject", "c:/FooProject", 4, 2, 0, CSharp, new ProjectLanguage[0], new string[0], "Project3")]
        public void CurrentProject_WhenHasCurrent_ShouldReturnCorrectValue(string expected, SolutionExplorer sut)
        {
            var actual = sut.CurrentProject.Match(
                x => x,
                () => string.Empty
            );

            Check.That(actual).IsEqualTo(expected);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooProject", "c:/FooProject", 0, -1, 0, CSharp, new ProjectLanguage[0], new string[0])]
        [SolutionExplorerInlineAutoData("FooProject", "c:/FooProject", 2, -1, 0, CSharp, new ProjectLanguage[0], new string[0])]
        public void CurrentProject_WhenHasNoCurrent_ShouldReturnNone(SolutionExplorer sut)
        {
            var actual = sut.CurrentProject;

            Check.That(actual.HasValue).IsFalse();
        }

        [Theory]
        [SolutionExplorerInlineAutoData("BarProject", "c:/FooProject", 4, 2, 0, CSharp, new[] { CSharp }, new string[0])]
        [SolutionExplorerInlineAutoData("BarProject", "c:/FooProject", 4, 2, 0, VisualBasic, new[] { CSharp, VisualBasic }, new string[0])]
        public void CurrentProject_WithLanguageIdenticalAsLanguageFilter_ShoulReturnProject(SolutionExplorer sut)
        {
            var project = sut.CurrentProject;

            Check.That(project.HasValue).IsTrue();
        }

        [Theory]
        [SolutionExplorerInlineAutoData("BarProject", "c:/FooProject", 4, 2, 0, CSharp, new[] { VisualBasic }, new string[0])]
        public void CurrentProject_WhenLanguageDifferentThanLanguageFilter_ShouldReturnNone(SolutionExplorer sut)
        {
            var actual = sut.CurrentProject;

            Check.That(actual.HasValue).IsFalse();
        }
    }
}