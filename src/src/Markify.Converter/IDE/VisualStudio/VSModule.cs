using System;
using EnvDTE80;

using Ninject.Modules;

namespace Markify.Core.IDE.VisualStudio
{
    public sealed class VSModule : NinjectModule
    {
        #region Fields

        private readonly Func<DTE2> _vsEnvironmentFactory;

        #endregion

        #region Constructors

        public VSModule(Func<DTE2> vsEnvironmentFactory)
        {
            _vsEnvironmentFactory = vsEnvironmentFactory;
        }

        #endregion

        #region Module Loading

        public override void Load()
        {
            Bind<DTE2>().ToMethod(c => _vsEnvironmentFactory());
            Bind<IIDEEnvironment>().To<VisualStudioEnvironment>();
        }

        #endregion
    }
}