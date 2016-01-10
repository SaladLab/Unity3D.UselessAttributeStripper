# Under the hood

### Attribute

.NET Framework relies heavily on [Attribute](https://msdn.microsoft.com/en-us/library/z0w1kczw.aspx).
C# code uses it extensively and compiler also uses it much more than expected.
For example, consider regular unity coroutine code:

```csharp
public class ResourceManager : MonoBehaviour {
  private IEnumerator LoadImageAsync(string directory, Res<Texture2D> arg)  
      yield return Resources.LoadAsync(directory + "/" + arg.Id);
      arg.Result = request.asset as Texture2D;
  }
}
```

There is nothing related attributes. Just plain generator.
But though [ILSpy](http://ilspy.net/), we can inspect compiled DLL containing .NET IL code.
Check disassembled code:

```csharp
public class ResourceManager : MonoBehaviour {
  [DebuggerHidden]
  private IEnumerator LoadImageAsync(string directory, Res<Texture2D> arg) {
    var iterator = new ResourceManager.GeneratedIterator();
    // trimmed
    return iterator;
  }

  [CompilerGenerated]
  private sealed class GeneratedIterator : IEnumerator, IDisposable, IEnumerator<object> {
    object IEnumerator<object>.Current {
      [DebuggerHidden] get;
    }
    object IEnumerator.Current {
      [DebuggerHidden] get;
    }
    [DebuggerHidden] public void Dispose();
    [DebuggerHidden] public void Reset();
    // trimmed
  }
```

There are 6 attributes! (1 [CompilerGenerated], 5 [DebuggerHidden])
C# Compiler attaches these attributes for helping debugging or inspecting.
But for running application, it doesn't work at all.
It can be considered safe to be removed.

### Code by IL2CPP

Unnecessary attributes in .NET DLL don't contribute to size of assembly much.
But after Il2Cpp translation, they become bigger. You can read generated code for
all attributes at generated IL2CppAttributes.cpp.
Code for handling [DebuggerHidden] of ResourceManager.LoadImageAsync looks like:
(Unity 5.3.1f1 is used for generating)

```cpp
extern TypeInfo* DebuggerHiddenAttribute_il2cpp_TypeInfo_var;
void ResourceManager_CustomAttributesCacheGenerator_LoadImageAsync(CustomAttributesCache* cache) {
  static bool s_Il2CppMethodIntialized;
  if (!s_Il2CppMethodIntialized) {
    DebuggerHiddenAttribute_il2cpp_TypeInfo_var = il2cpp_codegen_type_info_from_index(13857);
    s_Il2CppMethodIntialized = true;
  }
  cache->count = 1;
  cache->attributes = (Il2CppObject**)il2cpp_gc_alloc_fixed(sizeof(Object_t *) * cache->count, 0);
  DebuggerHiddenAttribute * tmp;
  tmp = (DebuggerHiddenAttribute *)il2cpp_codegen_object_new (DebuggerHiddenAttribute_il2cpp_TypeInfo_var);
  DebuggerHiddenAttribute__ctor(tmp, NULL);
  cache->attributes[0] = (Il2CppObject*)tmp;
}
```

It's small code. For every attribute instance, similar code will be generated.
In my project, almost 15,000 functions were found at IL2CppAttributes.cpp.

### AppStore DRM

Even that lots of attributes can bloat application code, we can think that it's ok
because ipa is a zipped container and they're good to deal with that.
But wait. AppStore will do something for DRM after uploading your ipa.
Your IPA will be decompressed, encrypted just for code section and compressed again.
Hmm, compresion after encryption? That means it won't be compressed at all.
(For detailed information, check [IL2CPP build size improvements](http://forum.unity3d.com/threads/il2cpp-build-size-improvements.322079/).)

From this moment, the size of code starts to matter. If you save 1KB from your code,
you can save 1KB for uploaded IPA. In my project, removing unnecessary attributes
saved 2.5MB for my code and helped a lot with reducing the size of app under 100MB.
(For detailed information, read [Sample Case](./SampleCase.md).)
