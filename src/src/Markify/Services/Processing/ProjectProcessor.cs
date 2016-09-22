using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Markify.Core.Analyzers;
using Markify.Services.Settings;
using Markify.Models.IDE;
using Markify.Models.Documents;
using Markify.Models.Definitions;

namespace Markify.Services.Processing
{
    internal sealed class ProjectProcessor : IProjectProcessor
    {
        #region Fields

        private readonly IProjectAnalyzer _analyzer;
        private readonly IDocumentationOrganizer _organizer;
        private readonly IDocumentSettingsProvider _settingsProvider;

        #endregion

        #region Constructors

        public ProjectProcessor(IProjectAnalyzer analyzer, IDocumentationOrganizer organizer, IDocumentSettingsProvider settingsProvider)
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
                ImmutableArray.Create<LibraryDefinition>(),
                (acc, c) => acc.Add(_analyzer.Analyze(c))
            );

            return _organizer.Organize(libraries, root, _settingsProvider.GetSettings());
        }

        #endregion
    }
}