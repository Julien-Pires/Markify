using System;
using Markify.CodeAnalyzer;
using NFluent;
using Xunit;

namespace Markify.Services.Rendering.Tests
{
    public sealed partial class CommentFormatterTests
    {
        [Fact]
        public void GetText_ShouldThrow_WhenCommentIsNull()
        {
            Check.ThatCode(() => CommentFormatter.GetText(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(SimpleTextComment))]
        [MemberData(nameof(SimpleBlockComment))]
        [MemberData(nameof(MixedContentComment))]
        [MemberData(nameof(DeepContentComment))]
        public void GetText_ShouldConcatenateText_WhenCommentHasContents(Comment comment, string expected)
        {
            Check.That(comment.GetText()).IsEqualTo(expected);
        }
    }
}