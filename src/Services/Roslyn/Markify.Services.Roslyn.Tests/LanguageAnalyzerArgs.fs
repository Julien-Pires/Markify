namespace Markify.Services.Roslyn.Tests

open System.Reflection
open Markify.Services.Roslyn
open Markify.Services.Roslyn.Common
open Markify.Services.Roslyn.Csharp
open Markify.Services.Roslyn.VisualBasic
open Ploeh.AutoFixture.Kernel

type LanguageAnalyzerArgs () =
    interface ISpecimenBuilder with
        member this.Create (request : obj, context : ISpecimenContext) =
            match request with
            | :? ParameterInfo as x ->
                let isExpectedObject = typeof<RoslynAnalyzer> = x.Member.DeclaringType
                let isExpectedArg = typeof<ILanguageAnalyzer seq> = x.ParameterType
                match (isExpectedObject, isExpectedArg) with
                | (true, true) -> seq {
                    yield CSharpAnalyzer() :> ILanguageAnalyzer
                    yield VisualBasicAnalyzer() :> ILanguageAnalyzer } :> obj
                | _ -> NoSpecimen() :> obj
            | _ -> NoSpecimen() :> obj