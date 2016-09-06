using System;
using System.Linq;
using Microsoft.FSharp.Core;
using Markify.Models.Definitions;

namespace Markify.Rendering.T4
{
    public static class DefinitionFormatter
    {
        #region Common Helper

        public static T GetValueOrDefault<T>(FSharpOption<T> option, T def = default(T)) => 
            FSharpOption<T>.get_IsSome(option) ? option.Value : def;

        #endregion

        #region Type Definition Helper

        public static string GetKind(TypeDefinition definition)
        {
            if(definition == null)
                throw new ArgumentNullException(nameof(definition));

            string result;
            switch(definition.Kind)
            {
                case StructureKind.Class:
                    result = "class";
                    break;

                case StructureKind.Struct:
                    result = "struct";
                    break;

                case StructureKind.Interface:
                    result = "interface";
                    break;

                case StructureKind.Enum:
                    result = "enum";
                    break;

                case StructureKind.Delegate:
                    result = "delegate";
                    break;

                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        public static string GetAccessModifiers(TypeDefinition definition) => 
            !definition.AccessModifiers.Any() ? "internal" : string.Join(" ", definition.AccessModifiers);

        public static string GetModifiers(TypeDefinition definition) => string.Join(", ", definition.Modifiers);

        public static string GetParents(TypeDefinition definition) => string.Join(", ", definition.BaseTypes);

        public static string GetNamespace(TypeDefinition definition) => GetValueOrDefault(definition.Identity.Namespace, string.Empty);

        public static string GetNameWithParameters(TypeDefinition definition)
        {
            if (!definition.Parameters.Any())
                return definition.Identity.Name;

            var parameters = string.Join(", ", definition.Parameters.Select(c => c.Identity));

            return $"{definition.Identity.Name}<{parameters}>";
        }

        #endregion
    }
}