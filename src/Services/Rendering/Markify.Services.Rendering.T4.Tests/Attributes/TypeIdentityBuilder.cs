using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Markify.Domain.Compiler;
using Microsoft.FSharp.Core;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    internal sealed class TypeIdentityBuilder : ISpecimenBuilder
    {
        #region Fields

        private readonly string _name;
        private readonly string _parent;
        private readonly string _namespace;
        private readonly IEnumerable<string> _modifiers;
        private readonly IEnumerable<string> _accessModifiers;
        private readonly IEnumerable<string> _baseTypes;
        private readonly IEnumerable<string> _parameters;

        #endregion

        #region Constructors

        public TypeIdentityBuilder(string name = "Foo", string parent = "", string nspace = "", IEnumerable<string> modifiers = null,
            IEnumerable<string> accessModifiers = null, IEnumerable<string> baseTypes = null, IEnumerable<string> parameters = null)
        {
            _name = name;
            _parent = parent;
            _namespace = nspace;
            _modifiers = modifiers;
            _accessModifiers = accessModifiers;
            _baseTypes = baseTypes;
            _parameters = parameters;
        }

        #endregion

        #region Builder

        public object Create(object request, ISpecimenContext context)
        {
            var parameterInfo = request as ParameterInfo;
            if (parameterInfo == null)
                return new NoSpecimen();

            if (parameterInfo.ParameterType != typeof(TypeIdentity))
                return new NoSpecimen();

            var parameters = _parameters?.Select(c => new GenericParameterDefinition(c, null, null));
            var parent = _parent != null ? FSharpOption<string>.Some(_parent) : FSharpOption<string>.None;
            var nspace = _namespace != null ? FSharpOption<string>.Some(_namespace) : FSharpOption<string>.None;

            return new TypeIdentity(_name, parent, nspace, _accessModifiers, _modifiers, _baseTypes, parameters);
        }

        #endregion
    }
}