﻿namespace Markify.CodeAnalyzer.Roslyn

open Markify.CodeAnalyzer

type TypeInfo = {
    Definition : TypeDefinition
    IsPartial : bool }

type NamespaceName = string
type NamespaceInfo = {
    Name : NamespaceName
    Types : TypeInfo list }

type AnalyzeResult = {
    Namespaces : NamespaceInfo list }