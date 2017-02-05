using System.Collections.Generic;
using Markify.Domain.Ide;

namespace Markify.Services.VisualStudio
{
    public interface IProjectFilterProvider
    {
        #region Properties

        ISet<ProjectLanguage> SupportedLanguages { get; }

        ISet<string> AllowedExtensions { get; }

        #endregion
    }
}