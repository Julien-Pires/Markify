using System;
using System.Linq;
using Markify.Models.Documents;
using Optional;
using Ploeh.AutoFixture;
using Moq;

namespace Markify.Rendering.Tests.Attributes
{
    internal sealed class PageRendererCustomization : ICustomization
    {
        #region Fields

        private readonly bool _hasTemplate;
        private readonly bool _withFailure;
        private readonly int _pageCount;

        #endregion

        #region Constructors

        public PageRendererCustomization(bool hasTemplate, bool withFailure, int pageCount)
        {
            _hasTemplate = hasTemplate;
            _withFailure = withFailure;
            _pageCount = pageCount;
        }

        #endregion

        #region Customization

        private static ITemplatesProvider CreateTemplateProvider(bool hasTemplate, bool withFailure)
        {
            var provider = new Mock<ITemplatesProvider>();
            Option<ITemplate> template;
            if (hasTemplate)
            {
                var mockTemplate = new Mock<ITemplate>();
                Func<Option<string>> apply;
                if (withFailure)
                    apply = () => { throw new InvalidOperationException(); };
                else
                    apply = () => Option.Some(string.Empty);
                mockTemplate.Setup(c => c.Apply(It.IsAny<object>())).Returns(apply);

                template = Option.Some(mockTemplate.Object);
            }
            else
                template = Option.None<ITemplate>();

            provider.Setup(c => c.GetTemplate(It.IsAny<object>())).Returns(template);

            return provider.Object;
        }

        private static TableOfContent CreateToc(int pageCount)
        {
            var pages = Enumerable.Range(0, pageCount)
                                  .Select(c => new Page(Guid.NewGuid().ToString(), new Uri("/", UriKind.RelativeOrAbsolute), null));

            return new TableOfContent(new Uri("/", UriKind.RelativeOrAbsolute), pages);
        }

        public void Customize(IFixture fixture)
        {
            fixture.Inject(new PagesRenderer(CreateTemplateProvider(_hasTemplate, _withFailure)));
            fixture.Register(() => CreateToc(_pageCount));
        }

        #endregion
    }
}