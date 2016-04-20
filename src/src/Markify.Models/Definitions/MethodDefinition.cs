using System.Collections.Generic;

namespace Markify.Models.Definitions
{
    public sealed class MethodDefinition : StructuralDefinition
    {
        #region Properties

        public IList<ParameterDefinition> GenericParameters => new List<ParameterDefinition>();

        public IList<ParameterDefinition> Parameters => new List<ParameterDefinition>();

        public ParameterDefinition Return { get; set; }

        #endregion

        #region Constructors

        public MethodDefinition(string name) : base(name)
        {
        }

        #endregion
    }
}