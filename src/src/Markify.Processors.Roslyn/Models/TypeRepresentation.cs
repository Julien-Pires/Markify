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

        private IEnumerable<GenericParameterRepresentation> _genericParameters = ImmutableArray<GenericParameterRepresentation>.Empty;

        #endregion

        #region Properties

        public Fullname Fullname { get; }

        public string Name => Fullname.Parts.Last();

        public StructureKind Kind { get; }

        public string AccessModifiers { get; set; }

        public string[] Modifiers { get; set; }

        public IEnumerable<GenericParameterRepresentation> GenericParameters
        {
            get { return _genericParameters; }
            set { _genericParameters = value ?? ImmutableArray<GenericParameterRepresentation>.Empty; }
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