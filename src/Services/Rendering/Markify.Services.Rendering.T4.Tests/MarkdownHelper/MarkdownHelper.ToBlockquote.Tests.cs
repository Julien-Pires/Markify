using System;
using NFluent;
using Xunit;

namespace Markify.Services.Rendering.T4.Tests
{
    public partial class MarkdownHelperTests
    {
        private const string Blockquote = ">";

        [Fact]
        public void ToBlockquote_ShouldThrowException_WhenTextIsNull()
        {
            Check.ThatCode(() => MarkdownHelper.ToBlockquote(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        public void ToBlockquote_ShouldReturnEmpty_WhenTextIsEmpty(string text)
        {
            Check.That(MarkdownHelper.ToBlockquote(text)).IsEmpty();
        }

        [Theory]
        [InlineData("Foo")]
        public void ToBlockquote_ShouldAddBlockquoteCharacter_WhenTextIsNotEmpty(string text)
        {
            Check.That(MarkdownHelper.ToBlockquote(text)).StartsWith(Blockquote);
        }

        [Theory]
        [InlineData("Foo\nBar", ">Foo\n>Bar")]
        [InlineData("Foo\nBar\nFoobar", ">Foo\n>Bar\n>Foobar")]
        [InlineData("Foo\nBar\r\nFoobar", ">Foo\n>Bar\r\n>Foobar")]
        public void ToBlockquote_ShouldAddBlockquoteForEachLine_WhenTextIsMultiLine(string text, string expected)
        {
            Check.That(MarkdownHelper.ToBlockquote(text)).IsEqualTo(expected);
        }
    }
}