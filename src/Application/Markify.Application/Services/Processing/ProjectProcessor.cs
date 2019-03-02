using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Markify.Application.Services.Settings;
using Markify.Domain.Compiler;
using Markify.Domain.Document;

namespace Markify.Application.Services.Processing
{
    internal sealed class ProjectProcessor : IProjectProcessor
    {
        #region Nested

        private class FileContent : IProjectContent
        {
            #region Fields

            private readonly string _path;

            #endregion

            #region Properties

            public string Content => File.Exists(_path) ? File.ReadAllText(_path) : string.Empty;

            public ProjectLanguage Language { get; }

            #endregion

            #region Constructors

            public FileContent(string path, ProjectLanguage language)
            {
                _path = path;
                Language = language;
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly IProjectAnalyzer _analyzer;
        private readonly IDocumentOrganizer _organizer;
        private readonly IDocumentSettingsProvider _settingsProvider;

        #endregion

        #region Constructors

        public ProjectProcessor(IProjectAnalyzer analyzer, IDocumentOrganizer organizer, IDocumentSettingsProvider settingsProvider)
        {
            _analyzer = analyzer;
            _organizer = organizer;
            _settingsProvider = settingsProvider;
        }

        #endregion

        #region Processing

        public TableOfContent Process(IEnumerable<Domain.Ide.Project> projects, Uri root)
        {
            var libraries = projects
                .Aggregate(ImmutableArray.Create<AssemblyDefinition>(), (acc, c) =>
                {
                    var files = c.Files.Aggregate(ImmutableArray.Create<FileContent>(),
                        (acc2, d) =>
                        {
                            var extension = Path.GetExtension(d.AbsolutePath);
                            switch (extension)
                            {
                                case ".cs":
                                    return acc2.Add(new FileContent(d.AbsolutePath, ProjectLanguage.CSharp));
                                case ".vb":
                                    return acc2.Add(new FileContent(d.AbsolutePath, ProjectLanguage.VisualBasic));
                                default:
                                    return acc2;
                            }
                        });
                    var project = new Domain.Compiler.Project(c.Name, files);

                    return acc.Add(_analyzer.Analyze(project));
                });

            return _organizer.Organize(libraries, root, _settingsProvider.GetSettings());
        }

        #endregion
    }
}