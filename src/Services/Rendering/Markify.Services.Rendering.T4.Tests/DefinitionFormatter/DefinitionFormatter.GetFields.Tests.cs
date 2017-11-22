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
        public void GetFields_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetFields(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ClassDefinitionData]
        [InterfaceDefinitionData]
        [StructDefinitionData]
        public void GetFields_ShouldReturnNoFields_WhenTypeDoesNotHave(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetFields(definition)).IsEmpty();
        }

        [Theory]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [ClassDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 1, values: new object[] { 2 })]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [StructDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 1, values: new object[] { 2 })]
        public void GetFields_ShouldReturnFieldsByVisiblity_WhenTypeHasSome(int expected, TypeDefinition definition)
		{
		    Check.That(DefinitionFormatter.GetFields(definition)).HasSize(expected);
		}

        [Theory]
        [ClassDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [ClassDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 10, values: new object[] { 20 })]
        [StructDefinitionData(membersVisibility: new[] { "public" }, membersCount: 1, values: new object[] { 1 })]
        [StructDefinitionData(membersVisibility: new[] { "public", "internal" }, membersCount: 10, values: new object[] { 20 })]
        public void GetFields_ShouldReturnExactFieldsCount_WhenTypeHasSome(int expected, TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetFields(definition).Aggregate(0, (acc, c) => acc + c.Count());

            Check.That(actual).IsEqualTo(expected);
        }

        [Theory]
        [EnumDefinitionData]
        [DelegateDefinitionData]
        public void GetFields_ShouldReturnNoFields_WhenTypeCannotHaveFields(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetFields(definition)).IsEmpty();
        }
    }
}