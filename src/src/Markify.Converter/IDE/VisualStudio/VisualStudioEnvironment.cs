using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using EnvDTE;
using EnvDTE80;

namespace Markify.Core.IDE.VisualStudio
{
    public sealed class VisualStudioEnvironment : IIDEEnvironment
    {
        #region Fields

        private readonly DTE2 _visualStudio;

        #endregion

        #region Properties

        public string CurrentProject
        {
            get
            {
                var projects = (Array)_visualStudio.ActiveSolutionProjects;

                return projects.Length > 0 ? ((Project)projects.GetValue(0)).FullName : null;
            }
        }

        public string CurrentSolution => Path.GetFileNameWithoutExtension(_visualStudio.Solution.FullName);

        #endregion

        #region Constructors

        public VisualStudioEnvironment(DTE2 visualStudio)
        {
            _visualStudio = visualStudio;
        }

        #endregion

        #region Helpers

        private static IEnumerable<Project> GetProjects(Solution solution)
        {
            var projects = new Queue<Project>(solution.Projects.Cast<Project>());
            while (projects.Count > 0)
            {
                var current = projects.Dequeue();
                if (current.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    var subProjects = current.ProjectItems.Cast<ProjectItem>()
                                                          .Select(c => c.SubProject)
                                                          .Where(c => c != null);
                    foreach (var sub in subProjects)
                        projects.Enqueue(sub);
                }
                else
                    yield return current;
            }
        }

        private static Project GetProject(string name, Solution solution)
        {
            return GetProjects(solution).FirstOrDefault(c => Path.GetFileNameWithoutExtension(name) == name);
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
                yield break;

            var project = GetProject(name, _visualStudio.Solution);
            if (project == null)
                yield break;

            var files = new Queue<ProjectItem>(project.ProjectItems.Cast<ProjectItem>());
            while (files.Count > 0)
            {
                var current = files.Dequeue();
                switch (current.Kind)
                {
                    case Constants.vsProjectItemKindPhysicalFolder:
                    case Constants.vsProjectItemKindVirtualFolder:
                        foreach (var item in current.ProjectItems)
                            files.Enqueue((ProjectItem)item);
                        break;

                    case Constants.vsProjectItemKindPhysicalFile:
                        yield return new Uri(current.FileNames[0]);
                        break;
                }
            }
        }

        #endregion
    }
}