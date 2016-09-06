﻿using System;
using Markify.Models.Definitions;
using Markify.Rendering.T4.Tests.Attributes;
using NFluent;
using Xunit;

namespace Markify.Rendering.T4.Tests
{
    public class DefinitionFormatter_Tests
    {
        #region Public Methods

        #region GetKind

        [Theory]
        [DefinitionData]
        public void GetKind_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetKind(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [DefinitionData("Foo", null, null, StructureKind.Class, "class")]
        [DefinitionData("Foo", null, null, StructureKind.Interface, "interface")]
        [DefinitionData("Foo", null, null, StructureKind.Struct, "struct")]
        [DefinitionData("Foo", null, null, StructureKind.Delegate, "delegate")]
        [DefinitionData("Foo", null, null, StructureKind.Enum, "enum")]
        public void GetKind_ShouldReturnCorrectKeyword_WhenKindIsKnown(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetKind(definition)).IsEqualTo(expected);
        }

        [Theory]
        [DefinitionData("Foo", null, null, StructureKind.Unknown)]
        public void GetKind_ShouldReturnEmpty_WhenKindIsUnknown(TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetKind(definition)).IsEqualTo(string.Empty);
        }

        #endregion

        #region GetModifiers

        [Theory]
        [DefinitionData]
        public void GetModifiers_ShouldThrow_WhenDefinitionIsNull()
        {
            Check.ThatCode(() => DefinitionFormatter.GetModifiers(null)).Throws<ArgumentNullException>();
        }

        [Theory]
        [DefinitionData("Foo", new string[0], null, "")]
        [DefinitionData("Foo", new [] { "sealed" }, null, "sealed")]
        [DefinitionData("Foo", new[] { "sealed", "abstract" }, null, "sealed, abstract")]
        public void GetModifiers_ShouldReturnCorrectValue(string expected, TypeDefinition definition)
        {
            Check.That(DefinitionFormatter.GetModifiers(definition)).IsEqualTo(expected);
        }

        #endregion

        #endregion
    }
}