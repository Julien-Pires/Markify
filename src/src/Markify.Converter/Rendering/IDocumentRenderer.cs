using static Markify.Models.Document;

namespace Markify.Core.Rendering
{
    public interface IDocumentRenderer
    {
        #region Methods

        void Render(TableOfContent toc);

        #endregion
    }
}