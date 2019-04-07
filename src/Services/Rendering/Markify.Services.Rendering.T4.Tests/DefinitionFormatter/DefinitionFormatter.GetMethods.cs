using System;
using System.Linq;
using Markify.CodeAnalyzer;
using Markify.Services.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Services.Rendering.T4.Tests
{
    public sealed partial class DefinitionFormatterTests
    {
        [Fact]
        public void GetMethods_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetMethods(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ClassDefinitionData]
        [InterfaceDefinitionData]
        [StructDefinitionData]
        public void GetMethods_ShouldReturnNoMethods_WhenTypeDoesNotHave(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetMethods(definition)).IsEmpty();
        }

        [Theory]
        [EnumDefinitionData]
        [DelegateDefinitionData]
        public void GetMethods_ShouldReturnNoMethods_WhenTypeCannotHaveMethods(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetMethods(definition)).IsEmpty();
        }

        [Theory]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [ClassDefinitionData(membersVisibility: new[] { "public", "private" }, membersCount: 1, values: new object[] { 2 })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public", "private" }, membersCount: 1, values: new object[] { 2 })]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [StructDefinitionData(membersVisibility: new[] { "public", "private" }, membersCount: 1, values: new object[] { 2 })]
        public void GetMethods_ShouldReturnMethodsByVisiblity_WhenTypeHasSome(int expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetMethods(definition)).HasSize(expected);
        }

        [Theory]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { new[] { "public" } })]
        [ClassDefinitionData(membersVisibility: new[] { "public", "private" }, membersCount: 1, values: new object[] { new[] { "public", "private" } })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { new[] { "public" } })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public", "private" }, membersCount: 1, values: new object[] { new[] { "public", "private" } })]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { new[] { "public" } })]
        [StructDefinitionData(membersVisibility: new[] { "public", "private" }, membersCount: 1, values: new object[] { new[] { "public", "private" } })]
        public void GetMethods_ShouldReturnExpectedGroupKey_WhenTypeHasSome(string[] expected, TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetMethods(definition).Select(c => c.Key);

            Check.That(actual).Contains(expected);
        }

        [Theory]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 100)]
        [InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 100)]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 100)]
        public void GetMethods_ShouldNotReturnNullElement_WhenTypeHasSome(TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetMethods(definition).SelectMany(c => c);

            Check.That(actual).Not.Contains(new object[] { null });
        }

        [Theory]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 10, values: new object[] { 10 })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 10, values: new object[] { 10 })]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 10, values: new object[] { 10 })]
        public void GetMethods_ShouldReturnExactMethodsCount_WhenTypeHasSome(int expected, TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetProperties(definition).Select(c => c.Count());

            Check.That(actual).IsOnlyMadeOf(expected);
        }
    }
}