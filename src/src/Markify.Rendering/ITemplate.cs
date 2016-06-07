using Optional;

namespace Markify.Rendering
{
    public interface ITemplate
    {
        #region Methods

        Option<string> Apply(object content);

        #endregion
    }
}