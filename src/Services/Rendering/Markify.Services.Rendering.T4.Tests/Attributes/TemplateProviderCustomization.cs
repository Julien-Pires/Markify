using System;
using System.Linq;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    internal sealed class TemplateProviderCustomization : ICustomization
    {
        #region Fields

        private readonly Type[] _templateBinding;
        private readonly Type _currentContent;

        #endregion

        #region Constructors

        public TemplateProviderCustomization(Type[] templateBinding, Type currentContent)
        {
            _templateBinding = templateBinding;
            _currentContent = currentContent;
        }

        #endregion

        #region Customization

        public void Customize(IFixture fixture)
        {
            fixture.Register(() =>
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