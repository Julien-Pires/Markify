using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Ploeh.AutoFixture;

using Markify.Models.IDE;

namespace Markify.Fixtures
{
    internal sealed class ProjectContextCustomization : ICustomization
    {
        #region Fields

        private readonly string[] _sourceFiles;

        #endregion

        #region Constructors

        public ProjectContextCustomization(string[] sourceFiles)
        {
            _sourceFiles = sourceFiles;
        }

        #endregion

        #region Customize

        public string CreateFullPath(string path)
        {
            var basePath = new UriBuilder(Assembly.GetExecutingAssembly().CodeBase);
            var cleanPath = Uri.UnescapeDataString(basePath.Path);

            return Path.Combine(Path.GetDirectoryName(cleanPath), path);
        }

        public void Customize(IFixture fixture)
        {
            var files = _sourceFiles.Select(c => new Uri(CreateFullPath(c)));
            fixture.Register(() => new Project("Test", new Uri("c:/Test/Test.csproj"), ProjectLanguage.CSharp, files));
        }

        #endregion
    }
}
