namespace Markify.CodeAnalyzer.Roslyn.Csharp

open Markify.Core.FSharp
open Markify.CodeAnalyzer
open Markify.CodeAnalyzer.Roslyn
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax

module TypeFactory =
    let fieldDefaultAccessModifier = [ CSharpKeyword.privateModifier ]

    let collectMember extract m =
        [ for x in m do
            yield { Source = x; Value = extract x } ]

    let collectMembers extract ms =
        [ for x in ms do
            yield! extract x |> Seq.map (fun d -> { Source = x; Value = d }) ]

    let findDefaultValue (initializer: EqualsValueClauseSyntax) =
        match initializer with
        | null -> None
        | x -> Some (string x.Value)

    let findAccessModifiers (modifiers: SyntaxTokenList) = 
        [ for modifier in modifiers do
            if CSharpKeyword.accessModifiers.Contains (modifier.Kind()) 
            then yield modifier.Text ]

    let tryFindAccessModifiers modifiers defaultAccessModifier =
        match findAccessModifiers modifiers with
        | [] -> defaultAccessModifier
        | x -> x

    let findModifiers (modifiers: SyntaxTokenList) = 
        [ for modifier in modifiers do
            if CSharpKeyword.accessModifiers.Contains (modifier.Kind()) |> not 
            then yield modifier.Text ]

    let findConstraints parameter (constraints: SyntaxList<TypeParameterConstraintClauseSyntax>) =
        constraints
        |> Seq.tryFind (fun c -> string c.Name = parameter)
        |> function 
            | Some x -> x.Constraints |> Seq.map string 
            | None -> Seq.empty

    let extractGenerics (parameters: TypeParameterListSyntax) (constraints: SyntaxList<TypeParameterConstraintClauseSyntax>) = 
        [ for parameter in parameters.Parameters do
            let name = parameter.Identifier.Text
            let variance =
                match parameter.VarianceKeyword.Value with
                | null -> None
                | x -> Some (string x)
            yield { Name = name
                    Modifier = variance
                    Constraints = constraints |> findConstraints name |> Seq.toList } ]

    let extractEnumValue (enumMember: EnumMemberDeclarationSyntax) : EnumValue = 
        { Name = enumMember.Identifier.Text
          Value = findDefaultValue enumMember.EqualsValue }

    let extractParameter (parameter: ParameterSyntax) = 
        { Name = parameter.Identifier.Text
          Type = match parameter.Type with | null -> parameter.Identifier.Text | _ -> string parameter.Type
          Modifier = findModifiers parameter.Modifiers |> List.tryHead 
          DefaultValue = findDefaultValue parameter.Default }

    let extractFields defaultAccessModifier (field: FieldDeclarationSyntax) = 
        [ for variable in field.Declaration.Variables do
            yield { Name = variable.Identifier.Text
                    Type = string field.Declaration.Type
                    AccessModifiers = tryFindAccessModifiers field.Modifiers defaultAccessModifier
                    Modifiers = findModifiers field.Modifiers
                    DefaultValue = findDefaultValue variable.Initializer }]

    let extractEvents defaultAccessModifier (event: MemberDeclarationSyntax) =
        match event with
        | IsEventDeclaration x -> 
            [ { Name = x.Identifier.Text
                Type = string x.Type
                AccessModifiers = tryFindAccessModifiers x.Modifiers defaultAccessModifier
                Modifiers = findModifiers x.Modifiers } ]
        | IsEventField x -> 
            [ for variable in x.Declaration.Variables do
                yield { EventInfo.Name = variable.Identifier.Text
                        Type = string x.Declaration.Type
                        AccessModifiers = tryFindAccessModifiers x.Modifiers defaultAccessModifier
                        Modifiers = findModifiers x.Modifiers } ]
        | _ -> []

    let tryFindAccessor (property: PropertyDeclarationSyntax) defaultAccessModifier accessor : AccessorInfo option =
        match property.AccessorList with
        | null -> None
        | x ->
            x.Accessors
            |> Seq.tryFind (fun c -> c.Keyword.Kind() = accessor)
            |> function
                | Some x -> Some { AccessModifiers = tryFindAccessModifiers x.Modifiers defaultAccessModifier }
                | None -> None

    let extractProperty defaultAccessModifier (property: PropertyDeclarationSyntax) =
        let findAccessor = tryFindAccessor property defaultAccessModifier
        { Name = property.Identifier.Text
          AccessModifiers = tryFindAccessModifiers property.Modifiers defaultAccessModifier
          Modifiers = findModifiers property.Modifiers
          Type = string property.Type
          DefaultValue = findDefaultValue property.Initializer
          IsWrite = findAccessor SyntaxKind.SetKeyword
          IsRead = findAccessor SyntaxKind.GetKeyword }

    let extractMethods defaultAccessModifier (method: MethodDeclarationSyntax) =
        let genericParameters = TypeSyntaxHelper.getGenericParameters method
        let genericConstraints = TypeSyntaxHelper.getGenericConstraints method
        { Name = method.Identifier.ValueText
          Generics = extractGenerics genericParameters genericConstraints
          AccessModifiers = tryFindAccessModifiers method.Modifiers defaultAccessModifier
          Modifiers = findModifiers method.Modifiers
          Parameters = method.ParameterList.Parameters |> Seq.map extractParameter |> Seq.toList
          ReturnType = string method.ReturnType }

    let extractBaseTypes types = 
        [ for baseType in types do
            yield string baseType ]

    let extractEnumMembers (enumNode: EnumDeclarationSyntax) = 
        { Values = collectMember extractEnumValue enumNode.Members }

    let extractDelegateMembers (delegateNode: DelegateDeclarationSyntax) = 
        { Parameters = collectMember extractParameter delegateNode.ParameterList.Parameters
          ReturnType = string delegateNode.ReturnType }

    let extractStructureMembers typeNode = 
        { Fields = 
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
        | IsClass x -> extractStructureMembers x |> Structure |> Success
        | IsStruct x -> extractStructureMembers x |> Structure |> Success
        | IsInterface x -> extractStructureMembers x |> Structure |> Success
        | IsEnum x -> extractEnumMembers x |> Enum |> Success
        | IsDelegate x -> extractDelegateMembers x |> Delegate |> Success
        | _ -> Failure ""

    let createIdentity node =
        let modifiers = TypeSyntaxHelper.getModifiers node
        let genericParameters = TypeSyntaxHelper.getGenericParameters node
        let genericConstraints = TypeSyntaxHelper.getGenericConstraints node
        let baseTypes = TypeSyntaxHelper.getBaseTypes node
        match SyntaxHelper.getName node with
        | Some x -> 
            Success { Name = x.ValueText
                      AccessModifiers = findAccessModifiers modifiers 
                      Modifiers = findModifiers modifiers
                      Generics = extractGenerics genericParameters genericConstraints
                      BaseType = extractBaseTypes baseTypes }
        | None -> Failure ""

    let create node =
        match createIdentity node with
        | Success x ->
            match createTypeInfo node with
            | Success y ->
                Success { Source = node
                          Identity = x
                          Members = y }
            | Failure y -> Failure y
        | Failure x -> Failure x