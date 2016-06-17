using System.Collections.Generic;

using static Markify.Models.Context;
using static Markify.Models.Document;
using static Markify.Models.Definitions;

namespace Markify.Core.Processors
{
    public interface IDocumentOrganizer
    {
        #region Methods

        TableOfContent Organize(IEnumerable<LibraryDefinition> library, Solution solution, DocumentSetting setting);

        #endregion
    }
}
