# Unity3D.UselessAttributeStripper

Useless attribute stripper for IL2CPPed executable. It will help for reducing app size for iOS.

### Setup

Unzip [a release zip file](https://github.com/SaladbowlCreative/Unity3D.UselessAttributeStripper/releases) and run setup.py.
- Run setup.py with administrative privilege because it may update your application directory.
- If you didn't install Unity3D to default directory, you need to check your unity3d installation path.

if you installed unity to default path:
```
setup.py install
```

if you didn't install unity to default path:
```
setup.py install [unity-path]
```

### Check what's going on

While unity3D builds application, it will write log on intermediate path.
For iOS with Unity5 & IL2CPP you can find log-file at
`./Client/Output/Data/Managed/UselessAttributeStripper.txt`. (you can see [sample log](./docs/SampleLog.txt))

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

### More

[Sample Case](./docs/SampleCase.md)
