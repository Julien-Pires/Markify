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
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, "Project1")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 6, -1, 0, "Project4")]
        public void GetProject_WithExistingProject_ShouldReturnProject(string name, ISolutionExplorer solution)
        {
            var actual = solution.GetProject(name);

            Assert.True(actual.HasValue);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, "Fooject")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, "Fooject")]
        public void GetProject_WithNotExistingProject_ShouldReturnNone(string name, ISolutionExplorer solution)
        {
            var actual = solution.GetProject(name);

            Assert.False(actual.HasValue);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, "Project1")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 3, -1, 0, "Project2")]
        public void GetProject_ShouldReturnCorrectName(string name, ISolutionExplorer solution)
        {
            var project = solution.GetProject(name);
            var actual = project.Match(
                some: x => x.Name,
                none: () => string.Empty
            );

            Assert.Equal(name, actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, "Project1", "c:/FooSolution/Project1/")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, "Project3", "c:/FooSolution/Project3/")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 6, -1, 0, "Project2", "c:/FooSolution/Project2/")]
        public void GetProject_ShouldReturnCorrectPath(string name, string expected, ISolutionExplorer solution)
        {
            var project = solution.GetProject(name);
            var actual = project.Match(
                some: x => x.Path,
                none: () => null
            );

            Assert.Equal(new Uri(expected), actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, "Project1", 0)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 10, "Project1", 10)]
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
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, "Project1", "")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 10, "Project1", "c:/FooSolution/Project1/File6.cs")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 20, "Project1", "c:/FooSolution/Project1/File1.cs c:/FooSolution/Project1/File12.cs")]
        public void GetProject_WithFiles_ShouldReturnCorrectFilesPath(string name, string expected, ISolutionExplorer solution)
        {
            var expectedPaths = expected.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(c => new Uri(c))
                                        .ToArray();

            var project = solution.GetProject(name);
            var paths = project.Match(
                some: x => x.Files,
                none: () => null
            );
            var actual = paths.Intersect(expectedPaths);

            Assert.Equal(expectedPaths.Length, actual.Count());
        }
    }
}