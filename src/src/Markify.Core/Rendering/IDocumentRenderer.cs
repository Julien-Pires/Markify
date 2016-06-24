using Markify.Models.Documents;

namespace Markify.Core.Rendering
{
    public interface IDocumentRenderer
    {
        #region Methods

        void Render(TableOfContent toc, IPageWriter writer);

        #endregion
    }
}