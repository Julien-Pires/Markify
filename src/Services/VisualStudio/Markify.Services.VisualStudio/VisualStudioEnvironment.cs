using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using EnvDTE80;
using Markify.Domain.Ide;
using Microsoft.FSharp.Core;

using VSProject = EnvDTE.Project;
using Project = Markify.Domain.Ide.Project;

namespace Markify.Services.VisualStudio
{
    public sealed class VisualStudioEnvironment : IIdeEnvironment
    {
        #region Fields

        private readonly DTE2 _visualStudio;
        private readonly IProjectFilterProvider _filterProvider;

        #endregion

        #region Properties

        public FSharpOption<Solution> CurrentSolution
        {
            get
            {
                var solution = _visualStudio.Solution;
                if (solution == null)
                    return FSharpOption<Solution>.None;

                var name = Path.GetFileNameWithoutExtension(solution.FileName);
                var path = Path.GetDirectoryName(solution.FullName);
                var projects = solution.GetProjects()
                    .Where(IsValidProject)
                    .Select(CreateProject)
                    .ToImmutableList();

                return FSharpOption<Solution>.Some(new Solution(name, new Uri($"{path}/"), projects));
            }
        }

        public FSharpOption<Project> CurrentProject
        {
            get
            {
                var projects = (Array)_visualStudio.ActiveSolutionProjects;

                return projects.Length > 0 ? 
                    FSharpOption<Project>.Some(CreateProject((VSProject) projects.GetValue(0))) 
                    : FSharpOption<Project>.None;
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