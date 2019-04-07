namespace Markify.CodeAnalyzer.Roslyn.Common

open Markify.CodeAnalyzer

module DefinitionHelper =
    let createIdentity getStructureIdentifier getNamespaceIdentifier node =
        {   Name = ""
            Parents = SyntaxHelper.getParentName node getStructureIdentifier 
            Namespace = SyntaxHelper.getNamespaceName node getNamespaceIdentifier
            AccessModifiers = []
            Modifiers = [] 
            BaseTypes = []
            Parameters = [] }