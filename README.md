# FSharp CoreRT and .Net Native tests

To test CoreRT, run the CoreRTConsole project in release mode.
 
To test .Net Native:

1. Remove the block on F# code by editing the line containing FSharpRule in `"C:\Program Files (x86)\Microsoft SDKs\UWPNuGetPackages\runtime.win10-x64.Microsoft.Net.Native.Compiler\2.1.8\tools\x64\ilc\tools\GatekeeperConfig.xml"` (or appropriate variation of that path - see the ILT0005 error in step 2 for where to look) to: `<FSharpRule on="false">`

2. Run the UWP project in release mode.
