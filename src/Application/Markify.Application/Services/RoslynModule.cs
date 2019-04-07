using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Markify.CodeAnalyzer;
using Markify.CodeAnalyzer.Roslyn;
using Markify.CodeAnalyzer.Roslyn.Common;
using Markify.CodeAnalyzer.Roslyn.Csharp;
using Markify.CodeAnalyzer.Roslyn.VisualBasic;
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