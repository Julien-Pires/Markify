using Markify.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Rendering.T4.Tests
{
    public sealed class T4TemplateTests
    {
        [Theory]
        [TemplateData("Foo")]
        internal void Apply_ShouldReturnValue_WhenTransformIsOk(T4Template sut)
        {
            Check.That(sut.Apply(null).HasValue).IsTrue();
        }

        [Theory]
        [TemplateData("Foo", null, "Foo")]
        [TemplateData("Foo", "Content", "Foo-Content")]
        internal void Apply_ShouldReturnTemplateResult_WhenTransformIsOk(string content, string expected, T4Template sut)
        {
            var actual = sut.Apply(content).ValueOr(string.Empty);

            Check.That(actual).IsEqualTo(expected);
        }

        [Theory]
        [TemplateData(true)]
        internal void Apply_ShouldReturnNone_WhenTransformFailed(T4Template sut)
        {
            Check.That(sut.Apply(null).HasValue).IsFalse();
        }
    }
}