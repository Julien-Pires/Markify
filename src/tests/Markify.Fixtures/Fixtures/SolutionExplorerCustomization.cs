using System;
using System.Linq;
using System.Collections.Generic;

using Markify.Core.IDE;

using Moq;
using Ploeh.AutoFixture;

using static Markify.Models.Context;

namespace Markify.Fixtures
{
    public class SolutionExplorerCustomization : ICustomization
    {
        #region Fields

        private readonly Mock<IIDEEnvironment> _ide;
        private readonly Mock<ISolutionExplorerFilterProvider> _filterProvider;

        #endregion

        #region Constructors

        public SolutionExplorerCustomization(
            string solution, 
            string root, 
            int projectsCount, 
            int currentProject, 
            int filesPerProject, 
            ProjectLanguage language,
            IEnumerable<ProjectLanguage> filteredLanguages)
        {
            _ide = new Mock<IIDEEnvironment>();
            _ide.SetupGet(c => c.CurrentSolution).Returns(solution);

            var solutionPath = (Uri)null;
            if (root != null)
            {
                if(!root.EndsWith("/"))
                    root += "/";

                solutionPath = new Uri(root);
            }

            _ide.Setup(c => c.GetSolutionPath(It.Is<string>(d => d == solution))).Returns(solutionPath);

            var projects = Enumerable.Range(0, projectsCount).Select(c => $"Project{c + 1}").ToArray();
            _ide.Setup(c => c.GetProjects(It.Is<string>(d => d == solution))).Returns(projects);

            _ide.Setup(c => c.GetProjectPath(It.IsAny<string>(), It.IsIn(projects)))
                .Returns<string, string>((s, p) => solutionPath != null ? new Uri(solutionPath, $"{p}/") : null);

            _ide.SetupGet(c => c.CurrentProject)
                .Returns(() => currentProject > -1 ? projects.ElementAt(currentProject) : null);

            var files = Enumerable.Range(0, filesPerProject).Select(c => $"File{c + 1}.cs");
            _ide.Setup(c => c.GetProjectFiles(It.IsAny<string>(), It.IsIn(projects)))
                .Returns<string, string>((s, p) => files.Select(f => new Uri(_ide.Object.GetProjectPath(It.IsAny<string>(), p), f)));

            _ide.Setup(c => c.GetProjectLanguage(It.IsAny<string>(), It.IsIn(projects)))
                .Returns<string, string>((s, p) => language);

            _filterProvider = new Mock<ISolutionExplorerFilterProvider>();
            _filterProvider.Setup(c => c.GetFilterRules())
                           .Returns(() => new SolutionExplorerFilter(filteredLanguages));

        }

        #endregion

        #region Customize

        public void Customize(IFixture fixture)
        {
            fixture.Register<ISolutionExplorer>(() => new SolutionExplorer(_ide.Object, _filterProvider.Object));
        }

        #endregion
    }
}