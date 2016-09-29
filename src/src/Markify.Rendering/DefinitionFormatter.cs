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

            string result = string.Empty;
            switch(definition.Tag)
            {
                case TypeDefinition.Tags.Class:
                    result = Class;
                    break;

                case TypeDefinition.Tags.Struct:
                    result = Struct;
                    break;

                case TypeDefinition.Tags.Interface:
                    result = Interface;
                    break;

                case TypeDefinition.Tags.Enum:
                    result = Enum;
                    break;

                case TypeDefinition.Tags.Delegate:
                    result = Delegate;
                    break;
            }

            return result;
        }

        public static string GetAccessModifiers(TypeDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return definition.Identity.AccessModifiers.Any() ? string.Join(AccessModifierDelimiter, definition.Identity.AccessModifiers) : DefaultAccessModifier;
        }

        public static string GetModifiers(TypeDefinition definition)
        {
            if(definition == null)
                throw new ArgumentNullException(nameof(definition));

            return string.Join(Delimiter, definition.Identity.Modifiers);
        }

        public static string GetParents(TypeDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return string.Join(Delimiter, definition.Identity.BaseTypes);
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

            if (!definition.Identity.Parameters.Any())
                return definition.Identity.Name;

            var parameters = string.Join(", ", definition.Identity.Parameters.Select(c => c.Name));

            return $"{definition.Identity.Name}<{parameters}>";
        }

        #endregion
    }
}