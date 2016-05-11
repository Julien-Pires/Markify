using System;
using System.Linq;

using Markify.Models;

using Moq;
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
            var project = new Mock<ProjectContext>();
            project.SetupGet(c => c.Files).Returns(files);

            fixture.Register(() => project.Object);
        }

        #endregion
    }
}
