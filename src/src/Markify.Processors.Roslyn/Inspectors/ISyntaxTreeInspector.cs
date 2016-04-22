using Microsoft.CodeAnalysis;

namespace Markify.Processors.Roslyn.Inspectors
{
    public interface ISyntaxTreeInspector
    {
        #region Methods

        RoslynContext Inspect(SyntaxTree tree);

        #endregion
    }
}