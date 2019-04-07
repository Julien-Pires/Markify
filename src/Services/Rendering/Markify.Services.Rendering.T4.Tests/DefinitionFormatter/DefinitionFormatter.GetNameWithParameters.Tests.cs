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
        public void GetNameWithParameters_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetNameWithParameters(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ClassDefinitionData("Foo", generics: new string[]{}, values:new object[]{ "Foo" })]
        [StructDefinitionData("Foo", generics: new string[] { }, values: new object[] { "Foo" })]
        [InterfaceDefinitionData("Foo", generics: new string[] { }, values: new object[] { "Foo" })]
        [DelegateDefinitionData("Foo", generics: new string[] { }, values: new object[] { "Foo" })]
        public void GetNameWithParameters_ShouldReturnOnlyName_WhenHasNoParameters(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNameWithParameters(definition.Identity)).IsEqualTo(expected);
        }

        [Theory]
        [ClassDefinitionData("Foo", generics: new [] { "T" }, values: new object[] { "Foo<T>" })]
        [StructDefinitionData("Foo", generics: new [] { "T" }, values: new object[] { "Foo<T>" })]
        [InterfaceDefinitionData("Foo", generics: new [] { "T" }, values: new object[] { "Foo<T>" })]
        [DelegateDefinitionData("Foo", generics: new [] { "T" }, values: new object[] { "Foo<T>" })]

        [ClassDefinitionData("Foo", generics: new [] { "T", "Y", "Z" }, values: new object[] { "Foo<T, Y, Z>" })]
        [StructDefinitionData("Foo", generics: new [] { "T", "Y", "Z" }, values: new object[] { "Foo<T, Y, Z>" })]
        [InterfaceDefinitionData("Foo", generics: new [] { "T", "Y", "Z" }, values: new object[] { "Foo<T, Y, Z>" })]
        [DelegateDefinitionData("Foo", generics: new [] { "T", "Y", "Z" }, values: new object[] { "Foo<T, Y, Z>" })]
        public void GetNameWithParameters_ShouldReturnNameWithParameters_WhenHasParameters(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetNameWithParameters(definition.Identity)).IsEqualTo(expected);
        }
    }
}
