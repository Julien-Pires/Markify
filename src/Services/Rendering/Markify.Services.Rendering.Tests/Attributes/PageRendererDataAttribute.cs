using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Services.Rendering.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    internal class PageRendererDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public PageRendererDataAttribute(int pageCount, params object[] values)
            : this(new PageRendererCustomization(true, false, pageCount), values)
        {
        }

        public PageRendererDataAttribute(int pageCount, bool hasTemplate, bool withFailure, params object[] values)
            : this(new PageRendererCustomization(hasTemplate, withFailure, pageCount), values)
        {
        }

        public PageRendererDataAttribute(ICustomization customization, params object[] values)
            : base(new AutoDataAttribute(new Fixture().Customize(customization)), values)
        {
        }

        #endregion
    }
}