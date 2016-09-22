using System;
using System.Diagnostics.CodeAnalysis;
using EnvDTE80;
using Ninject.Modules;

namespace Markify.Core.IDE.VisualStudio
{
    [ExcludeFromCodeCoverage]
    public sealed class VisualStudioModule : NinjectModule
    {
        #region Fields

        private readonly Func<DTE2> _vsEnvironmentFactory;

        #endregion

        #region Constructors

        public VisualStudioModule(Func<DTE2> vsEnvironmentFactory)
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