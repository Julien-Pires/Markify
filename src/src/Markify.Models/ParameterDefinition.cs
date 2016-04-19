namespace Markify.Models
{
    public sealed class ParameterDefinition : Definition
    {
        #region Properties

        public ParameterModifier Modifier { get; set; }

        #endregion

        #region Constructors

        public ParameterDefinition(string name) : base(name)
        {
        }

        #endregion
    }
}