using Ploeh.AutoFixture.Xunit2;

namespace Markify.Processors.Roslyn.Tests.Fixtures
{
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