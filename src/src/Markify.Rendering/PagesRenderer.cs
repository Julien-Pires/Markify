using Markify.Core.Rendering;

using static Markify.Models.Document;

namespace Markify.Rendering
{
    public sealed class PagesRenderer : IDocumentRenderer
    {
        #region Fields

        private readonly ITemplatesProvider _provider;

        #endregion

        #region Constructors

        public PagesRenderer(ITemplatesProvider provider)
        {
            _provider = provider;
        }

        #endregion

        #region Rendering

        public void Render(TableOfContent toc, IPageWriter writer)
        {
            foreach(var page in toc.Pages)
            {
                var template = _provider.GetTemplate(page.Content);
                var text = template.Match(
                    some: c => c.Apply(page.Content).ValueOr(string.Empty),
                    none : () => string.Empty
                );
                writer.Write(text, page, toc.Root);
            }
        }

        #endregion
    }
}