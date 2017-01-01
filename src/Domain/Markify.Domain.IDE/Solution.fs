namespace Markify.Domain.Ide

open System

type SolutionName = string

type SolutionPath = Uri

type Solution = {
    Name : SolutionName
    Path : SolutionPath
    Projects : Project seq }