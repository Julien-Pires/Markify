using System;

using Ploeh.AutoFixture.Xunit2;

namespace Markify.Fixtures
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SyntaxTreeInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public SyntaxTreeInlineAutoDataAttribute(string sourceFile, params object[] values) 
            : base(new SyntaxTreeAutoDataAttribute(sourceFile), values)
        {
        }

        #endregion
    }
}