using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using EnvDTE;
using EnvDTE80;

namespace Markify.Core.IDE.VisualStudio
{
    public sealed class VSEnvironment : IIDEEnvironment
    {
        #region Fields

        private readonly DTE2 _visualStudio;

        #endregion

        #region Properties

        public string CurrentProject
        {
            get
            {
                var projects = _visualStudio.ActiveSolutionProjects as Project[];

                return projects.Length > 0 ? projects[0].FullName : null;
            }
        }

        public string CurrentSolution => Path.GetFileNameWithoutExtension(_visualStudio.Solution.FullName);

        #endregion

        #region Constructors

        public VSEnvironment(DTE2 visualStudio)
        {
            _visualStudio = visualStudio;
        }

        #endregion

        #region Helpers

        private static IEnumerable<Project> GetProjects(Solution solution)
        {
            return solution.Projects.OfType<Project>();
        }

        private static Project GetProject(string name, Solution solution)
        {
            return GetProjects(solution).FirstOrDefault(c => Path.GetFileNameWithoutExtension(name) == name);
        }

        private static IEnumerable<ProjectItem> GetProjectItems(Project project)
        {
            return project.ProjectItems.OfType<ProjectItem>();
        }

        #endregion

        #region Methods

        public Uri GetSolutionPath(string solution)
        {
            if (CurrentSolution != solution)
                return null;

            if (_visualStudio.Solution == null)
                return null;

            return new Uri(Path.GetDirectoryName(_visualStudio.Solution.FullName));
        }

        public IEnumerable<string> GetProjects(string solution)
        {
            if (CurrentSolution != solution)
                return null;

            return GetProjects(_visualStudio.Solution).Select(c => Path.GetFileNameWithoutExtension(c.FullName));
        }

        public Uri GetProjectPath(string solution, string name)
        {
            if (CurrentSolution != solution)
                return null;

            var project = GetProject(name, _visualStudio.Solution);

            return project == null ? null : new Uri(Path.GetDirectoryName(project.FullName));
        }

        public IEnumerable<Uri> GetProjectFiles(string solution, string name)
        {
            if (CurrentSolution != solution)
                return null;

            var project = GetProject(name, _visualStudio.Solution);
            if (project == null)
                return null;

            return GetProjectItems(project).SelectMany(
                c => Enumerable.Range(0, c.FileCount).Select(d => new Uri(c.FileNames[(short)d]))
            );
        }

        #endregion
    }
}