using Markify.Core.IDE;
using Markify.Fixtures;

using Xunit;

namespace Markify.Core.Tests.IDE
{
    public partial class ISolutionExplorer_Tests
    {
        [Theory]
        [SolutionExplorerInlineAutoData("FooProject", "c:/FooProject", 1, 0, 0, "Project1")]
        [SolutionExplorerInlineAutoData("BarProject", "c:/FooProject", 4, 2, 0, "Project3")]
        public void CurrentProject_WhenHasCurrent_ShouldReturnCorrectValue(string expected, ISolutionExplorer explorer)
        {
            var actual = explorer.CurrentProject.Match(
                some: x => x,
                none: () => string.Empty
            );

            Assert.Equal(expected, actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooProject", "c:/FooProject")]
        [SolutionExplorerInlineAutoData("FooProject", "c:/FooProject", 2)]
        public void CurrentProject_WhenHasNoCurrent_ShouldReturnNone(ISolutionExplorer explorer)
        {
            var actual = explorer.CurrentProject;

            Assert.False(actual.HasValue);
        }
    }
}
