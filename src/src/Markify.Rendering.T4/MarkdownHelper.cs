using System;
using System.Collections.Generic;

namespace Markify.Rendering.T4
{
    internal static class MarkdownHelper
    {
        #region Fields

        private static readonly Dictionary<string, string> SpecialCharacters = new Dictionary<string, string>
        {
            ["<"] = "&lt;",
            [">"] = "&gt;"
        };

        #endregion

        #region Methods

        public static string EscapeString(string text)
        {
            if(text == null)
                throw new ArgumentNullException(nameof(text));

            if (string.IsNullOrEmpty(text))
                return string.Empty;

            foreach (var character in SpecialCharacters)
                text = text.Replace(character.Key, character.Value);

            return text;
        }

        #endregion
    }
}