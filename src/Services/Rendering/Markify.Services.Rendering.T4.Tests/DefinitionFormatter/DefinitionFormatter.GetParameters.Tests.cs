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
        public void GetParameters_ShouldThrowException_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetParameters(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [DelegateDefinitionData(0, 0)]
        [DelegateDefinitionData(10, 10)]
        public void GetParameters_ShouldReturnExpectedParameterCount(int expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetParameters(definition)).HasSize(expected);
        }

        [Theory]
        [ContainerDefinitionData("Foo", null, null, null)]
        [EnumDefinitionData(0)]
        public void GetParameters_ShouldReturnEmptySequence_WhenDefinitionIsNotDelegate(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetParameters(definition)).HasSize(0);
        }
    }
}