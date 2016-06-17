using static Markify.Models.Document;

namespace Markify.Services
{
    internal interface ISettingsProvider
    {
        #region Methods

        DocumentSetting GetSettings();

        #endregion
    }
}