using System.Collections.Generic;
using Markify.Processors.Roslyn.Models;
using Microsoft.CodeAnalysis;

namespace Markify.Processors.Roslyn.Inspectors
{
    public interface ISyntaxTreeInspector<out T> 
        where T : IItemRepresentation
    {
        #region Methods

        IEnumerable<T> Inspect(SyntaxTree tree);

        #endregion
    }
}