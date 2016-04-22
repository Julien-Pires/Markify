using System.Linq;
using System.Collections.Generic;
using Markify.Processors.Roslyn.Inspectors;
using Markify.Processors.Roslyn.Models;
using Markify.Processors.Roslyn.Tests.Fixtures;
using Microsoft.CodeAnalysis;
using Ploeh.AutoFixture.Xunit2;
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
            IEnumerable<TypeRepresentation> classes = inspector.Inspect(tree);

            Assert.Equal(1, classes.Count());
        }

        [Theory]
        [SyntaxTreeInlineAutoData("Class/MultipleClass.cs", 2)]
        public void Inspect_WhenSourceHasMultipleClass_WithResults(int count, ClassInspector inspector, SyntaxTree tree)
        {
            IEnumerable<TypeRepresentation> classes = inspector.Inspect(tree);

            Assert.Equal(count, classes.Count());
        }

        [Theory]
        [SyntaxTreeInlineAutoData("Class/VariousContextClass.cs", 2)]
        public void Inspect_WhenClassContextIsDifferent_WithResults(int count, ClassInspector inspector, SyntaxTree tree)
        {
            IEnumerable<TypeRepresentation> classes = inspector.Inspect(tree);

            Assert.Equal(count, classes.Count());
        }

        [Theory]
        [SyntaxTreeInlineAutoData("Class/NestedClass.cs", 2)]
        public void Inspect_WhenClassIsNested_WithResults(int count, ClassInspector inspector, SyntaxTree tree)
        {
            IEnumerable<TypeRepresentation> classes = inspector.Inspect(tree);

            Assert.Equal(count, classes.Count());
        }

        #endregion

        #endregion
    }
}