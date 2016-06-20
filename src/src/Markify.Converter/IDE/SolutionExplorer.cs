using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Optional;

using static Markify.Models.Context;

namespace Markify.Core.IDE
{
    public sealed class SolutionExplorer : ISolutionExplorer
    {
        #region Fields

        private readonly IIDEEnvironment _ideEnv;
        private readonly ISolutionExplorerFilterProvider _filterProvider;

        #endregion

        #region Properties

        public Option<Solution> CurrentSolution
        {
            get
            {
                var name = _ideEnv.CurrentSolution;
                if (name == null)
                    return Option.None<Solution>();

                var supportedProjects = FilterSupportedProjects(_ideEnv.GetProjects(name) ?? new string[0]);

                return new Solution(name, _ideEnv.GetSolutionPath(name), ImmutableList.CreateRange(supportedProjects)).Some();
            }
        }

        public Option<string> CurrentProject
        {
            get
            {
                var project = _ideEnv.CurrentProject;
                if (project == null)
                    return default(Option<string>);

                var supportedProjects = FilterSupportedProjects(new[] { project }).ToArray();

                return supportedProjects.Any() ? supportedProjects.First().Some() : default(Option<string>);
            }
        }

        #endregion

        #region Constructors

        public SolutionExplorer(IIDEEnvironment ideEnv, ISolutionExplorerFilterProvider filterProvider)
        {
            if (ideEnv == null)
                throw new ArgumentNullException(nameof(ideEnv));

            if (filterProvider == null)
                throw new ArgumentNullException(nameof(filterProvider));

            _ideEnv = ideEnv;
            _filterProvider = filterProvider;
        }

        #endregion

        #region Methods

        public Option<Project> GetProject(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return default(Option<Project>);

            var solution = _ideEnv.CurrentSolution;
            if (solution == null)
                return default(Option<Project>);

            var projectPath = _ideEnv.GetProjectPath(solution, name);
            if (projectPath == null)
                return default(Option<Project>);

            var supportedProjects = FilterSupportedProjects(new[] { name });
            if (!supportedProjects.Any())
                return default(Option<Project>);

            return new Project
            ( 
                name,
                projectPath,
                _ideEnv.GetProjectLanguage(solution, name),
                ImmutableList.CreateRange(_ideEnv.GetProjectFiles(solution, name) ?? new Uri[0])
            ).Some();
        }

        private bool IsSupportedLanguage(ProjectLanguage language)
        {
            var filters = _filterProvider.Filters;

            return !filters.SupportedLanguages.Any() || filters.SupportedLanguages.Any(c => c == language);
        }

        private IEnumerable<string> FilterSupportedProjects(IEnumerable<string> projects)
        {
            return projects.Where(c => {
                var language = _ideEnv.GetProjectLanguage(_ideEnv.CurrentSolution, c);

                return IsSupportedLanguage(language);
            });
        }

        #endregion
    }
}