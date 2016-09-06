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

        #endregion

        #region Constructors

        public TypeDefinitionCustomization(string name, string parent, string nspace, StructureKind kind,
            IEnumerable<string> modifiers, IEnumerable<string> accessModifiers)
        {
            _name = name;
            _parent = parent;
            _namespace = nspace;
            _kind = kind;
            _modifiers = modifiers;
            _accessModifiers = accessModifiers;
        }

        #endregion

        #region Customization

        public void Customize(IFixture fixture)
        {
            var identity = new DefinitionIdentity(_name, FSharpOption<string>.Some(_parent), FSharpOption<string>.Some(_namespace));
            var definition = new TypeDefinition(identity, _kind, _accessModifiers, _modifiers, null, null);
            fixture.Inject(definition);
        }

        #endregion
    }
}