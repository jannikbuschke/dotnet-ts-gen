module TinyTypeGen.Render.Module

open System
open System.Text
open System.Text.Json.Serialization.TypeCache
open TinyTypeGen
open TinyTypeGen.Config
open TinyTypeGen.Gen

type RenderModuleCallback = TsModuleWithDeps -> StringBuilder -> string

let sort getDependencies types =
  TopologicalSort.topologicalSort
    (fun v -> v |> getDependencies |> List.filter (fun x -> getModuleName x = getModuleName v))
    types

let filterRuntimeGenerics (config: Configuration) (t: Type) =
  if PredefinedTypes.isDefined config.PredefinedTypes t then
    true
  else if t.IsGenericType && not t.IsGenericTypeDefinition then
    match getKind t with
    | TypeKind.List -> false
    | TypeKind.Record -> isAnonymousRecord t
    | _ -> true
  else
    true

let renderImports dependencies (builder: StringBuilder) (allModules: TsModuleWithDeps list) =
  dependencies
  |> List.distinctBy getModuleName
  |> List.iter (fun v ->
    let m = allModules |> List.tryFind (fun x -> x.OriginalNamespacename = v.Namespace)
    match m with
    | Some m ->
      let name = getModuleName v
      builder.AppendLine($"import * as {name} from \"./{name}\"") |> ignore
    | None ->
      let name = getModuleName v
      builder.AppendLine($"import * as {name} from \"./{name}\"") |> ignore
  )

  builder.AppendLine() |> ignore

let renderHeader (builder: StringBuilder) =
  builder
    .AppendLine("//////////////////////////////////////")
    .AppendLine("//   This file is auto generated   //")
    .AppendLine("//////////////////////////////////////")
    .AppendLine("")
  |> ignore

let renderHubToString (config: Config.Configuration) (deps: Type list) (content: string) =
  let builder = StringBuilder()
  renderHeader builder

  let deps =
    deps
    |> List.filter (fun x -> config.IgnoredNamespaces |> List.contains (getModuleName x) |> not)
    |> List.filter (fun x -> config.IgnoredNamespaces |> List.contains x.Namespace |> not)

  renderImports deps builder []

  builder.AppendLine """import * as signalR from "@microsoft/signalr" """
  |> ignore

  builder.AppendLine "" |> ignore
  builder.AppendLine content |> ignore
  builder.ToString()

let renderModuleToString (env: Env) (config: Configuration) (m: TsModuleWithDeps) (allModules: TsModuleWithDeps list) =
  let builder = StringBuilder()

  renderHeader builder

  let deps =
    m.Dependencies
    |> List.filter (fun x -> config.IgnoredNamespaces |> List.contains (getModuleName x) |> not)
    |> List.filter (fun x -> config.IgnoredNamespaces |> List.contains x.Namespace |> not)

  renderImports deps builder allModules

  let tBuilder = StringBuilder()

  m.Types
  |> List.filter (fun x -> not x.IsArray && not (x.IsGenericType && not x.IsGenericTypeDefinition))
  |> List.distinct
  |> List.filter (filterRuntimeGenerics config)
  |> List.map (fun x -> renderType x env)
  |> List.map Utils.cleanTs
  |> List.iter (tBuilder.AppendLine >> ignore)

  let content =
    if String.IsNullOrWhiteSpace(tBuilder.ToString()) then
      builder
        .AppendLine("// this namespace does not contain types. Export something to make it a valid module")
        .AppendLine("export default null")
        .AppendLine(tBuilder.ToString())
      |> ignore
      builder.ToString()
    else
      builder.AppendLine(tBuilder.ToString()) |> ignore
      builder.ToString()

  try
    TinyTypeGen.Scriban.moduleTemplate
    |> Option.map _.Render({| content = content |})
    |> Option.defaultValue content
  with ex ->
    $"// Error rendering custom template: {ex.Message}"
