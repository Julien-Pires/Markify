using Markify.Models.Documents;

namespace Markify.Services
{
    internal interface ISettingsProvider
    {
        #region Methods

        DocumentSetting GetSettings();

        #endregion
    }
}