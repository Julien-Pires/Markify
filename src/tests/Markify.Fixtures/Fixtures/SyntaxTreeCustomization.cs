using System.IO;

using Microsoft.CodeAnalysis.CSharp;

using Ploeh.AutoFixture;

namespace Markify.Fixtures
{
    public class SyntaxTreeCustomization : ICustomization
    {
        #region Fields

        private readonly string _sourceFile;

        #endregion

        #region Constructors

        public SyntaxTreeCustomization(string sourceFile)
        {
            if(sourceFile != null)
                _sourceFile = Path.Combine(SourceHelper.SourceFolder, sourceFile);
        }

        #endregion

        #region Customize

        public void Customize(IFixture fixture)
        {
            fixture.Register(() => 
            {
                if (_sourceFile != null)
                    return CSharpSyntaxTree.ParseText(File.ReadAllText(_sourceFile));
                else
                    return CSharpSyntaxTree.Create(SyntaxFactory.EmptyStatement());
            });
        }

        #endregion
    }
}