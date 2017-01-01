namespace Markify.Domain.Ide

open System.Collections.Generic

type IProjectFilterProvider =
    abstract member SupportedLanguages: ProjectLanguage ISet with get

    abstract member AllowedExtensions: string ISet with get

type IIdeEnvironment =
    abstract member CurrentSolution: Solution Option with get

    abstract member CurrentProject: Project Option  with get