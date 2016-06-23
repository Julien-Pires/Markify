using Optional;

using Markify.Models.IDE;

namespace Markify.Core.IDE
{
    public interface ISolutionExplorer
    {
        #region Properties

        Option<Solution> CurrentSolution { get; }

        Option<string> CurrentProject { get; }

        #endregion

        #region Methods

        Option<Project> GetProject(string name);

        #endregion
    }
}