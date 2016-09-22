using System;
using System.Linq;
using Microsoft.FSharp.Core;
using Markify.Models.Definitions;

namespace Markify.Rendering
{
    public static class DefinitionFormatter
    {
        #region Fields

        private const string Delimiter = ", ";
        private const string AccessModifierDelimiter = " ";
        private const string DefaultAccessModifier = "internal";
        private const string Class = "class";
        private const string Struct = "struct";
        private const string Interface = "interface";
        private const string Enum = "enum";
        private const string Delegate = "delegate";

        #endregion

        #region Common Helper

        private static T GetValueOrDefault<T>(FSharpOption<T> option, T def = default(T)) => 
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
                    result = Class;
                    break;

                case StructureKind.Struct:
                    result = Struct;
                    break;

                case StructureKind.Interface:
                    result = Interface;
                    break;

                case StructureKind.Enum:
                    result = Enum;
                    break;

                case StructureKind.Delegate:
                    result = Delegate;
                    break;

                default:
                    result = string.Empty;
                    break;
            }

            return result;
        }

        public static string GetAccessModifiers(TypeDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return definition.AccessModifiers.Any() ? string.Join(AccessModifierDelimiter, definition.AccessModifiers) : DefaultAccessModifier;
        }

        public static string GetModifiers(TypeDefinition definition)
        {
            if(definition == null)
                throw new ArgumentNullException(nameof(definition));

            return string.Join(Delimiter, definition.Modifiers);
        }

        public static string GetParents(TypeDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return string.Join(Delimiter, definition.BaseTypes);
        }

        public static string GetNamespace(TypeDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return GetValueOrDefault(definition.Identity.Namespace, string.Empty);
        }

        public static string GetNameWithParameters(TypeDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            if (!definition.Parameters.Any())
                return definition.Identity.Name;

            var parameters = string.Join(", ", definition.Parameters.Select(c => c.Identity));

            return $"{definition.Identity.Name}<{parameters}>";
        }

        #endregion
    }
}