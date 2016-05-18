namespace Markify.Core.IO

module IO =
    open System.IO

    let private ioWorkflow = new IOWorkflow()

    let fileExists path =
          Success false
//        ioWorkflow {
//            let! exists = Success (File.Exists path)
//            return exists
//        } 

    let readFile path =
          IOException false
//        ioWorkflow{
//            let! exists = fileExists path
//            let! content = (fun c -> Success (File.ReadAllText c)) path
//            return content
//        }