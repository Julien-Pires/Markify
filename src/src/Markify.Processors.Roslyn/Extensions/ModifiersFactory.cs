using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Markify.Processors.Roslyn.Extensions
{
    internal static class ModifiersFactory
    {
        #region Fields

        private static readonly HashSet<SyntaxKind> AccessModifiers = new HashSet<SyntaxKind>
        {
            SyntaxKind.PublicKeyword,
            SyntaxKind.ProtectedKeyword,
            SyntaxKind.InternalKeyword,
            SyntaxKind.PrivateKeyword
        };

        private static readonly Dictionary<int, ImmutableList<string>> ModifiersMap = 
            new Dictionary<int, ImmutableList<string>>();

        #endregion

        #region Factory

        public static IEnumerable<string> GetAccessModifiers(ITypeDeclarationAdapter typeAdapater)
        {
            return EnsureModifiers(typeAdapater.Modifiers.Where(c => AccessModifiers.Contains(c.Kind())).ToArray());
        }

        public static IEnumerable<string> GetExtraModifiers(ITypeDeclarationAdapter typeAdapater)
        {
            return EnsureModifiers(typeAdapater.Modifiers.Where(c => !AccessModifiers.Contains(c.Kind())).ToArray());
        }

        private static ImmutableList<string> EnsureModifiers(SyntaxToken[] modifiers)
        {
            ImmutableList<string> result;
            int key = modifiers.Aggregate(0, (value, mod) => value + (int)mod.Kind());
            if (ModifiersMap.TryGetValue(key, out result))
                return result;

            result = ImmutableList.Create<string>().AddRange(modifiers.Select(c => c.Text));
            ModifiersMap[key] = result;

            return result;
        }

        #endregion
    }
}