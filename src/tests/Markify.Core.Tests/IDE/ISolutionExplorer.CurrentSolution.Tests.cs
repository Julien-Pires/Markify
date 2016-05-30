using System.Linq;

using Markify.Core.IDE;
using Markify.Fixtures;

using Xunit;

namespace Markify.Core.Tests.IDE
{
    public partial class ISolutionExplorer_Tests
    {
        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", new string[0], "FooSolution")]
        [SolutionExplorerInlineAutoData("FooBarSolution", null, null, "FooBarSolution")]
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
        [SolutionExplorerInlineAutoData(null, null, null)]
        public void CurrentSolution_WithNoCurrent_ShouldReturnNone(ISolutionExplorer explorer)
        {
            var actual = explorer.CurrentSolution;

            Assert.False(actual.HasValue);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", new string[0], "FooSolution")]
        [SolutionExplorerInlineAutoData("FooBarSolution", "c:/Projects/FooBarSolution", null, "c:/Projects/FooBarSolution")]
        public void CurrentSolution_ShouldReturnCorrectPath(string expected, ISolutionExplorer explorer)
        {
            var solution = explorer.CurrentSolution;
            var actual = solution.Match(
                some: x => x.Path.OriginalString,
                none: () => string.Empty
            );

            Assert.Equal(expected, actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", null, new string[0], 0)]
        [SolutionExplorerInlineAutoData("FooSolution", null, new []{"Foo", "Bar"}, 2)]
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
        [SolutionExplorerInlineAutoData("FooSolution", null, new string[0], new object[] { new string[0] })]
        [SolutionExplorerInlineAutoData("FooSolution", null, new[] { "Foo", "Bar" }, new object[] { new [] { "Foo", "Bar" } })]
        [SolutionExplorerInlineAutoData("FooSolution", null, new[] { "Foo", "Bar", "FooBar" }, new object[] { new[] { "Foo" } })]
        public void CurrentSolution_WithProjects_ShouldReturnCorrectName(string[] expected, ISolutionExplorer explorer)
        {
            var solution = explorer.CurrentSolution;
            var projects = solution.Match(
                some: x => x.Projects,
                none: () => null
            );
            var actual = projects.Intersect(expected);

            Assert.Equal(expected.Length, actual.Count());
        }
    }
}