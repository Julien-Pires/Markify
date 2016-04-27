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

        #region Type Naming

        public static Fullname GetFullname(this BaseTypeDeclarationSyntax typeDeclaration)
        {
            return GetNodeFullname(typeDeclaration);
        }

        private static Fullname GetNodeFullname(SyntaxNode node)
        {
            Stack<SyntaxNode> parents = new Stack<SyntaxNode>();
            parents.Push(node);

            List<string> nameParts = new List<string>();
            while (parents.Count > 0)
            {
                SyntaxNode currentParent = parents.Pop();
                NamespaceDeclarationSyntax parentNamespace = currentParent as NamespaceDeclarationSyntax;
                if(parentNamespace == null)
                {
                    ITypeDeclarationAdapter type = TypeDeclarationAdapterFactory.Create(currentParent);
                    if (type != null)
                    {
                        nameParts.Add(type.GetName());
                        parents.Push(currentParent.Parent);
                    }
                }
                else
                    nameParts.Add(parentNamespace.Name.ToString());
            }

            nameParts.Reverse();

            return new Fullname(nameParts.AsReadOnly());
        }

        internal static string GetName(this ITypeDeclarationAdapter typeAdapter)
        {
            return typeAdapter.ParameterList != null ? $"{typeAdapter.Name}'{typeAdapter.ParameterList.Parameters.Count}" : typeAdapter.Name;
        }

        #endregion

        #region Type Modifiers

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