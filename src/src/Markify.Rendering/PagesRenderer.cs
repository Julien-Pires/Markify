using Markify.Core.Rendering;

using Markify.Models.Documents;

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
                template.Match(
                    some : c => 
                    {
                        var text = c.Apply(page.Content).ValueOr(string.Empty);
                        writer.Write(text, page, toc.Root);
                    },
                    none : () => { }
                );
            }
        }

        #endregion
    }
}