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

        public void Customize(IFixture fixture)
        {
            var identity = new DefinitionIdentity(_name,
                _parent != null ? FSharpOption<string>.Some(_parent) : FSharpOption<string>.None,
                _namespace != null ? FSharpOption<string>.Some(_namespace) : FSharpOption<string>.None
            );
            var parameters = _parameters?.Select(c => new GenericParameterDefinition(c, null, null));
            var definition = new TypeDefinition(identity, _kind, _accessModifiers, _modifiers, _baseTypes, parameters);
            fixture.Inject(definition);
        }

        #endregion
    }
}