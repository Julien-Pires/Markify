using System.Reflection;
using Markify.CodeAnalyzer;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    internal sealed class TypeDefinitionBuilder : ISpecimenBuilder
    {
        #region Fields

        private readonly StructureKind _kind;

        #endregion

        #region Constructors

        public TypeDefinitionBuilder(StructureKind kind)
        {
            _kind = kind;
        }

        #endregion

        #region Builder

        private static TypeDefinition CreateDefinition(StructureKind kind, ISpecimenContext context)
        {
            TypeDefinition definition;
            switch (kind)
            {
                case StructureKind.Class:
                    definition = TypeDefinition.NewClass((ClassDefinition)context.Resolve(typeof(ClassDefinition)));
                    break;
                    
                case StructureKind.Struct:
                    definition = TypeDefinition.NewStruct((ClassDefinition)context.Resolve(typeof(ClassDefinition)));
                    break;

                case StructureKind.Interface:
                    definition = TypeDefinition.NewInterface((ClassDefinition)context.Resolve(typeof(ClassDefinition)));
                    break;

                case StructureKind.Enum:
                    definition = TypeDefinition.NewEnum((EnumDefinition)context.Resolve(typeof(EnumDefinition)));
                    break;

                case StructureKind.Delegate:
                    definition = TypeDefinition.NewDelegate((DelegateDefinition)context.Resolve(typeof(DelegateDefinition)));
                    break;

                default:
                    definition = null;
                    break;
            }

            return definition;
        }

        public object Create(object request, ISpecimenContext context)
        {
            var parameterInfo = request as ParameterInfo;
            if (parameterInfo == null)
                return new NoSpecimen();

            if (parameterInfo.ParameterType != typeof(TypeDefinition))
                return new NoSpecimen();
            
            return CreateDefinition(_kind, context);
        }

        #endregion
    }
}
