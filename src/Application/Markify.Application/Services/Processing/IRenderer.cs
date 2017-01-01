using Markify.Domain.Document;

namespace Markify.Application.Services.Processing
{
    internal interface IRenderer
    {
        #region Methods

        bool Render(TableOfContent toc);

        #endregion
    }
}