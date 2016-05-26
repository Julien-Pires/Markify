namespace Markify.Models

open System

module Context =
    type ProjectName = string

    type FilesList = Uri seq

    type Project = {
        Name : ProjectName
        Files : FilesList
    }