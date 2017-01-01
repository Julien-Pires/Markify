using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using Moq;

namespace Markify.Services.VisualStudio.Tests.Attributes
{
    internal sealed class VisualStudioMockHelper
    {
        internal class ProjectFactory
        {
            #region Fields

            private readonly int _foldersCount;
            private readonly Func<string> _nameBuilder;
            private readonly Func<string> _pathBuilder;
            private readonly string _language;
            private readonly IEnumerable<FilesFactory> _filesBuilder;

            #endregion

            #region Constructors

            public ProjectFactory(int foldersCount = 0, string language = "", Func<string> nameBuilder = null,
                Func<string> pathBuilder = null, IEnumerable<FilesFactory> filesBuilder = null)
            {
                _foldersCount = foldersCount;
                _language = language;
                _nameBuilder = nameBuilder;
                _pathBuilder = pathBuilder;
                _filesBuilder = filesBuilder ?? new FilesFactory[0];
            }

            #endregion

            #region Builder

            public IEnumerable<Project> Build(int count, string root)
            {
                var projects = new List<Project>(count);
                for (var i = 0; i < count; i++)
                {
                    var project = new Mock<Project>();
                    var projectName = Guid.NewGuid().ToString();
                    project.SetupGet(c => c.Name).Returns(_nameBuilder == null ? projectName : _nameBuilder());

                    var projectPath = $"{root}/{projectName}";
                    project.SetupGet(c => c.FullName).Returns(_pathBuilder == null ? projectPath : _pathBuilder());

                    var content = CreateProjectContent(projectPath);
                    project.SetupGet(c => c.ProjectItems).Returns(content);

                    var codeModel = new Mock<CodeModel>();
                    codeModel.SetupGet(c => c.Language).Returns(_language);
                    project.SetupGet(c => c.CodeModel).Returns(codeModel.Object);

                    projects.Add(project.Object);
                }

                return projects;
            }

            private ProjectItems CreateProjectContent(string root)
            {
                var items = new Mock<ProjectItems>();
                var content = new List<ProjectItem>();
                items.As<IEnumerable>().Setup(c => c.GetEnumerator()).Returns(() => content.GetEnumerator());

                content.AddRange(CreateFolders(root));
                content.AddRange(CreateFiles(root));

                return items.Object;
            }

            private IEnumerable<ProjectItem> CreateFolders(string root)
            {
                var folders = new List<ProjectItem>();
                for (int i = 0; i < _foldersCount; i++)
                {
                    var folder = new Mock<ProjectItem>();
                    var folderItems = new Mock<ProjectItems>();
                    folder.SetupGet(c => c.Kind).Returns(Constants.vsProjectItemKindPhysicalFolder);
                    folder.SetupGet(c => c.ProjectItems).Returns(folderItems.Object);

                    var folderFiles = CreateFiles($"{root}/Folder{i}");
                    folderItems.As<IEnumerable>().Setup(c => c.GetEnumerator()).Returns(() => folderFiles.GetEnumerator());

                    folders.Add(folder.Object);
                }

                return folders;
            }

            private IEnumerable<ProjectItem> CreateFiles(string root) =>
                _filesBuilder.Aggregate(Enumerable.Empty<ProjectItem>(), (acc, c) => c.Build(root).Concat(acc));

            #endregion
        }

        internal class FilesFactory
        {
            #region Fields

            private readonly int _filesCount;
            private readonly IEnumerable<string> _extensions;
            private readonly Func<string> _nameBuilder;

            #endregion

            #region Constructors

            public FilesFactory(int filesCount, IEnumerable<string> extensions = null, Func<string> nameBuilder = null)
            {
                _filesCount = filesCount;
                _extensions = extensions ?? new string[0];
                _nameBuilder = nameBuilder;
            }

            #endregion

            #region Builder

            public IEnumerable<ProjectItem> Build(string root)
            {
                var files = new List<ProjectItem>();
                foreach (var ext in _extensions)
                {
                    for (var i = 0; i < _filesCount; i++)
                    {
                        var file = new Mock<ProjectItem>();
                        file.SetupGet(c => c.Kind).Returns(Constants.vsProjectItemKindPhysicalFile);
                        file.SetupGet(c => c.get_FileNames(It.IsAny<short>()))
                            .Returns(_nameBuilder == null ? $"{root}/File{i}.{ext}" : _nameBuilder());
                        files.Add(file.Object);
                    }
                }

                return files;
            }

            #endregion
        }
    }
}