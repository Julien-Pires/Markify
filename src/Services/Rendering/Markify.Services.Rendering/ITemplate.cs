using Optional;

namespace Markify.Services.Rendering
{
    public interface ITemplate
    {
        #region Methods

        Option<string> Apply(object content);

        #endregion
    }
}