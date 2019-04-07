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
        public void GetPropertyAccess_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetPropertyAccess(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [PropertyData("public")]
        [PropertyData("public", "private")]
        [PropertyData("public", get: "private")]
        [PropertyData("public", "internal")]
        [PropertyData("public", get: "internal")]
        [PropertyData("public", "private", "private")]
        [PropertyData("public", "internal", "internal")]
        public void GetPropertyAccess_ShouldReturnEmptyString_WhenAccesorArePrivate(PropertyDefinition definition)
        {
            Check.That(DefinitionFormatter.GetPropertyAccess(definition)).IsEmpty();
        }

        [Theory]
        [PropertyData("public", "public")]
        [PropertyData("private", "public")]
        [PropertyData("protected", "public")]
        [PropertyData("protected internal", "protected internal")]
        [PropertyData("public", "public", "private")]
        public void GetPropertyAccess_ShouldReturnWriteOnly_WhenPropertyHasOnlySetAccessor(PropertyDefinition definition)
        {
            Check.That(DefinitionFormatter.GetPropertyAccess(definition)).IsEqualTo("Write-Only");
        }

        [Theory]
        [PropertyData("public", get: "public")]
        [PropertyData("private", get: "public")]
        [PropertyData("protected", get: "public")]
        [PropertyData("protected internal", get: "protected internal")]
        [PropertyData("public", "private", "public")]
        public void GetPropertyAccess_ShouldReturnReadOnly_WhenPropertyHasOnlyGetAccessor(PropertyDefinition definition)
        {
            Check.That(DefinitionFormatter.GetPropertyAccess(definition)).IsEqualTo("Read-Only");
        }

        [Theory]
        [PropertyData("public", "public", "public")]
        [PropertyData("protected", "public", "public")]
        [PropertyData("internal", "public", "public")]
        [PropertyData("internal protected", "internal protected", "internal protected")]
        public void GetPropertyAccess_ShouldReturnReadAndWrite_WhenProprtyHasBothAccessor(PropertyDefinition definition)
        {
            Check.That(DefinitionFormatter.GetPropertyAccess(definition)).IsEqualTo("Read/Write");
        }
    }
}