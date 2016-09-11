﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using EnvDTE;
using EnvDTE80;

using Markify.Models.IDE;

using VSProject = EnvDTE.Project;
using VSSolution = EnvDTE.Solution;

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

                return projects.Length > 0 ? ((VSProject)projects.GetValue(0)).Name : null;
            }
        }

        public string CurrentSolution => Path.GetFileNameWithoutExtension(_visualStudio.Solution?.FullName);

        #endregion

        #region Constructors

        public VisualStudioEnvironment(DTE2 visualStudio)
        {
            _visualStudio = visualStudio;
        }

        #endregion

        #region Helpers

        private static IEnumerable<VSProject> GetProjects(VSSolution solution)
        {
            var projects = new Queue<VSProject>(solution.Projects.Cast<VSProject>());
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

        private static VSProject GetProject(string name, VSSolution solution)
        {
            return GetProjects(solution).FirstOrDefault(c => c.Name == name);
        }

        #endregion

        #region Methods

        public Uri GetSolutionPath(string solution)
        {
            if(solution == null)
                throw new ArgumentNullException(nameof(solution));

            if (CurrentSolution != solution)
                return null;

            var path = Path.GetDirectoryName(_visualStudio.Solution?.FullName);

            return path == null ? null : new Uri($"{path}/");
        }

        public IEnumerable<string> GetProjects(string solution)
        {
            if(solution == null)
                throw new ArgumentNullException(nameof(solution));

            return CurrentSolution != solution ? null : GetProjects(_visualStudio.Solution).Select(c => c.Name);
        }

        public Uri GetProjectPath(string solution, string name)
        {
            if(solution == null)
                throw new ArgumentNullException(nameof(solution));

            if(name == null)
                throw new ArgumentNullException(nameof(name));

            if (CurrentSolution != solution)
                return null;

            var project = GetProject(name, _visualStudio.Solution);

            return project != null ? new Uri(Path.GetDirectoryName(project.FullName)) : null;
        }

        public IEnumerable<Uri> GetProjectFiles(string solution, string name)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            var files = new List<Uri>();
            if (CurrentSolution != solution)
                return files;

            var project = GetProject(name, _visualStudio.Solution);
            if (project == null)
                return files;

            var pendingItems = new Queue<ProjectItem>(project.ProjectItems.Cast<ProjectItem>());
            while (pendingItems.Count > 0)
            {
                var current = pendingItems.Dequeue();
                switch (current.Kind)
                {
                    case Constants.vsProjectItemKindPhysicalFolder:
                    case Constants.vsProjectItemKindVirtualFolder:
                        foreach (var item in current.ProjectItems)
                            pendingItems.Enqueue((ProjectItem)item);
                        break;

                    case Constants.vsProjectItemKindPhysicalFile:
                        files.Add(new Uri(current.FileNames[0]));
                        break;
                }
            }

            return files;
        }

        public ProjectLanguage GetProjectLanguage(string solution, string name)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (CurrentSolution != solution)
                return ProjectLanguage.Unsupported;

            var project = GetProject(name, _visualStudio.Solution);
            if (project?.CodeModel == null)
                return ProjectLanguage.Unsupported;

            ProjectLanguage result;
            switch(project.CodeModel.Language)
            {
                case CodeModelLanguageConstants.vsCMLanguageCSharp:
                    result = ProjectLanguage.CSharp;
                    break;

                case CodeModelLanguageConstants.vsCMLanguageVB:
                    result = ProjectLanguage.VisualBasic;
                    break;

                default:
                    result = ProjectLanguage.Unsupported;
                    break;
            }

            return result;
        }

        #endregion
    }
}