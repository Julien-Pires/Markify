﻿using Markify.Models.Definitions;

namespace Markify.Processors.Roslyn.Models
{
    public class TypeRepresentation : IItemRepresentation
    {
        #region Properties

        public Fullname Fullname { get; }

        public string Name { get; }

        public StructureKind Kind { get; }

        public string AccessModifiers { get; set; }

        public string[] Modifiers { get; set; }

        #endregion

        #region Constructors

        public TypeRepresentation(Fullname fullname, string name, StructureKind kind)
        {
            Fullname = fullname;
            Name = name;
            Kind = kind;
        }

        #endregion
    }
}