using System;
using System.Linq;

using Markify.Core.IDE;
using Markify.Fixtures;

using Xunit;

using static Markify.Models.Context;
using static Markify.Models.Context.ProjectLanguage;

namespace Markify.Core.Tests.IDE
{
    public partial class ISolutionExplorer_Tests
    {
        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], "FooSolution")]
        [SolutionExplorerInlineAutoData("FooBarSolution", "c:/FooBarSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], "FooBarSolution")]
        public void CurrentSolution_WithCurrent_ShouldReturnSolution(string expected, ISolutionExplorer sut)
        {
            var actual = sut.CurrentSolution;
            var name = actual.Match(
                some: x => x.Name,
                none: () => string.Empty
            );

            Assert.True(actual.HasValue);
            Assert.Equal(expected, name);
        }

        [Theory]
        [SolutionExplorerInlineAutoData(null, null, 0, -1, 0, CSharp, new ProjectLanguage[0])]
        public void CurrentSolution_WithNoCurrent_ShouldReturnNone(ISolutionExplorer sut)
        {
            var actual = sut.CurrentSolution;

            Assert.False(actual.HasValue);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], "c:/FooSolution/")]
        [SolutionExplorerInlineAutoData("FooBarSolution", "c:/Projects/FooBarSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], "c:/Projects/FooBarSolution/")]
        public void CurrentSolution_ShouldReturnCorrectPath(string expected, ISolutionExplorer sut)
        {
            var solution = sut.CurrentSolution;
            var actual = solution.Match(
                some: x => x.Path,
                none: () => null
            );

            Assert.Equal(new Uri(expected), actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], 0)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new ProjectLanguage[0], 4)]
        public void CurrentSolution_WithProjects_ShouldReturnCorrectCount(int expected, ISolutionExplorer sut)
        {
            var solution = sut.CurrentSolution;
            var actual = solution.Match(
                some: x => x.Projects.Count(),
                none: () => -1
            );

            Assert.Equal(expected, actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new[] { CSharp }, 0)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new[] { CSharp }, 4)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new[] { VisualBasic }, 0)]
        public void CurrentSolution_WithLanguageFilter_ShouldReturnCorrectCount(int expected, ISolutionExplorer sut)
        {
            var solution = sut.CurrentSolution;
            var actual = solution.Match(
                some: x => x.Projects.Count(),
                none: () => -1
            );

            Assert.Equal(expected, actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], "")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], "Project1")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 2, -1, 0, CSharp, new ProjectLanguage[0], "Project1 Project2")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new ProjectLanguage[0], "Project1 Project2 Project3 Project4")]
        public void CurrentSolution_WithProjects_ShouldReturnCorrectName(string expected, ISolutionExplorer sut)
        {
            var expectedProjects = expected.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var solution = sut.CurrentSolution;
            var projects = solution.Match(
                some: x => x.Projects,
                none: () => null
            );
            var actual = projects.Intersect(expectedProjects);

            Assert.Equal(expectedProjects.Length, actual.Count());
        }
    }
}