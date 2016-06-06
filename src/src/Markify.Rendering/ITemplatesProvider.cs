using Optional;

namespace Markify.Rendering
{
    public interface ITemplatesProvider
    {
        #region Methods

        Option<ITemplate> GetTemplate(object content);

        #endregion
    }
}