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
        public void GetNameWithParameters_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetNameWithParameters(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [TypeDefinitionData("Foo", null, null, StructureKind.Class, new string[] { }, "Foo")]
        public void GetNameWithParameters_ShouldReturnOnlyName_WhenHasNoParameters(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNameWithParameters(definition)).IsEqualTo(expected);
        }

        [Theory]
        [TypeDefinitionData("Foo", null, null, StructureKind.Class, new[] { "T" }, "Foo<T>")]
        [TypeDefinitionData("Foo", null, null, StructureKind.Class, new[] { "T", "Y", "Z" }, "Foo<T, Y, Z>")]
        public void GetNameWithParameters_ShouldReturnNameWithParameters_WhenHasParamters(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNameWithParameters(definition)).IsEqualTo(expected);
        }
    }
}
