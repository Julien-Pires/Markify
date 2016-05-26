using System;

namespace Markify.Core.IDE
{
    public interface ISolutionExplorer
    {
        #region Properties

        Uri Root { get; }

        string CurrentProject { get; }

        #endregion

        #region Methods

        void GetProjects(bool withSourceOnly = false);

        void GetProject(string name, bool withSourceOnly = false);

        #endregion
    }
}