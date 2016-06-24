using Markify.Models.Documents;

namespace Markify.Services.Settings
{
    internal sealed class DocumentSettingsProvider : IDocumentSettingsProvider
    {
        #region Methods

        public DocumentSetting GetSettings() => new DocumentSetting(".md");

        #endregion
    }
}