using System.Collections.Generic;

using Markify.Models.IDE;
using Markify.Models.Documents;
using Markify.Models.Definitions;

namespace Markify.Core.Processors
{
    public interface IDocumentOrganizer
    {
        #region Methods

        TableOfContent Organize(IEnumerable<LibraryDefinition> library, Solution solution, DocumentSetting setting);

        #endregion
    }
}
