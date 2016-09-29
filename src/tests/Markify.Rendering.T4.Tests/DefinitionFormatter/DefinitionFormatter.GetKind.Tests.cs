using System;
using Markify.Models.Definitions;
using Markify.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Rendering.T4.Tests
{
    public sealed partial class DefinitionFormatterTests
    {
        [Fact]
        public void GetKind_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetKind(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [TypeDefinitionData("Foo", null, null, StructureKind.Class, null, "class")]
        [TypeDefinitionData("Foo", null, null, StructureKind.Interface, null, "interface")]
        [TypeDefinitionData("Foo", null, null, StructureKind.Struct, null, "struct")]
        [TypeDefinitionData("Foo", null, null, StructureKind.Delegate, null, "delegate")]
        [TypeDefinitionData("Foo", null, null, StructureKind.Enum, null, "enum")]
        public void GetKind_ShouldReturnCorrectKeyword(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetKind(definition)).IsEqualTo(expected);
        }
    }
}