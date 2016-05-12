using System;
using System.Linq;

using static Markify.Models.Context;

using Ploeh.AutoFixture;

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

        public void Customize(IFixture fixture)
        {
            var files = _sourceFiles.Select(c => new Uri(c));
            fixture.Register(() => new ProjectContext(files));
        }

        #endregion
    }
}
