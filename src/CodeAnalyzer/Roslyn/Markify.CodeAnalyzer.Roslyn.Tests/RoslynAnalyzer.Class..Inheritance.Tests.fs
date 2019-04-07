namespace Markify.CodeAnalyzer.Roslyn.Tests

open Markify.CodeAnalyzer
open Markify.Tests.Extension
open Expecto
open Swensen.Unquote
open Fixtures

module RoslynAnalyzer_ClassInheritance_Tests =
    [<Tests>]
    let inheritanceTests =
        let contents = [
            (ProjectLanguage.CSharp, ["
                public class InheritType : Exception { }
                public class ImplementInterfaceType : IDisposable { }
                public class ImplementMultipleInterfaceType : IDisposable, IEnumerable { }
                public class MixedInheritanceType : Exception, IDisposable { }
            "])
            (ProjectLanguage.VisualBasic, ["
                Public Class InheritType
                    Inherits Exception
                End Class
                Public Class ImplementInterfaceType
                    Implements IDisposable
                End Class
                Public Class ImplementMultipleInterfaceType
                    Implements IDisposable, IEnumerable
                End Class
                Public Class MixedInheritanceType
                    Inherits Exception
                    Implements IDisposable
                End Class
            "])
        ]
        testList "Analyze/Class" [
            yield! testRepeatParameterized
                "should return base types when class inherits from other types" [
                (withProjects contents, ("InheritType", Set ["Exception"]))
                (withProjects contents, ("ImplementInterfaceType", Set ["IDisposable"]))
                (withProjects contents, ("ImplementMultipleInterfaceType", Set ["IDisposable"; "IEnumerable"]))
                (withProjects contents, ("MixedInheritanceType", Set ["Exception"; "IDisposable"]))]
                (fun sut project (name, expected) () ->
                    let result = sut.Analyze project |> findClass name
                        
                    test <@ result.Identity.BaseTypes |> Set 
                                                      |> (=) expected @>)
        ]