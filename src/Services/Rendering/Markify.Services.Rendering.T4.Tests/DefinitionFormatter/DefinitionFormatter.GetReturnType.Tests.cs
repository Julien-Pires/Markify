using System;
using Markify.Domain.Compiler;
using Markify.Services.Rendering.T4.Tests.Attributes;
using NFluent;
using Optional;
using Xunit;

namespace Markify.Services.Rendering.T4.Tests
{
    public sealed partial class DefinitionFormatterTests
    {
        [Fact]
        public void GetReturnType_ShouldThrowException_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetReturnType(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [DelegateDefinitionData(0)]
        public void GetReturnType_ShouldReturnDelegateReturnType_WhenDefinitionIsDelegate(TypeDefinition definition)
        {
            var actual = ((TypeDefinition.Delegate) definition).Item.ReturnType;

            Check.That(DefinitionFormatter.GetReturnType(definition)).IsEqualTo(Option.Some(actual));
        }

        [Theory]
        [EnumDefinitionData(0)]
        public void GetReturnType_ShouldReturnNone_WhenDefinitionIsNotDelegate(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetReturnType(definition)).IsEqualTo(Option.None<string>());
        }
    }
}