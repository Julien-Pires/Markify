using Markify.Models.Documents;

namespace Markify.Services.Settings
{
    internal sealed class SettingsProvider : ISettingsProvider
    {
        #region Methods

        public DocumentSetting GetSettings() => new DocumentSetting(".md");

        #endregion
    }
}