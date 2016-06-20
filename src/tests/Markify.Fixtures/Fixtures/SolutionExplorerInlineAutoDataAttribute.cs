using System;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

using static Markify.Models.Context;

namespace Markify.Fixtures
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SolutionExplorerInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public SolutionExplorerInlineAutoDataAttribute(
            string solution, 
            string root, 
            int projectsCount = 0,
            int currentProject = -1, 
            int filesPerProject = 0, 
            ProjectLanguage language = ProjectLanguage.Unsupported,
            ProjectLanguage[] filteredLanguages = null,
            params object[] values)
            : base(new AutoDataAttribute(
                    new Fixture().Customize(new SolutionExplorerCustomization(solution, root, projectsCount, 
                        currentProject, filesPerProject, language, filteredLanguages))
                ), values)
        {
        }

        #endregion
    }
}