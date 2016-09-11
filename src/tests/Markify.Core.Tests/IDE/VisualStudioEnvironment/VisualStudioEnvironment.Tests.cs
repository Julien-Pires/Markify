using System;
using System.Linq;
using Markify.Core.IDE;

namespace Markify.Core.Tests.IDE
{
    public sealed partial class VisualStudioEnvironment_Tests
    {
        #region Helpers

        private static string GetRandom(IIDEEnvironment environment, string solution)
        {
            var allProject = environment.GetProjects(solution).ToArray();

            return allProject.ElementAt(new Random().Next(allProject.Length));
        }

        #endregion
    }
}