using System.Collections.Generic;

using Markify.Models.IDE;
using Markify.Models.Documents;
using Markify.Models.Definitions;

namespace Markify.Core.Analyzers
{
    public interface IDocumentationOrganizer
    {
        #region Methods

        TableOfContent Organize(IEnumerable<LibraryDefinition> library, Solution solution, DocumentSetting setting);

        #endregion
    }
}
