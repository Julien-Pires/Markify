using Optional;
using Markify.Models.IDE;
using Markify.Core.Tests.Attributes;
using Markify.Core.IDE.VisualStudio;
using NFluent;
using Xunit;

namespace Markify.Core.Tests.IDE
{
    public sealed partial class VisualStudioEnvironmentTests
    {
        [Theory]
        [VisualStudioEnvironmentData(null)]
        public void CurrentSolution_ShouldReturnNone_WhenNoCurrentSolutionLoaded(VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentSolution).IsEqualTo(Option.None<Solution>());
        }

        [Theory]
        [VisualStudioEnvironmentData]
        public void CurrentSolution_ShouldReturnSolution_WhenCurrentSolutionIsLoaded(VisualStudioEnvironment sut)
        {
            var actual = sut.CurrentSolution;

            Check.That(actual.HasValue).IsTrue();
        }
    }
}