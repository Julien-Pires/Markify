using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace Markify.Services.Rendering.T4
{
    internal static class MarkdownHelper
    {
        #region Fields

        private const string Blockquote = ">";

        private static readonly string[] NewLines = { "\r\n", "\n" };
        private static readonly Dictionary<string, string> SpecialCharacters = new Dictionary<string, string>
        {
            ["<"] = "&lt;",
            [">"] = "&gt;"
        };

        #endregion

        #region Methods

        public static string ToBlockquote(string text)
        {
            if(text == null)
                throw new ArgumentNullException(nameof(text));

            if(string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var lines = Regex.Split(text, @"(?<=[\n])");
            var builder = new StringBuilder();
            foreach (var line in lines)
                builder.Append(Blockquote).Append(line);

            return builder.ToString();
        }

        public static string EscapeString(string text)
        {
            if(text == null)
                throw new ArgumentNullException(nameof(text));

            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            foreach (var character in SpecialCharacters)
                text = text.Replace(character.Key, character.Value);

            return text;
        }

        #endregion
    }
}