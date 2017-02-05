namespace Markify.Domain.Ide

open System.Collections.Generic

type IIdeEnvironment =
    abstract member CurrentSolution: Solution Option with get

    abstract member CurrentProject: Project Option  with get