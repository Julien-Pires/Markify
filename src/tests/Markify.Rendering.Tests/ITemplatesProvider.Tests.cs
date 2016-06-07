using System;

using Markify.Rendering.Tests.Attributes;

using Xunit;

namespace Markify.Rendering.Tests
{
    public partial class ITemplatesProvider_Tests
    {
        [Theory]
        [T4InlineAutoData(new[] { typeof(string)}, typeof(string))]
        [T4InlineAutoData(new[] { typeof(string), typeof(int), typeof(DateTime) }, typeof(DateTime))]
        public void GetTemplate_WithValidContent_ShouldReturnTemplateInstance(ITemplatesProvider sut, object content)
        {
            var actual = sut.GetTemplate(content);

            Assert.True(actual.HasValue);
        }

        [Theory]
        [T4InlineAutoData(new Type[0], null)]
        [T4InlineAutoData(new Type[0], typeof(int))]
        [T4InlineAutoData(new[] { typeof(string) }, typeof(int))]
        [T4InlineAutoData(new[] { typeof(string), typeof(int), typeof(DateTime) }, typeof(float))]
        public void GetTemplate_WithInvalidContent_ShouldReturnNone(ITemplatesProvider sut, object content)
        {
            var actual = sut.GetTemplate(content);

            Assert.False(actual.HasValue);
        }
    }
}