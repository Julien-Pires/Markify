using System;
using Markify.Models.Definitions;
using Markify.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Rendering.T4.Tests
{
    public sealed partial class DefinitionFormatter_Tests
    {
        [Fact]
        public void GetNamespace_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetNamespace(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [DefinitionData("Foo", null, null, StructureKind.Class, null)]
        public void GetNamespace_ShouldReturnEmpty_WhenHasNoNamespace(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNamespace(definition)).IsEqualTo(string.Empty);
        }

        [Theory]
        [DefinitionData("Foo", null, "Foospace", StructureKind.Class, null, "Foospace")]
        [DefinitionData("Foo", null, "Foospace.Inner", StructureKind.Class, null, "Foospace.Inner")]
        public void GetNamespace_ShouldReturnCorrectValue_WhenHasNamespace(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNamespace(definition)).IsEqualTo(expected);
        }
    }
}
