using System;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Fixtures
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SolutionExplorerInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public SolutionExplorerInlineAutoDataAttribute(string solution, string root, int projectsCount = 0,
            int currentProject = -1, int filesPerProject = 0, params object[] values)
            : base(new AutoDataAttribute(
                    new Fixture().Customize(new SolutionExplorerCustomization(solution, root, projectsCount, 
                        currentProject, filesPerProject))
                ), values)
        {
        }

        #endregion
    }
}