namespace Markify.Roslyn

open Microsoft.CodeAnalysis

module LanguageHelper =
    let inline getGenericParameters (x : ^T) =
        let parametersList = (^T : (member TypeParameterList : ^a)(x))
        match parametersList with
        | null -> Seq.empty
        | _ ->
            (^a : (member Parameters : 'b SeparatedSyntaxList)(parametersList))
            |> Seq.map (fun c -> 
                let name = (^b : (member Identifier : SyntaxToken)(c)).Text
                let modifier = 
                    match (^b : (member VarianceKeyword : SyntaxToken)(c)).Value with
                    | null -> "None"
                    | x -> x.ToString()
                let param = {
                    Name = name
                    Modifier = modifier}
                param)

    let inline getBaseTypes (x : ^T) =
        match (^T : (member BaseList : ^a)(x)) with
        | null -> Seq.empty
        | x ->
            (^a : (member Types : ^b)(x))
            |> Seq.map (fun c -> (^c : (member Type : ^d)(c)).ToString())