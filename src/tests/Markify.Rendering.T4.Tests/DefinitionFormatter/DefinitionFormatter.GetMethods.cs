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
        public void GetMethods_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetMethods(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [ContainerDefinitionData(new string[0], 0, values: new object[] { 0 })]
        [ContainerDefinitionData(new[] { "public" }, 1, values: new object[] { 1 })]
        [ContainerDefinitionData(new[] { "public", "private" }, 1, values: new object[] { 2 })]
        [ContainerDefinitionData(new[] { "public", "private", "protected internal" }, 1, values: new object[] { 3 })]
        public void GetMethods_ShouldReturnMethodsByVisiblity_WhenDefinitionHasSome(int expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetMethods(definition)).HasSize(expected);
        }

        [Theory]
        [ContainerDefinitionData(new string[0], 0, values: new object[] { new string[0] })]
        [ContainerDefinitionData(new[] { "public" }, 1, values: new object[] { new[] { "public" } })]
        [ContainerDefinitionData(new[] { "public", "private" }, 1, values: new object[] { new[] { "public", "private" } })]
        public void GetMethods_ShouldReturnExpectedGroupKey(string[] expected, TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetMethods(definition).Select(c => c.Key);

            Check.That(actual).Contains(expected);
        }

        [Theory]
        [ContainerDefinitionData(new string[0], 0)]
        [ContainerDefinitionData(new[] { "public" }, 100)]
        public void GetMethods_ShouldNotReturnListWithNullElement(TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetMethods(definition).SelectMany(c => c);

            Check.That(actual).Not.Contains(new object[] { null });
        }

        [Theory]
        [ContainerDefinitionData(new string[0], 0, values: new object[] { 0 })]
        [ContainerDefinitionData(new[] { "public" }, 1, values: new object[] { 1 })]
        [ContainerDefinitionData(new[] { "public" }, 10, values: new object[] { 10 })]
        [ContainerDefinitionData(new[] { "public", "private" }, 1, values: new object[] { 2 })]
        [ContainerDefinitionData(new[] { "public", "private" }, 10, values: new object[] { 20 })]
        public void GetMethods_ShouldReturnExpectedMethodsCount(int expected, TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetMethods(definition).SelectMany(c => c);

            Check.That(actual).HasSize(expected);
        }

        [Theory]
        [EnumDefinitionData(0)]
        [DelegateDefinitionData(0)]
        public void GetMethods_ShouldReturnEmptyList_WhenDefinitionIsNotValid(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetMethods(definition)).IsEmpty();
        }
    }
}