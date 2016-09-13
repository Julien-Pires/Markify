namespace Markify.Models.IDE

open System

type ProjectLanguage =
    | Unsupported = 0
    | CSharp = 1
    | VisualBasic = 2

type ProjectName = string
type ProjectPath = Uri
type FilesList = Uri seq
type Project = {
    Name : ProjectName
    Path : ProjectPath
    Language : ProjectLanguage
    Files : FilesList
}

type SolutionName = string
type SolutionPath = Uri
type Solution = {
    Name : SolutionName
    Path : SolutionPath
    Projects : Project seq
}