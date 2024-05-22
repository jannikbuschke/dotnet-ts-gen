#r "nuget: dotenv.net"

open System
open dotenv.net
open System.Diagnostics
open System

DotEnv.Load(DotEnvOptions(envFilePaths = [ ".env.local" ]))

let run (command: string) (arguments: string) =
    printfn "> %s %s" command arguments

    let startInfo =
        ProcessStartInfo(
            command,
            arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        )

    use p = new Process()
    p.StartInfo <- startInfo
    p.Start() |> ignore

    let output = p.StandardOutput.ReadToEnd()
    let error = p.StandardError.ReadToEnd()

    p.WaitForExit()

    let exitCode = p.ExitCode

    if exitCode = 0 then
        ()
    // printfn "Command executed successfully."
    else
        eprintfn "Command failed with exit code %d" exitCode

    // Output the command output and error (if any)
    if not (String.IsNullOrEmpty output) then
        printfn "%s" output

    if not (String.IsNullOrEmpty error) then
        eprintfn "%s" error

    exitCode

let apiKey = Environment.GetEnvironmentVariable("nuget-api-key")

if String.IsNullOrEmpty apiKey then
    eprintf "no api key found. Create a file '.env.local' with content 'nuget-api-key=<your api key>'"
    exit -1

let publishfile file =
    let _ =
        run "dotnet" $"nuget push {file} --source https://api.nuget.org/v3/index.json --api-key {apiKey}"

    ()

let deleteFolder folder =
    fun() ->
        if System.IO.Directory.Exists folder then
            System.IO.Directory.Delete(folder, true)

let restore () = run "dotnet" "restore" |> ignore

let pack (path: string) =
    fun () ->
        if run "dotnet" $"pack {path}" = 0 then
            ()
        else
            exit -1

let publish folderPath pattern =
    fun () ->
        let files = System.IO.Directory.EnumerateFiles(folderPath, pattern)

        printfn "%A" files
        files |> Seq.iter publishfile
        ()

let execute =
    (deleteFolder ".\\src\\TinyTypeGen\\bin")
    >> (deleteFolder ".\\src\\TinyTypeGen.Giraffe\\bin")
    >> (deleteFolder ".\\src\\TinyTypeGen.NodaTime\\bin")
    >> (deleteFolder ".\\src\\TinyTypeGen.AspNetCore\\bin")
    >> restore
    >> pack ".\\src\\TinyTypeGen\\TinyTypeGen.fsproj"
    >> pack ".\\src\\TinyTypeGen.Giraffe\\TinyTypeGen.Giraffe.fsproj"
    >> pack ".\\src\\TinyTypeGen.NodaTime\\TinyTypeGen.NodaTime.fsproj"
    >> pack ".\\src\\TinyTypeGen.AspNetCore\\TinyTypeGen.AspNetCore.fsproj"
    >> publish ".\\src\\TinyTypeGen\\bin\\Release" "TinyTypeGen.*.nupkg"
    >> publish ".\\src\\TinyTypeGen.Giraffe\\bin\\Release" "TinyTypeGen.*.nupkg"
    >> publish ".\\src\\TinyTypeGen.NodaTime\\bin\\Release" "TinyTypeGen.*.nupkg"
    >> publish ".\\src\\TinyTypeGen.AspNetCore\\bin\\Release" "TinyTypeGen.*.nupkg"


execute ()
