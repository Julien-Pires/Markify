namespace Markify.Models

open System

module Context =
    type ProjectName = string
    type ProjectPath = Uri
    type FilesList = Uri seq
    type Project = {
        Name : ProjectName
        Path : ProjectPath
        Files : FilesList
    }

    type SolutionName = string
    type SolutionPath = Uri
    type ProjectList = ProjectName seq
    type Solution = {
        Name : SolutionName
        Path : SolutionPath
        Projects : ProjectList
    }