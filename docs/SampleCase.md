# Sample Case

This case motivated me to write this tool. My team was struggling to find a way to
make app size under 100MB. After many trials including removing unused codes and resources,
size was still over 100MB. But with this tool we could make it sucessfully.
In this article, I will compare original app and attribute-stripped one
and show you how this tool help you.

### Environment

- Target: iOS / IL2CPP
- Unity 5.3.1f1 (OSX)
- OSX Yosemite 10.10.5 / XCode 7.2

### Original application executable

This project heavily depends on coroutine and that makes app DLL to contain a lot of
attributes. (check [UnderTheHood](./UnderTheHood.md).)
After building app without this tool, size of app and sections inside are checked.

Size of executable: 151,795,952 bytes (compressed to 40,210,173 bytes)
```
> size exe
__TEXT   __DATA  __OBJC  others     dec        hex
26656768 2473984 0       43909120   73039872   45a8000   exe (for architecture armv7)
30523392 3735552 0       4341760000 4376018944 104d4c000 exe (for architecture arm64)
```

### Removed attributes

Following attributes were removed with this tool.
Majority of attributes were CompilerGeneratedAttribute and DebuggerHiddenAttribute.

| Attrubute                                                        | Count |
| :--------------------------------------------------------------- | ----: |
| System.Runtime.CompilerServices.CompilerGeneratedAttribute       |  5566 |
| System.Diagnostics.DebuggerHiddenAttribute                       |  4641 |
| System.Runtime.InteropServices.ComVisibleAttribute               |  1380 |
| System.Runtime.CompilerServices.ExtensionAttribute               |   797 |
| System.MonoTODOAttribute                                         |   750 |
| ProtoBuf.ProtoMemberAttribute                                    |   745 |
| System.ObsoleteAttribute                                         |   615 |
| UnityEngine.Internal.ExcludeFromDocsAttribute                    |   478 |
| System.CLSCompliantAttribute                                     |   409 |
| System.AttributeUsageAttribute                                   |   327 |
| ProtoBuf.ProtoContractAttribute                                  |   269 |
| UnityEngine.AddComponentMenu                                     |   222 |
| System.Runtime.ConstrainedExecution.ReliabilityContractAttribute |   204 |
| System.Reflection.DefaultMemberAttribute                         |   180 |
| UnityEngine.HideInInspector                                      |   173 |
| UnityEngine.ExecuteInEditMode                                    |    65 |
| System.Diagnostics.DebuggerStepThroughAttribute                  |    55 |
| UnityEngine.ContextMenu                                          |    36 |
| UnityEngine.TooltipAttribute                                     |    30 |
| GameCommon.Data.EidCategoryAttribute                             |    18 |
| System.Diagnostics.CodeAnalysis.SuppressMessageAttribute         |    17 |  
| UnityEngine.DisallowMultipleComponent                            |    15 |
| System.Diagnostics.DebuggerDisplayAttribute                      |    13 |  
| GameCommon.Data.RefAttribute                                     |     7 |
| Total                                                            | 17012 |

### Attribute stripped application executable

After removing useless attributes, the size of app is decreased.

Size of executable: 132,189,648 bytes (compressed to 37,701,628 bytes)
```
> size exe_stripped
__TEXT   __DATA  __OBJC  others     dec        hex
25526272 2408448 0       35684352   63619072   3cac000   exe_stripped (for architecture armv7)
28966912 3637248 0       4333207552 4365811712 104390000 exe_stripped (for architecture arm64)
```

### Comparison

Here is comparison between original executable and attribute-stripped one.

| Project        | Original    | Stripped    | Delta       |
| :------------- | ----------: | ----------: | ----------: |
| __TEXT (armv7) |  26,656,768 |  25,526,272 |  -1,130,496 |
| __TEXT (arm64) |  30,523,392 |  28,966,912 |  -1,556,480 |
| __DATA (armv7) |   2,473,984 |   2,408,448 |     -65,536 |
| __DATA (arm64) |   3,735,552 |   3,637,248 |     -98,304 |
| others (armv7) |  43,909,120 |  35,684,352 |  -8,224,768 |
| others (arm64) |           - |           - |           - |
| Total          | 151,795,952 | 132,189,648 | -19,606,304 |

For code section, 2.5MB is stripped and expected to be same for uploaded IPA.
And for others secsion, almost 16MB is cut off and expected that it gives extra 2.5MB to IPA.
