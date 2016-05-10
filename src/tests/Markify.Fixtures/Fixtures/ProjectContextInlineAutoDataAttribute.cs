using System;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Fixtures
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ProjectContextInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public ProjectContextInlineAutoDataAttribute(string[] sourceFiles, params object[] values) 
            : base(new AutoDataAttribute(
                    new Fixture().Customize(new ProjectContextCustomization(sourceFiles))),
                values)
        {
        }

        #endregion
    }
}