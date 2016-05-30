using System;
using System.Collections.Generic;

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

        #endregion
    }
}