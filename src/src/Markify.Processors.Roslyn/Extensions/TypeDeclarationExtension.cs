using System.Linq;
using System.Collections.Generic;
using Markify.Processors.Roslyn.Models;
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

        public static Fullname GetFullname(this BaseTypeDeclarationSyntax typeDeclaration)
        {
            Stack<SyntaxNode> parents = new Stack<SyntaxNode>();
            parents.Push(typeDeclaration);

            List<string> nameParts = new List<string>();
            while (parents.Count > 0)
            {
                SyntaxNode currentParent = parents.Pop();
                BaseTypeDeclarationSyntax parentType = currentParent as BaseTypeDeclarationSyntax;
                if (parentType != null)
                {
                    nameParts.Add(parentType.Identifier.ToString());
                    parents.Push(parentType.Parent);
                }
                else
                {
                    NamespaceDeclarationSyntax parentNamespace = currentParent as NamespaceDeclarationSyntax;
                    if (parentNamespace != null)
                        nameParts.Add(parentNamespace.Name.ToString());
                }
            }

            nameParts.Reverse();

            return new Fullname(nameParts.AsReadOnly());
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