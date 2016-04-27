using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Processors.Roslyn.Tests.Fixtures
{
    public class SyntaxTreeAutoDataAttribute : AutoDataAttribute
    {
        #region Constructors

        public SyntaxTreeAutoDataAttribute() : this(null)
        {
        }

        public SyntaxTreeAutoDataAttribute(string sourceFile) 
            : base(new Fixture().Customize(new SyntaxTreeCustomization(sourceFile)))
        {
        }

        #endregion
    }
}