namespace Markify.Core.FSharp

module Operators =
    let (>>=) m f = Option.bind f m