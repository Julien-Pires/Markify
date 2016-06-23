using Markify.Models.IDE;
using Markify.Models.Definitions;

namespace Markify.Core.Processors
{
    public interface IProjectAnalyzer
    {
        #region Methods

        LibraryDefinition Analyze(Project project);

        #endregion
    }
}