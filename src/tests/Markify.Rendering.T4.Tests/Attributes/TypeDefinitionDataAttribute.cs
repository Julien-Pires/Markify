using System;
using Markify.Models.Definitions;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Rendering.T4.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class TypeDefinitionDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public TypeDefinitionDataAttribute(params object[] values)
            : base(values)
        {
        }

        public TypeDefinitionDataAttribute(string name, string parent, string nspace, StructureKind kind, string[] parameters, params object[] values)
            :this(new TypeDefinitionCustomization(name, parent, nspace, kind, null, null, null, parameters), values)
        {
        }

        public TypeDefinitionDataAttribute(string name, string[] modifiers, string[] accessModifiers, string[] baseTypes, params object[] values)
            : this(new TypeDefinitionCustomization(name, null, null, StructureKind.Class, modifiers, accessModifiers, 
                baseTypes, null), values)
        {
        }

        private TypeDefinitionDataAttribute(ICustomization customization, params object[] values)
            : base(new AutoDataAttribute(new Fixture().Customize(customization)), values)
        {
        }

        #endregion   
    }
}