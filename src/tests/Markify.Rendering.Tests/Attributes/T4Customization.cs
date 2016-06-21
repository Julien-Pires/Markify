using System;
using System.Linq;

using Markify.Rendering.T4;
using Microsoft.VisualStudio.TextTemplating;

using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Rendering.Tests.Attributes
{
    public sealed class T4Customization : ICustomization
    {
        #region Fields

        private readonly Type[] _templateBinding;
        private readonly Type _currentContent;

        #endregion

        #region Constructors

        public T4Customization(Type[] templateBinding, Type currentContent)
        {
            _templateBinding = templateBinding;
            _currentContent = currentContent;
        }

        #endregion

        #region Customization

        public void Customize(IFixture fixture)
        {
            fixture.Register<ITemplatesProvider>(() =>
            {
                var transform = new Mock<T4TemplateBase>();
                var templates = _templateBinding.Select(c => new Tuple<T4TemplateBase, Type>(transform.Object, c));

                return new T4TemplateProvider(templates);
            });
            fixture.Register(() => _currentContent != null ? new SpecimenContext(fixture).Resolve(_currentContent) : null);
        }

        #endregion
    }
}