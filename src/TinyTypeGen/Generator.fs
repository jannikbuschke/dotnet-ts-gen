namespace TinyTypeGen

open System
open TsGen
open TsGen.Collect

type Generator(collector:Collector,config:Config.Configuration)=
  let collector = collector
  let renderer = Config.build config

  member this.GetModuleDependencies(m: TsModule) =
    collector.getModuleDependencies m
  member this.GetRawModuleDependencies(m: TsModule) =
    collector.getRawDeps m
  member this.GetModules() =
    collector.collectModules(config.Types)
  member this.RenderApiToFile(file: string) =
    renderer.renderApiToFile file
  member this.RenderTypesToDirectory(directory: string) =
    renderer.renderTypesToDirectory directory

type GeneratorBuilder() =
  let mutable config = TsGen.Config.withDefaults()

  member this.AddTypes(types: Type array)=
    config <- { config with Types = config.Types @ (types |> Seq.toList) }
  member this.AddEndpoints(endpoints: ApiEndpoint array)=
    config <- config |> Config.withEndpoints (config.ApiEndPoints @ (endpoints |> Seq.toList))
  member this.Build() =
    let collector = Collector(config.PredefinedTypes)
    Generator(collector,config)