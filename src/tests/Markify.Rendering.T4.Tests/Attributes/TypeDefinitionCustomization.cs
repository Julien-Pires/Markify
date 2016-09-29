using System.Linq;
using System.Collections.Generic;
using Markify.Models.Definitions;
using Microsoft.FSharp.Core;
using Ploeh.AutoFixture;

namespace Markify.Rendering.T4.Tests.Attributes
{
    internal sealed class TypeDefinitionCustomization : ICustomization
    {
        #region Fields

        private readonly string _name;
        private readonly string _parent;
        private readonly string _namespace;
        private readonly StructureKind _kind;
        private readonly IEnumerable<string> _modifiers;
        private readonly IEnumerable<string> _accessModifiers;
        private readonly IEnumerable<string> _baseTypes;
        private readonly IEnumerable<string> _parameters;

        #endregion

        #region Constructors

        public TypeDefinitionCustomization(string name, string parent, string nspace, StructureKind kind, IEnumerable<string> modifiers,
            IEnumerable<string> accessModifiers, IEnumerable<string> baseTypes, IEnumerable<string> parameters)
        {
            _name = name;
            _parent = parent;
            _namespace = nspace;
            _kind = kind;
            _modifiers = modifiers;
            _accessModifiers = accessModifiers;
            _baseTypes = baseTypes;
            _parameters = parameters;
        }

        #endregion

        #region Customization

        private static TypeDefinition CreateDefinition(TypeIdentity identity, StructureKind kind)
        {
            TypeDefinition definition;
            switch(kind)
            {
                case StructureKind.Class:
                    definition = TypeDefinition.NewClass(new ClassDefinition(identity));
                    break;

                case StructureKind.Struct:
                    definition = TypeDefinition.NewStruct(new ClassDefinition(identity));
                    break;

                case StructureKind.Interface:
                    definition = TypeDefinition.NewInterface(new ClassDefinition(identity));
                    break;

                case StructureKind.Enum:
                    definition = TypeDefinition.NewEnum(new ClassDefinition(identity));
                    break;

                case StructureKind.Delegate:
                    definition = TypeDefinition.NewDelegate(new ClassDefinition(identity));
                    break;

                default:
                    definition = null;
                    break;
            }

            return definition;
        }

        public void Customize(IFixture fixture)
        {
            var parameters = _parameters?.Select(c => new GenericParameterDefinition(c, null, null));
            var identity = new TypeIdentity(_name,
                _parent != null ? FSharpOption<string>.Some(_parent) : FSharpOption<string>.None,
                _namespace != null ? FSharpOption<string>.Some(_namespace) : FSharpOption<string>.None,
               _accessModifiers, _modifiers, _baseTypes, parameters
            );
            fixture.Inject(CreateDefinition(identity, _kind));
        }

        #endregion
    }
}