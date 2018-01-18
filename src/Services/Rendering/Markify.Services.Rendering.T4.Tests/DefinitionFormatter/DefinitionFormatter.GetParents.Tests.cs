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
        public void GetParents_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetParents(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ClassDefinitionData]
        [InterfaceDefinitionData]
        [StructDefinitionData]
        [EnumDefinitionData]
        public void GetParents_ShouldReturnEmptyString_WhenTypeHasNoBaseType(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetParents(definition)).IsEmpty();
        }

        [Theory]
        [ClassDefinitionData(baseTypes: new []{ "IDisposable" }, values: new object[] { "IDisposable" })]
        [InterfaceDefinitionData(baseTypes: new [] { "IDisposable" }, values: new object[] { "IDisposable" })]
        [StructDefinitionData(baseTypes: new [] { "IDisposable" }, values: new object[] { "IDisposable" })]
        [EnumDefinitionData(baseTypes: new [] { "byte" }, values: new object[] { "byte" })]
        [ClassDefinitionData(baseTypes: new[] { "IDisposable", "IEnumerable" }, values: new object[] { "IDisposable, IEnumerable" })]
        [InterfaceDefinitionData(baseTypes: new[] { "IDisposable", "IEnumerable" }, values: new object[] { "IDisposable, IEnumerable" })]
        [StructDefinitionData(baseTypes: new[] { "IDisposable", "IEnumerable" }, values: new object[] { "IDisposable, IEnumerable" })]
        public void GetParents_ShouldReturnCorrectValue(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetParents(definition)).IsEqualTo(expected);
        }
    }
}
