using System.Linq;
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

        [Theory]
        [SyntaxTreeInlineAutoData("EmptySource.cs", 0)]
        [SyntaxTreeInlineAutoData("Class/SingleClass.cs", 1)]
        [SyntaxTreeInlineAutoData("Class/MultipleClass.cs", 2)]
        [SyntaxTreeInlineAutoData("Class/VariousContextClass.cs", 2)]
        [SyntaxTreeInlineAutoData("Class/NestedClass.cs", 2)]
        public void Inspect_WhenUsingSourceFile_WithSuccess(int count, ClassInspector inspector, SyntaxTree tree)
        {
            StructureContainer[] classes = inspector.Inspect(tree).ToArray();

            Assert.Equal(count, classes.Length);
            Assert.True(classes.All(c => c.Representation.Kind == StructureKind.Class));
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
            StructureContainer[] classes = inspector.Inspect(tree).ToArray();

            Assert.Equal(modifier, classes.First(c => c.Representation.Name == className).Representation.AccessModifiers);
        }

        #endregion

        #region Detect Modifier

        [Theory]
        [SyntaxTreeInlineAutoData("Class/AbstractClass.cs", "abstract")]
        [SyntaxTreeInlineAutoData("Class/SealedClass.cs", "sealed")]
        [SyntaxTreeInlineAutoData("Class/PartialClass.cs", "partial")]
        [SyntaxTreeInlineAutoData("Class/StaticClass.cs", "static")]
        public void Inspect_WhenClassHasSingleModifier_WithCorrectModifier(string modifier,
            ClassInspector inspector, SyntaxTree tree)
        {
            StructureContainer[] classes = inspector.Inspect(tree).ToArray();

            Assert.NotNull(classes.First().Representation.Modifiers.SingleOrDefault(c => c == modifier));
        }

        [Theory]
        [SyntaxTreeInlineAutoData("Class/AbstractPartialClass.cs", "abstract partial")]
        [SyntaxTreeInlineAutoData("Class/SealedPartialClass.cs", "sealed partial")]
        public void Inspect_WhenClassHasMultipleModifiers_WithCorrectModifiers(string modifier,
            ClassInspector inspector, SyntaxTree tree)
        {
            string[] modifiers = modifier.Split(' ');
            StructureContainer[] classes = inspector.Inspect(tree).ToArray();
            StructureContainer type = classes.First();

            Assert.True((modifiers.Length == type.Representation.Modifiers.Length) && 
                (modifiers.Intersect(type.Representation.Modifiers).Count() == modifiers.Length));
        }

        #endregion

        #region Names

        [Theory]
        [SyntaxTreeInlineAutoData("Class/AbstractClass.cs", "AbstractClass")]
        [SyntaxTreeInlineAutoData("Class/InternalClass.cs", "InternalClass")]
        public void Inspect_WithCorrectName(string name, ClassInspector inspector, SyntaxTree tree)
        {
            StructureContainer[] classes = inspector.Inspect(tree).ToArray();

            Assert.Equal(name, classes.First().Representation.Name);
        }

        [Theory]
        [SyntaxTreeInlineAutoData("Class/AbstractClass.cs", "AbstractClass")]
        [SyntaxTreeInlineAutoData("Class/ProtectedClass.cs", "ProtectedFoo.ProtectedClass")]
        [SyntaxTreeInlineAutoData("Class/VariousContextClass.cs", "FooSpace.InsideNamespaceClass")]
        public void Inspect_WithCorrectFullname(string fullname, ClassInspector inspector, SyntaxTree tree)
        {
            StructureContainer[] classes = inspector.Inspect(tree).ToArray();

            Assert.NotNull(classes.SingleOrDefault(c => c.Fullname == fullname));
        }

        #endregion

        #endregion
    }
}