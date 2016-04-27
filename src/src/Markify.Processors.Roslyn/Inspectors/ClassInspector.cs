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
        #region Fields

        private readonly ISyntaxTreeInspector<GenericParameterRepresentation> _genericsInspector;

        #endregion

        #region Constructors

        public ClassInspector(ISyntaxTreeInspector<GenericParameterRepresentation> genericsInspector)
        {
            _genericsInspector = genericsInspector;
        }

        #endregion

        #region Inspect Methods

        public IEnumerable<StructureContainer> Inspect(SyntaxNode node)
        {
            IEnumerable<ClassDeclarationSyntax> classes = node.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (ClassDeclarationSyntax classDeclaration in classes)
            {
                TypeRepresentation representation = new TypeRepresentation(classDeclaration.GetFullname(), StructureKind.Class)
                {
                    AccessModifiers = string.Join(" ", classDeclaration.GetAccessModifiers()),
                    Modifiers = classDeclaration.GetExtraModifiers().ToArray(),
                    GenericParameters = _genericsInspector.Inspect(classDeclaration)
                };

                yield return new StructureContainer(representation);
            }
        }

        #endregion
    }
}