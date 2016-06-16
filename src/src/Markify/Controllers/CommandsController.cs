using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Markify.Core.IDE;
using Markify.Services;

using Optional;

using static Markify.Models.Context;

namespace Markify.Controllers
{
    internal class CommandsController
    {
        #region Fields

        private readonly ISolutionExplorer _explorer;
        private readonly IDocumentationGenerator _generator;

        #endregion

        #region Constructors

        public CommandsController(ISolutionExplorer explorer, IDocumentationGenerator generator)
        {
            _explorer = explorer;
            _generator = generator;
        }

        #endregion

        #region Commands Actions

        public bool GenerateForCurrentProject()
        {
            return GenerateDocumentation(GetCurrentProject());
        }

        public bool GenerateForCurrentSolution()
        {
            return GenerateDocumentation(GetProjectsFromCurrentSolution());
        }

        private bool GenerateDocumentation(Option<IEnumerable<Project>> projects)
        {
            return projects.Match(
                x => _generator.Generate(x),
                () => false
            );
        }

        #endregion

        #region Explorer helpers

        private Option<IEnumerable<Project>> GetProjectsFromCurrentSolution()
        {
            return _explorer.CurrentSolution.Match(
                x => GetProjects(x.Projects).Some(),
                Option.None<IEnumerable<Project>>
            );
        }

        private Option<IEnumerable<Project>> GetCurrentProject()
        {
            return _explorer.CurrentProject.Match(
                x => GetProjects(new[] {x}).Some(),
                Option.None<IEnumerable<Project>>
            );
        }

        private IEnumerable<Project> GetProjects(IEnumerable<string> projectNames)
        {
            return projectNames.Aggregate(
                ImmutableArray.Create<Project>(),
                (acc, c) => _explorer.GetProject(c).Match(acc.Add, () => acc)
            );
        }

        #endregion
    }
}