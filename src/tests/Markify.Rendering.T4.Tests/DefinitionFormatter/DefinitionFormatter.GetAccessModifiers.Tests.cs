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
        public void GetAccessModifiers_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetAccessModifiers(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [TypeDefinitionData("Foo", null, new string[] { }, null, "internal")]
        public void GetAccessModifiers_ShouldReturnDefaultModifier_WhenHasNoAccessModifiers(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetAccessModifiers(definition)).IsEqualTo(expected);
        }

        [Theory]
        [TypeDefinitionData("Foo", null, new string[] { "public" }, null, "public")]
        [TypeDefinitionData("Foo", null, new string[] { "protected", "internal" }, null, "protected internal")]
        public void GetAccessModifiers_ShouldReturnCorrectModifiers(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetAccessModifiers(definition)).IsEqualTo(expected);
        }
    }
}
