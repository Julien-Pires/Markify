using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Markify.Core.Processors;

using static Markify.Models.Context;
using static Markify.Models.Document;
using static Markify.Models.Definitions;

namespace Markify.Services.Processing
{
    internal sealed class ProjectProcessor : IProjectProcessor
    {
        #region Fields

        private readonly IProjectAnalyzer _analyzer;
        private readonly IDocumentOrganizer _organizer;
        private readonly ISettingsProvider _settingsProvider;

        #endregion

        #region Constructors

        public ProjectProcessor(IProjectAnalyzer analyzer, IDocumentOrganizer organizer, ISettingsProvider settingsProvider)
        {
            _analyzer = analyzer;
            _organizer = organizer;
            _settingsProvider = settingsProvider;
        }

        #endregion

        #region Processing

        public TableOfContent Process(IEnumerable<Project> projects, Solution solution)
        {
            var libraries = projects.Aggregate(
                ImmutableArray.Create<LibraryDefinition>(),
                (acc, c) => acc.Add(_analyzer.Analyze(c))
            );

            return _organizer.Organize(libraries, solution, _settingsProvider.GetSettings());
        }

        #endregion
    }
}