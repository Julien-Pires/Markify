﻿using System;
using Markify.Models.Definitions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Rendering.T4.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class DefinitionDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public DefinitionDataAttribute(params object[] values)
            : base(values)
        {
        }

        public DefinitionDataAttribute(string name, string parent, string nspace, StructureKind kind, params object[] values)
            :this(new TypeDefinitionCustomization(name, parent, nspace, kind, null, null), values)
        {
        }

        public DefinitionDataAttribute(string name, string[] modifiers, string[] accessModifiers, params object[] values)
            : this(new TypeDefinitionCustomization(name, null, null, StructureKind.Class, modifiers, accessModifiers), values)
        {
        }

        private DefinitionDataAttribute(ICustomization customization, params object[] values)
            : base(new AutoDataAttribute(new Fixture().Customize(customization)), values)
        {
        }

        #endregion   
    }
}