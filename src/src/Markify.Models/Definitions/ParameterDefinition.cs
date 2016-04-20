namespace Markify.Models.Definitions
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