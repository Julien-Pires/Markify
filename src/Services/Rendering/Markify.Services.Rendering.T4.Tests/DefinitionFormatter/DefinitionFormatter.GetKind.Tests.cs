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
        public void GetKind_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetKind(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ClassDefinitionData(values: new object[] { "class" })]
        [InterfaceDefinitionData(values: new object[] { "interface" })]
        [StructDefinitionData(values: new object[] { "struct" })]
        [DelegateDefinitionData(values: new object[] { "delegate" })]
        [EnumDefinitionData(values: new object[] { "enum" })]
        public void GetKind_ShouldReturnCorrectTypeKeyword(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetKind(definition)).IsEqualTo(expected);
        }
    }
}