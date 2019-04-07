using System.Collections.Generic;
using System.Linq;
using Markify.CodeAnalyzer;

namespace Markify.Services.Rendering.Tests
{
    public sealed partial class CommentFormatterTests
    {
        #region Fixtures

        public static IEnumerable<object[]> SimpleTextComment
        {
            get
            {
                yield return new object[]
                {
                    new Comment(
                        "Summary",
                        new []{ CommentContent.NewText("Foo"), CommentContent.NewText("Bar") },
                        Enumerable.Empty<CommentParameter>()
                    ),
                    "FooBar"
                };
            }
        }

        public static IEnumerable<object[]> SimpleBlockComment
        {
            get
            {
                yield return new object[]
                {
                    new Comment(
                        "Summary",
                        new []
                        {
                            CommentContent.NewBlock(
                                new Comment(
                                    "Code",
                                    new []{ CommentContent.NewText("Foo") },
                                    Enumerable.Empty<CommentParameter>()
                                )
                            ),
                            CommentContent.NewBlock(
                                new Comment(
                                    "Code",
                                    new []{ CommentContent.NewText("Bar") },
                                    Enumerable.Empty<CommentParameter>()
                                )
                            )
                        },
                        Enumerable.Empty<CommentParameter>()
                    ),
                    "FooBar"
                };
            }
        }

        public static IEnumerable<object[]> MixedContentComment
        {
            get
            {
                yield return new object[]
                {
                    new Comment(
                        "Summary",
                        new []
                        {
                            CommentContent.NewBlock(
                                new Comment(
                                    "Code",
                                    new []{ CommentContent.NewText("Foo") },
                                    Enumerable.Empty<CommentParameter>()
                                )
                            ),
                            CommentContent.NewText("Bar"), 
                        },
                        Enumerable.Empty<CommentParameter>()
                    ),
                    "FooBar"
                };
            }
        }

        public static IEnumerable<object[]> DeepContentComment
        {
            get
            {
                yield return new object[]
                {
                    new Comment(
                        "Summary",
                        new []
                        {
                            CommentContent.NewBlock(
                                new Comment(
                                    "Code",
                                    new []
                                    {
                                        CommentContent.NewText("Foo"), 
                                        CommentContent.NewBlock(
                                            new Comment(
                                                "Code",
                                                new []
                                                {
                                                    CommentContent.NewText("Bar")
                                                },
                                                Enumerable.Empty<CommentParameter>()
                                            )
                                        )
                                    },
                                    Enumerable.Empty<CommentParameter>()
                                )
                            ),
                            CommentContent.NewText("!"),
                        },
                        Enumerable.Empty<CommentParameter>()
                    ),
                    "FooBar!"
                };
            }
        }

        #endregion
    }
}