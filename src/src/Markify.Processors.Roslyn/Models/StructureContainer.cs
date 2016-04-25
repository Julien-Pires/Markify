namespace Markify.Processors.Roslyn.Models
{
    public class StructureContainer : ITypeContainer, IItemRepresentation
    {
        #region Properties

        public TypeRepresentation Representation { get; }

        public string Fullname => Representation.Fullname;

        #endregion

        #region Constructors

        public StructureContainer(TypeRepresentation representation)
        {
            Representation = representation;
        }

        #endregion
    }
}