using System.Collections.Generic;
using Markify.Domain.Ide;
using Moq;
using Ploeh.AutoFixture;

namespace Markify.Services.VisualStudio.Tests.Attributes
{
    internal sealed class ProjectFilterCustomization : ICustomization
    {
        #region Fields

        private readonly IEnumerable<string> _allowedExtensions;
        private readonly IEnumerable<ProjectLanguage> _supportedLanguages;

        #endregion

        #region Constructors

        public ProjectFilterCustomization(IEnumerable<string> allowedExtensions, IEnumerable<ProjectLanguage> supportedLanguages)
        {
            _allowedExtensions = allowedExtensions ?? new string[0];
            _supportedLanguages = supportedLanguages ?? new ProjectLanguage[0];
        }

        #endregion

        #region Customization

        private IProjectFilterProvider CreateProvider()
        {
            var provider = new Mock<IProjectFilterProvider>();
            provider.SetupGet(c => c.AllowedExtensions).Returns(new HashSet<string>(_allowedExtensions));
            provider.SetupGet(c => c.SupportedLanguages).Returns(new HashSet<ProjectLanguage>(_supportedLanguages));

            return provider.Object;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Inject(CreateProvider());
        }

        #endregion
    }
}