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
        public void GetNamespace_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetNamespace(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ClassDefinitionData]
        [InterfaceDefinitionData]
        [StructDefinitionData]
        [EnumDefinitionData]
        [DelegateDefinitionData]
        public void GetNamespace_ShouldReturnEmpty_WhenHasNoNamespace(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNamespace(definition)).IsEqualTo(string.Empty);
        }

        [Theory]
        [ClassDefinitionData(nspace: "Foospace", values: new object[] { "Foospace" })]
        [InterfaceDefinitionData(nspace: "Foospace", values: new object[] { "Foospace" })]
        [StructDefinitionData(nspace: "Foospace", values: new object[] { "Foospace" })]
        [DelegateDefinitionData(nspace: "Foospace", values: new object[] { "Foospace" })]
        [EnumDefinitionData(nspace: "Foospace", values: new object[] { "Foospace" })]
        [ClassDefinitionData(nspace: "Foospace.Inner", values: new object[] { "Foospace.Inner" })]
        [InterfaceDefinitionData(nspace: "Foospace.Inner", values: new object[] { "Foospace.Inner" })]
        [StructDefinitionData(nspace: "Foospace.Inner", values: new object[] { "Foospace.Inner" })]
        [DelegateDefinitionData(nspace: "Foospace.Inner", values: new object[] { "Foospace.Inner" })]
        [EnumDefinitionData(nspace: "Foospace.Inner", values: new object[] { "Foospace.Inner" })]
        public void GetNamespace_ShouldReturnCorrectValue_WhenHasNamespace(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNamespace(definition)).IsEqualTo(expected);
        }
    }
}
