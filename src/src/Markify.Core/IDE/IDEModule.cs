using System.Diagnostics.CodeAnalysis;
using Ninject.Modules;

namespace Markify.Core.IDE
{
    [ExcludeFromCodeCoverage]
    public sealed class IDEModule : NinjectModule
    {
        #region Module Loading

        public override void Load()
        {
            Bind<ISolutionExplorer>().To<SolutionExplorer>();
        }

        #endregion
    }
}