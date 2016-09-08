using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Rendering.T4.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TemplateDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public TemplateDataAttribute(string value, params object[] values)
            : this(new TemplateCustomization(value, false), values)
        {
        }

        public TemplateDataAttribute(bool mustThrow, params object[] values)
            : this(new TemplateCustomization(string.Empty, mustThrow), values)
        {
        }

        public TemplateDataAttribute(ICustomization customization, params object[] values)
            : base(new AutoDataAttribute(new Fixture().Customize(customization)), values)
        {
        }

        #endregion
    }
}