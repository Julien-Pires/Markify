using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Core.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class VisualStudioEnvironmentDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public VisualStudioEnvironmentDataAttribute(string name, int projectCount, int solutionFolder, params object[] values)
            : this(new VisualStudioEnvironmentCustomization(name, projectCount, solutionFolder, 0, 0, false, string.Empty), values)
        {
        }

        public VisualStudioEnvironmentDataAttribute(string name, int projectCount, int solutionFolder, string language, params object[] values)
            : this(new VisualStudioEnvironmentCustomization(name, projectCount, solutionFolder, 0, 0, false, language), values)
        {
        }

        public VisualStudioEnvironmentDataAttribute(string name, int projectCount, int solutionFolder, bool hasCurrentProject, params object[] values)
            : this(new VisualStudioEnvironmentCustomization(name, projectCount, solutionFolder, 0, 0, hasCurrentProject, string.Empty), values)
        {
        }

        public VisualStudioEnvironmentDataAttribute(string name, int projectCount, int solutionFolder, int files, int folders, params object[] values)
            : this(new VisualStudioEnvironmentCustomization(name, projectCount, solutionFolder, files, folders, false, string.Empty), values)
        {
        }

        private VisualStudioEnvironmentDataAttribute(ICustomization customization, params object[] values)
            : base(new AutoDataAttribute(new Fixture().Customize(customization)), values)
        {
        }

        #endregion
    }
}