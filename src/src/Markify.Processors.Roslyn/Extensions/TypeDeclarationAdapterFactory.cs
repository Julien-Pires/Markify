using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Markify.Processors.Roslyn.Extensions
{
    internal static class TypeDeclarationAdapterFactory
    {
        #region Factory

        public static ITypeDeclarationAdapter Create(SyntaxNode node)
        {
            TypeDeclarationSyntax typeNode = node as TypeDeclarationSyntax;
            if (typeNode != null)
                return new TypeDeclarationAdapter(typeNode);

            DelegateDeclarationSyntax delegateNode = node as DelegateDeclarationSyntax;
            if (delegateNode != null)
                return new DelegateDeclarationAdapter(delegateNode);

            return null;
        }

        #endregion
    }
}
