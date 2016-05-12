namespace Markify.Models

open System

module Context =

    type FilesList = Uri seq
    type ProjectContext = {
        Files : FilesList
    }