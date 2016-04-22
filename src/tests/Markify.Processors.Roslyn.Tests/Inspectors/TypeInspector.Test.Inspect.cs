using Markify.Processors.Roslyn.Inspectors;
using Markify.Processors.Roslyn.Tests.Fixtures;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Markify.Processors.Roslyn.Tests.Inspectors
{
    public partial class TypeInspector_Test
    {
        #region Class Inspection

        [Theory, SyntaxTreeAutoData("")]
        public void Inspect_WhenEmptySource_WithSuccess(TypeInspector inspector, SyntaxTree tree)
        {
            RoslynContext context = inspector.Inspect(tree);

            Assert.Equal(0, context.Types.Count);
        }

        #endregion
    }
}