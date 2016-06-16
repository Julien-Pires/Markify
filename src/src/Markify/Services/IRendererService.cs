using static Markify.Models.Document;

namespace Markify.Services
{
    internal interface IRendererService
    {
        #region Methods

        bool Render(TableOfContent toc);

        #endregion
    }
}