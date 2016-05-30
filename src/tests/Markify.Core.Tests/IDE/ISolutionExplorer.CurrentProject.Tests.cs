using Markify.Core.IDE;
using Markify.Fixtures;

using Xunit;

namespace Markify.Core.Tests.IDE
{
    public partial class ISolutionExplorer_Tests
    {
        [Theory]
        [SolutionExplorerInlineAutoData("FooProject", "FooProject")]
        [SolutionExplorerInlineAutoData("BarProject", "BarProject")]
        public void CurrentProject_WhenHasCurrent_ShouldReturnCorrectValue(string expected, ISolutionExplorer explorer)
        {
            var actual = explorer.CurrentProject.Match(
                some: x => x,
                none: () => string.Empty
            );

            Assert.Equal(expected, actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData(null)]
        public void CurrentProject_WhenHasNoCurrent_ShouldReturnNone(ISolutionExplorer explorer)
        {
            var actual = explorer.CurrentProject;

            Assert.False(actual.HasValue);
        }
    }
}
