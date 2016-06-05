using System.Collections.Generic;

using static Markify.Models.Document;
using static Markify.Models.Definitions;

namespace Markify.Core.Processors
{
    public interface IDocumentProcessor
    {
        #region Methods

        TableOfContent Process(IEnumerable<LibraryDefinition> library, DocumentSetting setting);

        #endregion
    }
}
