using System;
using System.IO;
using System.Linq;
using System.Collections.Immutable;
using Markify.Models.IDE;
using EnvDTE80;
using Optional;

using VSProject = EnvDTE.Project;
using Project = Markify.Models.IDE.Project;

namespace Markify.Core.IDE.VisualStudio
{
    public sealed class VisualStudioEnvironment : IIDEEnvironment
    {
        #region Fields

        private readonly DTE2 _visualStudio;
        private readonly IProjectFilterProvider _filterProvider;

        #endregion

        #region Properties

        public Option<Solution> CurrentSolution
        {
            get
            {
                var solution = _visualStudio.Solution;
                if (solution == null)
                    return Option.None<Solution>();

                var name = Path.GetFileNameWithoutExtension(solution.FileName);
                var path = Path.GetDirectoryName(solution.FullName);
                var projects = solution.GetProjects()
                    .Where(IsValidProject)
                    .Select(CreateProject)
                    .ToImmutableList();

                return Option.Some(new Solution(name, new Uri($"{path}/"), projects));
            }
        }

        public Option<Project> CurrentProject
        {
            get
            {
                var projects = (Array)_visualStudio.ActiveSolutionProjects;

                return projects.Length > 0 ? CreateProject((VSProject) projects.GetValue(0)).Some() : default(Option<Project>);
            }
        }

        #endregion

        #region Constructors

        public VisualStudioEnvironment(DTE2 visualStudio, IProjectFilterProvider filterProvider)
        {
            _visualStudio = visualStudio;
            _filterProvider = filterProvider;
        }

        #endregion

        #region Methods

        private bool IsValidProject(VSProject project)
        {
            if (_filterProvider.SupportedLanguages.Any() &&
                !_filterProvider.SupportedLanguages.Contains(project.GetLanguage()))
                return false;

            return project.Name != null && project.FullName != null;
        }

        private bool IsValidFile(Uri file)
        {
            var ext = Path.GetExtension(file.OriginalString).Replace(".", "");

            return _filterProvider.AllowedExtensions.Contains(ext);
        }

        private Project CreateProject(VSProject project)
        {
            var path = project.GetPath();
            var files = project.GetFiles();
            if (_filterProvider.AllowedExtensions.Any())
                files = files.Where(IsValidFile);

            return new Project(project.Name, path, ProjectLanguage.Unsupported, files);
        }

        #endregion
    }
}