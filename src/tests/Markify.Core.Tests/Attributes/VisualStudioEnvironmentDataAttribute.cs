using System;
using System.Linq;
using System.Collections.Generic;
using Markify.Models.IDE;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Core.Tests.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class VisualStudioEnvironmentDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public VisualStudioEnvironmentDataAttribute(string solution = "Foo", string folder = null, int project = 0,
            int solutionFolder = 0, bool hasCurrentProject = false, int files = 0, int fileFolders = 0, string language = null, 
            string[] extensions = null, string[] allowedExtensions = null, object[] supportedLanguages = null, 
            object[] values = null)
            : this(new ICustomization[] {
                new VisualStudioEnvironmentCustomization(solution, folder, project, solutionFolder, files, fileFolders,
                    hasCurrentProject, language, extensions),
                new ProjectFilterCustomization(allowedExtensions, supportedLanguages?.Cast<ProjectLanguage>())
            }, values ?? new object[0])
        {
        }

        private VisualStudioEnvironmentDataAttribute(IEnumerable<ICustomization> customizations, params object[] values)
            : base(new AutoDataAttribute(new Fixture().Customize(new CompositeCustomization(customizations))), values)
        {
        }

        #endregion
    }
}