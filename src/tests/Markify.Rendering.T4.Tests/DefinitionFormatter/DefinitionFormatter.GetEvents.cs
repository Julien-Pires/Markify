using System;
using System.Linq;
using Markify.Models.Definitions;
using Markify.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Rendering.T4.Tests
{
    public sealed partial class DefinitionFormatterTests
    {
        [Fact]
        public void GetEvents_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetEvents(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ContainerDefinitionData(new string[0], 0, values: new object[] { 0 })]
        [ContainerDefinitionData(new string[] { "public" }, 1, values: new object[] { 1 })]
        [ContainerDefinitionData(new string[] { "public", "private" }, 1, values: new object[] { 2 })]
        [ContainerDefinitionData(new string[] { "public", "private", "protected internal" }, 1, values: new object[] { 3 })]
        public void GetEvents_ShouldReturnEventsByVisiblity_WhenDefinitionHasSome(int expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetEvents(definition)).HasSize(expected);
        }

        [Theory]
        [ContainerDefinitionData(new string[0], 0, values: new object[] { new string[0] })]
        [ContainerDefinitionData(new string[] { "public" }, 1, values: new object[] { new string[] { "public" } })]
        [ContainerDefinitionData(new string[] { "public", "private" }, 1, values: new object[] { new string[] { "public", "private" } })]
        public void GetEvents_ShouldReturnExpectedGroupKey(string[] expected, TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetEvents(definition).Select(c => c.Key);

            Check.That(actual).Contains(expected);
        }

        [Theory]
        [ContainerDefinitionData(new string[0], 0)]
        [ContainerDefinitionData(new string[] { "public" }, 100)]
        public void GetEvents_ShouldNotReturnListWithNullElement(TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetEvents(definition).SelectMany(c => c);

            Check.That(actual).Not.Contains(new object[] { null });
        }

        [Theory]
        [ContainerDefinitionData(new string[0], 0, values: new object[] { 0 })]
        [ContainerDefinitionData(new string[] { "public" }, 1, values: new object[] { 1 })]
        [ContainerDefinitionData(new string[] { "public" }, 10, values: new object[] { 10 })]
        [ContainerDefinitionData(new string[] { "public", "private" }, 1, values: new object[] { 2 })]
        [ContainerDefinitionData(new string[] { "public", "private" }, 10, values: new object[] { 20 })]
        public void GetEvents_ShouldReturnExpectedEventsCount(int expected, TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetEvents(definition).SelectMany(c => c);

            Check.That(actual).HasSize(expected);
        }

        [Theory]
        [EnumDefinitionData(0)]
        [DelegateDefinitionData(0)]
        public void GetEvents_ShouldReturnEmptyList_WhenDefinitionIsNotValid(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetEvents(definition)).IsEmpty();
        }
    }
}