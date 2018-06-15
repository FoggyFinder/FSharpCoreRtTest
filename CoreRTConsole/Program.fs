open System
open FSLibrary

[<EntryPoint>]
let main argv =
    Console.WriteLine (FSLibrary.testString())
    0 // return an integer exit code