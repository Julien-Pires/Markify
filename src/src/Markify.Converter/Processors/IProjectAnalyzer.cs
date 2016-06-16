using static Markify.Models.Context;
using static Markify.Models.Definitions;

namespace Markify.Core.Processors
{
    public interface IProjectAnalyzer
    {
        #region Methods

        LibraryDefinition Analyze(Project project);

        #endregion
    }
}