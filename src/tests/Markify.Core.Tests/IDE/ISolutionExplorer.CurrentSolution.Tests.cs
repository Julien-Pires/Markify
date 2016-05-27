using System.Linq;

using Markify.Core.IDE;

using Xunit;

namespace Markify.Core.Tests.IDE
{
    public partial class ISolutionExplorer_Tests
    {
        [Theory]
        public void CurrentSolution_WithCurrent_ShouldReturnSolution(ISolutionExplorer explorer)
        {
            var actual = explorer.CurrentSolution;

            Assert.True(actual.HasValue);
        }

        [Theory]
        public void CurrentSolution_WithNoCurrent_ShouldReturnNone(ISolutionExplorer explorer)
        {
            var actual = explorer.CurrentSolution;

            Assert.False(actual.HasValue);
        }

        [Theory]
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
        public void CurrentSolution_WithProjects_ShouldReturnCorrectPath(string[] expected, ISolutionExplorer explorer)
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