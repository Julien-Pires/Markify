using Markify.Core.FSharp;
using Markify.Services.VisualStudio.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Services.VisualStudio.Tests.VisualStudioEnvironment
{
    public sealed partial class VisualStudioEnvironmentTests
    {
        [Theory]
        [VisualStudioEnvironmentData(project: 10, hasCurrentProject: true)]
        public void CurrentProject_ShouldReturnSome_WhenIdeHasCurrentProject(VisualStudio.VisualStudioEnvironment sut)
        {
            var actual = sut.CurrentProject;

            Check.That(actual.IsSome()).IsTrue();
            Check.That(actual.Value).IsNotNull();
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 10)]
        public void CurrentProject_ShouldReturnNone_WhenIdeHasNotCurrentProject(VisualStudio.VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentProject.IsSome()).IsFalse();
        }
    }
}