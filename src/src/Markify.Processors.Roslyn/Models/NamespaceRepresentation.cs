namespace Markify.Processors.Roslyn.Models
{
    public sealed class NamespaceRepresentation : IItemRepresentation
    {
        #region Properties

        public Fullname Fullname { get; }

        #endregion

        #region Constructors

        public NamespaceRepresentation(Fullname fullname)
        {
            Fullname = fullname;
        }

        #endregion
    }
}