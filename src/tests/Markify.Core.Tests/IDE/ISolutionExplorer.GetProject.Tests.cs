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
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], "Project1")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 6, -1, 0, CSharp, new ProjectLanguage[0], "Project4")]
        public void GetProject_WithExistingProject_ShouldReturnProject(string name, ISolutionExplorer sut)
        {
            var actual = sut.GetProject(name);

            Assert.True(actual.HasValue);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], "Fooject")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new ProjectLanguage[0], "Fooject")]
        public void GetProject_WithNotExistingProject_ShouldReturnNone(string name, ISolutionExplorer sut)
        {
            var actual = sut.GetProject(name);

            Assert.False(actual.HasValue);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new[] { CSharp }, "Project1", CSharp)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new[] { CSharp, VisualBasic }, "Project1", CSharp)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, VisualBasic, new[] { CSharp, VisualBasic }, "Project1", VisualBasic)]
        public void GetProject_WithLanguageIdenticalAsLanguageFilter_ShoulReturnProject(string name, ProjectLanguage expected, ISolutionExplorer sut)
        {
            var project = sut.GetProject(name);
            var actual = project.Match(
                x => x.Language,
                () => Unsupported
            );

            Assert.True(project.HasValue);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, VisualBasic, new[] { CSharp }, "Project1")]
        public void GetProject_WithLanguageDifferentThanLanguageFilter_ShoulReturnNone(string name, ISolutionExplorer sut)
        {
            var actual = sut.GetProject(name);

            Assert.False(actual.HasValue);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], "Project1")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 3, -1, 0, CSharp, new ProjectLanguage[0], "Project2")]
        public void GetProject_ShouldReturnCorrectName(string name, ISolutionExplorer sut)
        {
            var project = sut.GetProject(name);
            var actual = project.Match(
                some: x => x.Name,
                none: () => string.Empty
            );

            Assert.Equal(name, actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], "Project1", "c:/FooSolution/Project1/")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new ProjectLanguage[0], "Project3", "c:/FooSolution/Project3/")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 6, -1, 0, CSharp, new ProjectLanguage[0], "Project2", "c:/FooSolution/Project2/")]
        public void GetProject_ShouldReturnCorrectPath(string name, string expected, ISolutionExplorer sut)
        {
            var project = sut.GetProject(name);
            var actual = project.Match(
                some: x => x.Path,
                none: () => null
            );

            Assert.Equal(new Uri(expected), actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], "Project1", 0)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 10, CSharp, new ProjectLanguage[0], "Project1", 10)]
        public void GetProject_WithFiles_ShouldReturnCorrectCount(string name, int expected, ISolutionExplorer sut)
        {
            var project = sut.GetProject(name);
            var actual = project.Match(
                some: x => x.Files.Count(),
                none: () => -1
            );

            Assert.Equal(expected, actual);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], "Project1", "")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 10, CSharp, new ProjectLanguage[0], "Project1", "c:/FooSolution/Project1/File6.cs")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 20, CSharp, new ProjectLanguage[0], "Project1", "c:/FooSolution/Project1/File1.cs c:/FooSolution/Project1/File12.cs")]
        public void GetProject_WithFiles_ShouldReturnCorrectFilesPath(string name, string expected, ISolutionExplorer sut)
        {
            var expectedPaths = expected.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(c => new Uri(c))
                                        .ToArray();

            var project = sut.GetProject(name);
            var paths = project.Match(
                some: x => x.Files,
                none: () => null
            );
            var actual = paths.Intersect(expectedPaths);

            Assert.Equal(expectedPaths.Length, actual.Count());
        }
    }
}