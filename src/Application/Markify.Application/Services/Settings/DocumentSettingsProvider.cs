using Markify.Domain.Document;

namespace Markify.Application.Services.Settings
{
    internal sealed class DocumentSettingsProvider : IDocumentSettingsProvider
    {
        #region Methods

        public DocumentSetting GetSettings() => new DocumentSetting(".md");

        #endregion
    }
}