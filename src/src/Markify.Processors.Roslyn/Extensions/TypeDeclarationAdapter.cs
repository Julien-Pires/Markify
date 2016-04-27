using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Markify.Processors.Roslyn.Extensions
{
    internal sealed class TypeDeclarationAdapter : ITypeDeclarationAdapter
    {
        #region Fields

        private readonly TypeDeclarationSyntax _node;

        #endregion

        #region Properties

        public string Name => _node.Identifier.ToString();

        public TypeParameterListSyntax ParameterList => _node.TypeParameterList;

        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => _node.ConstraintClauses;

        #endregion

        #region Constructors

        public TypeDeclarationAdapter(TypeDeclarationSyntax node)
        {
            _node = node;
        }

        #endregion
    }
}
