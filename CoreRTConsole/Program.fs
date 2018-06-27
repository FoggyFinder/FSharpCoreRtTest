open System
open FSLibrary
open FSLibrary.CoreType

[<EntryPoint>]
let main argv =
   let testData = SampleData.testData
   let json = Serialization.serializeIndented testData 
   Console.WriteLine("Json")
   Console.WriteLine(json)
   let undesData = Serialization.deserialize<CoreType.Record list>(json);
   Console.WriteLine("obj")
   match undesData with
   | Ok ok -> Console.WriteLine(ok)
   | Error error -> Console.WriteLine("Error = {0}", error)
   Console.ReadLine()
   0