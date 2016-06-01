using System;
using System.Linq;
using System.Collections.Immutable;

using Optional;

using static Markify.Models.Context;

namespace Markify.Core.IDE
{
    public sealed class SolutionExplorer : ISolutionExplorer
    {
        #region Fields

        private readonly IIDEEnvironment _ideEnv;

        #endregion

        #region Properties

        public Option<Solution> CurrentSolution
        {
            get
            {
                var name = _ideEnv.CurrentSolution;
                if (name == null)
                    return Option.None<Solution>();

                return new Solution
                (
                    _ideEnv.CurrentSolution,
                    _ideEnv.GetSolutionPath(name),
                    ImmutableList.CreateRange(_ideEnv.GetProjects(name) ?? new string[0])
                ).Some();
            }
        }

        public Option<string> CurrentProject
        {
            get
            {
                return _ideEnv.CurrentProject != null ? 
                    _ideEnv.CurrentProject.Some() :
                    Option.None<string>();
            }
        }

        #endregion

        #region Constructors

        public SolutionExplorer(IIDEEnvironment ideEnv)
        {
            _ideEnv = ideEnv;
        }

        #endregion

        #region Methods

        public Option<Project> GetProject(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return default(Option<Project>);

            var projectPath = _ideEnv.GetProjectPath(_ideEnv.CurrentSolution, name);
            var solutionPath = _ideEnv.GetSolutionPath(_ideEnv.CurrentSolution);
            if (projectPath == null || solutionPath == null)
                return default(Option<Project>);

            return new Project
            ( 
                name,
                projectPath ?? new Uri(solutionPath, name),
                ImmutableList.CreateRange(_ideEnv.GetProjectFiles(_ideEnv.CurrentSolution, name) ?? new Uri[0])
            ).Some();
        }

        #endregion
    }
}
