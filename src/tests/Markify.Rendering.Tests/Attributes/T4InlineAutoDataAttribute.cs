using System;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Rendering.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class T4InlineAutoDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public T4InlineAutoDataAttribute(Type[] templatesBinding, Type currentContent, params object[] values)
            : base(new AutoDataAttribute(new Fixture().Customize(new T4Customization())), values)
        {
        }

        #endregion
    }
}