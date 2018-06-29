open System
open FSLibrary
open CSLibrary

let prTest() = 
    let str = FSLibrary.testString()
    Console.WriteLine (str)
    printfn "%s" str   
    
let eTest() = 
    let c = C()
    let h1 = ChannelChangedHandler(fun _ ch -> Console.WriteLine("Channel = {0}", ch))
    c.ChannelChanged.AddHandler(h1)
    c.ChangeChannel(3)

let expTest() = 
    [| 0..5 |].Map(fun x -> x * 2)
    |> Seq.iter(fun v -> Console.WriteLine(v))

[<EntryPoint>]
let main argv =
    prTest()
    // eTest()
    // expTest()
    Console.ReadLine() |> ignore
    0 // return an integer exit code