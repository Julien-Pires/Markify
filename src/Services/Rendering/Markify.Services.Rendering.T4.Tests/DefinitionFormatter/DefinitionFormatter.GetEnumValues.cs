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
        [EnumDefinitionData(0, 0)]
        [EnumDefinitionData(10, 10)]
        public void GetEnumValues_ShouldReturnCorrectValuesCount(int expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetEnumValues(definition)).HasSize(expected);
        }
    }
}