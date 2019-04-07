using Markify.CodeAnalyzer;
using Markify.Services.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Services.Rendering.T4.Tests
{
    public sealed partial class DefinitionFormatterTests
    {
        [Theory]
        [ClassDefinitionData(hasComments: true)]
        [InterfaceDefinitionData(hasComments: true)]
        [StructDefinitionData(hasComments: true)]
        [EnumDefinitionData(hasComments: true)]
        [DelegateDefinitionData(hasComments: true)]
        public void GetTypeComment_ShouldReturnSome_WhenCommentExist(TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetTypeComment(definition, c => c.Summary);

            Check.That(actual.IsSome()).IsTrue();
        }

        [Theory]
        [ClassDefinitionData]
        [InterfaceDefinitionData]
        [StructDefinitionData]
        [EnumDefinitionData]
        [DelegateDefinitionData]
        public void GetTypeComment_ShouldReturnNone_WhenCommentDoesNotExist(TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetTypeComment(definition, c => c.Summary);

            Check.That(actual.IsSome()).IsFalse();
        }
    }
}