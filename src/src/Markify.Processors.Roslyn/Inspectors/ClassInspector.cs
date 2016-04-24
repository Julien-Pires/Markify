using System.Linq;
using System.Collections.Generic;

using Markify.Models.Definitions;
using Markify.Processors.Roslyn.Models;
using Markify.Processors.Roslyn.Extensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Markify.Processors.Roslyn.Inspectors
{
    public sealed class ClassInspector : ISyntaxTreeInspector<TypeRepresentation>
    {
        #region Inspect Methods

        public IEnumerable<TypeRepresentation> Inspect(SyntaxTree tree)
        {
            var root = tree.GetRoot();
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var classDeclaration in classes)
            {
                yield return new TypeRepresentation(classDeclaration.GetFullname(),
                    classDeclaration.Identifier.ToString(), StructureKind.Class)
                {
                    AccessModifiers = string.Join(" ", classDeclaration.GetAccessModifiers()),
                    Modifiers = classDeclaration.GetExtraModifiers().ToArray()
                };
            }
        }

        #endregion
    }
}