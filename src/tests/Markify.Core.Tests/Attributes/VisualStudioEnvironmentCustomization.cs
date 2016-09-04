using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Markify.Core.IDE.VisualStudio;
using Moq;
using Ploeh.AutoFixture;

namespace Markify.Core.Tests.Attributes
{
    public class VisualStudioEnvironmentCustomization : ICustomization
    {
        #region Fields

        private const string Root = "c:";

        private readonly Solution _solution;

        #endregion

        #region Constructors

        public VisualStudioEnvironmentCustomization(string name, int projectCount, int solutionFolder)
        {
            _solution = CreateSolution(name, Root, projectCount, solutionFolder);
        }

        #endregion

        #region Customization

        private static Solution CreateSolution(string name, string root, int projectCount, int solutionFolder)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var solution = new Mock<Solution>();
            solution.SetupGet(c => c.FullName).Returns($"{root}/{name}.sln");

            var projects = new Mock<Projects>();
            var allProjects = CreateAllProjects(projectCount, solutionFolder, root);
            projects.As<IEnumerable>().Setup(c => c.GetEnumerator()).Returns(() => allProjects.GetEnumerator());
            solution.SetupGet(c => c.Projects).Returns(projects.Object);

            return solution.Object;
        }

        private static Project[] CreateAllProjects(int projectCount, int folderCount, string root)
        {
            var p = CreateProjects(projectCount, root);
            var s = CreateSolutionFolder(folderCount, projectCount, root);

            return p.Concat(s).ToArray();
        }

        private static IEnumerable<Project> CreateProjects(int count, string root)
        {
            var projects = new Project[count];
            for (var i = 0; i < count; i++)
            {
                var project = new Mock<Project>();
                var projectName = Guid.NewGuid().ToString();
                var projectPath = $"{root}/{projectName}";
                project.SetupGet(c => c.Name).Returns(projectName);
                project.SetupGet(c => c.FullName).Returns(projectPath);

                projects[i] = project.Object;
            }

            return projects;
        }

        private static IEnumerable<Project> CreateSolutionFolder(int count, int projectCount, string root)
        {
            var folders = new Project[count];
            for (var i = 0; i < count; i++)
            {
                var folder = new Mock<Project>();
                var folderName = Guid.NewGuid().ToString();
                var folderPath = $"{root}/{folderName}/";
                folder.SetupGet(c => c.FullName).Returns(folderPath);
                folder.SetupGet(c => c.Kind).Returns(ProjectKinds.vsProjectKindSolutionFolder);

                var projectItems = new Mock<ProjectItems>();
                var subProjects = CreateProjects(projectCount, folderPath);
                var items = subProjects.Select(c =>
                {
                    var projectItem = new Mock<ProjectItem>();
                    projectItem.SetupGet(d => d.SubProject).Returns(c);

                    return projectItem.Object;
                }).ToArray();
                projectItems.As<IEnumerable>().Setup(c => c.GetEnumerator()).Returns(() => items.GetEnumerator());
                folder.SetupGet(c => c.ProjectItems).Returns(projectItems.Object);

                folders[i] = folder.Object;
            }

            return folders;
        }

        public void Customize(IFixture fixture)
        {
            var vsEnv = new Mock<DTE2>();
            vsEnv.SetupGet(c => c.Solution).Returns(_solution);

            fixture.Inject(new VisualStudioEnvironment(vsEnv.Object));
        }

        #endregion
    }
}