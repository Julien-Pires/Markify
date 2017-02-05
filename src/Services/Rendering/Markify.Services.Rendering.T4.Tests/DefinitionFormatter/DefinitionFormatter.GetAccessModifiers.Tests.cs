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
        public void GetAccessModifiers_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetAccessModifiers(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ContainerDefinitionData("Foo", null, new string[] { }, null, "internal")]
        public void GetAccessModifiers_ShouldReturnDefaultModifier_WhenHasNoAccessModifiers(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetAccessModifiers(definition)).IsEqualTo(expected);
        }

        [Theory]
        [ContainerDefinitionData("Foo", null, new[] { "public" }, null, "public")]
        [ContainerDefinitionData("Foo", null, new[] { "protected", "internal" }, null, "protected internal")]
        public void GetAccessModifiers_ShouldReturnCorrectModifiers(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetAccessModifiers(definition)).IsEqualTo(expected);
        }
    }
}
