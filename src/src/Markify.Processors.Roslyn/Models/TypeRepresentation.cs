using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Markify.Models.Definitions;

namespace Markify.Processors.Roslyn.Models
{
    public class TypeRepresentation : IItemRepresentation
    {
        #region Fields

        private IEnumerable<GenericParameterRepresentation> _genericParameters = ImmutableList<GenericParameterRepresentation>.Empty;
        private IEnumerable<string> _accessModifiers = ImmutableList<string>.Empty;
        private IEnumerable<string> _modifiers = ImmutableList<string>.Empty;
        private IEnumerable<string> _baseTypes = ImmutableList<string>.Empty;

        #endregion

        #region Properties

        public Fullname Fullname { get; }

        public string Name => Fullname.Parts.Last();

        public StructureKind Kind { get; }

        public IEnumerable<string> AccessModifiers
        {
            get { return _accessModifiers; }
            set { _accessModifiers = value ?? ImmutableList<string>.Empty; }
        }

        public IEnumerable<string> Modifiers
        {
            get { return _modifiers; }
            set { _modifiers = value ?? ImmutableList<string>.Empty; }
        }

        public IEnumerable<GenericParameterRepresentation> GenericParameters
        {
            get { return _genericParameters; }
            set { _genericParameters = value ?? ImmutableList<GenericParameterRepresentation>.Empty; }
        }

        public IEnumerable<string> BaseTypes
        {
            get { return _baseTypes; }
            set { _baseTypes = value ?? ImmutableList<string>.Empty; }
        }

        #endregion

        #region Constructors

        public TypeRepresentation(Fullname fullname, StructureKind kind)
        {
            if (fullname == null)
                throw new ArgumentNullException(nameof(fullname));

            Fullname = fullname;
            Kind = kind;
        }

        #endregion
    }
}