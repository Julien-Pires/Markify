namespace Markify.Models
{
    public sealed class PropertyDefinition : StructuralDefinition
    {
        #region Properties

        public bool Read { get; set; } = true;

        public bool Write { get; set; } = true;

        #endregion

        #region Constructors

        public PropertyDefinition(string name) : base(name)
        {
        }

        #endregion
    }
}