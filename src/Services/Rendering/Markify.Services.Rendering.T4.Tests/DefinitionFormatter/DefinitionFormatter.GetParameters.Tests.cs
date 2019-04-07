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
        public void GetParameters_ShouldThrowException_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetParameters(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [DelegateDefinitionData(parameters: 0, values: new object[] { 0 })]
        [DelegateDefinitionData(parameters : 10, values : new object[] { 10 })]
        public void GetParameters_ShouldReturnExpectedParameterCount(int expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetParameters(definition)).HasSize(expected);
        }

        [Theory]
        [ClassDefinitionData]
        [InterfaceDefinitionData]
        [StructDefinitionData]
        [EnumDefinitionData]
        public void GetParameters_ShouldReturnEmptySequence_WhenDefinitionIsNotDelegate(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetParameters(definition)).HasSize(0);
        }
    }
}