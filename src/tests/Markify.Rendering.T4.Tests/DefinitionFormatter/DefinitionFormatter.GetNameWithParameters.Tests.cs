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
        public void GetNameWithParameters_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetNameWithParameters(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [DefinitionData("Foo", null, null, StructureKind.Class, new string[] { }, "Foo")]
        public void GetNameWithParameters_ShouldReturnOnlyName_WhenHasNoParameters(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNameWithParameters(definition)).IsEqualTo(expected);
        }

        [Theory]
        [DefinitionData("Foo", null, null, StructureKind.Class, new string[] { "T" }, "Foo<T>")]
        [DefinitionData("Foo", null, null, StructureKind.Class, new string[] { "T", "Y", "Z" }, "Foo<T, Y, Z>")]
        public void GetNameWithParameters_ShouldReturnNameWithParameters_WhenHasParamters(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNameWithParameters(definition)).IsEqualTo(expected);
        }
    }
}
