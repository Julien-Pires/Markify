using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Markify.Processors.Roslyn.Extensions
{
    internal interface ITypeDeclarationAdapter
    {
        #region Properties

        string Name { get; }

        TypeParameterListSyntax ParameterList { get; }

        SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses { get; }

        SyntaxTokenList Modifiers { get; }

        #endregion
    }
}
