# Sample Case

### Environment

- Target: iOS / IL2CPP
- Unity 5.3.1f1 (OSX)
- OSX Yosemite 10.10.5 / XCode 7.2

### Result executable size (original)

Size of executable: 151,795,952 bytes
```
> size exe
__TEXT   __DATA  __OBJC  others     dec        hex
26656768 2473984 0       43909120   73039872   45a8000   exe (for architecture armv7)
30523392 3735552 0       4341760000 4376018944 104d4c000 exe (for architecture arm64)
```

### Result executable size (attribute stripped)

Size of executable: 132,189,648 bytes
```
> size exe_stripped
__TEXT   __DATA  __OBJC  others     dec        hex
25526272 2408448 0       35684352   63619072   3cac000   exe_stripped (for architecture armv7)
28966912 3637248 0       4333207552 4365811712 104390000 exe_stripped (for architecture arm64)
```

### Compare

| Project        | Original    | Stripped    | Delta       |
| :------------- | ----------: | ----------: | ----------: |
| __TEXT (armv7) |  26,656,768 |  25,526,272 |  -1,130,496 |
| __TEXT (arm64) |  30,523,392 |  28,966,912 |  -1,556,480 |
| __DATA (armv7) |   2,473,984 |   2,408,448 |     -65,536 |
| __DATA (arm64) |   3,735,552 |   3,637,248 |     -98,304 |
| others (armv7) |  43,909,120 |  35,684,352 |  -8,224,768 |
| others (arm64) |           - |           - |           - |
| Total          | 151,795,952 | 132,189,648 | -19,606,304 |
