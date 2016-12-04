﻿using System;
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
        public void GetFields_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetFields(null)).Throws<ArgumentNullException>();
        }

		[Theory]
		[ContainerDefinitionData(new string[0], 0, values: new object[] { 0 })]
        [ContainerDefinitionData(new string[0], 0, StructureKind.Delegate, values: new object[] { 0 })]
        [ContainerDefinitionData(new [] { "public" }, 1, values: new object[] { 1 })]
        [ContainerDefinitionData(new[] { "public", "internal" }, 1, values: new object[] { 2 })]
        [ContainerDefinitionData(new[] { "Public", "Friend" }, 10, values: new object[] { 2 })]
        public void GetFields_ShouldReturnFieldsByVisiblity_WhenDefinitionHasSome(int expected, TypeDefinition definition)
		{
		    Check.That(DefinitionFormatter.GetFields(definition)).HasSize(expected);
		}

        [Theory]
        [ContainerDefinitionData(new string[0], 0, values: new object[] { 0 })]
        [ContainerDefinitionData(new []{ "public" }, 1, values: new object[] { 1 })]
        [ContainerDefinitionData(new[] { "public", "private" }, 1, values: new object[] { 2 })]
        [ContainerDefinitionData(new[] { "public", "private" }, 10, values: new object[] { 20 })]
        public void GetFields_ShouldReturnExactFieldsCount(int expected, TypeDefinition definition)
        {
            var actual = DefinitionFormatter.GetFields(definition).SelectMany(c => c);

            Check.That(actual).HasSize(expected);
        }

        [Theory]
        [EnumDefinitionData(0)]
        [DelegateDefinitionData(0)]
        public void GetFields_ShouldReturnEmptyList_WhenDefinitionIsNotValid(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetFields(definition)).IsEmpty();
        }
    }
}