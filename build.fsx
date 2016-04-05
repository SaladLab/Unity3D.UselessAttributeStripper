#I @"packages/FAKE/tools"
#I @"packages/FAKE.BuildLib/lib/net451"
#r "FakeLib.dll"
#r "BuildLib.dll"

open Fake
open BuildLib

let solution = 
    initSolution
        "./UselessAttributeStripper.sln" "Release" 
        [ ]

Target "Clean" <| fun _ -> cleanBin

Target "Restore" <| fun _ -> restoreNugetPackages solution

Target "Build" <| fun _ -> buildSolution solution

Target "Package" (fun _ -> 
    // pack IncrementalCompiler.exe with dependent module dlls to packed one
    let ilrepackExe = (getNugetPackage "ILRepack" "2.0.9") @@ "tools" @@ "ILRepack.exe"
    let errorCode = 
        Shell.Exec
            (ilrepackExe, "/wildcards /out:UselessAttributeStripper.packed.exe UselessAttributeStripper.exe *.dll", 
             "./src/UselessAttributeStripper/bin/Release")
    // copy & zip
    let workDir = binDir @@ "work"
    CreateDir workDir
    "./src/UselessAttributeStripper/bin/Release/UselessAttributeStripper.packed.exe" 
    |> CopyFile(workDir @@ "UselessAttributeStripper.exe")
    "./src/InstallScript/setup.py" |> CopyFile workDir
    !!(workDir @@ "**") |> Zip workDir (binDir @@ "UselessAttributeStripper.zip"))


Target "Help" <| fun _ -> 
    showUsage solution (fun name -> 
        if name = "package" then Some("Build package", "")
        else None)

"Clean"
  ==> "Restore"
  ==> "Build"
  ==> "Package"

RunTargetOrDefault "Help"
