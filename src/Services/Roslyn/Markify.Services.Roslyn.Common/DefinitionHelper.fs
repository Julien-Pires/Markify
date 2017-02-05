namespace Markify.Services.Roslyn.Common

open Markify.Domain.Compiler

module DefinitionHelper =
    let createIdentity getStructureIdentifier getNamespaceIdentifier node =
        {   Name = ""
            Parents = SyntaxHelper.getParentName node getStructureIdentifier 
            Namespace = SyntaxHelper.getNamespaceName node getNamespaceIdentifier
            AccessModifiers = []
            Modifiers = [] 
            BaseTypes = []
            Parameters = [] }