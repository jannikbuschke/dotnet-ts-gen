module TinyTypGen.Run

open System
open System.Text
open Artifact
open TinyTypeGen
open Config
open TinyTypeGen.Gen
open TinyTypeGen.Render.Module

type RenderApiCallback = StringBuilder -> string

module Callbacks =
  let modulesDoNothing: RenderModuleCallback = fun _ y -> y.ToString()
  let apiDoNothing: RenderApiCallback = _.ToString()

let renderModulesToArtifacts (env: Env) (config: Configuration) path (modules: TsModuleWithDeps list) =
  modules
  |> List.distinctBy _.Name
  |> List.choose (fun v ->
    if config.IgnoredNamespaces |> List.contains v.OriginalNamespacename then
      None
    else
      renderModuleToString env config v modules
      |> (fun fs -> toArtifact $"{v.Name}.ts" v.OriginalNamespacename fs path)
      |> Some
  )

let renderArtifactToDiskUnconditionally (artifact: Artifact) =
  let path = IO.Path.Combine(artifact.Path, artifact.FileName)
  IO.Directory.CreateDirectory(artifact.Path) |> ignore
  System.IO.File.WriteAllText(path, artifact.Content)

let renderArtifactToDisk path artifact =
  path
  |> getCachedCacheInfo
  |> Seq.tryFind (fun x -> x.Name = artifact.FileName)
  |> function
    | Some cache ->
      if cache.HashValue = artifact.Hash then
        // printfn "Skip %s (no changes)" artifact.FileName
        // do nothing
        ()
      else
        // printfn "Render %s to disk (changed)" artifact.FileName
        renderArtifactToDiskUnconditionally artifact
    | None ->
      // printfn "Render %s to disk (no cache)" artifact.FileName
      renderArtifactToDiskUnconditionally artifact

let cacheFilePath path = $"{path}/cacheinfo"

let updateCacheInfo path (artifacts: Artifact list) =
  let sb = StringBuilder()

  artifacts
  |> Seq.iter (fun x -> sprintf "%s=%s" x.FileName (x.Hash.ToString()) |> sb.AppendLine |> ignore)

  let cacheInfoPath = cacheFilePath path
  IO.File.WriteAllText(cacheInfoPath, sb.ToString())

let renderModulesToDisk (env: Env) (config: Configuration) path (modules: TsModuleWithDeps list) =
  if not (IO.Directory.Exists(path)) then
    System.IO.Directory.CreateDirectory path |> ignore
    ()

  let artifacts = renderModulesToArtifacts env config path modules
  artifacts |> List.iter (renderArtifactToDisk path)

  updateCacheInfo path artifacts

let renderTypesToDisk (env: Env) (config: Configuration) (modules: TsModuleWithDeps list) path renderModuleCallback =
  let stopWatch = Diagnostics.Stopwatch.StartNew()

  renderModulesToDisk env config path modules

  stopWatch.Stop()

  // printfn "Generated client types in %d ms" (stopWatch.Elapsed.TotalMilliseconds |> Math.Round |> Convert.ToInt32)
  ()


let renderHubToArtifact (config: Configuration) (deps: Type list) (content: string) (path: string) =
  let content = renderHubToString config deps content

  let name, path =
    if path.EndsWith ".ts" then
      IO.Path.GetFileName path, IO.Path.GetDirectoryName path
    else
      "api.ts", path

  toArtifact name null content path

let renderApiToArtifact (env: Env) (modules: TsModuleWithDeps list) api =
  RenderApiWithScriban.renderApiToArtifact env modules api

let syncArtifactsToDisc path artifacts =
  artifacts |> List.iter (renderArtifactToDisk path)

type Hub =
  {
    Deps: Type list
    Content: string
    Path: string
  }

let renderModulesAndApi (env: Env) (config: Configuration) (modules: TsModuleWithDeps list) path =

  let moduleArtifacts = renderModulesToArtifacts env config path modules
  let apis = config.Apis |> List.map (renderApiToArtifact env modules)
  let allArtifacts = (apis @ moduleArtifacts)
  syncArtifactsToDisc path allArtifacts
  updateCacheInfo path allArtifacts
  ()

let renderHub (config: Configuration) (hub: Hub) =
  renderHubToArtifact config hub.Deps hub.Content hub.Path

let renderModulesAndApiAndHubs
  (env: Env)
  (config: Configuration)
  (modules: TsModuleWithDeps list)
  (hubs: Hub list)
  path
  =
  let moduleArtifacts = renderModulesToArtifacts env config path modules
  let api = config.Apis |> List.map (renderApiToArtifact env modules)
  let hubs = hubs |> List.map (renderHub config)
  let allArtifacts = (api @ moduleArtifacts @ hubs)
  syncArtifactsToDisc path allArtifacts
  updateCacheInfo path allArtifacts
  ()

let init (env: Env) (config: Configuration) modules =
  let renderType = fun x -> renderType x env

  {|
    renderTypesToDirectory = renderTypesToDisk env config modules
    renderType = renderType
    renderModule = renderModuleToString env config
    renderValue = renderType
    renderTypeAndValue = renderType
    renderTypes = fun () -> modules |> List.map (fun m -> m, renderModuleToString env config m)
    renderModulesAndApi = renderModulesAndApi env config modules
    renderModulesAndApiAndHubs = fun path hubs -> renderModulesAndApiAndHubs env config modules hubs path
  |}
