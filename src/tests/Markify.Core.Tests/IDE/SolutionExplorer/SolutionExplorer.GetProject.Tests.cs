using System;
using System.Linq;
using Markify.Core.IDE;
using Markify.Models.IDE;
using Markify.Core.Tests.Attributes;
using NFluent;
using Xunit;

using static Markify.Models.IDE.ProjectLanguage;

namespace Markify.Core.Tests.IDE
{
    public partial class SolutionExplorer_Tests
    {
        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project1")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 6, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project4")]
        public void GetProject_WithExistingProject_ShouldReturnProject(string name, SolutionExplorer sut)
        {
            var actual = sut.GetProject(name);

            Check.That(actual.HasValue).IsTrue();
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 0, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Fooject")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Fooject")]
        public void GetProject_WithNotExistingProject_ShouldReturnNone(string name, SolutionExplorer sut)
        {
            var actual = sut.GetProject(name);

            Check.That(actual.HasValue).IsFalse();
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new[] { CSharp }, new string[0], "Project1", CSharp)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new[] { CSharp, VisualBasic }, new string[0], "Project1", CSharp)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, VisualBasic, new[] { CSharp, VisualBasic }, new string[0], "Project1", VisualBasic)]
        public void GetProject_WithLanguageIdenticalAsLanguageFilter_ShoulReturnProject(string name, ProjectLanguage expected, SolutionExplorer sut)
        {
            var project = sut.GetProject(name);
            var actual = project.Match(
                x => x.Language,
                () => Unsupported
            );

            Check.That(project.HasValue).IsTrue();
            Check.That(actual).IsEqualTo(expected);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, VisualBasic, new[] { CSharp }, new string[0], "Project1")]
        public void GetProject_WithLanguageDifferentThanLanguageFilter_ShoulReturnNone(string name, SolutionExplorer sut)
        {
            var actual = sut.GetProject(name);

            Check.That(actual.HasValue).IsFalse();
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project1")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 3, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project2")]
        public void GetProject_ShouldReturnCorrectName(string expected, SolutionExplorer sut)
        {
            var project = sut.GetProject(expected);
            var actual = project.Match(
                x => x.Name,
                () => string.Empty
            );

            Check.That(actual).IsEqualTo(expected);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project1", "c:/FooSolution/Project1/")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 4, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project3", "c:/FooSolution/Project3/")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 6, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project2", "c:/FooSolution/Project2/")]
        public void GetProject_ShouldReturnCorrectPath(string name, string expected, SolutionExplorer sut)
        {
            var project = sut.GetProject(name);
            var actual = project.Match(
                x => x.Path,
                () => null
            );

            Check.That(actual).IsEqualTo(new Uri(expected));
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project1", 0)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 10, CSharp, new ProjectLanguage[0], new string[0], "Project1", 10)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], new[] { ".cs" }, "Project1", 0)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 10, CSharp, new ProjectLanguage[0], new[] { ".cs" }, "Project1", 10)]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 10, VisualBasic, new ProjectLanguage[0], new[] { ".cs" }, "Project1", 0)]
        public void GetProject_WithFiles_ShouldReturnCorrectFileCount(string name, int expected, SolutionExplorer sut)
        {
            var project = sut.GetProject(name);
            var actual = project.Match(
                x => x.Files.Count(),
                () => -1
            );

            Check.That(actual).IsEqualTo(expected);
        }

        [Theory]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 0, CSharp, new ProjectLanguage[0], new string[0], "Project1", "")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 10, CSharp, new ProjectLanguage[0], new string[0], "Project1", "c:/FooSolution/Project1/File6.cs")]
        [SolutionExplorerInlineAutoData("FooSolution", "c:/FooSolution", 1, -1, 20, CSharp, new ProjectLanguage[0], new string[0], "Project1", "c:/FooSolution/Project1/File1.cs c:/FooSolution/Project1/File12.cs")]
        public void GetProject_WithFiles_ShouldReturnCorrectFilesPath(string name, string expected, SolutionExplorer sut)
        {
            var expectedPaths = expected.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(c => new Uri(c))
                                        .ToArray();

            var project = sut.GetProject(name);
            var paths = project.Match(
                x => x.Files,
                () => null
            );
            var actual = paths.Intersect(expectedPaths);

            Check.That(actual.Count()).IsEqualTo(expectedPaths.Length);
        }
    }
}