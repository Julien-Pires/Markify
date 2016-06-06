namespace Markify.Rendering
{
    public interface ITemplate
    {
        #region Methods

        string Apply(object content);

        #endregion
    }
}