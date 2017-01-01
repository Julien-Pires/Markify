using System;
using Markify.Domain.Compiler;
using Markify.Services.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Services.Rendering.T4.Tests
{
    public sealed partial class DefinitionFormatterTests
    {
        [Fact]
        public void GetNameWithParameters_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetNameWithParameters(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ContainerDefinitionData("Foo", null, null, StructureKind.Class, new string[] { }, "Foo")]
        public void GetNameWithParameters_ShouldReturnOnlyName_WhenHasNoParameters(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNameWithParameters(definition.Identity)).IsEqualTo(expected);
        }

        [Theory]
        [ContainerDefinitionData("Foo", null, null, StructureKind.Class, new[] { "T" }, "Foo<T>")]
        [ContainerDefinitionData("Foo", null, null, StructureKind.Class, new[] { "T", "Y", "Z" }, "Foo<T, Y, Z>")]
        public void GetNameWithParameters_ShouldReturnNameWithParameters_WhenHasParameters(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNameWithParameters(definition.Identity)).IsEqualTo(expected);
        }
    }
}
