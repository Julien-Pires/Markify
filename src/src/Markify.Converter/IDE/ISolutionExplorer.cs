using System;
using System.Collections.Generic;

using static Markify.Models.Context;

namespace Markify.Core.IDE
{
    public interface ISolutionExplorer
    {
        #region Properties

        string Name { get; }

        Uri Root { get; }

        #endregion

        #region Methods

        IEnumerable<Project> GetProjects(bool withSourceOnly = false);

        Project GetProject(string name, bool withSourceOnly = false);

        #endregion
    }
}