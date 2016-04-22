using System;
using System.Linq;
using System.Collections.Generic;

using Markify.Models.Definitions;
using Markify.Processors.Roslyn.Models;

namespace Markify.Processors.Roslyn
{
    public sealed class RoslynContext
    {
        #region Fields

        private Dictionary<string, NamespaceRepresentation> _namespacesMap;
        private Dictionary<string, TypeRepresentation> _typesMap;
        private Dictionary<TypeRepresentation, NamespaceRepresentation> _namespaceLinks;

        #endregion

        #region Properties

        private Dictionary<string, NamespaceRepresentation> NamespacesMap => _namespacesMap ?? (_namespacesMap = new Dictionary<string, NamespaceRepresentation>());

        private Dictionary<string, TypeRepresentation> TypesMap => _typesMap ?? (_typesMap = new Dictionary<string, TypeRepresentation>());

        private Dictionary<TypeRepresentation, NamespaceRepresentation> NamespaceLinks => _namespaceLinks ?? (_namespaceLinks = new Dictionary<TypeRepresentation, NamespaceRepresentation>());

        public IReadOnlyCollection<NamespaceRepresentation> Namespaces => NamespacesMap.Values; 

        public IReadOnlyCollection<TypeRepresentation> Types => TypesMap.Values;

        #endregion

        #region Type Methods

        public TypeRepresentation GetOrCreateType(StructureKind structure, string fullname, string name = null)
        {
            if(!Enum.IsDefined(typeof(StructureKind), structure))
                throw new ArgumentOutOfRangeException(nameof(structure));

            if (string.IsNullOrWhiteSpace(fullname))
                throw new ArgumentNullException(nameof(fullname));

            TypeRepresentation type;
            if (TypesMap.TryGetValue(fullname, out type))
                return type;

            string[] nameParts = fullname.Split('.');
            if(nameParts.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException($"{nameof(fullname)}: {fullname} is malformed, it contains invalid parts");

            type = new TypeRepresentation(fullname, name ?? nameParts[nameParts.Length - 1], structure);
            TypesMap[fullname] = type;

            if (nameParts.Length > 1)
                AddTypeToNamespace(string.Join(".", nameParts, 0, nameParts.Length - 1), type);

            return type;
        }

        private void AddTypeToNamespace(string namespaceName, TypeRepresentation type)
        {
            if(string.IsNullOrWhiteSpace(namespaceName))
                throw new ArgumentNullException(nameof(namespaceName));

            NamespaceRepresentation space = EnsureNamespace(namespaceName);
            NamespaceLinks[type] = space;
        }

        private NamespaceRepresentation EnsureNamespace(string fullname)
        {
            NamespaceRepresentation space;
            if (NamespacesMap.TryGetValue(fullname, out space))
                return space;

            space = new NamespaceRepresentation(fullname);
            NamespacesMap[fullname] = space;

            return space;
        }

        #endregion
    }
}