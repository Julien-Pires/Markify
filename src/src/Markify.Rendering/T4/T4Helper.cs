using System.Linq;

using static Markify.Models.Definitions;

namespace Markify.Rendering.T4
{
    internal static class T4Helper
    {
        #region Type Definition Helper

        public static string GetKind(TypeDefinition definition)
        {
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

        public static string GetAccessModifiers(TypeDefinition definition)
        {
            if (definition.AccessModifiers.Count() == 0)
                return "internal";

            return string.Join(" & ", definition.AccessModifiers);
        }

        public static string GetModifiers(TypeDefinition definition)
        {
            return string.Join(", ", definition.Modifiers);
        }

        public static string GetParents(TypeDefinition definition)
        {
            return string.Join(", ", definition.BaseTypes);
        }

        public static string GetNameWithParameters(TypeDefinition definition)
        {
            if (definition.Parameters.Count() == 0)
                return definition.Identity.Name;

            var parameters = string.Join(", ", definition.Parameters.Select(c => c.Identity.Name));

            return $"{definition.Identity.Name}<{parameters}>";
        }

        #endregion
    }
}