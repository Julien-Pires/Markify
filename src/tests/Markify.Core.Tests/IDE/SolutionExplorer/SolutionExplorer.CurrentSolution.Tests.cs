using System;
using System.Linq;
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
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "FooSolution")]
        [SolutionExplorerInlineAutoData("FooBarSolution", "c:/FooBarSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "FooBarSolution")]
        public void CurrentSolution_WithCurrent_ShouldReturnSolution(string expected, SolutionExplorer sut)
        {
            var solution = sut.CurrentSolution;
            var actual = solution.Match(
                x => x.Name,
                () => string.Empty
            );

            Check.That(solution.HasValue).IsTrue();
            Check.That(actual).IsEqualTo(expected);
        }

        [Theory]
        [SolutionExplorerInlineAutoData(null, null, 0, -1, 0, CSharp, new ProjectLanguage[0], new string[0])]
        public void CurrentSolution_WithNoCurrent_ShouldReturnNone(SolutionExplorer sut)
        {
            var actual = sut.CurrentSolution;

            Check.That(actual.HasValue).IsFalse();
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "c:/FooSolution/")]
        [SolutionExplorerInlineAutoData("FooBarSolution", "c:/Projects/FooBarSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "c:/Projects/FooBarSolution/")]
        public void CurrentSolution_ShouldReturnCorrectPath(string expected, SolutionExplorer sut)
        {
            var solution = sut.CurrentSolution;
            var actual = solution.Match(
                x => x.Path,
                () => null
            );

            Check.That(actual).IsEqualTo(new Uri(expected));
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], new string[0], 0)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new ProjectLanguage[0], new string[0], 4)]
        public void CurrentSolution_WithProjects_ShouldReturnCorrectCount(int expected, SolutionExplorer sut)
        {
            var solution = sut.CurrentSolution;
            var actual = solution.Match(
                x => x.Projects.Count(),
                () => -1
            );

            Check.That(actual).IsEqualTo(expected);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new[] { CSharp }, new string[0], 0)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new[] { CSharp }, new string[0], 4)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new[] { VisualBasic }, new string[0], 0)]
        public void CurrentSolution_WithLanguageFilter_ShouldReturnCorrectCount(int expected, SolutionExplorer sut)
        {
            var solution = sut.CurrentSolution;
            var actual = solution.Match(
                x => x.Projects.Count(),
                () => -1
            );

            Check.That(actual).IsEqualTo(expected);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project1")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 2, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project1 Project2")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project1 Project2 Project3 Project4")]
        public void CurrentSolution_WithProjects_ShouldReturnCorrectName(string expected, SolutionExplorer sut)
        {
            var expectedProjects = expected.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var solution = sut.CurrentSolution;
            var projects = solution.Match(
                x => x.Projects,
                () => null
            );
            var actual = projects.Intersect(expectedProjects);

            Check.That(actual.Count()).IsEqualTo(expectedProjects.Length);
        }
    }
}