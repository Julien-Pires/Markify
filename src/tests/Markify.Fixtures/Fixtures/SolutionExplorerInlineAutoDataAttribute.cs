using System;

using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit2;

namespace Markify.Fixtures
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SolutionExplorerInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        #region Constructors

        public SolutionExplorerInlineAutoDataAttribute(
            string currentSolution, 
            string solutionPath,
            string[] projects,
            params object[] values) :
            this(currentSolution, solutionPath, null, projects, null, values)
        {
        }

        public SolutionExplorerInlineAutoDataAttribute(
            string currentProject, 
            params object[] values)
            : this(null, null, currentProject, null, null, values)
        {
        }

        public SolutionExplorerInlineAutoDataAttribute(
            string currentSolution,
            string project, 
            string path,
            string[] files,
            params object[] values) :
            this(currentSolution, null, null, new[] { project }, new [] { path }, files, values)
        {
        }

        private SolutionExplorerInlineAutoDataAttribute(
            string currentSolution,
            string solutionPath,
            string currentProject,
            string[] projects,
            string[] projectsPath,
            string[] files,
            params object[] values)
            : base(new AutoDataAttribute(
                    new Fixture().Customize(new SolutionExplorerCustomization())),
                values)
        {
        }

        #endregion
    }
}