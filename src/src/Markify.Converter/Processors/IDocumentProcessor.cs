using static Markify.Models.Document;
using static Markify.Models.Definitions;

namespace Markify.Core.Processors
{
    public interface IDocumentProcessor
    {
        #region Methods

        TableOfContent Process(LibraryDefinition library);

        #endregion
    }
}
