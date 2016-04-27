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
            List<StructureContainer> result = new List<StructureContainer>();
            IEnumerable<ClassDeclarationSyntax> classes = node.DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (ClassDeclarationSyntax classDeclaration in classes)
            {
                TypeRepresentation representation = new TypeRepresentation(classDeclaration.GetFullname(), StructureKind.Class)
                {
                    AccessModifiers = classDeclaration.GetAccessModifiers(),
                    Modifiers = classDeclaration.GetExtraModifiers(),
                    GenericParameters = _genericsInspector.Inspect(classDeclaration),
                    BaseTypes = classDeclaration.GetBaseTypes()
                };

                result.Add(new StructureContainer(representation));
            }

            return result.AsReadOnly();
        }

        #endregion
    }
}