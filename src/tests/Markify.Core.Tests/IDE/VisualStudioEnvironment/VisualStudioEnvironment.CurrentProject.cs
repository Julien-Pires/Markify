using Markify.Core.IDE.VisualStudio;
using Markify.Core.Tests.Attributes;
using Markify.Models.IDE;
using NFluent;
using Xunit;

namespace Markify.Core.Tests.IDE
{
    public sealed partial class VisualStudioEnvironmentTests
    {
        [Theory]
        [VisualStudioEnvironmentData(project: 10, hasCurrentProject: true)]
        public void CurrentProject_ShouldReturnSome_WhenIdeHasCurrentProject(VisualStudioEnvironment sut)
        {
            var actual = sut.CurrentProject;

            Check.That(actual.HasValue).IsTrue();
            Check.That(actual.ValueOr((Project)null)).IsNotNull();
        }

        [Theory]
        [VisualStudioEnvironmentData(project: 10)]
        public void CurrentProject_ShouldReturnNone_WhenIdeHasNotCurrentProject(VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentProject.HasValue).IsFalse();
        }
    }
}