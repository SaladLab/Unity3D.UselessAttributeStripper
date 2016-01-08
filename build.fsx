#I @"packages/FAKE/tools"
#r "FakeLib.dll"

open Fake
open Fake.FileHelper
open Fake.ProcessHelper
open Fake.ZipHelper

// ------------------------------------------------------------------------------ Project

let buildSolutionFile = "./UselessAttributeStripper.sln"
let buildConfiguration = "Release"
  
// ---------------------------------------------------------------------------- Variables

let binDir = "bin"
let testDir = binDir @@ "test"

// ------------------------------------------------------------------------------ Targets

Target "Clean" (fun _ -> 
    CleanDirs [binDir]
)

Target "Build" (fun _ ->
    !! buildSolutionFile
    |> MSBuild "" "Rebuild" [ "Configuration", buildConfiguration ]
    |> Log "Build-Output: "
)

Target "Package" (fun _ ->
    // pack IncrementalCompiler.exe with dependent module dlls to packed one
    let errorCode = Shell.Exec("./packages/ILRepack/tools/ILRepack.exe",
                               "/wildcards /out:UselessAttributeStripper.packed.exe UselessAttributeStripper.exe *.dll",
                               "./src/UselessAttributeStripper/bin/Release")
    // copy & zip
    let workDir = binDir @@ "work"
    CreateDir workDir
    "./src/UselessAttributeStripper/bin/Release/UselessAttributeStripper.packed.exe" |> CopyFile (workDir @@ "UselessAttributeStripper.exe")
    "./src/InstallScript/setup.py" |> CopyFile workDir
    !! (workDir @@ "**") |> Zip workDir (binDir @@ "UselessAttributeStripper.zip")
)

Target "Help" (fun _ ->  
    List.iter printfn [
      "usage:"
      "build [target]"
      ""
      " Targets for building:"
      " * Build        Build"
      " * Package      Make packages"
      ""]
)

// --------------------------------------------------------------------------- Dependency

// Build order
"Clean"
  ==> "Build"
  ==> "Package"

RunTargetOrDefault "Help"
