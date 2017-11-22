using System;
using System.Linq;
using Markify.Domain.Compiler;
using Markify.Services.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Services.Rendering.T4.Tests
{
    public sealed partial class DefinitionFormatterTests
    {
        [Fact]
        public void GetProperties_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetProperties(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ClassDefinitionData]
        [InterfaceDefinitionData]
        [StructDefinitionData]
        public void GetProperties_ShouldReturnNoProperties_WhenTypeDoesNotHave(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetProperties(definition)).IsEmpty();
        }

        [Theory]
        [DelegateDefinitionData]
        [EnumDefinitionData]
        public void GetProperties_ShouldReturnNoProperties_WhenTypeCannotHaveProperties(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetProperties(definition)).IsEmpty();
        }

		[Theory]
        [ClassDefinitionData(membersVisibility: new [] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [ClassDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 1, values: new object[] { 2 })]
		[InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
		[InterfaceDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 1, values: new object[] { 2 })]
		[StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
		[StructDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 1, values: new object[] { 2 })]
        public void GetProperties_ShouldReturnPropertiesByVisiblity_WhenTypeHasSome(int expected, TypeDefinition definition)
		{
		    Check.That(DefinitionFormatter.GetProperties(definition)).HasSize(expected);
		}

        [Theory]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [ClassDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 10, values: new object[] { 20 })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [InterfaceDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 10, values: new object[] { 20 })]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [StructDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 10, values: new object[] { 20 })]
        public void GetProperties_ShouldReturnExactPropertiesCount_WhenTypeHasSome(int expected, TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetProperties(definition).Aggregate(0, (acc, c) => acc + c.Count());

            Check.That(actual).IsEqualTo(expected);
        }
    }
}