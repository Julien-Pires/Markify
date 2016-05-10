using Markify.Models;

using Ploeh.AutoFixture;

namespace Markify.Fixtures
{
    internal sealed class ProjectContextCustomization : ICustomization
    {
        #region Constructors

        public ProjectContextCustomization(string[] sourceFiles)
        {
        }

        #endregion

        #region Customize

        public void Customize(IFixture fixture)
        {
            fixture.Register(() => new ProjectContext());
        }

        #endregion
    }
}
