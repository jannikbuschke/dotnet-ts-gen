#r "nuget: dotenv.net"

open System
open dotenv.net
open System.Diagnostics
open System

DotEnv.Load(DotEnvOptions(envFilePaths = [ ".env.local" ]))

let run (command: string) (arguments: string) =
  printfn "> %s %s" command arguments

  let startInfo =
    ProcessStartInfo(command, arguments, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, CreateNoWindow = true)

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

  if not (String.IsNullOrEmpty error) then eprintfn "%s" error
  exitCode

let apiKey = Environment.GetEnvironmentVariable("nuget-api-key")

if String.IsNullOrEmpty apiKey then
  eprintf "no api key found. Create a file '.env.local' with content 'nuget-api-key=<your api key>'"
  exit -1

let publishfile file =
  let _ =
    run "dotnet" $"nuget push {file} --source https://api.nuget.org/v3/index.json --api-key {apiKey}"

  ()

let deleteBinfolder () =
  if System.IO.Directory.Exists ".\\src\\bin" then
    System.IO.Directory.Delete(".\\src\\bin", true)

let restore () = run "dotnet" "restore" |> ignore

let pack () =
  if run "dotnet" "pack .\\src\\TinyTypeGen.fsproj" = 0 then
    ()
  else
    exit -1

let publish () =
  let files =
    System.IO.Directory.EnumerateFiles(".\\src\\bin\\Release", "TinyTypeGen.*.nupkg")

  printfn "%A" files
  files |> Seq.iter publishfile
  ()

let execute = deleteBinfolder >> restore >> pack >> publish

execute ()
