using System;
using System.IO;
using System.Linq;

using Markify.Core.IDE;

using Moq;
using Ploeh.AutoFixture;

namespace Markify.Fixtures
{
    public class SolutionExplorerCustomization : ICustomization
    {
        #region Fields

        private readonly Mock<IIDEEnvironment> _ide;

        #endregion

        #region Constructors

        public SolutionExplorerCustomization(string solution, string root, int projectsCount, int currentProject, int filesPerProject)
        {
            _ide = new Mock<IIDEEnvironment>();
            _ide.SetupGet(c => c.CurrentSolution).Returns(solution);

            var solutionPath = new Uri(Path.Combine(root, solution));
            _ide.Setup(c => c.GetSolutionPath(It.Is<string>(d => d == solution))).Returns(solutionPath);

            var projects = Enumerable.Range(0, projectsCount).Select(c => $"Project{c + 1}");
            _ide.Setup(c => c.GetProjects(It.Is<string>(d => d == solution))).Returns(projects);
            _ide.Setup(c => c.GetProjectPath(It.IsIn(projects))).Returns<string>(c => new Uri(solutionPath, $"/{c}"));
            _ide.SetupGet(c => c.CurrentProject).Returns(() => currentProject > -1 ? projects.ElementAt(currentProject) : null);
        }

        #endregion

        #region Customize

        public void Customize(IFixture fixture)
        {
            fixture.Register<ISolutionExplorer>(() => new SolutionExplorer(_ide.Object));
        }

        #endregion
    }
}