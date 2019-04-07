using System;
using Markify.CodeAnalyzer;
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
        [ClassDefinitionData(values: new object[]{ "internal" })]
        public void GetAccessModifiers_ShouldReturnDefaultModifier_WhenHasNoAccessModifiers(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetAccessModifiers(definition)).IsEqualTo(expected);
        }

        [Theory]
        [ClassDefinitionData(accessModifiers: new[] { "public" }, values: new object[] { "public" })]
        [ClassDefinitionData(accessModifiers: new[] { "protected", "internal" }, values: new object[] { "protected internal" })]
        public void GetAccessModifiers_ShouldReturnCorrectModifiers(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetAccessModifiers(definition)).IsEqualTo(expected);
        }
    }
}
