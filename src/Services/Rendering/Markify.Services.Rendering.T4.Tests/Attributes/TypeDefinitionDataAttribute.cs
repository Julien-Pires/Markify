using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal abstract class TypeDefinitionDataAttribute : InlineAutoDataAttribute
    {
        #region Fields

        protected static readonly ISpecimenBuilder DefaultDefinitionBuilder = new TypeDefinitionBuilder(StructureKind.Class);
        protected static readonly ISpecimenBuilder DefaultIdentityBuilder = new TypeIdentityBuilder();

        #endregion

        #region Constructors

        protected TypeDefinitionDataAttribute(ISpecimenBuilder[] builders, params object[] values)
            : base(new AutoDataAttribute(CreateFixture(builders)), values)
        {
        }

        #endregion

        #region Fixtures Initialization

        private static IFixture CreateFixture(params ISpecimenBuilder[] builders)
        {
            var fixture = new Fixture();
            foreach (var b in builders)
                fixture.Customizations.Add(b);

            return fixture;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class ContainerDefinitionDataAttribute : TypeDefinitionDataAttribute
    {
        #region Constructors

        public ContainerDefinitionDataAttribute(string name, string parent, string nspace, StructureKind kind, 
            string[] parameters, params object[] values)
            : base(new ISpecimenBuilder[] 
            {
                new TypeIdentityBuilder(name, parent, nspace, parameters: parameters),
                new TypeDefinitionBuilder(kind)
            }, values)
        {
        }

        public ContainerDefinitionDataAttribute(string name, string[] modifiers, string[] accessModifiers, 
            string[] baseTypes, params object[] values)
            : base(new [] 
            {
                new TypeIdentityBuilder(name, modifiers: modifiers, accessModifiers: accessModifiers, baseTypes: baseTypes),
                DefaultDefinitionBuilder
            }, values)
        {
        }

        public ContainerDefinitionDataAttribute(string[] membersVisibility, int membersCount, 
            StructureKind kind = StructureKind.Class, string setVisibilty = null, string getVisibility = null,
            object[] values = null)
            : base(new [] 
            {
                new TypeDefinitionBuilder(kind), 
                DefaultIdentityBuilder,
                new TypePropertiesBuilder(membersCount, membersVisibility, setVisibilty, getVisibility),
                new TypeFieldsBuilder(membersCount, membersVisibility), 
                new TypeEventsBuilder(membersCount, membersVisibility),
                new TypeMethodsBuilder(membersCount, membersVisibility), 
            }, values ?? new object[0])
        {
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class DelegateDefinitionDataAttribute : TypeDefinitionDataAttribute
    {
        #region Constructors

        public DelegateDefinitionDataAttribute(int parametersCount, params object[] values)
            : base(new[] 
            {
                new TypeDefinitionBuilder(StructureKind.Delegate),
                DefaultIdentityBuilder,
                new DelegateParametersBuilder(parametersCount)
            }, values)
        {
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class EnumDefinitionDataAttribute : TypeDefinitionDataAttribute
    {
        #region Constructors

        public EnumDefinitionDataAttribute(int valueCount, params object[] values) 
            : base(new []
            {
                new TypeDefinitionBuilder(StructureKind.Enum),
                DefaultIdentityBuilder,
                new EnumValuesBuilder(valueCount)
            }, values)
        {
        }

        #endregion
    }
}