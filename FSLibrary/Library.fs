module FSLibrary

open LibraryTestFx
open System.Text

let stringbuilder = StringBuilder()
let print x = stringbuilder.AppendLine(x.ToString()) |> ignore; x

let result = 1 + 1 = 2 |> print
let hello = "Hello" + " World" |> print
let verbatimXml = @"<book title=""Paradise Lost"">" |> print
let tripleXml = """<book title="Paradise Lost">""" |> print
let poem = 
    "The lesser world was daubed\n\
     By a colorist of modest skill\n\
     A master limned you in the finest inks\n\
     And with a fresh-cut quill."  |> print
let b, i, l = 86uy |> print, 86 |> print, 86L |> print
let s, f, d, bi = 4.14F |> print, 4.14 |> print, 0.7833M |> print, 9999I  |> print
let negate x = x * -1
let minus2 = negate 2 |> print
let square x = x * x
let square2 = square 2 |> print
let squareNegate x =
    negate (square x) 
let minussquare2 = squareNegate 2 |> print
let ``square, negate`` x = 
    x |> square |> negate
let minussquare2' = ``square, negate`` 2 |> print
let sumOfLengths (xs : string []) = 
    xs 
    |> Array.map (fun s -> s.Length)
    |> Array.sum
let sumOfLengthsTest = sumOfLengths [| "one"; "seven"; ""|] |> print
let squareNegate'' = 
    square >> negate
let minussquare2'' = squareNegate 2 |> print
let rec fact x =
    if x < 1 then 1
    else x * fact (x - 1)
let fiveFactorial = fact 5 |> print
let rec even x =
   if x = 0 then true 
   else odd (x - 1)
and odd x =
   if x = 0 then false
   else even (x - 1)
let sevenIsEven = even 7 |> print
let sevenIsOdd = odd 7 |> print
let rec fib n =
    match n with
    | 0 -> 0
    | 1 -> 1
    | _ -> fib (n - 1) + fib (n - 2)
let fib5 = fib 5 |> print
let sign x = 
    match x with
    | 0 -> 0
    | x when x < 0 -> -1
    | x -> 1
let sign7 = sign 7 |> print
let fst' (x, _) = x
let fst34 = fst' (3,4) |> print
let rec fib' = function
    | 0 -> 0
    | 1 -> 1
    | n -> fib' (n - 1) + fib' (n - 2)
let fib5' = fib' 5 |> print
let list1 = [ "a"; "b" ] |> print
let list2 = "c" :: list1 |> print
let list3 = list1 @ list2 |> print
let rec sum list = 
    match list with
    | [] -> 0
    | x :: xs -> x + sum xs
let sumSomeList = sum [1;2;3] |> print
let array1 = [| "a"; "b" |] |> print
let first = array1.[0] |> print
let seq1 = 
    seq {
        // "yield" adds one element
        yield 1
        yield 2
        // "yield!" adds a whole subsequence
        yield! [5..10]
    } |> print
let xs = [ 1..2..9 ] |> print
let ys = [| for i in 0..4 -> 2 * i + 1 |] |> print
let zs = List.init 5 (fun i -> 2 * i + 1) |> print
let xs' = Array.fold (fun str (n:int) -> 
            (str + ", " + n.ToString())) "" [| 0..9 |] |> print
let last xs = List.reduce (fun acc x -> x) xs
let lastxs = last xs |> print
let ys' = Array.map (fun x -> x * x) [| 0..9 |] |> print
let _ = List.iter (print >> ignore) [ 0..9 ]
let zs' =
    seq { 
        for i in 0..9 do
            print ("Adding " + i.ToString()) |> ignore
            yield i
    } |> print
let x = (1, "Hello") |> print
let y = ("one", "two", "three")  |> print
let (a', b') = x |> print
let c' = fst (1, 2) |> print
let d' = snd (1, 2) |> print
let print' tuple =
    match tuple with
    | (a, b) -> print "matched"
print' (1,2) |> ignore
type Person = { Name : string; Age : int }
let paul = { Name = "Paul"; Age = 28 } // print here doesn't work with UWP
let paulIs28 = paul.Age = 28 |> print
let paulsTwin = { paul with Name = "Jim" }
let paulsTwinsName = paulsTwin.Name |> print
type Person with
    member x.Info = (x.Name, x.Age)
let isPaul person =
    match person with
    | { Name = "Paul" } -> true
    | _ -> false
let paulIsPaul = isPaul paul |> print
let paulsTwinIsPaul = isPaul paulsTwin |> print
type Tree<'T> =
    | Node of Tree<'T> * 'T * Tree<'T>
    | Leaf
let rec depth = function
    | Node(l, _, r) -> 1 + max (depth l) (depth r)
    | Leaf -> 0
let someDepth = depth (Node(Node(Leaf,5,Leaf),3,Leaf)) |> print
let optionPatternMatch input =
   match input with
    | Some i -> print ("input is an int=" + i.ToString()) |> ignore
    | None -> print "input is missing" |> ignore
optionPatternMatch (Some 8)
type OrderId = Order of string
let orderId = Order "12" // |> print doesn't work here on UWP
let (Order id) = orderId
let _ = id |> print     // OK until here
type Vector(x : float, y : float) =
    let mag = sqrt(x * x + y * y)
    member this.X = x
    member this.Y = y
    member this.Mag = mag
    member this.Scale(s) =
        Vector(x * s, y * s)
    static member (+) (a : Vector, b : Vector) =
        Vector(a.X + b.X, a.Y + b.Y)
let vec = Vector(3.,2.) |> print
let scaledVec = vec.Scale(2.) |> print
let magScaledVec = scaledVec.Mag |> print
type Animal() =
    member __.Rest() = () 
type Dog() =
    inherit Animal()
    member __.Run() =
        base.Rest()
let dog = Dog() |> print
let animal = dog :> Animal |> print
let shouldBeADog = animal :?> Dog |> print
type IVector =
    abstract Scale : float -> IVector
type Vector'(x, y) =
    interface IVector with
        member __.Scale(s) =
            Vector'(x * s, y * s) :> IVector
    member __.X = x
    member __.Y = y
let v' = Vector(3.,4.)
let nine = v'.Scale(3.).X |> print
type ICustomer =
    abstract Name : string
    abstract Age : int
let createCustomer name age =
    { new ICustomer with
        member __.Name = name
        member __.Age = age }
let john = (createCustomer "John" 99).Name |> print
let (|Even|Odd|) i = 
    if i % 2 = 0 then Even else Odd
let SevenisOdd =
    match 7 with
    | Odd -> true
    | Even -> false
    |> print
let testNumber i =
    match i with
    | Even -> print (i.ToString() + " is even")
    | Odd -> print (i.ToString() + " is odd" )
testNumber 7 |> ignore
let (|DivisibleBy|_|) by n = 
    if n % by = 0 then Some DivisibleBy else None
let SixIsDivisibleByFive =
    match 6 with
    | DivisibleBy 5 -> true
    | _ -> false
    |> print
let fizzBuzz = function 
    | DivisibleBy 3 & DivisibleBy 5 -> "FizzBuzz" 
    | DivisibleBy 3 -> "Fizz" 
    | DivisibleBy 5 -> "Buzz" 
    | i -> string i
let fizzBuzz7 = fizzBuzz 7 |> print

// Define a string testString for display.
let testString() =
    stringbuilder.ToString()