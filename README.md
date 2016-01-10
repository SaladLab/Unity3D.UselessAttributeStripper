# Unity3D.UselessAttributeStripper

For iOS application, Unity3D uses [IL2CPP](http://blogs.unity3d.com/kr/2015/05/06/an-introduction-to-ilcpp-internals/)
for translating your .NET IL code to native one,
which makes the size of your executable much larger than you expect.
It's not easy problem to make it smaller. However, it is mandatory to keep the size
under 100MB to allow iOS users to download your app over the air.

This tools will give you small margin to shrink your app a little.
IL2CPP makes a small code for every attribute in your app assembly. It's small.
But with thousands of .NET attributes, it may bloat your code size.
This tool provide a way to remove useless attributes from your app,
which doens't affect running at all.
(For detailed information, read [Under The Hood](./docs/UnderTheHood.md).)

The margin depends on your code patern. For my project which heavily exploited coroutines,
it could remove 17,000 attributes from whole assemblies and reduced the uncompressed app size
to 126MB from 144MB (-18MB). It seemed small but because code area in app
will be encrypted before compression, this size really mattered.
(For detailed information of this case, read [Sample Case](./docs/SampleCase.md).)

### Setup

Unzip [a release zip file](https://github.com/SaladbowlCreative/Unity3D.UselessAttributeStripper/releases) and run setup.py.
- Run setup.py with administrative privilege because it may update your application directory.
- If you didn't install Unity3D to default directory, you need to check your unity3d installation path.

For OSX
```sh
# if unity3d was installed to default directory
sudo python setup.py install    

# if unity3d was installed to /Application/Unity531
sudo python setup.py install /Application/Unity531
```

For Windows, run a command as administrator,
```sh
# if unity3d was installed to default directory
setup.py install    

# if unity3d was installed to C:\Program Files\Unity531
setup.py install "C:\Program Files\Unity531"
```

After installed, this utility will be running whenever unity3d builds application with IL2CPP.

### Check what's going on

During unity3D builds application it runs a few programs like mono compiler and IL2CPP.
At this time this tool will be executed and it will write log messages
to file in intermediate directory. You can find log file at `./Project/Output/Data/Managed/UselessAttributeStripper.txt`.
(you can see [sample log](./docs/SampleLog.txt))

With log, you can know which attributes were removed from DLLs and the amount of removal.
```
- ProcessDll : /builder/GameClient/Temp/StagingArea/Data/Managed/mscorlib.dll
  - System.Runtime.InteropServices.ComVisibleAttribute : 1311
  - System.MonoTODOAttribute : 461
  - System.CLSCompliantAttribute : 374
  - (trimmed)
```
At the end of log, total removal infomation will be written.
```
* Summary *
  - System.Runtime.CompilerServices.CompilerGeneratedAttribute : 5566
  - System.Diagnostics.DebuggerHiddenAttribute : 4641
  - System.Runtime.InteropServices.ComVisibleAttribute : 1380
  - System.Runtime.CompilerServices.ExtensionAttribute : 797
  - (trimmed)
```

### Configuration

This tool has default attribute [lists](https://github.com/SaladbowlCreative/Unity3D.UselessAttributeStripper/blob/master/src/UselessAttributeStripper/BuiltinConfiguration.cs) to be removed.
These were chosen in a conservative attitude to avoid running problem.
And if you want to remove other attributes, you can specify
your own attribute lists on Assets/`strip-attribute.xml` file.
This file looks like
```xml
<strip-attribute>
  <!-- Protobuf.net (precompiler makes app don't need these attributes) -->
  <type fullname="ProtoBuf.ProtoContractAttribute"/>
  <type fullname="ProtoBuf.ProtoMemberAttribute"/>
  <!-- Project-related -->
  <type fullname="GameCommon.Data.EidCategoryAttribute"/>
  <type fullname="GameCommon.Data.RefAttribute"/>
</strip-attribute>
```
To let tool detecting list xml, `links.xml` should exist at the same directory.
(Assets/links.xml for Assets/strip-attribute.xml)
