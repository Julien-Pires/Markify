namespace Markify.Domain.Ide

open System

type IIDEExplorer =
    abstract member Projects: Project seq with get

    abstract member ActiveProject: Project Option  with get

    abstract member ActiveSolution: Solution Option with get