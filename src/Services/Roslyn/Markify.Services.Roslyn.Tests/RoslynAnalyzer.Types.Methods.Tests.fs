namespace Markify.Services.Roslyn.Tests

module RoslynAnalyzerTypesMethodsTests =
    open System
    open Markify.Domain.Ide
    open Markify.Domain.Compiler
    open Markify.Services.Roslyn
    open Xunit
    open Swensen.Unquote

    let getMethods = function
        | Class c | Struct c | Interface c -> c.Methods
        | _ -> Seq.empty

    let findMethod methodName (types : TypeDefinition seq) typeName =
        types
        |> Seq.find (fun d -> d.Identity.Name = typeName)
        |> getMethods
        |> Seq.find (fun d -> d.Identity.Name = methodName)

    let findParameter parameterName definition =
        definition.Parameters
        |> Seq.find (fun c -> c.Name = parameterName)

    [<Theory>]
    [<MultiProjectData("TypeMembers/ClassMethods", ProjectLanguage.CSharp, "FooType", 11)>]
    [<MultiProjectData("TypeMembers/StructMethods", ProjectLanguage.CSharp, "FooType", 9)>]
    [<MultiProjectData("TypeMembers/InterfaceMethods", ProjectLanguage.CSharp, "FooType", 6)>]
    [<MultiProjectData("TypeMembers/ClassMethods", ProjectLanguage.VisualBasic, "FooType", 9)>]
    [<MultiProjectData("TypeMembers/StructMethods", ProjectLanguage.VisualBasic, "FooType", 7)>]
    [<MultiProjectData("TypeMembers/InterfaceMethods", ProjectLanguage.VisualBasic, "FooType", 5)>]
    let ``Analyze should return expected methods count`` (typeName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let count =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = typeName)
                    |> getMethods
                count::acc) []

        test <@ actual |> List.forall (fun c -> (c |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/ClassMethods", ProjectLanguage.CSharp, "FooType", "PublicMethod")>]
    [<MultiProjectData("TypeMembers/StructMethods", ProjectLanguage.CSharp, "FooType", "PublicMethod")>]
    [<MultiProjectData("TypeMembers/InterfaceMethods", ProjectLanguage.CSharp, "FooType", "IntMethod")>]
    [<MultiProjectData("TypeMembers/ClassMethods", ProjectLanguage.VisualBasic, "FooType", "PublicMethod")>]
    [<MultiProjectData("TypeMembers/StructMethods", ProjectLanguage.VisualBasic, "FooType", "PublicMethod")>]
    [<MultiProjectData("TypeMembers/InterfaceMethods", ProjectLanguage.VisualBasic, "FooType", "IntMethod")>]
    let ``Analyze should return expected method name`` (typeName, methodName, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methods =
                    library.Types
                    |> Seq.find (fun d -> d.Identity.Name = typeName)
                    |> getMethods
                    |> Seq.filter (fun d -> d.Identity.Name = methodName)
                methods::acc) []
        
        test <@ actual |> List.forall (fun c -> (c |> Seq.length) = 1) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/ContainerMethods", ProjectLanguage.CSharp, "FooType", "Method", "private")>]
    [<MultiProjectData("TypeMembers/ClassMethods", ProjectLanguage.CSharp, "FooType", "InternalProtectedMethod", "internal;protected")>]
    [<MultiProjectData("TypeMembers/InterfaceMethods", ProjectLanguage.CSharp, "FooType", "Method", "public")>]
    [<MultiProjectData("TypeMembers/ContainerMethods", ProjectLanguage.VisualBasic, "FooType", "Method", "Private")>]
    [<MultiProjectData("TypeMembers/ClassMethods", ProjectLanguage.VisualBasic, "FooType", "InternalProtectedMethod", "Friend;Protected")>]
    [<MultiProjectData("TypeMembers/InterfaceMethods", ProjectLanguage.VisualBasic, "FooType", "Method", "Public")>]
    let ``Analyze should return expected method access modifiers`` (typeName, methodName, modifiers : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methodDefinition = findMethod methodName library.Types typeName
                let accessModifiers = Set methodDefinition.Identity.AccessModifiers
                accessModifiers::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/ClassMethods", ProjectLanguage.CSharp, "FooType", "VirtualMethod", "virtual")>]
    [<MultiProjectData("TypeMembers/ContainerMethods", ProjectLanguage.CSharp, "FooType", "PartialMethod", "partial")>]
    [<MultiProjectData("TypeMembers/ClassMethods", ProjectLanguage.VisualBasic, "FooType", "VirtualMethod", "Overridable")>]
    [<MultiProjectData("TypeMembers/ContainerMethods", ProjectLanguage.VisualBasic, "FooType", "PartialMethod", "Partial")>]
    let ``Analyze should return expected method modifiers`` (typeName, methodName, modifiers : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| modifiers.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methodDefinition = findMethod methodName library.Types typeName
                let accessModifiers = Set methodDefinition.Identity.Modifiers
                accessModifiers::acc) []

        test <@ actual |> List.forall ((=) expected) @>
    
    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp, "FooType", "SingleGenericMethod", 1)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp, "FooType", "MultiGenericMethod", 2)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic, "FooType", "SingleGenericMethod", 1)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic, "FooType", "MultiGenericMethod", 2)>]
    let ``Analyze should return expected method generic parameters count`` (typeName, methodName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methodDefinition = findMethod methodName library.Types typeName
                let parameters = methodDefinition.Identity.Parameters
                parameters::acc) []

        test <@ actual |> List.forall (fun c -> (c |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp, "FooType", "SingleGenericMethod", "T")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp, "FooType", "MultiGenericMethod", "Y")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic, "FooType", "SingleGenericMethod", "T")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic, "FooType", "MultiGenericMethod", "Y")>]
    let ``Analyze should return expected method generic parameter name`` (typeName, methodName, parameterName, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methodDefinition = findMethod methodName library.Types typeName
                let parameter = 
                    methodDefinition.Identity.Parameters
                    |> Seq.filter (fun d -> d.Name = parameterName)
                parameter::acc) []

        test <@ actual |> List.forall (fun c -> (c |> Seq.length) = 1) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp, "FooType", "SingleGenericMethod", "T", 0)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp, "FooType", "MultiGenericMethod", "T", 1)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp, "FooType", "MultiGenericMethod", "Y", 2)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic, "FooType", "SingleGenericMethod", "T", 0)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic, "FooType", "MultiGenericMethod", "T", 1)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic, "FooType", "MultiGenericMethod", "Y", 2)>]
    let ``Analyze should return expected method generic parameter constraints count`` (typeName, methodName, parameterName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methodDefinition = findMethod methodName library.Types typeName
                let parameter =
                    methodDefinition.Identity.Parameters
                    |> Seq.find (fun d -> d.Name = parameterName)
                parameter.Constraints::acc)[]

        test <@ actual |> List.forall (fun c -> (c |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp, "FooType", "MultiGenericMethod", "T", "IList")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp, "FooType", "MultiGenericMethod", "Y", "IDisposable;IEnumerable")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic, "FooType", "MultiGenericMethod", "T", "IList")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic, "FooType", "MultiGenericMethod", "Y", "IDisposable;IEnumerable")>]
    let ``Analyze should return expected method generic parameter constraint name`` (typeName, methodName, parameterName, constraints : string, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let expected = Set <| constraints.Split ([|';'|], StringSplitOptions.RemoveEmptyEntries)
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methodDefinition = findMethod methodName library.Types typeName
                let parameter =
                    methodDefinition.Identity.Parameters
                    |> Seq.find (fun d -> d.Name = parameterName)
                let parameterConstraints = Set parameter.Constraints
                parameterConstraints::acc) []

        test <@ actual |> List.forall ((=) expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "Method", 0)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "SingleGenericMethod", 1)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "WithParametersMethod", 3)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "Method", 0)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "SingleGenericMethod", 1)>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "WithParametersMethod", 3)>]
    let ``Analyze should return expected method parameters count`` (typeName, methodName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methodDefinition = findMethod methodName library.Types typeName
                methodDefinition.Parameters::acc) []

        test <@ actual |> List.forall (fun c -> (c |> Seq.length) = expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "SingleGenericMethod", "foo")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "WithParametersMethod", "bar")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "SingleGenericMethod", "foo")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "WithParametersMethod", "bar")>]
    let ``Analyze should return expected method parameter name`` (typeName, methodName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methodDefinition = findMethod methodName library.Types typeName
                methodDefinition.Parameters::acc) []
        
        test <@ actual |> List.forall (fun c -> c |> Seq.exists (fun d -> d.Name = expected)) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "WithParametersMethod", "foo", "int")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "SingleGenericMethod", "foo", "T")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "WithNoNameParameterMethod", "__arglist", "__arglist")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "WithParametersMethod", "foo", "Integer")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "SingleGenericMethod", "foo", "T")>]
    let ``Analyze should return expected method parameter type`` (typeName, methodName, parameterName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameter =
                    (library.Types, typeName)
                    ||> findMethod methodName
                    |> findParameter parameterName
                parameter::acc) []

        test <@ actual |> List.forall (fun c -> c.Type = expected) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "WithParametersMethod", "foo")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "WithParametersMethod", "foo")>]
    let ``Analyze should return no modifier when method parameter has none`` (typeName, methodName, parameterName, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameter =
                    (library.Types, typeName)
                    ||> findMethod methodName
                    |> findParameter parameterName
                parameter.Modifier::acc) []

        test <@ actual |> List.forall ((=) None) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "WithParametersMethod", "bar", "ref")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "WithParametersMethod", "foobar", "out")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "WithParametersMethod", "bar", "ByRef")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "WithParametersMethod", "foobar", "ByVal")>]
    let ``Analyze should return expected method parameter modifiers`` (typeName, methodName, parameterName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameter =
                    (library.Types, typeName)
                    ||> findMethod methodName
                    |> findParameter parameterName
                parameter.Modifier::acc) []

        test <@ actual |> List.forall ((=) (Some expected)) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "WithParametersMethod", "foo")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "WithParametersMethod", "foo")>]
    let ``Analyze should return no default value when method parameter has none`` (typeName, methodName, parameterName, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameter =
                    (library.Types, typeName)
                    ||> findMethod methodName
                    |> findParameter parameterName
                parameter.DefaultValue::acc) []

        test <@ actual |> List.forall ((=) None) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "SingleGenericMethod", "foo", "default(T)")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "SingleGenericMethod", "foo", "Nothing")>]
    let ``Analyze should return expected method parameter default value`` (typeName, methodName, parameterName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let parameter =
                    (library.Types, typeName)
                    ||> findMethod methodName
                    |> findParameter parameterName
                parameter.DefaultValue::acc) []

        test <@ actual |> List.forall ((=) (Some expected)) @>

    [<Theory>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "IntMethod", "int")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.CSharp,"FooType", "SingleGenericMethod", "T")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "IntMethod", "Integer")>]
    [<MultiProjectData("TypeMembers/AllTypesMethods", ProjectLanguage.VisualBasic,"FooType", "SingleGenericMethod", "T")>]
    let ``Analyze should return expected method return type`` (typeName, methodName, expected, sut : RoslynAnalyzer, projects : ProjectInfo[]) =
        let actual =
            projects
            |> Seq.fold (fun acc c ->
                let library = (sut :> IProjectAnalyzer).Analyze c.Project
                let methodDefinition = findMethod methodName library.Types typeName
                methodDefinition.ReturnType::acc) []

        test <@ actual |> List.forall ((=) expected) @>