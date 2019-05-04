namespace Markify.CodeAnalyzer.Roslyn.Csharp

open Markify.Core.FSharp
open Markify.CodeAnalyzer
open Markify.CodeAnalyzer.Roslyn
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

module TypeFactory =
    let fieldDefaultAccessModifier = [CSharpKeyword.privateModifier]

    let collectMember extract members =
        members
        |> Seq.map (fun c -> { Source = c; Value = extract c })
        |> Seq.toList

    let collectMembers extract members =
        members
        |> Seq.collect (fun c -> extract c |> Seq.map (fun d -> { Source = c; Value = d}))
        |> Seq.toList

    let findDefaultValue (initializer : EqualsValueClauseSyntax) =
        match initializer with
        | null -> None
        | x -> Some (string x.Value)

    let findAccessModifiers (modifiers : SyntaxTokenList) =
        modifiers
        |> Seq.filter (fun c -> 
            CSharpKeyword.accessModifiers 
            |> Set.contains (c.Kind()))
        |> Seq.map (fun c -> c.Text)
        |> Seq.toList

    let tryFindAccessModifiers modifiers defaultAccessModifier =
        match findAccessModifiers modifiers with
        | [] -> defaultAccessModifier
        | x -> x

    let findModifiers (modifiers : SyntaxTokenList) =
        modifiers
        |> Seq.filter (fun c -> 
            CSharpKeyword.accessModifiers 
            |> Set.contains (c.Kind()) 
            |> not)
        |> Seq.map (fun c -> c.Text)
        |> Seq.toList

    let findConstraints parameter (constraints : SyntaxList<TypeParameterConstraintClauseSyntax>) =
        constraints
        |> Seq.tryFind (fun c -> string c.Name = parameter)
        |> function | Some x -> x.Constraints |> Seq.map string | None -> Seq.empty

    let extractGenerics (parameters : TypeParameterListSyntax) (constraints : SyntaxList<TypeParameterConstraintClauseSyntax>) =
        parameters.Parameters
        |> Seq.map (fun c ->
            let name = c.Identifier.Text
            let variance =
                match c.VarianceKeyword.Value with
                | null -> None
                | x -> Some (string x)
            {   Name = name
                Modifier = variance
                Constraints = constraints |> findConstraints name |> Seq.toList })
        |> Seq.toList

    let extractEnumValue (enumMember : EnumMemberDeclarationSyntax) : EnumValue = {
        Name = enumMember.Identifier.Text
        Value = findDefaultValue enumMember.EqualsValue }

    let extractParameter (parameter : ParameterSyntax) = {
        Name = parameter.Identifier.Text
        Type = match parameter.Type with | null -> parameter.Identifier.Text | _ -> string parameter.Type
        Modifier = findModifiers parameter.Modifiers |> List.tryHead 
        DefaultValue = findDefaultValue parameter.Default }

    let extractFields defaultAccessModifier (field : FieldDeclarationSyntax) =
        field.Declaration.Variables
        |> Seq.map (fun c -> {   
            Name = c.Identifier.Text
            Type = string field.Declaration.Type
            AccessModifiers = tryFindAccessModifiers field.Modifiers defaultAccessModifier
            Modifiers = findModifiers field.Modifiers
            DefaultValue = findDefaultValue c.Initializer })

    let extractEvents defaultAccessModifier (event : MemberDeclarationSyntax) =
        match event with
        | IsEventDeclaration x -> [{  
            Name = x.Identifier.Text
            Type = string x.Type
            AccessModifiers = tryFindAccessModifiers x.Modifiers defaultAccessModifier
            Modifiers = findModifiers x.Modifiers }]
        | IsEventField x ->
            x.Declaration.Variables
            |> Seq.fold (fun acc d -> {
                EventInfo.Name = d.Identifier.Text
                Type = string x.Declaration.Type
                AccessModifiers = tryFindAccessModifiers x.Modifiers defaultAccessModifier
                Modifiers = findModifiers x.Modifiers }::acc) []
        | _ -> []

    let tryFindAccessor (property : PropertyDeclarationSyntax) accessor defaultAccessModifier : AccessorInfo option =
        match property.AccessorList with
        | null -> None
        | x ->
            x.Accessors
            |> Seq.tryFind (fun c -> c.Keyword.Kind() = accessor)
            |> function
                | Some x -> Some { AccessModifiers = tryFindAccessModifiers x.Modifiers defaultAccessModifier }
                | None -> None

    let extractProperty defaultAccessModifier (property : PropertyDeclarationSyntax)  = {
        Name = property.Identifier.Text
        AccessModifiers = tryFindAccessModifiers property.Modifiers defaultAccessModifier
        Modifiers = findModifiers property.Modifiers
        Type = string property.Type
        DefaultValue = findDefaultValue property.Initializer
        IsWrite = tryFindAccessor property SyntaxKind.SetKeyword defaultAccessModifier
        IsRead = tryFindAccessor property SyntaxKind.GetKeyword defaultAccessModifier }

    let extractMethods defaultAccessModifier (method : MethodDeclarationSyntax) =
        let genericParameters = TypeSyntaxHelper.getGenericParameters method
        let genericConstraints = TypeSyntaxHelper.getGenericConstraints method
        {   Name = method.Identifier.ValueText
            Generics = extractGenerics genericParameters genericConstraints
            AccessModifiers = tryFindAccessModifiers method.Modifiers defaultAccessModifier
            Modifiers = findModifiers method.Modifiers
            Parameters = method.ParameterList.Parameters |> Seq.map extractParameter |> Seq.toList
            ReturnType = string method.ReturnType }

    let extractBaseTypes baseTypes =
        baseTypes
        |> Seq.map string 
        |> Seq.toList

    let extractEnumMembers (enumNode : EnumDeclarationSyntax) : TypeMembers = Enum {
        Values = collectMember extractEnumValue enumNode.Members }

    let extractDelegateMembers (delegateNode : DelegateDeclarationSyntax) = Delegate { 
        Parameters = collectMember extractParameter delegateNode.ParameterList.Parameters
        ReturnType = string delegateNode.ReturnType }

    let extractStructureMembers typeNode = Structure {
        Fields = 
            FieldSyntaxCollector().Visit(typeNode) 
            |> collectMembers (extractFields fieldDefaultAccessModifier)
        Properties = 
            PropertySyntaxCollector().Visit(typeNode) 
            |> collectMember (extractProperty fieldDefaultAccessModifier)
        Events = 
            EventSyntaxCollector().Visit(typeNode) 
            |> collectMembers (extractEvents fieldDefaultAccessModifier)
        Methods = 
            MethodSyntaxCollector().Visit(typeNode) 
            |> collectMember (extractMethods fieldDefaultAccessModifier) }

    let createTypeInfo node =
        match node with
        | IsClass x -> extractStructureMembers x |> Success
        | IsStruct x -> extractStructureMembers x |> Success
        | IsInterface x -> extractStructureMembers x |> Success
        | IsEnum x -> extractEnumMembers x |> Success
        | IsDelegate x -> extractDelegateMembers x |> Success
        | _ -> Failure ""

    let createIdentity node =
        let modifiers = TypeSyntaxHelper.getModifiers node
        let genericParameters = TypeSyntaxHelper.getGenericParameters node
        let genericConstraints = TypeSyntaxHelper.getGenericConstraints node
        let baseTypes = TypeSyntaxHelper.getBaseTypes node
        match SyntaxHelper.getName node with
        | Some x -> Success {
            Name = x.ValueText
            AccessModifiers = findAccessModifiers modifiers 
            Modifiers = findModifiers modifiers
            Generics = extractGenerics genericParameters genericConstraints
            BaseType = extractBaseTypes baseTypes }
        | None -> Failure ""

    let create node =
        match createIdentity node with
        | Success x ->
            match createTypeInfo node with
            | Success y -> Success {
                Source = node
                Identity = x
                Members = y }
            | Failure y -> Failure y
        | Failure x -> Failure x