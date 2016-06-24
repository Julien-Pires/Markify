using Markify.Models.Documents;

namespace Markify.Services.Processing
{
    internal interface IRenderer
    {
        #region Methods

        bool Render(TableOfContent toc);

        #endregion
    }
}