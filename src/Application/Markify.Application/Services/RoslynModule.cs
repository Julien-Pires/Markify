using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Markify.Domain.Compiler;
using Markify.Services.Roslyn;
using Markify.Services.Roslyn.Common;
using Ninject.Modules;

namespace Markify.Application.Services
{
    [ExcludeFromCodeCoverage]
    internal sealed class RoslynModule : NinjectModule
    {
        #region Module Loading

        public override void Load()
        {
            var analyzers = new[]
            {
                Tuple.Create<Func<string, SourceContent>, IEnumerable<string>>(CSharpAnalyzer.analyze, new [] { "cs" }),
                Tuple.Create<Func<string, SourceContent>, IEnumerable<string>>(VisualBasicAnalyzer.analyze, new [] { "vb" })
            };

            Bind<IProjectAnalyzer>().To<RoslynAnalyzer>().WithConstructorArgument(analyzers);
        }

        #endregion
    }
}