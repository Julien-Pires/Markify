using System.Collections.Generic;
using Markify.Processors.Roslyn.Models;
using Microsoft.CodeAnalysis;

namespace Markify.Processors.Roslyn.Inspectors
{
    public sealed class ClassInspector : ISyntaxTreeInspector<TypeRepresentation>
    {
        #region Inspect Methods

        public IEnumerable<TypeRepresentation> Inspect(SyntaxTree tree)
        {
            return null;
        }

        #endregion
    }
}