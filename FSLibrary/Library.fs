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
module TestJsonSerialiation =
    type Tree =
        | Branch of Tree * Tree
        | Leaf of int
    let tree = Branch(Branch(Leaf 3, Leaf 4),Leaf 4)
    type IdTree =
        { Id:int
          Tree: Tree }
    let idTree = {Id = 3; Tree = tree}
    let serializedIdTree = JsonConvert.SerializeObject(idTree)
    let serializedIdTree' = serializedIdTree |> JsonConvert.DeserializeObject<IdTree> |> JsonConvert.SerializeObject
    s.Append(serializedIdTree) |> ignore
    Assert.AreEqual(serializedIdTree',serializedIdTree)

type ChannelChangedHandler = delegate of obj * int -> unit
type C() =  
    let channelChanged = new Event<ChannelChangedHandler,_>()
    [<CLIEvent>]    
    member self.ChannelChanged = channelChanged.Publish
    member self.ChangeChannel(n) = channelChanged.Trigger(self,n)

let testString() = s.ToString()