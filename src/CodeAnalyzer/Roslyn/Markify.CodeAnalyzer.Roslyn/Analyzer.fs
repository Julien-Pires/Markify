namespace Markify.CodeAnalyzer.Roslyn

open Microsoft.CodeAnalysis
open Markify.CodeAnalyzer

type TypeMember<'a> = {
    Source : SyntaxNode 
    Value : 'a }

type StructureMembers = {
    Fields : TypeMember<FieldInfo> list
    Properties : TypeMember<PropertyInfo> list
    Events : TypeMember<EventInfo> list
    Methods : TypeMember<MethodInfo> list }

type EnumMembers = {
    Values : TypeMember<EnumValue> list }

type DelegateMembers = {
    Parameters : TypeMember<ParameterInfo> list
    ReturnType : TypeName }

type TypeMembers =
    | Structure of StructureMembers
    | Enum of EnumMembers
    | Delegate of DelegateMembers

type TypeAnalysis = {
    Source : SyntaxNode 
    Identity : Identity
    Members : TypeMembers }