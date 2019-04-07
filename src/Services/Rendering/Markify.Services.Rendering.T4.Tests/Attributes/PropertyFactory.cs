using System;
using System.Linq;
using Markify.CodeAnalyzer;
using Microsoft.FSharp.Core;

namespace Markify.Services.Rendering.T4.Tests.Attributes
{
    internal static class PropertyFactory
    {
        #region Factory

        private static FSharpOption<AccessorDefinition> CreateAccessor(string visiblity)
        {
            return string.IsNullOrWhiteSpace(visiblity)
                ? FSharpOption<AccessorDefinition>.None
                : FSharpOption<AccessorDefinition>.Some(new AccessorDefinition(new[] { visiblity }));
        }

        public static PropertyDefinition Create(string visibility, string set, string get)
        {
            return new PropertyDefinition(
                Guid.NewGuid().ToString(),
                "int",
                new[] { visibility },
                Enumerable.Empty<string>(),
                FSharpOption<string>.None,
                CreateAccessor(set),
                CreateAccessor(get)
            );
        }

        #endregion
    }
}