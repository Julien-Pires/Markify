using System.Collections.Generic;
using Markify.Models.IDE;

namespace Markify.Core.IDE
{
    public interface IProjectFilterProvider
    {
        #region Properties

        ISet<ProjectLanguage> SupportedLanguages { get; }

        ISet<string> AllowedExtensions { get; }

        #endregion
    }
}