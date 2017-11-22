using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class PropertyDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public PropertyDataAttribute(string visibility = "private", string set = null, string get = null, params object[] values)
            : base(new AutoDataAttribute(new Fixture(new PropertyBuilder(visibility, set, get), new MultipleRelay())), values)
        {
        }

        #endregion
    }
}
