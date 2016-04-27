using System.Collections.Generic;
using System.Collections.Immutable;

namespace Markify.Processors.Roslyn.Models
{
    public class GenericParameterRepresentation : IItemRepresentation
    {
        #region Fields

        private string _modifier;
        private IEnumerable<string> _constraints = ImmutableList<string>.Empty;

        #endregion

        #region Properties

        public Fullname Fullname { get; }

        public string Modifier
        {
            get { return _modifier; }
            set { _modifier = value == string.Empty ? null : value; }
        }

        public IEnumerable<string> Constraints
        {
            get { return _constraints; }
            set { _constraints = value ?? ImmutableList<string>.Empty; }
        }

        #endregion

        #region Constructors

        public GenericParameterRepresentation(Fullname fullname)
        {
            Fullname = fullname;
        }

        #endregion
    }
}