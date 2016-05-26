using System.Linq;

using Markify.Core.IDE;

using Xunit;

namespace Markify.Core.Tests.IDE
{
    public partial class ISolutionExplorer_Tests
    {
        [Theory]
        public void GetProjects_ShouldReturnCorrectCount(int expected, ISolutionExplorer solution)
        {
            var actual = solution.GetProjects().Count();

            Assert.Equal(expected, actual);
        }

        [Theory]
        public void GetProjects_ShouldReturnWithCorrectName(string name, ISolutionExplorer solution)
        {
            var actual = solution.GetProjects().First(c => c.Name == name);

            Assert.NotNull(actual);
        }

        [Theory]
        public void GetProject_WithExistingProject_ShouldReturnProject(string name, ISolutionExplorer solution)
        {
            var actual = solution.GetProject(name);

            Assert.NotNull(actual);
        }

        [Theory]
        public void GetProject_WithNotExistingProject_ShouldReturnNone(string name, ISolutionExplorer solution)
        {
            var actual = solution.GetProject(name);

            Assert.Null(actual);
        }
    }
}