using Optional;
using Markify.Models.IDE;

namespace Markify.Core.IDE
{
    public interface IIDEEnvironment
    {
        #region Properties

        Option<Solution> CurrentSolution { get; }

        Option<Project> CurrentProject { get; }

        #endregion
    }
}