module Testing.UI

open canopy.types
open canopy.runner.classic
open canopy.configuration
open canopy.classic
open System
open System.IO
open System.Diagnostics

type CanopyMode =
    | Browser = 1
    | Headless = 2

let fpJoin a b =
    Path.Join [| a ; b |]
    |> Path.GetFullPath

let topDir = fpJoin __SOURCE_DIRECTORY__ ".."
let deployDir =  fpJoin topDir "deploy"
let contentRoot =  fpJoin deployDir "public"

let homeDir =
    if Environment.OSVersion.Platform = PlatformID.Unix || Environment.OSVersion.Platform = PlatformID.MacOSX then
        Environment.GetEnvironmentVariable("HOME")
    else
        Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

let buildProject () =
    let p = new Process()
    p.StartInfo.FileName <- "/usr/bin/env" // windows users, fix this or use wls or devcontainer
    p.StartInfo.Arguments <- "fake build -t debug"
    p.StartInfo.WorkingDirectory <- topDir
    p.Start () |> ignore
    p.WaitForExit ()

let runServer () =
    let p = new Process()
    System.Environment.SetEnvironmentVariable ("CONTENT_ROOT", contentRoot)
    p.StartInfo.FileName <- "/usr/bin/env" // windows users, fix this or use wsl or devcontainer
    p.StartInfo.Arguments <- "dotnet Server.dll"
    p.StartInfo.WorkingDirectory <- deployDir
    p.Start () |> ignore
    Async.Sleep 2000 |> Async.RunSynchronously
    p

// Hack for nixos
let setChromeDir () =
    let nixDir = fpJoin homeDir ".nix-profile/bin"
    if Directory.Exists nixDir then
        chromeDir <- nixDir
    else
        ()

let testUI (mode : CanopyMode) =
    setChromeDir ()
    if not (File.Exists (Path.Join [| deployDir; "Server" |])) then
        buildProject ()
    else
        ()
    let server = runServer ()

    match mode with
    | CanopyMode.Headless -> start BrowserStartMode.ChromeHeadless
    | _ -> start BrowserStartMode.Chrome

    //this is how you define a test
    "test canopy" &&& fun _ ->
        url "http://localhost:8085"
        "#navtitle" == "INF-3910-5 Go!"
        click "#increment"
        click "#increment"
        click "#decrement"

    run ()
    if mode = CanopyMode.Browser then
        // keep the browser open for 10 seconds
        Async.Sleep 5000 |> Async.RunSynchronously
    else
        ()
    quit ()
    server.Kill ()
    0
