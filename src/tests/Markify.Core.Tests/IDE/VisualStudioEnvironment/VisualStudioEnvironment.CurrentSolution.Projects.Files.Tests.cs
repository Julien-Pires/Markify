using System.Linq;
using Markify.Core.IDE.VisualStudio;
using Markify.Core.Tests.Attributes;
using Markify.Models.IDE;
using NFluent;
using Xunit;

namespace Markify.Core.Tests.IDE
{
    public sealed partial class VisualStudioEnvironment_Tests
    {
        [Theory]
        [VisualStudioEnvironmentData(project: 1, files: 1, extensions: new[] { "cs" }, values: new object[] { 1 })]
        [VisualStudioEnvironmentData(project: 1, files: 1, fileFolders: 1, extensions: new[] { "cs" }, values: new object[] { 2 })]
        [VisualStudioEnvironmentData(project: 1, files: 10, fileFolders: 2, extensions: new[] { "cs", "xml" }, values: new object[] { 60 })]
        public void ProjectFiles_ShouldReturnCorrectCount_WhenProjectHasSome(int expected, VisualStudioEnvironment sut)
        {
            var solution = sut.CurrentSolution.ValueOr(default(Solution));
            var actual = solution.Projects.First().Files;

            Check.That(actual).HasSize(expected);
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 1, files: 0, extensions: new[] { "cs" })]
        [VisualStudioEnvironmentData(project: 1, files: 0, fileFolders: 10, extensions: new[] { "cs" })]
        public void ProjectFiles_ShouldReturnZero_WhenProjectHasNone(VisualStudioEnvironment sut)
        {
            var solution = sut.CurrentSolution.ValueOr(default(Solution));
            var actual = solution.Projects.First().Files;

            Check.That(actual).HasSize(0);
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 1, files: 0, extensions: new[] { "cs" }, allowedExtensions: new[] { "cs" }, values: new object[] { 0 })]
        [VisualStudioEnvironmentData(project: 1, files: 10, extensions: new[] { "cs" }, allowedExtensions: new[] { "cs" }, values: new object[] { 10 })]
        [VisualStudioEnvironmentData(project: 1, files: 10, extensions: new[] { "cs" }, allowedExtensions: new[] { "vb" }, values: new object[] { 0 })]
        [VisualStudioEnvironmentData(project: 1, files: 10, extensions: new[] { "cs", "xml" }, allowedExtensions: new[] { "cs" }, values: new object[] { 10 })]
        [VisualStudioEnvironmentData(project: 1, files: 10, extensions: new[] { "cs", "xml" }, allowedExtensions: new[] { "cs", "xml" }, values: new object[] { 20 })]
        public void ProjectFiles_ShouldReturnCorrectCount_WhenFilesAreFiltered(int expected, VisualStudioEnvironment sut)
        {
            var solution = sut.CurrentSolution.ValueOr(default(Solution));
            var actual = solution.Projects.First().Files;

            Check.That(actual).HasSize(expected);
        }
    }
}