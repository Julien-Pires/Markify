using System;
using System.Collections.Generic;

using static Markify.Models.Context;

namespace Markify.Core.IDE
{
    public interface IIDEEnvironment
    {
        #region Properties

        string CurrentSolution { get; }

        string CurrentProject { get; }

        #endregion

        #region Methods

        Uri GetSolutionPath(string solution);

        IEnumerable<string> GetProjects(string solution);

        Uri GetProjectPath(string solution, string project);

        IEnumerable<Uri> GetProjectFiles(string solution, string project);

        ProjectLanguage GetProjectLanguage(string solution, string project);

        #endregion
    }
}