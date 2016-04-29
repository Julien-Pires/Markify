using System.IO;

namespace Markify.Fixtures
{
    public static class SourceHelper
    {
        #region Fields

        public static readonly string ProjectsFolder;
        public static readonly string SourceFolder;

        #endregion

        #region Constructors

        static SourceHelper()
        {
            ProjectsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Projects");
            SourceFolder = Path.Combine(ProjectsFolder, "Source");
        }

        #endregion
    }
}