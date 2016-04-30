using System;
using System.Linq;

using Markify.Processors.Roslyn.Models;
using Markify.Processors.Roslyn.Inspectors;
using Markify.Processors.Roslyn.Tests.Fixtures;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Xunit;

namespace Markify.Processors.Roslyn.Tests.Inspectors
{
    public partial class GenericParameterInspector_Test
    {
        #region Helpers

        private static SyntaxNode GetTypeNode(SyntaxNode root)
        {
            return root.DescendantNodes().First(c => c is BaseTypeDeclarationSyntax || c is DelegateDeclarationSyntax);
        }

        #endregion

        #region Detect Parameters

        

        [Theory]
        [SyntaxTreeInlineAutoData("Generics/GenericClass.cs", "T")]
        [SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", "T")]
        [SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", "Y")]
        public void Inspect_WithCorrectName(string name, ISyntaxTreeInspector<GenericParameterRepresentation> inspector,
            SyntaxTree tree)
        {
            var node = GetTypeNode(tree.GetRoot());
            GenericParameterRepresentation[] generics = inspector.Inspect(node).ToArray();

            Assert.NotNull(generics.SingleOrDefault(c => c.Fullname == name));
        }

        #endregion

        #region Detect Modifiers

        [Theory]
        [SyntaxTreeInlineAutoData("Generics/GenericClass.cs", null, "T")]
        [SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", "out", "Y")]
        [SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", "in", "T")]
        public void Inspect_WhenHasModifier_WithExactModifiers(string modifier, string parameter,
            ISyntaxTreeInspector<GenericParameterRepresentation> inspector, SyntaxTree tree)
        {
            var node = GetTypeNode(tree.GetRoot());
            GenericParameterRepresentation[] generics = inspector.Inspect(node).ToArray();

            Assert.Equal(modifier, generics.Single(c => c.Fullname == parameter).Modifier);
        }

        #endregion

        #region Detect Constraints

        [Theory]
        [SyntaxTreeInlineAutoData("Generics/GenericClass.cs", 2, "T")]
        [SyntaxTreeInlineAutoData("Generics/GenericDelegate.cs", 3, "T")]
        [SyntaxTreeInlineAutoData("Generics/GenericInterface.cs", 0, "T")]
        [SyntaxTreeInlineAutoData("Generics/GenericStruct.cs", 1, "T")]
        public void Inspect_WhenHasConstraints_WithAllConstraints(int count, string parameter,
            ISyntaxTreeInspector<GenericParameterRepresentation> inspector, SyntaxTree tree)
        {
            var node = GetTypeNode(tree.GetRoot());
            GenericParameterRepresentation[] generics = inspector.Inspect(node).ToArray();

            Assert.Equal(count, generics.Single(c => c.Fullname == parameter).Constraints.Count());
        }

        #endregion
    }
}