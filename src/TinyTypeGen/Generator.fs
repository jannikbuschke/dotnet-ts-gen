namespace TinyTypeGen

open System
open System.Text.Json.Serialization
open TinyTypGen.Run
open TinyTypeGen.Config
open TinyTypeGen.Render.Module

[<AutoOpen>]
module private Helper =
  open TinyTypeGen.Render

  let isOverridden (predefinedTypes: PreDefinedTypes) t =
    PredefinedTypes.isDefined predefinedTypes t

  let renderOverride (predefinedTypes: PreDefinedTypes) t =
    let result =
      t
      |> PredefinedTypes.tryPredefinedType predefinedTypes
      |> Option.map (fun value ->
        if t.IsGenericType && not t.IsGenericTypeDefinition then
          PredefinedType.renderPredefinedType (t.GetGenericTypeDefinition()) value
        else
          PredefinedType.renderPredefinedType t value
      )

    if result.IsNone then
      failwith (sprintf "Expected %s to be overridden, but was not found" t.Name)

    result.Value

type Generator(moduls, config: Config.Configuration) =

  let getName = fun x -> getName x config.PredefinedTypes

  let getSignature = fun x -> getSignature x config.PredefinedTypes

  let env =
    { new Env with
        member this.Encoding: JsonUnionEncoding = config.JsonUnionEncoding

        member this.GetDependencies(arg1: Type) : Type list =
          Collect2._getDependencies config.PredefinedTypes arg1

        member this.GetFullName(arg1: Type) : string =
          let m, n = getSignature arg1
          sprintf "%s.%s" m n

        member this.GetName: GetName = getName

        member this.GetPropertySignature (arg1: string | null) (arg2: Type) : string =
          getPropertySignature arg1 arg2 getSignature getName

        member this.GetSignature: GetSignature = getSignature

        member this.IsOverridden(arg1: Type) : bool =
          isOverridden config.PredefinedTypes arg1

        member this.PreDefinedTypes: PreDefinedTypes = config.PredefinedTypes

        member this.RenderOverridden(arg1: Type) : string =
          renderOverride config.PredefinedTypes arg1

        member this.PropertyCasing: PropertyCasing = config.PropertyCasing

    }

  let renderer = init env config moduls

  member _.Env = env
  member _.GetModules() = moduls

  member _.RenderTypeToString(t: Type) = renderer.renderType t

  member _.RenderModuleToString(t: TsModuleWithDeps, allModuls: TsModuleWithDeps list) =
    renderer.renderModule t allModuls

  member _.RenderTypesToDirectory(directory: string) =
    renderer.renderTypesToDirectory directory (fun _ builder -> builder.ToString())

  member _.RenderTypesToDirectory(directory: string, cb: RenderModuleCallback) =
    renderer.renderTypesToDirectory directory cb

  member _.RenderAll(directory: string) = renderer.renderModulesAndApi directory

  member _.RenderAllIncludingHubs(directory: string, hubs: Hub list) =
    renderer.renderModulesAndApiAndHubs directory hubs

  static member RenderType(t: Type) =
    Generator.RenderType(t, Config.withDefaults ())

  static member RenderType(t: Type, encoding: JsonUnionEncoding) =
    Generator.RenderType(t, { Config.withDefaults () with JsonUnionEncoding = encoding })

  static member RenderType(t: Type, config: Configuration) =
    let config = { config with Types = (t :: config.Types) |> List.distinct }
    let collector = Collect2.Collector(config)
    let moduls = config.Types |> collector
    Generator(moduls, config).RenderTypeToString(t)

type GeneratorBuilder() =
  let mutable config = Config.withDefaults ()

  member this.ConfigPath(path: string) = this

  member this.WithEncoding(encoding: JsonUnionEncoding) =
    config <- { config with JsonUnionEncoding = encoding }
    this

  member this.WithCasing(casing: PropertyCasing) =
    config <- { config with PropertyCasing = casing }
    this

  member this.AddTypes(types: Type seq) =
    config <- { config with Types = config.Types @ (types |> Seq.toList) }
    this

  member this.AddOverrides(types: Type, predefinedType: PredefinedType) =
    config.PredefinedTypes.Add(types, predefinedType)
    this

  member this.AddToIgnoredNamespaces(ns: string) =
    config <- { config with IgnoredNamespaces = config.IgnoredNamespaces @ [ ns ] }
    this

  member this.IgnoreNamespaces(ns: string seq) =
    config <- { config with IgnoredNamespaces = config.IgnoredNamespaces @ (ns |> Seq.toList) }
    this

  member this.AddApi(api: Api) =
    config <- { config with Apis = config.Apis @ [ api ] }
    this

  member this.AddHub(hubs: Hub) =
    // TODO: add hubs to config
    config <- { config with Types = config.Types @ hubs.Deps }
    this

  member this.WithMaxRecursionDepth(depth: int) =
    config <- { config with MaxRecursionDepth = depth }
    this

  member _.Build() =
    let collector = Collect2.Collector(config)
    let moduls =
      (config.Apis
       |> List.collect (fun api -> api.Endpoints |> List.collect (fun api -> [ api.Request; api.Response ])))
      @ config.Types
      |> List.distinct
      |> collector
    Generator(moduls, config)
