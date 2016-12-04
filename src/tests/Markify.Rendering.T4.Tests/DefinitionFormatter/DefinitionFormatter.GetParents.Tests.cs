using System;
using Markify.Models.Definitions;
using Markify.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Rendering.T4.Tests
{
    public sealed partial class DefinitionFormatterTests
    {
        [Fact]
        public void GetParents_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetParents(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ContainerDefinitionData("Foo", null, null, new string[]{}, "")]
        [ContainerDefinitionData("Foo", null, null, new[] { "IDisposable" }, "IDisposable")]
        [ContainerDefinitionData("Foo", null, null, new[] { "IDisposable", "IEnumerable" }, "IDisposable, IEnumerable")]
        public void GetParents_ShouldReturnCorrectValue(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetParents(definition)).IsEqualTo(expected);
        }
    }
}
