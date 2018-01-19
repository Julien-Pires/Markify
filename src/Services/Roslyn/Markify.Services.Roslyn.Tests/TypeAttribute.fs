namespace Markify.Services.Roslyn.Tests

open System
open Markify.Domain.Ide
open Markify.Services.Roslyn.Csharp
open Ploeh.AutoFixture
open Ploeh.AutoFixture.Xunit2
open Xunit.Sdk
open System.Reflection
open Markify.Services.Roslyn.VisualBasic
open Markify.Services.Roslyn.Common

module LanguageModules =
    let modules = [
        (ProjectLanguage.CSharp, CSharpModule() :> ILanguageModule);
        (ProjectLanguage.VisualBasic, VisualBasicModule() :> ILanguageModule)]

    let getModules includes =
        modules 
        |> Seq.filter (fun c ->
            match includes with
            | [] -> true
            | _ -> includes |> Seq.contains (fst c))

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type MultipleDataAttribute([<ParamArray>] attributes : DataAttribute[]) =
    inherit DataAttribute()
        override this.GetData (method : MethodInfo) =
            attributes
            |> Seq.collect (fun c -> c.GetData(method))

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type LanguageDataAttribute(project, language, languageModule : ILanguageModule, [<ParamArray>] values) =
    inherit InlineAutoDataAttribute(
        AutoDataAttribute(LanguageDataAttribute.Create project language languageModule),
        values)

    static member Create project language languageModule : IFixture = 
        let fixture = Fixture()
        fixture.Customizations.Add(LanguageModuleArgs(languageModule))
        fixture.Customize(ProjectCustomization(project, language))

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type MultiLanguageDataAttribute(project, includesOnly, [<ParamArray>] values) =
    inherit MultipleDataAttribute(
        (LanguageModules.getModules >> (MultiLanguageDataAttribute.GetLanguageAttributes project values)) includesOnly)

    static member private GetLanguageAttributes project values languageModules =
        languageModules 
        |> Seq.map (fun c -> LanguageDataAttribute(project, fst c, snd c, values) :> DataAttribute)
        |> Seq.toArray

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type ProjectDataAttribute(project, [<ParamArray>] values) =
    inherit MultiLanguageDataAttribute((sprintf "Projects/%s" project), [], values)

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type SingleLanguageProjectDataAttribute(project, language, [<ParamArray>] values) =
    inherit MultiLanguageDataAttribute((sprintf "Projects/%s" project), [language], values)