using static Markify.Models.Document;

namespace Markify.Services.Processing
{
    internal interface IRendererService
    {
        #region Methods

        bool Render(TableOfContent toc);

        #endregion
    }
}