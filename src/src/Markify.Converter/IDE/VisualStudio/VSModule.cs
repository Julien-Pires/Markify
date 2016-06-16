using Ninject.Modules;

namespace Markify.Core.IDE.VisualStudio
{
    public sealed class VSModule : NinjectModule
    {
        #region Module Loading

        public override void Load()
        {
            Bind<IIDEEnvironment>().To<VSEnvironment>();
        }

        #endregion
    }
}