using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Markify.Processors.Roslyn.Extensions
{
    internal sealed class DelegateDeclarationAdapter : ITypeDeclarationAdapter
    {
        #region Fields

        private readonly DelegateDeclarationSyntax _node;

        #endregion

        #region Properties

        public string Name => _node.Identifier.ToString();

        public TypeParameterListSyntax ParameterList => _node.TypeParameterList;

        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => _node.ConstraintClauses;

        #endregion

        #region Constructors

        public DelegateDeclarationAdapter(DelegateDeclarationSyntax node)
        {
            _node = node;
        }

        #endregion
    }
}
