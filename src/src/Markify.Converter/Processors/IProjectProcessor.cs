using static Markify.Models.Context;
using static Markify.Models.Definitions;

namespace Markify.Core.Processors
{
    public interface IProjectProcessor
    {
        #region Methods

        LibraryDefinition Process(Project project);

        #endregion
    }
}