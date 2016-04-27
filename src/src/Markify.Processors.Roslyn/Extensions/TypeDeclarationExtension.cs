using System.Collections.Generic;

using Markify.Processors.Roslyn.Models;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Markify.Processors.Roslyn.Extensions
{
    public static class TypeDeclarationExtension
    {
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
                    if (type == null)
                        continue;

                    nameParts.Add(type.GetName());
                    parents.Push(currentParent.Parent);
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

        public static IEnumerable<string> GetAccessModifiers(this BaseTypeDeclarationSyntax typeNode)
        {
            return ModifiersFactory.GetAccessModifiers(TypeDeclarationAdapterFactory.Create(typeNode));
        }

        public static IEnumerable<string> GetExtraModifiers(this BaseTypeDeclarationSyntax typeNode)
        {
            return ModifiersFactory.GetExtraModifiers(TypeDeclarationAdapterFactory.Create(typeNode));
        }

        #endregion
    }
}