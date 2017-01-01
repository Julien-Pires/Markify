namespace Markify.Services.Roslyn.Tests

open System.Reflection
open Markify.Services.Roslyn
open Markify.Services.Roslyn.Common
open Ploeh.AutoFixture.Kernel

type LanguageAnalyzerArgs () =
    interface ISpecimenBuilder with
        member this.Create (request : obj, context : ISpecimenContext) =
            match request with
            | :? ParameterInfo as x ->
                let isExpectedObject = typeof<RoslynAnalyzer> = x.Member.DeclaringType
                let isExpectedArg = typeof<((string -> SourceContent) * string seq) seq> = x.ParameterType
                match (isExpectedObject, isExpectedArg) with
                | (true, true) -> seq {
                    yield CSharpAnalyzer.analyze, Seq.singleton "cs"
                    yield VisualBasicAnalyzer.analyze, Seq.singleton "vb" } :> obj
                | _ -> NoSpecimen() :> obj
            | _ -> NoSpecimen() :> obj