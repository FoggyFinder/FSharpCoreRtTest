module FSLibrary

open LibraryTestFx
open Newtonsoft.Json

let s = System.Text.StringBuilder()

// Fails on UWP and CoreRT. We can get this to work using rd.xml on CoreRT. Can we do this on UWP?
//s.Append(sprintf "%s" "Test1") |> ignore

// Quotations to Expressions, useful for LINQ from F#, fail on UWP.
//let expr = 
//    <@ System.Func<int,int>(fun i -> i + 1) @>
//    |> Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.QuotationToExpression
//    :?> System.Linq.Expressions.Expression<int>

// NewtonSoft.Json works on UWP. On CoreRT it requires rd.xml.
//module TestJsonSerialiation =
//    type Tree =
//        | Branch of Tree * Tree
//        | Leaf of int
//    let tree = Branch(Branch(Leaf 3, Leaf 4),Leaf 4)
//    type IdTree =
//        { Id:int
//          Tree: Tree }
//    let idTree = {Id = 3; Tree = tree}
//    let serializedIdTree = JsonConvert.SerializeObject(idTree)
//    let serializedIdTree' = serializedIdTree |> JsonConvert.DeserializeObject<IdTree> |> JsonConvert.SerializeObject
//    s.AppendLine(serializedIdTree) |> ignore
//    Assert.AreEqual(serializedIdTree',serializedIdTree)

module TailRecursion =
    // from https://gist.github.com/dsyme/82c49d5cb63d04f3b2b3502d51e6277c
    let HugeInt = 100
    
    // Mutually recurisve tail call, compiler will emit tail instruction
    // does not work on UWP when HugeInt = 10000000
    // works on CoreRT when HugeInt = 10000000
    // works on UWP and CoreRT when HugeInt = 100
    // Conclusion: UWP ignores .tail while CoreRT implements it.
    let rec mutualTail1IsOdd x = 
        match x with
        | 1 -> true
        | n -> mutualTail1IsEven (x - 1)
    and mutualTail1IsEven x =
        match x with
        | 1 -> false
        | 0 -> true
        | n -> mutualTail1IsOdd (x - 1)
    let odd = mutualTail1IsOdd HugeInt
    let even = mutualTail1IsEven HugeInt
    s.AppendLine("HugeInt is odd: " + odd.ToString()) |> ignore
    s.AppendLine("HugeInt is even: " + even.ToString()) |> ignore

//type ChannelChangedHandler = delegate of obj * int -> unit
//type C() =  
//    let channelChanged = new Event<ChannelChangedHandler,_>()
//    [<CLIEvent>]    
//    member self.ChannelChanged = channelChanged.Publish
//    member self.ChangeChannel(n) = channelChanged.Trigger(self,n)

let testString = s.ToString()