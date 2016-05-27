using System.Linq;

using Markify.Core.IDE;

using Xunit;

namespace Markify.Core.Tests.IDE
{
    public partial class ISolutionExplorer_Tests
    {
        [Theory]
        public void GetProject_WithExistingProject_ShouldReturnProject(string name, ISolutionExplorer solution)
        {
            var actual = solution.GetProject(name);

            Assert.True(actual.HasValue);
        }

        [Theory]
        public void GetProject_WithNotExistingProject_ShouldReturnNone(string name, ISolutionExplorer solution)
        {
            var actual = solution.GetProject(name);

            Assert.False(actual.HasValue);
        }

        [Theory]
        public void GetProject_ShouldReturnCorrectName(string name, string expected, ISolutionExplorer solution)
        {
            var project = solution.GetProject(name);
            var actual = project.Match(
                some: x => x.Name,
                none: () => string.Empty
            );

            Assert.Equal(expected, actual);
        }

        [Theory]
        public void GetProject_ShouldReturnCorrectPath(string name, string expected, ISolutionExplorer solution)
        {
            var project = solution.GetProject(name);
            var actual = project.Match(
                some: x => x.Path.OriginalString,
                none: () => string.Empty
            );

            Assert.Equal(expected, actual);
        }

        [Theory]
        public void GetProject_WithFiles_ShouldReturnCorrectCount(string name, int expected, ISolutionExplorer solution)
        {
            var project = solution.GetProject(name);
            var actual = project.Match(
                some: x => x.Files.Count(),
                none: () => -1
            );

            Assert.Equal(expected, actual);
        }

        [Theory]
        public void GetProject_WithFiles_ShouldReturnCorrectFilesPath(string name, string[] expected, ISolutionExplorer solution)
        {
            var project = solution.GetProject(name);
            var paths = project.Match(
                some: x => x.Files.Select(c => c.OriginalString),
                none: () => null
            );
            var actual = paths.Intersect(expected);

            Assert.Equal(expected.Length, actual.Count());
        }
    }
}