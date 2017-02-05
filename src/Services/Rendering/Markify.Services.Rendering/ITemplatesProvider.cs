using Optional;

namespace Markify.Services.Rendering
{
    public interface ITemplatesProvider
    {
        #region Methods

        Option<ITemplate> GetTemplate(object content);

        #endregion
    }
}