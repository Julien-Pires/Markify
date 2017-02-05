using System.Collections.Generic;
using Markify.Domain.Ide;
using Markify.Services.VisualStudio;

namespace Markify.Application.Services.Settings
{
    internal sealed class SolutionExplorerFilterProvider : IProjectFilterProvider
    {
        #region Properties

        public ISet<string> AllowedExtensions { get; } = new HashSet<string>(new[]
        {
            "cs",
            "vb"
        });

        public ISet<ProjectLanguage> SupportedLanguages { get; } = new HashSet<ProjectLanguage>(new[]
        {
            ProjectLanguage.CSharp,
            ProjectLanguage.VisualBasic
        });

        #endregion
    }
}