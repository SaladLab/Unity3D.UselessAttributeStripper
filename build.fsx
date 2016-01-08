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
    let workDir = binDir @@ "work"
    CreateDir workDir
    "./src/UselessAttributeStripper/bin/Release/UselessAttributeStripper.exe" |> CopyFile workDir
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
