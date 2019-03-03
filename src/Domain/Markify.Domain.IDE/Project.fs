namespace Markify.Domain.Ide

open System

type ProjectLanguage =
    | Unsupported = 0
    | CSharp = 1
    | VisualBasic = 2

type ProjectName = string
type ProjectPath = Uri
type File = Uri
type SolutionPath = Uri

type Project = {
    Name : ProjectName
    Path : ProjectPath
    Language : ProjectLanguage
    Files : File seq }

type Solution = {
    Path : SolutionPath }