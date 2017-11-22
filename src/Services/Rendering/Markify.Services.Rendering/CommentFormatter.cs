using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markify.Domain.Compiler;

namespace Markify.Services.Rendering
{
    public static class CommentFormatter
    {
        #region Methods

        public static string GetText(this Comment comment)
        {
            if(comment == null)
                throw new ArgumentNullException(nameof(comment));

            var result = new StringBuilder();
            var contents = new List<CommentContent>(comment.Content.Reverse());
            while (contents.Any())
            {
                var current = contents.Last();
                contents.RemoveAt(contents.Count -1);

                switch (current)
                {
                    case CommentContent.Block block:
                        contents.AddRange(block.Item.Content.Reverse());
                        break;
                    case CommentContent.Text text:
                        result.Append(text.Item);
                        break;
                }
            }

            return result.ToString();
        }

        #endregion
    }
}