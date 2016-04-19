namespace Markify.Models
{
    public abstract class StructuralDefinition : Definition
    {
        #region Properties

        public string Remarks { get; set; }

        public string Code { get; set; }

        #endregion

        #region Constructors

        protected StructuralDefinition(string name) : base(name)
        {
        }

        #endregion
    }
}