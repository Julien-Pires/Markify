using System;
using System.Linq;

using Markify.Core.IDE;
using Markify.Fixtures;

using Xunit;

namespace Markify.Core.Tests.IDE
{
    public partial class ISolutionExplorer_Tests
    {
        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, "FooSolution")]
        [SolutionExplorerInlineAutoData("FooBarSolution", "c:/FooBarSolution", 0, -1, 0, "FooBarSolution")]
        public void CurrentSolution_WithCurrent_ShouldReturnSolution(string expected, ISolutionExplorer explorer)
        {
            var actual = explorer.CurrentSolution;
            var name = actual.Match(
                some: x => x.Name,
                none: () => string.Empty
            );

            Assert.True(actual.HasValue);
            Assert.Equal(expected, name);
        }

        [Theory]
        [SolutionExplorerInlineAutoData(null, null)]
        public void CurrentSolution_WithNoCurrent_ShouldReturnNone(ISolutionExplorer explorer)
        {
            var actual = explorer.CurrentSolution;

            Assert.False(actual.HasValue);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, "c:/FooSolution/")]
        [SolutionExplorerInlineAutoData("FooBarSolution", "c:/Projects/FooBarSolution", 0, -1, 0, "c:/Projects/FooBarSolution/")]
        public void CurrentSolution_ShouldReturnCorrectPath(string expected, ISolutionExplorer explorer)
        {
            var solution = explorer.CurrentSolution;
            var actual = solution.Match(
                some: x => x.Path,
                none: () => null
            );

            Assert.Equal(new Uri(expected), actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, 0)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, 4)]
        public void CurrentSolution_WithProjects_ShouldReturnCorrectCount(int expected, ISolutionExplorer explorer)
        {
            var solution = explorer.CurrentSolution;
            var actual = solution.Match(
                some: x => x.Projects.Count(),
                none: () => -1
            );

            Assert.Equal(expected, actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, "")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, "Project1")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 2, -1, 0, "Project1 Project2")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, "Project1 Project2 Project3 Project4")]
        public void CurrentSolution_WithProjects_ShouldReturnCorrectName(string expected, ISolutionExplorer explorer)
        {
            var expectedProjects = expected.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var solution = explorer.CurrentSolution;
            var projects = solution.Match(
                some: x => x.Projects,
                none: () => null
            );
            var actual = projects.Intersect(expectedProjects);

            Assert.Equal(expectedProjects.Length, actual.Count());
        }
    }
}