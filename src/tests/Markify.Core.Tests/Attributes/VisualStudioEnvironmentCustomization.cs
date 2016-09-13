using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Moq;
using Ploeh.AutoFixture;

namespace Markify.Core.Tests.Attributes
{
    public class VisualStudioEnvironmentCustomization : ICustomization
    {
        #region Fields

        private const string Root = "c:";

        private readonly string _solutionName;
        private readonly string _solutionRoot;
        private readonly int _projectCount;
        private readonly int _solutionFolderCount;
        private readonly int _filesCount;
        private readonly int _foldersCount;
        private readonly bool _hasCurrentProject;
        private readonly string _language;
        private readonly IEnumerable<string> _fileExtensions;

        #endregion

        #region Constructors

        public VisualStudioEnvironmentCustomization(string solutionName, string solutionRoot, int projectCount, 
            int solutionFolderCount, int filesCount, int foldersCount, bool hasCurrentProject, string language, string[] fileExtensions)
        {
            _solutionName = solutionName;
            _solutionRoot = solutionRoot ?? Root;
            _projectCount = projectCount;
            _solutionFolderCount = solutionFolderCount;
            _filesCount = filesCount;
            _foldersCount = foldersCount;
            _hasCurrentProject = hasCurrentProject;
            _language = language ?? CodeModelLanguageConstants.vsCMLanguageCSharp;
            _fileExtensions = fileExtensions ?? new string[0];
        }

        #endregion

        #region Customization

        private DTE2 CreateVisualStudioInterface()
        {
            var visualStudio = new Mock<DTE2>();
            var solution = CreateSolution();
            visualStudio.SetupGet(c => c.Solution).Returns(solution);

            var projects = Array.Empty<Project>();
            if (_hasCurrentProject && solution != null)
            {
                projects = solution.Projects.Cast<Project>()
                                            .Where(c => c.Kind != ProjectKinds.vsProjectKindSolutionFolder)
                                            .Take(1)
                                            .ToArray();
            }
            visualStudio.SetupGet(c => c.ActiveSolutionProjects).Returns(projects);

            return visualStudio.Object;
        }

        private Solution CreateSolution()
        {
            if (string.IsNullOrWhiteSpace(_solutionName))
                return null;

            var filename = $"{_solutionName}.sln";
            var solutionPath = $"{_solutionRoot}/{_solutionName}";
            var solution = new Mock<Solution>();
            solution.SetupGet(c => c.FileName).Returns(filename);
            solution.SetupGet(c => c.FullName).Returns($"{solutionPath}/{filename}");

            var projects = new Mock<Projects>();
            solution.SetupGet(c => c.Projects).Returns(projects.Object);

            var solutionContent = CreateSolutionContent(solutionPath);
            projects.As<IEnumerable>().Setup(c => c.GetEnumerator()).Returns(() => solutionContent.GetEnumerator());

            return solution.Object;
        }

        private IEnumerable<Project> CreateSolutionContent(string root)
        {
            var topProjects = CreateProjects(root);
            var nestedProjects = CreateSolutionFolder(root);

            return topProjects.Concat(nestedProjects).ToArray();
        }

        private IEnumerable<Project> CreateSolutionFolder(string root)
        {
            var folders = new List<Project>(_solutionFolderCount);
            for (var i = 0; i < _solutionFolderCount; i++)
            {
                var folder = new Mock<Project>();
                var folderName = Guid.NewGuid().ToString();
                var folderPath = $"{root}/{folderName}/";
                folder.SetupGet(c => c.FullName).Returns(folderPath);
                folder.SetupGet(c => c.Kind).Returns(ProjectKinds.vsProjectKindSolutionFolder);

                var projectItems = new Mock<ProjectItems>();
                var subProjects = CreateProjects(folderPath);
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

        private IEnumerable<Project> CreateProjects(string root)
        {
            var fileFactories = new []{
                new VisualStudioMockHelper.FilesFactory(_filesCount, _fileExtensions),
                new VisualStudioMockHelper.FilesFactory(_filesCount, nameBuilder: () => null)
            };
            var projectFactories = new []{
                new VisualStudioMockHelper.ProjectFactory(_foldersCount, _language, filesBuilder: fileFactories),
                new VisualStudioMockHelper.ProjectFactory(_foldersCount, _language, pathBuilder: () => null),
                new VisualStudioMockHelper.ProjectFactory(_foldersCount, _language, () => null)
            };

            return projectFactories.Aggregate(Enumerable.Empty<Project>(), (acc, c) => acc.Concat(c.Build(_projectCount, root)))
                                   .ToArray();
        }

        public void Customize(IFixture fixture)
        {
            fixture.Inject(CreateVisualStudioInterface());
        }

        #endregion
    }
}