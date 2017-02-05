namespace Markify.Core.FSharp

open System
open System.Runtime.CompilerServices

[<Extension>]
type FSharpOptionExtension =
    [<Extension>]
    static member IsSome(option : Option<'a>) =
        match option with
        | Some _ -> true
        | None -> false

    [<Extension>]
    static member IsNone (option : Option<'a>) = 
        FSharpOptionExtension.IsSome option |> not

    [<Extension>]
    static member Match (option : Option<'a>, some : Func<'a,'b>, none : Func<'b>) =
        match option with
        | Some x -> some.Invoke(x)
        | None -> none.Invoke()