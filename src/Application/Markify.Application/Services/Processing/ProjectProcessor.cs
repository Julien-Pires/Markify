using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Markify.Application.Services.Settings;
using Markify.Domain.Compiler;
using Markify.Domain.Document;
using Markify.Domain.Ide;

namespace Markify.Application.Services.Processing
{
    internal sealed class ProjectProcessor : IProjectProcessor
    {
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

        public TableOfContent Process(IEnumerable<Project> projects, Uri root)
        {
            var libraries = projects.Aggregate(
                ImmutableArray.Create<AssemblyDefinition>(),
                (acc, c) => acc.Add(_analyzer.Analyze(c))
            );

            return _organizer.Organize(libraries, root, _settingsProvider.GetSettings());
        }

        #endregion
    }
}