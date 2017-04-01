using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Markify.Domain.Compiler;
using Markify.Services.Roslyn;
using Markify.Services.Roslyn.Common;
using Markify.Services.Roslyn.Csharp;
using Markify.Services.Roslyn.VisualBasic;
using Ninject.Modules;

namespace Markify.Application.Services
{
    [ExcludeFromCodeCoverage]
    internal sealed class RoslynModule : NinjectModule
    {
        #region Module Loading

        public override void Load()
        {
            IEnumerable<ILanguageModule> analyzers = new ILanguageModule[]
            {
                new CSharpModule(), 
                new VisualBasicModule(),
            };

            Bind<IProjectAnalyzer>().To<RoslynAnalyzer>().WithConstructorArgument(analyzers);
        }

        #endregion
    }
}