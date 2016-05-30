using Markify.Core.IDE;

using Moq;

using Ploeh.AutoFixture;

namespace Markify.Fixtures
{
    public class SolutionExplorerCustomization : ICustomization
    {
        #region Properties

        public SolutionExplorerCustomization()
        {
            
        }

        #endregion

        #region Customize

        public void Customize(IFixture fixture)
        {
            var ideEnv = new Mock<IIDEEnvironment>();
            fixture.Register<ISolutionExplorer>(() => new SolutionExplorer(ideEnv.Object));
        }

        #endregion
    }
}