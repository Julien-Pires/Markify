using System;
using System.Collections.Generic;
using System.Linq;
using Markify.CodeAnalyzer;
using Microsoft.FSharp.Core;
using Optional;

namespace Markify.Services.Rendering
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
        private const string FullPropertyAccess = "Read/Write";
        private const string ReadOnlyProperty = "Read-Only";
        private const string WriteOnlyProperty = "Write-Only";

        private static readonly Dictionary<string, HashSet<string>> VisibilityPrecedence = new Dictionary
            <string, HashSet<string>>
        {
            ["public"] = new HashSet<string>
            {
                "internal", "protected", "private" ,
                "internal protected", "protected internal", "friend protected", "protected friend"
            },
            ["internal"] = new HashSet<string> { "protected", "private" },
            ["friend"] = new HashSet<string> { "protected", "private" },
            ["protected"] = new HashSet<string> { "internal", "private" },
            ["internal protected"] = new HashSet<string> { "private" },
            ["protected internal"] = new HashSet<string> { "private" },
            ["friend protected"] = new HashSet<string> { "private" },
            ["protected friend"] = new HashSet<string> { "private" },
            ["private"] = new HashSet<string>()
        };

        #endregion

        #region Common Helper

        public static T GetValueOrDefault<T>(this FSharpOption<T> option, T defaultValue = default(T)) => 
            FSharpOption<T>.get_IsSome(option) ? option.Value : defaultValue;

        public static bool IsSome<T>(this FSharpOption<T> option) => FSharpOption<T>.get_IsSome(option);

        private static Option<ClassDefinition> GetContainerTypeDefinition(TypeDefinition definition)
        {
            Option<ClassDefinition> result;
            switch(definition.Tag)
            {
                case TypeDefinition.Tags.Class:
                    result = Option.Some(((TypeDefinition.Class)definition).Item);
                    break;

                case TypeDefinition.Tags.Struct:
                    result = Option.Some(((TypeDefinition.Struct)definition).Item);
                    break;

                case TypeDefinition.Tags.Interface:
                    result = Option.Some(((TypeDefinition.Interface)definition).Item);
                    break;

                default:
                    result = Option.None<ClassDefinition>();
                    break;
            }

            return result;
        }

        private static Option<EnumDefinition> GetEnumDefinition(TypeDefinition definition)
        {
            Option<EnumDefinition> result;
            switch (definition.Tag)
            {
                case TypeDefinition.Tags.Enum:
                    result = Option.Some(((TypeDefinition.Enum)definition).Item);
                    break;

                default:
                    result = Option.None<EnumDefinition>();
                    break;
            }

            return result;
        }

        private static Option<DelegateDefinition> GetDelegateDefinition(TypeDefinition definition)
        {
            Option<DelegateDefinition> result;
            switch(definition.Tag)
            {
                case TypeDefinition.Tags.Delegate:
                    result = Option.Some(((TypeDefinition.Delegate)definition).Item);
                    break;

                default:
                    result = Option.None<DelegateDefinition>();
                    break;
            }

            return result;
        }

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

        public static string GetNameWithParameters(TypeIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            if (!identity.Parameters.Any())
                return identity.Name;

            var parameters = string.Join(", ", identity.Parameters.Select(c => c.Name));

            return $"{identity.Name}<{parameters}>";
        }

        public static Option<string> GetReturnType(TypeDefinition definition)
        {
            if(definition == null)
                throw new ArgumentNullException(nameof(definition));

            return GetDelegateDefinition(definition).Match(
                c => Option.Some(c.ReturnType),
                Option.None<string>);
        }

        #endregion

        #region Members Definition Helper

        private static IEnumerable<IGrouping<string, TMember>> GetMembers<TMember>(TypeDefinition definition, 
            Func<ClassDefinition, IEnumerable<TMember>> membersSelector, 
            Func<TMember, string> groupBySelector)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            var members = GetContainerTypeDefinition(definition).Match(membersSelector, Enumerable.Empty<TMember>);

            return members.GroupBy(groupBySelector);
        }

        public static IEnumerable<IGrouping<string, PropertyDefinition>> GetProperties(TypeDefinition definition)
        {
            return GetMembers(definition, c => c.Properties, c => string.Join(" ", c.AccessModifiers));
        }

        public static IEnumerable<IGrouping<string, FieldDefinition>> GetFields(TypeDefinition definition)
        {
            return GetMembers(definition, c => c.Fields, c => string.Join(" ", c.AccessModifiers));
        }

        public static IEnumerable<IGrouping<string, EventDefinition>> GetEvents(TypeDefinition definition)
        {
            return GetMembers(definition, c => c.Events, c => string.Join(" ", c.AccessModifiers));
        }

        public static IEnumerable<IGrouping<string, DelegateDefinition>> GetMethods(TypeDefinition definition)
        {
            return GetMembers(definition, c => c.Methods, c => string.Join(" ", c.Identity.AccessModifiers));
        }

        public static IEnumerable<EnumValue> GetEnumValues(TypeDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return GetEnumDefinition(definition).Match(
                c => c.Values,
                Enumerable.Empty<EnumValue>);
        }

        public static IEnumerable<ParameterDefinition> GetParameters(TypeDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return GetDelegateDefinition(definition).Match(
                c => c.Parameters,
                Enumerable.Empty<ParameterDefinition>);
        }

        private static bool IsAccessorAccessible(IEnumerable<string> propertyVisiblity, FSharpOption<AccessorDefinition> definition)
        {
            if (!definition.IsSome())
                return false;

            var accessor = definition.Value;
            var visibility = string.Join(" ", propertyVisiblity).ToLower();
            HashSet<string> precedence;
            if (!VisibilityPrecedence.TryGetValue(visibility, out precedence))
                return true;

            return !precedence.Contains(string.Join(" ", accessor.AccessModifiers).ToLower());
        }

        public static string GetPropertyAccess(PropertyDefinition definition)
        {
            if(definition == null)
                throw new ArgumentNullException(nameof(definition));

            var isReadAccessible = IsAccessorAccessible(definition.AccessModifiers, definition.IsRead);
            var isWriteAccessible = IsAccessorAccessible(definition.AccessModifiers, definition.IsWrite);
            if (isReadAccessible)
                return isWriteAccessible ? FullPropertyAccess : ReadOnlyProperty;

            return isWriteAccessible ? WriteOnlyProperty : string.Empty;
        }

        #endregion

        #region Comments Helper

        private static TypeComments GetTypeComment(TypeDefinition definition)
        {
            TypeComments comment;
            switch (definition.Tag)
            {
                case TypeDefinition.Tags.Class:
                case TypeDefinition.Tags.Struct:
                case TypeDefinition.Tags.Interface:
                    comment = GetContainerTypeDefinition(definition).ValueOr((ClassDefinition)null).Comments;
                    break;

                case TypeDefinition.Tags.Enum:
                    comment = ((TypeDefinition.Enum)definition).Item.Comments;
                    break;

                case TypeDefinition.Tags.Delegate:
                    comment = ((TypeDefinition.Delegate)definition).Item.Comments;
                    break;

                default:
                    comment = null;
                    break;
            }

            return comment;
        }

        public static T GetTypeComment<T>(TypeDefinition definition, 
            Func<TypeComments, T> extractComment)
        {
            var comment = GetTypeComment(definition);

            return extractComment(comment);
        }

        #endregion
    }
}