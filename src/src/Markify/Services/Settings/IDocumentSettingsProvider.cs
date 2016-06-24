using Markify.Models.Documents;

namespace Markify.Services.Settings
{
    internal interface IDocumentSettingsProvider
    {
        #region Methods

        DocumentSetting GetSettings();

        #endregion
    }
}