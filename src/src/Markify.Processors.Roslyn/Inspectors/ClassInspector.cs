using System.Linq;
using System.Collections.Generic;

using Markify.Models.Definitions;
using Markify.Processors.Roslyn.Models;
using Markify.Processors.Roslyn.Extensions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Markify.Processors.Roslyn.Inspectors
{
    public sealed class ClassInspector : ISyntaxTreeInspector<StructureContainer>
    {
        #region Inspect Methods

        public IEnumerable<StructureContainer> Inspect(SyntaxNode node)
        {
            var classes = node.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var classDeclaration in classes)
            {
                var representation = new TypeRepresentation(classDeclaration.GetFullname(),
                    classDeclaration.Identifier.ToString(), StructureKind.Class)
                {
                    AccessModifiers = string.Join(" ", classDeclaration.GetAccessModifiers()),
                    Modifiers = classDeclaration.GetExtraModifiers().ToArray()
                };

                yield return new StructureContainer(representation);
            }
        }

        #endregion
    }
}