namespace Markify.CodeAnalyzer.Roslyn.VisualBasic

open Markify.Core.FSharp
open Markify.CodeAnalyzer
open Markify.CodeAnalyzer.Roslyn
open Markify.CodeAnalyzer.Roslyn.VisualBasic
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.VisualBasic
open Microsoft.CodeAnalysis.VisualBasic.Syntax

module TypeFactory =
    let fieldDefaultAccessModifier = [VisualBasicKeyword.privateModifier]

    let collectMember extract members =
        members
        |> Seq.map (fun c -> { Source = c; Value = extract c })
        |> Seq.toList

    let collectMembers extract members =
        members
        |> Seq.collect (fun c -> extract c |> Seq.map (fun d -> { Source = c; Value = d}))
        |> Seq.toList

    let findDefaultValue (initializer : EqualsValueSyntax) =
        match initializer with
        | null -> None
        | x -> Some (string x.Value)

    let findAccessModifiers (modifiers : SyntaxTokenList) =
        modifiers
        |> Seq.filter (fun c -> 
            VisualBasicKeyword.accessModifiers 
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
            VisualBasicKeyword.accessModifiers 
            |> Set.contains (c.Kind()) 
            |> not)
        |> Seq.map (fun c -> c.Text)
        |> Seq.toList

    let findType (clause : AsClauseSyntax) =
        match clause with
        | null -> "Object"
        | _ -> string (clause.Type())

    let findReturnType (delegateType : SyntaxToken) clause =
        match delegateType.Kind() with
        | SyntaxKind.SubKeyword -> "Void"
        | _ -> findType clause

    let extractGenerics (parameters : TypeParameterListSyntax) =
        parameters.Parameters
        |> Seq.map (fun c ->
            let variance =
                match c.VarianceKeyword.Value with
                | null -> None
                | x -> Some (string x)
            {   Name = c.Identifier.Text
                Modifier = variance
                Constraints = TypeSyntaxHelper.getGenericConstraints c |> Seq.map string |> Seq.toList })
        |> Seq.toList

    let extractEnumValue (enumMember : EnumMemberDeclarationSyntax) : EnumValue = {
        Name = enumMember.Identifier.Text
        Value = findDefaultValue enumMember.Initializer }

    let extractParameter (parameter : ParameterSyntax) = {
        Name = parameter.Identifier.Identifier.Text
        Type = findType parameter.AsClause
        Modifier = findModifiers parameter.Modifiers |> List.tryHead 
        DefaultValue = findDefaultValue parameter.Default }

    let extractFields defaultAccessModifier (field : FieldDeclarationSyntax) =
        field.Declarators
        |> Seq.collect (fun c -> c.Names |> Seq.map (fun d -> (c, d)))
        |> Seq.map (fun (c, name) -> {
            Name = name.Identifier.Text
            Type = findType c.AsClause
            AccessModifiers = tryFindAccessModifiers field.Modifiers defaultAccessModifier
            Modifiers = findModifiers field.Modifiers
            DefaultValue = findDefaultValue c.Initializer })

    let extractEvent defaultAccessModifier (event : EventStatementSyntax) = [{
        EventInfo.Name = event.Identifier.Text 
        Type = findType event.AsClause
        AccessModifiers = tryFindAccessModifiers event.Modifiers defaultAccessModifier
        Modifiers = findModifiers event.Modifiers }]

    let tryFindAccessorFromBlock (property : PropertyBlockSyntax) accessor defaultAccessModifier =
        property.Accessors
        |> Seq.tryFind (fun c -> c.Kind() = accessor)
        |> function
            | Some x -> Some { AccessModifiers = tryFindAccessModifiers x.AccessorStatement.Modifiers defaultAccessModifier }
            | None -> None

    let tryFindAccessorFromStatement (property : PropertyStatementSyntax) accessor defaultAccessModifier =
        match property.Modifiers |> Seq.exists (fun c -> c.Kind() = accessor) with
        | true -> Some { AccessorInfo.AccessModifiers = tryFindAccessModifiers property.Modifiers defaultAccessModifier }
        | false -> None

    let extractProperty (property : PropertyStatementSyntax) defaultAccessModifier = {
        Name = property.Identifier.Text
        AccessModifiers = tryFindAccessModifiers property.Modifiers defaultAccessModifier
        Modifiers = findModifiers property.Modifiers
        Type = findType property.AsClause
        DefaultValue = findDefaultValue property.Initializer
        IsWrite = None
        IsRead = None }

    let extractPropertyFromBlock defaultAccessModifier (property : PropertyBlockSyntax)  = {
        extractProperty property.PropertyStatement defaultAccessModifier with
            IsWrite = tryFindAccessorFromBlock property SyntaxKind.SetKeyword defaultAccessModifier
            IsRead = tryFindAccessorFromBlock property SyntaxKind.GetKeyword defaultAccessModifier }

    let extractPropertyFromStatement defaultAccessModifier (property : PropertyStatementSyntax)  = {
        extractProperty property defaultAccessModifier with
            IsWrite = tryFindAccessorFromStatement property SyntaxKind.SetKeyword defaultAccessModifier
            IsRead = tryFindAccessorFromStatement property SyntaxKind.GetKeyword defaultAccessModifier }

    let extractMethod defaultAccessModifier (method : MethodStatementSyntax) =
        let genericParameters = TypeSyntaxHelper.getGenericParameters method
        {   Name = method.Identifier.ValueText
            Generics = extractGenerics genericParameters
            AccessModifiers = tryFindAccessModifiers method.Modifiers defaultAccessModifier
            Modifiers = findModifiers method.Modifiers
            Parameters = method.ParameterList.Parameters |> Seq.map extractParameter |> Seq.toList
            ReturnType = findReturnType method.SubOrFunctionKeyword method.AsClause }

    let extractBaseTypes baseTypes =
        baseTypes
        |> Seq.map string 
        |> Seq.toList

    let extractEnumMembers (enumNode : EnumBlockSyntax) =
        let values = enumNode.Members |> Seq.map (fun c -> c :?> EnumMemberDeclarationSyntax)
        Enum { Values = collectMember extractEnumValue values }

    let extractDelegateMembers (delegateNode : DelegateStatementSyntax) = Delegate { 
        Parameters = collectMember extractParameter delegateNode.ParameterList.Parameters
        ReturnType = findReturnType delegateNode.SubOrFunctionKeyword delegateNode.AsClause }

    let extractStructureMembers typeNode = Structure {
        Fields = 
            FieldSyntaxCollector().Visit(typeNode) 
            |> collectMembers (extractFields fieldDefaultAccessModifier)
        Properties = seq {
            yield! PropertyBlockSyntaxCollector().Visit(typeNode) 
            |> collectMember (extractPropertyFromBlock fieldDefaultAccessModifier)
            yield! PropertyStatementSyntaxCollector().Visit(typeNode) 
            |> collectMember (extractPropertyFromStatement fieldDefaultAccessModifier)} |> Seq.toList
        Events = 
            EventSyntaxCollector().Visit(typeNode)
            |> collectMembers (extractEvent fieldDefaultAccessModifier)
        Methods = 
            MethodSyntaxCollector().Visit(typeNode) 
            |> collectMember (extractMethod fieldDefaultAccessModifier) }

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
        let baseTypes = TypeSyntaxHelper.getBaseTypes node
        match SyntaxHelper.getName node with
        | Some x -> Success {
            Name = x.ValueText
            AccessModifiers = findAccessModifiers modifiers 
            Modifiers = findModifiers modifiers
            Generics = extractGenerics genericParameters
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