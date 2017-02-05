using Markify.Core.FSharp;
using Markify.Domain.Ide;
using Markify.Services.VisualStudio.Tests.Attributes;
using Microsoft.FSharp.Core;
using NFluent;
using Xunit;

namespace Markify.Services.VisualStudio.Tests.VisualStudioEnvironment
{
    public sealed partial class VisualStudioEnvironmentTests
    {
        [Theory]
        [VisualStudioEnvironmentData(null)]
        public void CurrentSolution_ShouldReturnNone_WhenNoCurrentSolutionLoaded(VisualStudio.VisualStudioEnvironment sut)
        {
            Check.That(sut.CurrentSolution).IsEqualTo(FSharpOption<Solution>.None);
        }

        [Theory]
        [VisualStudioEnvironmentData]
        public void CurrentSolution_ShouldReturnSolution_WhenCurrentSolutionIsLoaded(VisualStudio.VisualStudioEnvironment sut)
        {
            var actual = sut.CurrentSolution;

            Check.That(actual.IsSome()).IsTrue();
        }
    }
}