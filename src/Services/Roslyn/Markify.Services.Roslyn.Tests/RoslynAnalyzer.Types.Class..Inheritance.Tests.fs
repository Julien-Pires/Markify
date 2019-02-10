namespace Markify.Services.Roslyn.Tests

open Markify.Services.Roslyn
open Markify.Domain.Compiler
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzerTypesInheritanceTests =
    [<Tests>]
    let inheritanceTests =
        let contents = [
            (
                ProjectLanguage.CSharp,
                ["
                    public class InheritType : Exception { }

                    public class ImplementInterfaceType : IDisposable { }

                    public class MixedInheritanceType : Exception, IDisposable { }
                "]
            )
            (
                ProjectLanguage.VisualBasic, 
                ["
                    Public Class InheritType
                        Inherits Exception
                    End Class

                    Public Class ImplementInterfaceType
                        Implements IDisposable
                    End Class

                    Public Class MixedInheritanceType
                        Inherits Exception
                        Implements IDisposable
                    End Class
                "]
            )
        ]
        testList "Analyze" [
            yield! testRepeatParameterized
                "should return base types when class inherits from other types" [
                (withProjects contents, ("InheritType", ["Exception"]))
                (withProjects contents, ("ImplementInterfaceType", ["IDisposable"]))
                (withProjects contents, ("MixedInheritanceType", ["Exception"; "IDisposable"]))]
                (fun sut project (name, baseTypes) () ->
                    let assemblies = sut.Analyze project
                    let result = findClass assemblies name
                        
                    test <@ Set result.Identity.BaseTypes |> Set.isSubset (Set baseTypes) @>)
        ]