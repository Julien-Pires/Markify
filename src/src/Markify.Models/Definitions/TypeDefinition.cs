using System.Collections.Generic;

namespace Markify.Models.Definitions
{
    public sealed class TypeDefinition : StructuralDefinition
    {
        #region Properties

        public IList<ParameterDefinition> GenericParameters => new List<ParameterDefinition>();

        public IList<PropertyDefinition> Fields => new List<PropertyDefinition>();

        public IList<PropertyDefinition> Properties => new List<PropertyDefinition>();

        public IList<MethodDefinition> Methods => new List<MethodDefinition>();

        #endregion

        #region Constructors

        public TypeDefinition(string name) : base(name)
        {
        }

        #endregion
    }
}