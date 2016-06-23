using Markify.Models.Documents;

namespace Markify.Services.Processing
{
    internal interface IRendererService
    {
        #region Methods

        bool Render(TableOfContent toc);

        #endregion
    }
}