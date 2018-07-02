open System
open FSLibrary

[<EntryPoint>]
let main argv =
    Console.WriteLine(testString)
    Console.ReadLine() |> ignore
    0 // return an integer exit code