using System.Linq;
using System.Collections.Generic;
using Markify.Models.Definitions;
using Markify.Processors.Roslyn.Inspectors;
using Markify.Processors.Roslyn.Models;
using Markify.Processors.Roslyn.Tests.Fixtures;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Markify.Processors.Roslyn.Tests.Inspectors
{
    public partial class ClassInspector_Test
    {
        #region Class Inspection

        #region Detect Classes

        [Theory, SyntaxTreeAutoData("EmptySource.cs")]
        public void Inspect_WhenEmptySource_WithNoResult(ClassInspector inspector, SyntaxTree tree)
        {
            IEnumerable<TypeRepresentation> classes = inspector.Inspect(tree);

            Assert.Equal(0, classes.Count());
        }

        [Theory, SyntaxTreeAutoData("Class/SingleClass.cs")]
        public void Inspect_WhenSourceHasOneClass_WithOneResult(ClassInspector inspector, SyntaxTree tree)
        {
            TypeRepresentation[] classes = inspector.Inspect(tree).ToArray();

            Assert.Equal(1, classes.Length);
            Assert.True(classes.All(c => c.Kind == StructureKind.Class));
        }

        [Theory]
        [SyntaxTreeInlineAutoData("Class/MultipleClass.cs", 2)]
        public void Inspect_WhenSourceHasMultipleClass_WithResults(int count, ClassInspector inspector, SyntaxTree tree)
        {
            TypeRepresentation[] classes = inspector.Inspect(tree).ToArray();

            Assert.Equal(count, classes.Length);
            Assert.True(classes.All(c => c.Kind == StructureKind.Class));
        }

        [Theory]
        [SyntaxTreeInlineAutoData("Class/VariousContextClass.cs", 2)]
        public void Inspect_WhenClassContextIsDifferent_WithResults(int count, ClassInspector inspector, SyntaxTree tree)
        {
            TypeRepresentation[] classes = inspector.Inspect(tree).ToArray();

            Assert.Equal(count, classes.Length);
            Assert.True(classes.All(c => c.Kind == StructureKind.Class));
        }

        [Theory]
        [SyntaxTreeInlineAutoData("Class/NestedClass.cs", 2)]
        public void Inspect_WhenClassIsNested_WithResults(int count, ClassInspector inspector, SyntaxTree tree)
        {
            TypeRepresentation[] classes = inspector.Inspect(tree).ToArray();

            Assert.Equal(count, classes.Length);
            Assert.True(classes.All(c => c.Kind == StructureKind.Class));
        }

        #endregion

        #region Detect Access Modifier

        [Theory]
        [SyntaxTreeInlineAutoData("Class/SingleClass.cs", "public", "SingleClassWithoutNamespace")]
        [SyntaxTreeInlineAutoData("Class/InternalClass.cs", "internal", "InternalClass")]
        [SyntaxTreeInlineAutoData("Class/ProtectedClass.cs", "protected", "ProtectedClass")]
        [SyntaxTreeInlineAutoData("Class/ProtectedInternalClass.cs", "protected internal", "ProtectedInternalClass")]
        [SyntaxTreeInlineAutoData("Class/PrivateClass.cs", "private", "PrivateClass")]
        public void Inspect_WhenClassHasAccessModifier_WithCorrectAccessModifier(string modifier, string className,
            ClassInspector inspector, SyntaxTree tree)
        {
            TypeRepresentation[] classes = inspector.Inspect(tree).ToArray();

            Assert.Equal(modifier, classes.First(c => c.Name == className).AccessModifiers);
        }

        #endregion

        #region Detect Modifier

        [Theory]
        [SyntaxTreeInlineAutoData("Class/AbstractClass.cs", "abstract")]
        [SyntaxTreeInlineAutoData("Class/SealedClass.cs", "sealed")]
        [SyntaxTreeInlineAutoData("Class/PartialClass.cs", "partial")]
        [SyntaxTreeInlineAutoData("Class/StaticClass.cs", "static")]
        public void Inspect_WhenClasHasModifiers_WithCorrectModifier(string modifier,
            ClassInspector inspector, SyntaxTree tree)
        {
            TypeRepresentation[] classes = inspector.Inspect(tree).ToArray();

            Assert.NotNull(classes.First().Modifiers.SingleOrDefault(c => c == modifier));
        }

        [Theory]
        [SyntaxTreeInlineAutoData("Class/AbstractPartialClass.cs", "abstract partial")]
        [SyntaxTreeInlineAutoData("Class/SealedPartialClass.cs", "sealed partial")]
        public void Inspect_WhenClassHasMultipleModifiers_WithCorrectModifiers(string modifier,
            ClassInspector inspector, SyntaxTree tree)
        {
            string[] modifiers = modifier.Split(' ');
            TypeRepresentation[] classes = inspector.Inspect(tree).ToArray();
            TypeRepresentation type = classes.First();

            Assert.True((modifiers.Length == type.Modifiers.Length) && 
                (modifiers.Intersect(type.Modifiers).Count() == modifiers.Length));
        }

        #endregion

        #endregion
    }
}