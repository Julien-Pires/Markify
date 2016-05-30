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
                if (_ideEnv.CurrentSolution == null)
                    return Option.None<Solution>();

                var name = _ideEnv.CurrentSolution;
                var solution = new Solution
                (
                    _ideEnv.CurrentSolution,
                    _ideEnv.GetSolutionPath(name),
                    _ideEnv.GetProjects(name)
                );

                return Option.Some(solution);
            }
        }

        public Option<string> CurrentProject
        {
            get
            {
                return _ideEnv.CurrentProject != null ? 
                    Option.Some(_ideEnv.CurrentProject) :
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
            return default(Option<Project>);
        }

        #endregion
    }
}
