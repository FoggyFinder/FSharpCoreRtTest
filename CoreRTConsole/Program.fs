open System
open FSLibrary
open CSLibrary

[<EntryPoint>]
let main argv =
    Console.WriteLine(testString)
    // eTest()
    // expTest()
    Console.ReadLine() |> ignore
    0 // return an integer exit code