using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using Ploeh.AutoFixture;

namespace Markify.Processors.Roslyn.Tests.Fixtures
{
    public class SyntaxTreeCustomization : ICustomization
    {
        #region Fields

        private readonly string _sourceFile;

        #endregion

        #region Constructors

        public SyntaxTreeCustomization(string sourceFile)
        {
            _sourceFile = sourceFile;
        }

        #endregion

        #region Customize

        public void Customize(IFixture fixture)
        {
            fixture.Register(() => CSharpSyntaxTree.ParseText(""));
        }

        #endregion
    }
}