namespace Markify.Processors.Roslyn.Models
{
    public class StructureContainer : ITypeContainer, IItemRepresentation
    {
        #region Properties

        public Fullname Fullname => Representation.Fullname;

        public TypeRepresentation Representation { get; }

        #endregion

        #region Constructors

        public StructureContainer(TypeRepresentation representation)
        {
            Representation = representation;
        }

        #endregion
    }
}