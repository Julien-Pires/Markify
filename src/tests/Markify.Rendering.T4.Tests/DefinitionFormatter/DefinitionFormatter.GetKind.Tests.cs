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
        public void GetKind_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetKind(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [DefinitionData("Foo", null, null, StructureKind.Class, null, "class")]
        [DefinitionData("Foo", null, null, StructureKind.Interface, null, "interface")]
        [DefinitionData("Foo", null, null, StructureKind.Struct, null, "struct")]
        [DefinitionData("Foo", null, null, StructureKind.Delegate, null, "delegate")]
        [DefinitionData("Foo", null, null, StructureKind.Enum, null, "enum")]
        public void GetKind_ShouldReturnCorrectKeyword_WhenKindIsKnown(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetKind(definition)).IsEqualTo(expected);
        }

        [Theory]
        [DefinitionData("Foo", null, null, StructureKind.Unknown, null)]
        public void GetKind_ShouldReturnEmpty_WhenKindIsUnknown(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetKind(definition)).IsEqualTo(string.Empty);
        }
    }
}