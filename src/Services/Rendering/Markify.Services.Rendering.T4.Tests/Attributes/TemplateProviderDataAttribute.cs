using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal sealed class TemplateProviderDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public TemplateProviderDataAttribute(Type[] templatesBinding, Type currentContent, params object[] values)
            : this(new TemplateProviderCustomization(templatesBinding, currentContent), values)
        {
        }

        public TemplateProviderDataAttribute(ICustomization customization, params object[] values)
            : base(new AutoDataAttribute(new Fixture().Customize(customization)), values)
        {
        }

        #endregion
    }
}