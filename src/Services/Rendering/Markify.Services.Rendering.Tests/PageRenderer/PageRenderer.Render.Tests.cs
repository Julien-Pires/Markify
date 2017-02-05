using Markify.Domain.Document;
using Markify.Services.Rendering.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Services.Rendering.Tests
{
    public sealed class PageRendererTests
    {
        [Theory]
        [PageRendererData(0, 0)]
        [PageRendererData(10, 10)]
        internal void Render_ShouldWrite_WhenTemplateIsFoundForContent(int expected, PagesRenderer sut, TableOfContent toc, FakePageWriter writer)
        {
            sut.Render(toc, writer);

            Check.That(writer.Written).IsEqualTo(expected);
        }

        [Theory]
        [PageRendererData(10, false, false, 0)]
        internal void Render_ShouldNotWrite_WhenNoTemplateFound(int expected, PagesRenderer sut, TableOfContent toc, FakePageWriter writer)
        {
            sut.Render(toc, writer);

            Check.That(writer.Written).IsEqualTo(expected);
        }
    }
}