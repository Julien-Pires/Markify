using System.Linq;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Markify.Processors.Roslyn.Extensions
{
    public static class TypeDeclarationExtension
    {
        #region Fields

        private static readonly SyntaxKind[] AccessModifiers =
        {
            SyntaxKind.PublicKeyword,
            SyntaxKind.ProtectedKeyword,
            SyntaxKind.InternalKeyword,
            SyntaxKind.PrivateKeyword
        };

        #endregion

        #region Type Declaration Extension

        public static string GetFullname(this BaseTypeDeclarationSyntax typeDeclaration)
        {
            string fullname = typeDeclaration.Identifier.ToString();
            if (typeDeclaration.Parent == null)
                return fullname;

            Stack<SyntaxNode> parents = new Stack<SyntaxNode>();
            parents.Push(typeDeclaration.Parent);
            while (parents.Count > 0)
            {
                SyntaxNode currentParent = parents.Pop();
                BaseTypeDeclarationSyntax parentType = currentParent as BaseTypeDeclarationSyntax;
                if (parentType != null)
                {
                    fullname = $"{parentType.Identifier}.{fullname}";
                    parents.Push(parentType.Parent);
                }
                else
                {
                    NamespaceDeclarationSyntax parentNamespace = currentParent as NamespaceDeclarationSyntax;
                    if (parentNamespace != null)
                        fullname = $"{parentNamespace.Name}.{fullname}";
                }
            }

            return fullname;
        }

        public static IEnumerable<string> GetAccessModifiers(this BaseTypeDeclarationSyntax typeDeclaration)
        {
            return typeDeclaration.Modifiers.Where(c => AccessModifiers.Contains(c.Kind())).Select(c => c.ToString());
        }

        public static IEnumerable<string> GetExtraModifiers(this BaseTypeDeclarationSyntax typeDeclaration)
        {
            return typeDeclaration.Modifiers.Where(c => !AccessModifiers.Contains(c.Kind())).Select(c => c.ToString());
        }

        #endregion
    }
}