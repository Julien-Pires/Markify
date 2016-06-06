namespace Markify.Rendering
{
    public interface ITemplatesProvider
    {
        #region Methods

        ITemplate GetTemplate(object content);

        #endregion
    }
}