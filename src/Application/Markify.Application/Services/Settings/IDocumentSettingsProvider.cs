using Markify.Domain.Document;

namespace Markify.Application.Services.Settings
{
    internal interface IDocumentSettingsProvider
    {
        #region Methods

        DocumentSetting GetSettings();

        #endregion
    }
}