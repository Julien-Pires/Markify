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

        public VisualStudioEnvironmentCustomization(string name, int projectCount, int solutionFolder, int files, int folders)
        {
            _solution = CreateSolution(name, projectCount, solutionFolder, files, folders, Root);
        }

        #endregion

        #region Customization

        private static Solution CreateSolution(string name, int projectCount, int solutionFolder, int files, int folders, string root)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var solution = new Mock<Solution>();
            solution.SetupGet(c => c.FullName).Returns($"{root}/{name}.sln");

            var projects = new Mock<Projects>();
            solution.SetupGet(c => c.Projects).Returns(projects.Object);

            var solutionContent = CreateSolutionContent(projectCount, solutionFolder, files, folders, root);
            projects.As<IEnumerable>().Setup(c => c.GetEnumerator()).Returns(() => solutionContent.GetEnumerator());

            return solution.Object;
        }

        private static List<Project> CreateSolutionContent(int projectCount, int solutionFolder, int files, int folders, string root)
        {
            Func<string, IEnumerable<Project>> projectBuilder = c => CreateProjects(projectCount, files, folders, c);
            var topProjects = projectBuilder(root);
            var nestedProjects = CreateSolutionFolder(solutionFolder, projectBuilder, root);

            return topProjects.Concat(nestedProjects).ToList();
        }

        private static IEnumerable<Project> CreateProjects(int count, int files, int folders, string root)
        {
            var projects = new List<Project>(count);
            for (var i = 0; i < count; i++)
            {
                var project = new Mock<Project>();
                var projectName = Guid.NewGuid().ToString();
                var projectPath = $"{root}/{projectName}";
                project.SetupGet(c => c.Name).Returns(projectName);
                project.SetupGet(c => c.FullName).Returns(projectPath);
                project.SetupGet(c => c.ProjectItems).Returns(CreateProjectContent(files, folders, projectPath));

                projects.Add(project.Object);
            }

            return projects;
        }

        private static ProjectItems CreateProjectContent(int files, int folders, string root)
        {
            var items = new Mock<ProjectItems>();
            var content = new List<ProjectItem>();
            items.As<IEnumerable>().Setup(c => c.GetEnumerator()).Returns(() => content.GetEnumerator());

            content.AddRange(CreateFiles(files, root));
            content.AddRange(CreateFolders(folders, files, root));

            return items.Object;
        }

        private static IEnumerable<ProjectItem> CreateFiles(int count, string root)
        {
            var files = new List<ProjectItem>();
            for (int i = 0; i < count; i++)
            {
                var file = new Mock<ProjectItem>();
                file.SetupGet(c => c.Kind).Returns(Constants.vsProjectItemKindPhysicalFile);
                file.SetupGet(c => c.get_FileNames(It.IsAny<short>())).Returns($"{root}/File{i}.cs");
                files.Add(file.Object);
            }

            return files;
        }

        private static IEnumerable<ProjectItem> CreateFolders(int count, int files, string root)
        {
            var folders = new List<ProjectItem>();
            for (int i = 0; i < count; i++)
            {
                var folder = new Mock<ProjectItem>();
                var folderItems = new Mock<ProjectItems>();
                folder.SetupGet(c => c.Kind).Returns(Constants.vsProjectItemKindPhysicalFolder);
                folder.SetupGet(c => c.ProjectItems).Returns(folderItems.Object);

                var folderFiles = CreateFiles(files, $"{root}/Folder{i}");
                folderItems.As<IEnumerable>().Setup(c => c.GetEnumerator()).Returns(() => folderFiles.GetEnumerator());

                folders.Add(folder.Object);
            }

            return folders;
        }

        private static IEnumerable<Project> CreateSolutionFolder(int count, Func<string, IEnumerable<Project>> projectBuilder, string root)
        {
            var folders = new List<Project>(count);
            for (var i = 0; i < count; i++)
            {
                var folder = new Mock<Project>();
                var folderName = Guid.NewGuid().ToString();
                var folderPath = $"{root}/{folderName}/";
                folder.SetupGet(c => c.FullName).Returns(folderPath);
                folder.SetupGet(c => c.Kind).Returns(ProjectKinds.vsProjectKindSolutionFolder);

                var projectItems = new Mock<ProjectItems>();
                var subProjects = projectBuilder(folderPath);
                var items = subProjects.Select(c =>
                {
                    var projectItem = new Mock<ProjectItem>();
                    projectItem.SetupGet(d => d.SubProject).Returns(c);

                    return projectItem.Object;
                }).ToArray();
                projectItems.As<IEnumerable>().Setup(c => c.GetEnumerator()).Returns(() => items.GetEnumerator());
                folder.SetupGet(c => c.ProjectItems).Returns(projectItems.Object);

                folders.Add(folder.Object);
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