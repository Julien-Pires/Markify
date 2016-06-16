using Markify.Core.Rendering;

using static Markify.Models.Document;

namespace Markify.Services.Impl
{
    internal class RendererService : IRendererService
    {
        #region Fields

        private readonly IDocumentRenderer _renderer;
        private readonly IPageWriter _writer;

        #endregion

        #region Constructors

        public RendererService(IDocumentRenderer renderer, IPageWriter writer)
        {
            _renderer = renderer;
            _writer = writer;
        }

        #endregion

        #region Rendering

        public bool Render(TableOfContent toc)
        {
            if(toc == null)
                return false;

            _renderer.Render(toc, _writer);

            return true;
        }

        #endregion
    }
}