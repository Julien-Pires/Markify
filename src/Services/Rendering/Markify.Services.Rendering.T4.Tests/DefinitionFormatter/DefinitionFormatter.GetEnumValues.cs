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
        public void GetEnumValues_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetEnumValues(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [EnumDefinitionData]
        public void GetEnumValues_ShouldReturnNoValues_WhenEnumHasNone(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetEnumValues(definition)).IsEmpty();
        }

        [Theory]
        [EnumDefinitionData(enumValues: 1, values: new object[] { 1 })]
        [EnumDefinitionData(enumValues: 100, values: new object[] { 100 })]
        public void GetEnumValues_ShouldReturnExpectedValuesCount_WhenEnumHasSome(int expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetEnumValues(definition)).HasSize(expected);
        }
    }
}