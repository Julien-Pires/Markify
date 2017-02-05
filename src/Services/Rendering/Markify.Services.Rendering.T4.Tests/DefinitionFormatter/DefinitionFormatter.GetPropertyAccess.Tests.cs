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
        public void GetPropertyAccess_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetPropertyAccess(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ContainerDefinitionData(new[] { "public" }, 1, setVisibilty: "public", values: new object []{ "write-only" })]
        [ContainerDefinitionData(new[] { "public" }, 1, setVisibilty: "private", values: new object[] { "" })]
        [ContainerDefinitionData(new[] { "private" }, 1, setVisibilty: "public", values: new object[] { "write-only" })]
        [ContainerDefinitionData(new[] { "protected" }, 1, setVisibilty: "public", values: new object[] { "write-only" })]
        [ContainerDefinitionData(new[] { "protected" }, 1, setVisibilty: "internal", values: new object[] { "" })]
        [ContainerDefinitionData(new[] { "protected internal" }, 1, setVisibilty: "internal protected", values: new object[] { "write-only" })]
        [ContainerDefinitionData(new[] { "public" }, 1, getVisibility: "public", values: new object[] { "read-only" })]
        [ContainerDefinitionData(new[] { "public" }, 1, getVisibility: "private", values: new object[] { "" })]
        [ContainerDefinitionData(new[] { "private" }, 1, getVisibility: "public", values: new object[] { "read-only" })]
        [ContainerDefinitionData(new[] { "protected" }, 1, getVisibility: "public", values: new object[] { "read-only" })]
        [ContainerDefinitionData(new[] { "protected" }, 1, getVisibility: "internal", values: new object[] { "" })]
        [ContainerDefinitionData(new[] { "protected internal" }, 1, getVisibility: "internal protected", values: new object[] { "read-only" })]
        [ContainerDefinitionData(new[] { "public" }, 1, setVisibilty: "public", getVisibility: "public", values: new object[] { "read/write" })]
        [ContainerDefinitionData(new[] { "internal" }, 1, setVisibilty: "public", getVisibility: "public", values: new object[] { "read/write" })]
        [ContainerDefinitionData(new[] { "protected" }, 1, setVisibilty: "private", getVisibility: "private", values: new object[] { "" })]
        [ContainerDefinitionData(new[] { "internal protected" }, 1, setVisibilty: "protected internal", getVisibility: "internal protected", values: new object[] { "read/write" })]
        public void GetPropertyAccess_ShouldReturnCorrectAccess_WhenPropertyHasAccessor(string expected,
            TypeDefinition definition)
        {
            var sut = GetProperties(definition).First();

            Check.That(DefinitionFormatter.GetPropertyAccess(sut)).IsEqualIgnoringCase(expected);
        }
    }
}