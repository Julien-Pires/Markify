using System;
using NFluent;
using Xunit;

namespace Markify.Services.Rendering.T4.Tests
{
    public partial class MarkdownHelperTests
    {
		[Fact]
        public void EscapeString_ShouldThrowException_WhenTextIsNull()
        {
            Check.ThatCode(() => MarkdownHelper.EscapeString(null)).Throws<ArgumentNullException>();
        }

		[Fact]
        public void EscapeString_ShouldReturnEmpty_WhenTextIsEmpty()
        {
            Check.That(MarkdownHelper.EscapeString(string.Empty)).IsEqualTo(string.Empty);
        }

		[Theory]
		[InlineData("List<int>", "List&lt;int&gt;")]
        public void EscapeString_ShouldEscapeSpecialCharacter_WhenTextHasSome(string text, string expected)
        {
            Check.That(MarkdownHelper.EscapeString(text)).IsEqualTo(expected);
        }
    }
}