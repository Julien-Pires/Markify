using System.Linq;
using Markify.Core.IDE.VisualStudio;
using Markify.Core.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Core.Tests.IDE
{
    public sealed partial class VisualStudioEnvironment_Tests
    {
        [Theory]
        [VisualStudioEnvironmentData("Foo", 2, 0, false)]
        public void CurrentProject_ShouldReturnNull_WhenHasNoActiveProject(VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentProject).IsNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 0, 0, true)]
        public void CurrentProject_ShouldReturnNull_WhenSolutionHasNoProject(VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentProject).IsNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 2, 0, true)]
        public void CurrentProject_ShouldReturnProjectName_WhenHasActiveProject(VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentProject).IsNotNull();
        }

        [Theory]
        [VisualStudioEnvironmentData("Foo", 2, 0, true, "Foo")]
        public void CurrentProject_ShouldReturnProjectNameFromCurrentSolution_WhenHasActiveProject(string solution, VisualStudioEnvironment sut)
        {
            var projects = sut.GetProjects(solution);

            Check.That(projects.Contains(sut.CurrentProject)).IsTrue();
        }
    }
}
