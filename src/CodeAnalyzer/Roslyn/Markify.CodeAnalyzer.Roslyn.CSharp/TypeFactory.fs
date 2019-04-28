namespace Markify.CodeAnalyzer.Roslyn.Csharp

open Markify.Core.FSharp
open Markify.CodeAnalyzer
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

module TypeFactory =
    let fieldDefaultAccessModifier = [CSharpKeyword.privateModifier]

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

    let extractEnumValues (enum : EnumDeclarationSyntax) : EnumValue seq =
        enum.Members
        |> Seq.map (fun c -> {
            Name = c.Identifier.Text
            Value = findDefaultValue c.EqualsValue })

    let extractParameters (parametersList : ParameterListSyntax) =
        parametersList.Parameters
        |> Seq.map (fun c -> {
            Name = c.Identifier.Text
            Type = match c.Type with | null -> c.Identifier.Text | _ -> string c.Type
            Modifier = findModifiers c.Modifiers |> List.tryHead 
            DefaultValue = findDefaultValue c.Default })

    let extractFields defaultAccessModifier (fields : FieldDeclarationSyntax seq) =
        fields
        |> Seq.collect (fun c -> c.Declaration.Variables |> Seq.map (fun d -> (c, d)))
        |> Seq.fold (fun acc (field, name) -> {   
            Name = name.Identifier.Text
            Type = string field.Declaration.Type
            AccessModifiers = tryFindAccessModifiers field.Modifiers defaultAccessModifier
            Modifiers = findModifiers field.Modifiers
            DefaultValue = findDefaultValue name.Initializer }::acc) []

    let extractEvents defaultAccessModifier (events : MemberDeclarationSyntax seq) =
        events
        |> Seq.collect (fun c ->
            match c with
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
            | _ -> [])
        |> Seq.toList

    let tryFindAccessor (property : PropertyDeclarationSyntax) accessor defaultAccessModifier : AccessorInfo option =
        match property.AccessorList with
        | null -> None
        | x ->
            x.Accessors
            |> Seq.tryFind (fun c -> c.Keyword.Kind() = accessor)
            |> function
                | Some x -> Some { AccessModifiers = tryFindAccessModifiers x.Modifiers defaultAccessModifier }
                | None -> None

    let extractProperties defaultAccessModifier (properties : PropertyDeclarationSyntax seq)  =
        properties
        |> Seq.map (fun c -> {
            Name = c.Identifier.Text
            AccessModifiers = tryFindAccessModifiers c.Modifiers defaultAccessModifier
            Modifiers = findModifiers c.Modifiers
            Type = string c.Type
            DefaultValue = findDefaultValue c.Initializer
            IsWrite = tryFindAccessor c SyntaxKind.SetKeyword defaultAccessModifier
            IsRead = tryFindAccessor c SyntaxKind.GetKeyword defaultAccessModifier })
        |> Seq.toList

    let extractMethods defaultAccessModifier (methods : MethodDeclarationSyntax seq) : MethodInfo list =
        methods
        |> Seq.map (fun c -> 
            let genericParameters = TypeSyntaxHelper.getGenericParameters c
            let genericConstraints = TypeSyntaxHelper.getGenericConstraints c
            {   Name = c.Identifier.ValueText
                Generics = extractGenerics genericParameters genericConstraints
                AccessModifiers = tryFindAccessModifiers c.Modifiers defaultAccessModifier
                Modifiers = findModifiers c.Modifiers
                Parameters = extractParameters c.ParameterList 
                ReturnType = string c.ReturnType })
        |> Seq.toList

    let extractBaseTypes baseTypes =
        baseTypes
        |> Seq.map string 
        |> Seq.toList

    let extractEnumInfo enumNode = {
        Values = extractEnumValues enumNode |> Seq.toList }

    let extractDelegateInfo (delegateNode : DelegateDeclarationSyntax) = { 
        Parameters = extractParameters delegateNode.ParameterList |> Seq.toList
        ReturnType = string delegateNode.ReturnType }

    let extractStructureInfo typeNode = {
        Fields = FieldSyntaxCollector().Visit(typeNode) |> extractFields fieldDefaultAccessModifier
        Properties = PropertySyntaxCollector().Visit(typeNode) |> extractProperties fieldDefaultAccessModifier
        Events = EventSyntaxCollector().Visit(typeNode) |> extractEvents fieldDefaultAccessModifier
        Methods = MethodSyntaxCollector().Visit(typeNode) |> extractMethods fieldDefaultAccessModifier }

    let createTypeInfo node =
        match node with
        | IsClass x -> extractStructureInfo x |> Class |> Success
        | IsStruct x -> extractStructureInfo x |> Struct |> Success
        | IsInterface x -> extractStructureInfo x |> Interface |> Success
        | IsEnum x -> extractEnumInfo x |> Enum |> Success
        | IsDelegate x -> extractDelegateInfo x |> Delegate |> Success
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