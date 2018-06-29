module FSLibrary

open LibraryTestFx
open System
// Define a string testString for display.

let testString() = "Hello World"

type ChannelChangedHandler = delegate of obj * int -> unit
type C() =  
    let channelChanged = new Event<ChannelChangedHandler,_>()

    [<CLIEvent>]    
    member self.ChannelChanged = channelChanged.Publish
    member self.ChangeChannel(n) = channelChanged.Trigger(self,n)