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
        public void GetEvents_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetEvents(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ClassDefinitionData]
        [InterfaceDefinitionData]
        [StructDefinitionData]
        public void GetEvents_ShouldReturnNoEvents_WhenTypeDoesNotHave(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetEvents(definition)).IsEmpty();
        }

        [Theory]
        [DelegateDefinitionData]
        [EnumDefinitionData]
        public void GetEvents_ShouldReturnNoEvents_WhenTypeCannotHave(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetEvents(definition)).IsEmpty();
        }

        [Theory]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [ClassDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 1, values: new object[] { 2 })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 1, values: new object[] { 2 })]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [StructDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 1, values: new object[] { 2 })]
        public void GetEvents_ShouldReturnEventsByVisiblity_WhenTypeHasSome(int expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetEvents(definition)).HasSize(expected);
        }

        [Theory]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { new[] { "public" } })]
        [ClassDefinitionData(membersVisibility: new[] { "public", "private" }, membersCount: 1, values: new object[] { new[] { "public", "private" } })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { new[] { "public" } })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public", "private" }, membersCount: 1, values: new object[] { new[] { "public", "private" } })]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { new[] { "public" } })]
        [StructDefinitionData(membersVisibility: new[] { "public", "private" }, membersCount: 1, values: new object[] { new[] { "public", "private" } })]
        public void GetEvents_ShouldReturnExpectedGroupKey_WhenTypeHasSome(string[] expected, TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetEvents(definition).Select(c => c.Key);

            Check.That(actual).Contains(expected);
        }

        [Theory]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 100)]
        [InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 100)]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 100)]
        public void GetEvents_ShouldNotReturnNullElement_WhenTypeHasSome(TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetEvents(definition).SelectMany(c => c);

            Check.That(actual).Not.Contains(new object[] { null });
        }

        [Theory]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 10, values: new object[] { 10 })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 10, values: new object[] { 10 })]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 10, values: new object[] { 10 })]
        public void GetEvents_ShouldReturnExactEventsCount_WhenTypeHasSome(int expected, TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetProperties(definition).Select(c => c.Count());

            Check.That(actual).IsOnlyMadeOf(expected);
        }
    }
}