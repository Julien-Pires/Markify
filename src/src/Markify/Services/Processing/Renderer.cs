using Markify.Core.Rendering;

using Markify.Models.Documents;

namespace Markify.Services.Processing
{
    internal class Renderer : IRenderer
    {
        #region Fields

        private readonly IDocumentRenderer _renderer;
        private readonly IPageWriter _writer;

        #endregion

        #region Constructors

        public Renderer(IDocumentRenderer renderer, IPageWriter writer)
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