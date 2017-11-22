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

        protected ContainerDefinitionDataAttribute(
            StructureKind kind = StructureKind.Class,
            string name = "Foo", 
            string parent = "", 
            string nspace = "",
            string[] modifiers = null, 
            string[] accessModifiers = null,
            string[] baseTypes = null,
            string[] generics = null,
            bool hasComments = false,
            int membersCount = 0,
            string[] membersVisibility = null,
            string setVisibilty = null, 
            string getVisibility = null,
            object[] values = null)
            : base(new ISpecimenBuilder[] 
            {
                new TypeDefinitionBuilder(kind),
                new TypeIdentityBuilder(name, parent, nspace, modifiers, accessModifiers, baseTypes, generics),
                new PropertiesBuilder(membersCount, membersVisibility, setVisibilty, getVisibility), 
                new TypeFieldsBuilder(membersCount, membersVisibility),
                new TypeEventsBuilder(membersCount, membersVisibility),
                new TypeMethodsBuilder(membersCount, membersVisibility),
                new TypeCommentBuilder(hasComments)
            }, values ?? new object[0])
        {
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class ClassDefinitionDataAttribute : ContainerDefinitionDataAttribute
    {
        #region Constructors

        public ClassDefinitionDataAttribute(
            string name = "Foo",
            string parent = "",
            string nspace = "",
            string[] modifiers = null,
            string[] accessModifiers = null,
            string[] baseTypes = null,
            string[] generics = null,
            bool hasComments = false,
            int membersCount = 0,
            string[] membersVisibility = null,
            string setVisibilty = null,
            string getVisibility = null,
            object[] values = null)
            : base(
                StructureKind.Class,
                name,
                parent,
                nspace,
                modifiers,
                accessModifiers,
                baseTypes,
                generics,
                hasComments,
                membersCount,
                membersVisibility,
                setVisibilty,
                getVisibility,
                values
            )
        {
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class StructDefinitionDataAttribute : ContainerDefinitionDataAttribute
    {
        #region Constructors

        public StructDefinitionDataAttribute(
            string name = "Foo",
            string parent = "",
            string nspace = "",
            string[] modifiers = null,
            string[] accessModifiers = null,
            string[] baseTypes = null,
            string[] generics = null,
            bool hasComments = false,
            int membersCount = 0,
            string[] membersVisibility = null,
            string setVisibilty = null,
            string getVisibility = null,
            object[] values = null)
            : base(
                StructureKind.Struct,
                name,
                parent,
                nspace,
                modifiers,
                accessModifiers,
                baseTypes,
                generics,
                hasComments,
                membersCount,
                membersVisibility,
                setVisibilty,
                getVisibility,
                values
            )
        {
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class InterfaceDefinitionDataAttribute : ContainerDefinitionDataAttribute
    {
        #region Constructors

        public InterfaceDefinitionDataAttribute(
            string name = "Foo",
            string parent = "",
            string nspace = "",
            string[] modifiers = null,
            string[] accessModifiers = null,
            string[] baseTypes = null,
            string[] generics = null,
            bool hasComments = false,
            int membersCount = 0,
            string[] membersVisibility = null,
            string setVisibilty = null,
            string getVisibility = null,
            object[] values = null)
            : base(
                StructureKind.Interface,
                name,
                parent,
                nspace,
                modifiers,
                accessModifiers,
                baseTypes,
                generics,
                hasComments,
                membersCount,
                membersVisibility,
                setVisibilty,
                getVisibility,
                values
            )
        {
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class DelegateDefinitionDataAttribute : TypeDefinitionDataAttribute
    {
        #region Constructors

        public DelegateDefinitionDataAttribute(
            string name = "Foo",
            string parent = "",
            string nspace = "",
            string[] modifiers = null,
            string[] accessModifiers = null,
            string[] baseTypes = null,
            string[] generics = null,
            int parameters = 0, 
            bool hasComments = false, 
            object[] values = null)
            : base(new ISpecimenBuilder[]
            {
                new TypeDefinitionBuilder(StructureKind.Delegate),
                new TypeIdentityBuilder(name, parent, nspace, modifiers, accessModifiers, baseTypes, generics),
                new DelegateParametersBuilder(parameters),
                new TypeCommentBuilder(hasComments) 
            }, values ?? new object[0])
        {
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class EnumDefinitionDataAttribute : TypeDefinitionDataAttribute
    {
        #region Constructors

        public EnumDefinitionDataAttribute(
            string name = "Foo",
            string parent = "",
            string nspace = "",
            string[] accessModifiers = null,
            string[] baseTypes = null,
            int enumValues = 0, 
            bool hasComments = false,
            object[] values = null)
            : base(new ISpecimenBuilder[]
            {
                new TypeDefinitionBuilder(StructureKind.Enum),
                new TypeIdentityBuilder(name, parent, nspace, null, accessModifiers, baseTypes),
                new EnumValuesBuilder(enumValues),
                new TypeCommentBuilder(hasComments) 
            }, values ?? new object[0])
        {
        }

        #endregion
    }
}