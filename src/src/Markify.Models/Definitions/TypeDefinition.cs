using System.Collections.Generic;

namespace Markify.Models.Definitions
{
    public sealed class TypeDefinition : StructuralDefinition
    {
        #region Properties

        public IEnumerable<string> AccessModifiers { get; }

        public IEnumerable<string> Modifiers { get; } 

        public IEnumerable<ParameterDefinition> GenericParameters { get; }

        public IEnumerable<PropertyDefinition> Fields { get; }

        public IEnumerable<PropertyDefinition> Properties { get; }

        public IEnumerable<MethodDefinition> Methods { get; }

        public IEnumerable<string> BaseTypes { get; }

        #endregion

        #region Constructors

        public TypeDefinition(string name) : base(name)
        {
        }

        #endregion
    }
}