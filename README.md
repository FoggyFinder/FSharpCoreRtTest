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

# Description of current status

## Json serialization

.NET Native

Documentation's page - [Serialization and Metadata](https://docs.microsoft.com/en-us/dotnet/framework/net-native/serialization-and-metadata).

But if you use JSON.NET you don't have to edit your rd.file (at least currently) cause specification for this library is included by default.

CoreRT

There is example ([WebAPI](https://github.com/dotnet/corert/tree/master/samples/WebApi)) where shows using of Json.NET.

So the basic rd file looks like:

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

See example in [JsonSerialization](https://github.com/FoggyFinder/FSharpCoreRtTest/tree/JsonSerialization) brunch.

## Printf / Sprintf

CoreRT

There is a separate issue in visualfsharp repo:

[Some FSharp.Core constructs don't run on CoreRT #4954](https://github.com/Microsoft/visualfsharp/issues/4954).

Short summary:

It is happening due to restriction of CoreRT/.NET Native on `MakeGenericType`/`MakeGenericMethod`.

> RD.XML (a file passed to the CoreRT compiler) can be used to tell the compiler that particular code needs to be generated (even though it statically looks like it isn't needed). If there's a reasonable bound on what MakeGenericType/MakeGenericMethod gets called with, RD.XML is all that's needed to make this work.

Yep, really, you can add specification to rd file and it's will work.
See simple example in this repo.  In practice much easily just override .ToString for custom types and use `Console.WriteLine` instead of `printfn`.

.NET Native

Even simple test doesn't work:

```fsharp

```

^ it throws the `System.IndexOutOfRangeException` exception with the follow StackTrace

```bash
   at Internal.Runtime.CompilerHelpers.ThrowHelpers.ThrowIndexOutOfRangeException() in f:\dd\ndp\fxcore\CoreRT\src\System.Private.CoreLib\src\Internal\Runtime\CompilerHelpers\ThrowHelpers.cs:line 25
   at Internal.TypeSystem.Instantiation.get_GenericParameters(Int32 index) in f:\dd\ndp\fxcore\CoreRT\src\Common\src\TypeSystem\Common\Instantiation.cs:line 29
   at Internal.Runtime.TypeLoader.NativeLayoutInfoLoadContext.GetType(NativeParser& parser) in f:\dd\ndp\fxcore\CoreRT\src\System.Private.TypeLoader\src\Internal\Runtime\TypeLoader\NativeLayoutInfoLoadContext.cs:line 124
   at Internal.Runtime.TypeLoader.TypeLoaderEnvironment.GetConstructedTypeFromParserAndNativeLayoutContext(NativeParser& parser, NativeLayoutInfoLoadContext nativeLayoutContext) in f:\dd\ndp\fxcore\CoreRT\src\System.Private.TypeLoader\src\Internal\Runtime\TypeLoader\TypeLoaderEnvironment.cs:line 225
   at Internal.Runtime.TypeLoader.TypeLoaderEnvironment.GetCallingConverterDataFromMethodSignature_NativeLayout_Common(TypeSystemContext context, RuntimeSignature methodSig, Instantiation typeInstantiation, Instantiation methodInstantiation, Boolean& hasThis, TypeDesc[]& parameters, Boolean[]& parametersWithGenericDependentLayout, NativeReader nativeReader, UInt64[] debuggerPreparedExternalReferences) in f:\dd\ndp\fxcore\CoreRT\src\System.Private.TypeLoader\src\Internal\Runtime\TypeLoader\TypeLoaderEnvironment.SignatureParsing.cs:line 344
   at Internal.Runtime.TypeLoader.TypeLoaderEnvironment.GetCallingConverterDataFromMethodSignature_NativeLayout(TypeSystemContext context, RuntimeSignature methodSig, Instantiation typeInstantiation, Instantiation methodInstantiation, Boolean& hasThis, TypeDesc[]& parameters, Boolean[]& parametersWithGenericDependentLayout) in f:\dd\ndp\fxcore\CoreRT\src\System.Private.TypeLoader\src\Internal\Runtime\TypeLoader\TypeLoaderEnvironment.SignatureParsing.cs:line 278
   at Internal.Runtime.TypeLoader.TypeLoaderEnvironment.GetCallingConverterDataFromMethodSignature(TypeSystemContext context, RuntimeSignature methodSig, Instantiation typeInstantiation, Instantiation methodInstantiation, Boolean& hasThis, TypeDesc[]& parameters, Boolean[]& parametersWithGenericDependentLayout) in f:\dd\ndp\fxcore\CoreRT\src\System.Private.TypeLoader\src\Internal\Runtime\TypeLoader\TypeLoaderEnvironment.SignatureParsing.cs:line 261
   at Internal.Runtime.TypeLoader.CallConversionInfo.EnsureCallConversionInfoLoaded() in f:\dd\ndp\fxcore\CoreRT\src\System.Private.TypeLoader\src\Internal\Runtime\TypeLoader\CallConverterThunk.CallConversionInfo.cs:line 193
   at Internal.Runtime.TypeLoader.CallConversionInfo.get_ArgIteratorData() in f:\dd\ndp\fxcore\CoreRT\src\System.Private.TypeLoader\src\Internal\Runtime\TypeLoader\CallConverterThunk.CallConversionInfo.cs:line 499
   at Internal.Runtime.TypeLoader.CallConversionParameters..ctor(CallConversionInfo conversionInfo, IntPtr callerTransitionBlockParam) in f:\dd\ndp\fxcore\CoreRT\src\System.Private.TypeLoader\src\Internal\Runtime\TypeLoader\CallConverterThunk.CallConversionParameters.cs:line 159
   at Internal.Runtime.TypeLoader.CallConverterThunk.CallConversionThunk(IntPtr callerTransitionBlockParam, IntPtr callConversionId)
   at Microsoft.FSharp.Core.OptimizedClosures.Invoke@2806.Invoke(T2 u)
   at <StartupCode$FSLibrary>.$FSLibrary..cctor() 
```

Related links:

[Dynamic programming differences](https://docs.microsoft.com/en-us/dotnet/framework/net-native/migrating-your-windows-store-app-to-net-native#dynamic-programming-differences)

## List of features that are blocked due to restriction above:

* Events
* Linq.Expressions

MCVE:

* Events

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

* Linq.Expressions

```csharp
public static class Ext
{
    public static IEnumerable<U> Map<T, U>(this IEnumerable<T> seq, Expression<Func<T,U>> expression)
    {
        return seq.Select(expression.Compile());
    }
}
```

```fsharp
[| 0..5 |].Map(fun x -> x * 2)
|> Seq.iter(fun v -> Console.WriteLine(v))
```

```bash
Unhandled Exception: EETypeRva:0x0063A160: MakeGenericMethod() cannot create this generic method instantiation because the instantiation was not metadata-enabled: 'Microsoft.FSharp.Core.Operators.op_Multiply<System.Int32,System.Int32,System.Int32>(System.Int32,System.Int32)' For more information, please visit http://go.microsoft.com/fwlink/?LinkID=616868
   at CoreRTConsole!<BaseAddress>+0x13e2d
   at CoreRTConsole!<BaseAddress>+0x8d6af
   at CoreRTConsole!<BaseAddress>+0x8900e
   at CoreRTConsole!<BaseAddress>+0x202aca
   at Microsoft.FSharp.Quotations.PatternsModule.bindModuleDefn@1508(ExprConstInfo, FSharpList`1) + 0x1f
   at Microsoft.FSharp.Quotations.PatternsModule.u_Expr@1415-4.Invoke(PatternsModule.BindingEnv) + 0xad
   at Microsoft.FSharp.Quotations.PatternsModule.u_Expr@1429-6.Invoke(PatternsModule.BindingEnv) + 0x37
   at Microsoft.FSharp.Primitives.Basics.List.map[T, TResult](FSharpFunc`2, FSharpList`1) + 0x2d
   at Microsoft.FSharp.Quotations.PatternsModule.u_Expr@1415-4.Invoke(PatternsModule.BindingEnv) + 0x36
   at Microsoft.FSharp.Quotations.PatternsModule.u_Expr@1437-9.Invoke(PatternsModule.BindingEnv) + 0x1c
   at Microsoft.FSharp.Quotations.PatternsModule.deserialize(Type, Type[], Type[], FSharpExpr[], Byte[]) + 0x8a
   at Program.expTest() + 0x1ae
   at Program.main(String[]) + 0xd
   at CoreRTConsole!<BaseAddress>+0x270ee6
```


