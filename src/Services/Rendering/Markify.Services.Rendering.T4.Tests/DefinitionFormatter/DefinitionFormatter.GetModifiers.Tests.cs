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
        public void GetModifiers_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetModifiers(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ClassDefinitionData]
        [InterfaceDefinitionData]
        [StructDefinitionData]
        [EnumDefinitionData]
        [DelegateDefinitionData]
        public void GetModifiers_ShouldReturnEmpty_WhenTypeHasNoModifiers(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetModifiers(definition)).IsEmpty();
        }

        [Theory]
        [ClassDefinitionData(modifiers: new[] { "sealed" }, values: new object[]{ "sealed" })]
        [InterfaceDefinitionData(modifiers: new[] { "sealed" }, values: new object[] { "sealed" })]
        [StructDefinitionData(modifiers: new[] { "sealed" }, values: new object[] { "sealed" })]
        [DelegateDefinitionData(modifiers: new[] { "sealed" }, values: new object[] { "sealed" })]
        [ClassDefinitionData(modifiers: new[] { "sealed", "abstract" }, values: new object[] { "sealed, abstract" })]
        [InterfaceDefinitionData(modifiers: new[] { "sealed", "abstract" }, values: new object[] { "sealed, abstract" })]
        [StructDefinitionData(modifiers: new[] { "sealed", "abstract" }, values: new object[] { "sealed, abstract" })]
        [DelegateDefinitionData(modifiers: new[] { "sealed", "abstract" }, values: new object[] { "sealed, abstract" })]
        public void GetModifiers_ShouldReturnCorrectValue(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetModifiers(definition)).IsEqualTo(expected);
        }
    }
}
