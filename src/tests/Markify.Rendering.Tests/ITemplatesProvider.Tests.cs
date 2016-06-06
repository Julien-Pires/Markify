using System;

using Markify.Rendering;
using Markify.Rendering.Tests.Attributes;

using Xunit;

namespace Markify.Rendering.Tests
{
    public partial class ITemplatesProvider_Tests
    {
        [Theory]
        [T4InlineAutoData(new[] { typeof(string)}, typeof(string))]
        public void GetTemplate_WithValidContent_ShouldReturnTemplateInstance(ITemplatesProvider sut, object content)
        {
            var actual = sut.GetTemplate(content);

            Assert.True(actual.HasValue);
        }

        [Theory]
        public void GetTemplate_WithInvalidContent_ShouldReturnNone(ITemplatesProvider sut, object content)
        {
            var actual = sut.GetTemplate(content);

            Assert.False(actual.HasValue);
        }
    }
}