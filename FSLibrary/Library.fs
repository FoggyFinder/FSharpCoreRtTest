namespace FSLibrary

module CoreType = 
    open System

    type Access = 
        | ReadOnly | Full 
        with override x.ToString() = 
                match x with
                | ReadOnly -> "ReadOnly"
                | Full -> "Full"

    type User = {
        Login : string
        Access : Access
        Date : DateTime
    } with override x.ToString() = 
            String.Format("{0}[{1}] {2}", 
                x.Login, x.Access, x.Date.ToShortDateString())
    
    type Note = string * string
    type Article = string * string * string
    
    type News = 
        | Short of Note
        | Detailed of Article
        with override x.ToString() = 
                match x with
                | Short (header, source) -> 
                    String.Format("{0}\nSource : {1}\n", header, source)
                | Detailed (header, desc, source) -> 
                    String.Format("{0}\n{1}\nSource : {2}\n", header, desc, source)       
    
    type Record  = {
        User : User
        Notes : News list        
    } with override x.ToString() = 
            let notes = 
                 x.Notes 
                 |> List.map (fun n -> n.ToString()) 
                 |> String.concat "\n"
            String.Format("{0}\n{1}\n", x.User, notes)
    
    let rnd = Random()
    
    let createUser login access = {
        Login = login
        Access = access
        Date = DateTime.Today
    }
    
    let createRecord user notes = {
        User = user
        Notes = notes
    }

    let addNote record note = {
        record with Notes = note :: record.Notes
    }

module Serialization = 

    open Newtonsoft.Json
    
    let serialize value = JsonConvert.SerializeObject value
    let serializeIndented value = 
        JsonConvert.SerializeObject(value, Formatting.Indented)

    let deserialize<'a> s =
        try 
            JsonConvert.DeserializeObject<'a> s
            |> Ok
        with exp -> 
            Error(exp.Message)

[<RequireQualifiedAccess>]
module SampleData = 
    open CoreType

    let testData = 
        [
            createUser "Scylla" Full, 
                [ Short 
                    ("How Philip K Dick redefined what it means to be (in)human",
                     "https://www.independent.co.uk/arts-entertainment/philip-k-dick-human-andriod-blade-runner-empathy-a8299026.html"); 
                  Short 
                    ("China plans historic mission to the moon's 'dark' side", 
                     "https://www.nbcnews.com/mach/science/china-plans-historic-mission-moon-s-dark-side-ncna883591");
                  Detailed 
                    ("Antarctica Is Losing An Insane Amount of Ice. Nothing About This Is Good.", 
                     "Antarctica has lost 3 trillion tons of ice in the past 25 years, and that ice loss has accelerated rapidly over the last five years.",
                     "https://www.space.com/40879-antarctica-3-trillion-tons-ice-lost.html")]
            createUser "Charybdis" ReadOnly, 
                [ ]
        ] 
        |> List.map((<||) createRecord)
