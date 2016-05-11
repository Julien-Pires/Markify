using System.Collections.Generic;

namespace Markify.Models.Definitions
{
    public sealed class ParameterDefinition : Definition
    {
        #region Properties

        public ParameterModifier Modifier { get; }

        public IEnumerable<string> Constraints { get; }

        #endregion

        #region Constructors

        public ParameterDefinition(string name) : base(name)
        {
        }

        #endregion
    }
}