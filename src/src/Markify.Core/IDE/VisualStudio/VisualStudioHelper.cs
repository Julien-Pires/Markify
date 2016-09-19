using System;
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
    internal static class VisualStudioHelper
    {
        #region Solution Extension

        public static IEnumerable<VSProject> GetProjects(this VSSolution solution)
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

        #endregion

        #region Project Extension

        public static Uri GetPath(this VSProject project)
        {
            var path = Path.GetDirectoryName(project?.FullName);

            return path == null ? null : new Uri(path);
        }

        public static IEnumerable<Uri> GetFiles(this VSProject project)
        {
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
                        yield return new Uri(current.FileNames[0]);
                        break;
                }
            }
        }

        public static ProjectLanguage GetLanguage(this VSProject project)
        {
            if (project?.CodeModel == null)
                return ProjectLanguage.Unsupported;

            ProjectLanguage result;
            switch (project.CodeModel.Language)
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
