using Markify.Models.Definitions;

namespace Markify.Processors.Roslyn.Models
{
    public sealed class TypeRepresentation
    {
        #region Properties

        public string Fullname { get; }

        public string Name { get; }

        public StructureKind Structure { get; }

        public bool IsSealed { get; set; }

        public bool IsAbstract { get; set; }

        public bool IsStatic { get; set; }

        #endregion

        #region Constructors

        public TypeRepresentation(string fullname, string name, StructureKind structure)
        {
            Fullname = fullname;
            Name = name;
            Structure = structure;
        }

        #endregion
    }
}