# FSharp CoreRT and .Net Native tests

To test CoreRT, run the following command:

```cmd
dotnet publish -r <RID> -c <Configuration>
```

for example:
```cmd
dotnet publish -r win-x64 -c release
```

For more information see [restore-and-publish-your-app](https://github.com/dotnet/corert/tree/master/samples/HelloWorld#restore-and-publish-your-app)


To test .Net Native:

1. Remove the block on F# code by editing the line containing FSharpRule in `"C:\Program Files (x86)\Microsoft SDKs\UWPNuGetPackages\runtime.win10-x64.Microsoft.Net.Native.Compiler\2.1.8\tools\x64\ilc\tools\GatekeeperConfig.xml"` (or appropriate variation of that path - see the ILT0005 error in step 2 for where to look) to: `<FSharpRule on="false">`

2. Run the UWP project in release mode.

# Description of current issues

We have identified several issues while testing. The main code tested was the F# cheatsheet, a production F# Xamarin.Forms app, and tail recursion and tail recursion and nested generics.

The code demonstrating the issues found is here: https://github.com/FoggyFinder/FSharpCoreRtTest/tree/MainIssues . Please test further and give your results and conclusisions here.

## NewtonSoft.Json serialization: OK on UWP and CoreRT

**.NET Native:** works by default.

Documentation: [Serialization and Metadata](https://docs.microsoft.com/en-us/dotnet/framework/net-native/serialization-and-metadata). But if you use NewtonSoft.Json you don't have to edit your rd.file (at least currently) because the specification for this library is included by default.

**CoreRT:** requires rd.xml.

This example ([WebAPI](https://github.com/dotnet/corert/tree/master/samples/WebApi)) shows how to use NewtonSoft.Json. The basic rd file:

```xml
<Assembly Name="Newtonsoft.Json">
    <Type Name="Newtonsoft.Json.Serialization.ObjectConstructor`1[[System.Object,System.Private.CoreLib]]" Dynamic="Required All" />
</Assembly>
<Assembly Name="System.Linq.Expressions">
    <Type Name="System.Linq.Expressions.ExpressionCreator`1[[Newtonsoft.Json.Serialization.ObjectConstructor`1[[System.Object,System.Private.CoreLib]],Newtonsoft.Json]]" Dynamic="Required All" />
    <Type Name="System.Linq.Expressions.ExpressionCreator`1[[System.Func`2[[System.Object,System.Private.CoreLib],[System.Object,System.Private.CoreLib]],System.Private.CoreLib]]" Dynamic="Required All" />
</Assembly>
<Assembly Name="Namespace" Dynamic="Required All" />
```

where `Namespace` is namespace in your app which contains types for serialization.

It may be not enough. For example if type contains F# list you have to add this line:

```xml
<Assembly Name="FSharp.Core" Dynamic="Required All">
    <Type Name="Microsoft.FSharp.Collections.ListModule" Dynamic="Required All">
        <Method Name="OfSeq" Dynamic="Required">
            <GenericArgument Name="System.Object, System.Private.CoreLib" />
        </Method>            
    </Type>
</Assembly>
```

## Sprintf, and .ToString() on record types and DUs: these fail on UWP and CoreRT.

**.NET Native:** just override .ToString for custom types and use `String.Format` instead of `sprintf`.

It is possible that rd.xml will enable sprintf on primitive types but we have not managed to get this working yet.

**CoreRT:** partially fixed by appropriate rd.xml for primitive types. See [Some FSharp.Core constructs don't run on CoreRT #4954](https://github.com/Microsoft/visualfsharp/issues/4954). The issue results from CoreRT/.NET Native restricting `MakeGenericType`/`MakeGenericMethod`. RD.XML (a file passed to the CoreRT compiler) can be used to tell the compiler that particular code needs to be generated (even though it statically looks like it isn't needed). If there's a reasonable bound on what MakeGenericType/MakeGenericMethod gets called with, RD.XML is all that's needed to make this work. See the simple example in this repo.

Related link: [Dynamic programming differences](https://docs.microsoft.com/en-us/dotnet/framework/net-native/migrating-your-windows-store-app-to-net-native#dynamic-programming-differences)

## F# Quotation to Expression: fails on CoreRT and UWP

This is useful for LINQ queries from F#.

Workaround: use an alternative to LINQ (e.g. SQL), or use LINQ from C#.

E.g. the following code fails:

```fsharp
let expr = 
    <@ System.Func<int,int>(fun i -> i + 1) @>
    |> Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.QuotationToExpression
    :?> System.Linq.Expressions.Expression<int>
```

## The .tail instruction: ignored on UWP

It appears that .Net Native ignores tail instructions, while CoreRT implements them. Lack of support for .tail calls on UWP does not break typical F# code. It is therefore recommended not to create code in UWP that is heavily dependent on tail call optimization.

## F# Event: works on UWP, fails on CoreRT

This needs more investigation on CoreRT.

```fsharp
type ChannelChangedHandler = delegate of obj * int -> unit
type C() =  
    let channelChanged = new Event<ChannelChangedHandler,_>()

    [<CLIEvent>]    
    member self.ChannelChanged = channelChanged.Publish
    member self.ChangeChannel(n) = channelChanged.Trigger(self,n)

let test() = 
    let c = C()
    let h1 = ChannelChangedHandler(fun _ ch -> Console.WriteLine("Channel = {0}", ch))
    c.ChannelChanged.AddHandler(h1)
    c.ChangeChannel(3)
```

```bash
Unhandled Exception: System.TypeInitializationException: A type initializer threw an exception. To determine which type, inspect the InnerException's StackTrace property. ---> EETypeRva:0x0063BF58: This object cannot be invoked because it was metadata-enabled for browsing only: 'FSLibrary.ChannelChangedHandler.Invoke(System.Object,System.Int32)' For more information, please visit  http://go.microsoft.com/fwlink/?LinkID=616867
   at CoreRTConsole!<BaseAddress>+0x13edd
   at CoreRTConsole!<BaseAddress>+0x8d667
   at CoreRTConsole!<BaseAddress>+0x88fc6
   at CoreRTConsole!<BaseAddress>+0x89388
   at CoreRTConsole!<BaseAddress>+0x850fb
   at CoreRTConsole!<BaseAddress>+0x21b7dd
   at CoreRTConsole!<BaseAddress>+0x1d8a90

   --- End of inner exception stack trace ---
   at CoreRTConsole!<BaseAddress>+0x1d8b38
   at CoreRTConsole!<BaseAddress>+0x1d8964
   at Microsoft.FSharp.Control.FSharpEvent`2.Trigger(Object, TArgs) + 0x23
   at Program.main(String[]) + 0xd
   at CoreRTConsole!<BaseAddress>+0x270f6a
```
