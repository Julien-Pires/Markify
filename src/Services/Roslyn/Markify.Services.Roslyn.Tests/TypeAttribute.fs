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
type MultiLanguageDataAttribute(project, [<ParamArray>] values) =
    inherit MultipleDataAttribute(
        LanguageDataAttribute(project, ProjectLanguage.CSharp, CSharpModule(), values),
        LanguageDataAttribute(project, ProjectLanguage.VisualBasic, VisualBasicModule(), values))

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type ProjectDataAttribute(project, [<ParamArray>] values) =
    inherit MultiLanguageDataAttribute((sprintf "Projects/%s" project), values)

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type ClassDataAttribute(project, [<ParamArray>] values) =
    inherit MultiLanguageDataAttribute((sprintf "Class/%s" project), values)

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type StructDataAttribute(project, [<ParamArray>] values) =
    inherit MultiLanguageDataAttribute((sprintf "Struct/%s" project), values)

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type InterfaceDataAttribute(project, [<ParamArray>] values) =
    inherit MultiLanguageDataAttribute((sprintf "Interface/%s" project), values)

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type EnumDataAttribute(project, [<ParamArray>] values) =
    inherit MultiLanguageDataAttribute((sprintf "Enum/%s" project), values)

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true)>]
type DelegateDataAttribute(project, [<ParamArray>] values) =
    inherit MultiLanguageDataAttribute((sprintf "Delegate/%s" project), values)