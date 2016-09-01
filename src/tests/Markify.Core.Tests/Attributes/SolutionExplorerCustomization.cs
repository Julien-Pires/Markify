using System;
using System.Linq;
using System.Collections.Generic;

using Markify.Core.IDE;
using Markify.Models.IDE;

using Moq;
using Ploeh.AutoFixture;

namespace Markify.Core.Tests.Attributes
{
    public class SolutionExplorerCustomization : ICustomization
    {
        #region Fields

        private readonly IIDEEnvironment _ide;
        private readonly ISolutionExplorerFilterProvider _filterProvider;

        #endregion

        #region Constructors

        public SolutionExplorerCustomization(
            string solution, 
            string root, 
            int projectsCount, 
            int currentProject, 
            int filesPerProject, 
            ProjectLanguage language,
            IEnumerable<ProjectLanguage> filteredLanguages,
            IEnumerable<string> filteredFiles)
        {
            _ide = CreateIDEEnvironment(solution, root, projectsCount, currentProject, filesPerProject, language);
            _filterProvider = CreateExplorerFilter(filteredLanguages, filteredFiles);
        }

        #endregion

        #region Customize

        private static IIDEEnvironment CreateIDEEnvironment(string solution, string root, int projectsCount, int currentProject,
            int filesPerProject, ProjectLanguage language)
        {
            var ide = new Mock<IIDEEnvironment>();
            ide.SetupGet(c => c.CurrentSolution).Returns(solution);

            var solutionPath = (Uri)null;
            if (root != null)
            {
                if (!root.EndsWith("/"))
                    root += "/";

                solutionPath = new Uri(root);
            }

            ide.Setup(c => c.GetSolutionPath(It.Is<string>(d => d == solution))).Returns(solutionPath);

            var projects = Enumerable.Range(0, projectsCount).Select(c => $"Project{c + 1}").ToArray();
            ide.Setup(c => c.GetProjects(It.Is<string>(d => d == solution))).Returns(projects);

            ide.Setup(c => c.GetProjectPath(It.IsAny<string>(), It.IsIn(projects)))
                .Returns<string, string>((s, p) => solutionPath != null ? new Uri(solutionPath, $"{p}/") : null);

            ide.SetupGet(c => c.CurrentProject)
                .Returns(() => currentProject > -1 ? projects.ElementAt(currentProject) : null);

            var fileExt = language == ProjectLanguage.CSharp ? "cs" : "vb";
            var files = Enumerable.Range(0, filesPerProject).Select(c => $"File{c + 1}.{fileExt}");
            ide.Setup(c => c.GetProjectFiles(It.IsAny<string>(), It.IsIn(projects)))
                .Returns<string, string>((s, p) => files.Select(f => new Uri(ide.Object.GetProjectPath(It.IsAny<string>(), p), f)));

            ide.Setup(c => c.GetProjectLanguage(It.IsAny<string>(), It.IsIn(projects)))
                .Returns<string, string>((s, p) => language);

            return ide.Object;
        }

        private static ISolutionExplorerFilterProvider CreateExplorerFilter(IEnumerable<ProjectLanguage> filteredLanguages, 
            IEnumerable<string> filteredFiles)
        {
            var filterProvider = new Mock<ISolutionExplorerFilterProvider>();
            filterProvider.SetupGet(c => c.Filters)
                           .Returns(() => new SolutionExplorerFilter(filteredLanguages, filteredFiles));

            return filterProvider.Object;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Inject(_ide);
            fixture.Inject(_filterProvider);
        }

        #endregion
    }
}