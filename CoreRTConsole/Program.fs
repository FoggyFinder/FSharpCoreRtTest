open System
open FSLibrary

[<EntryPoint>]
let main argv =
    Console.WriteLine (FSLibrary.testString())
    Console.ReadLine() |> ignore
    0 // return an integer exit code