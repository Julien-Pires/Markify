namespace Markify.Processors.Roslyn.Models
{
    public sealed class NamespaceRepresentation
    {
        #region Properties

        public string Fullname { get; }

        #endregion

        #region Constructors

        public NamespaceRepresentation(string fullname)
        {
            Fullname = fullname;
        }

        #endregion
    }
}