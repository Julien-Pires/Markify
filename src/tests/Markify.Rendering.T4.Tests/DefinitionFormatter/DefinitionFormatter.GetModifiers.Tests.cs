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
        public void GetModifiers_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetModifiers(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ContainerDefinitionData("Foo", new string[0], null, null, "")]
        [ContainerDefinitionData("Foo", new[] { "sealed" }, null, null, "sealed")]
        [ContainerDefinitionData("Foo", new[] { "sealed", "abstract" }, null, null, "sealed, abstract")]
        public void GetModifiers_ShouldReturnCorrectValue(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetModifiers(definition)).IsEqualTo(expected);
        }
    }
}
