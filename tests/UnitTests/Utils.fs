namespace Test

open System.Text.Json
open System.Text.RegularExpressions
open Expecto
open TinyTypeGen
open TinyTypeGen.Config

module Regex =
  let replace (pattern: string) (replacement: string) (input: string) =
    Regex.Replace(input, pattern, replacement)

module Expect =
  let eq actual expected =
    "Should be equal" |> Expect.equal actual expected

  let private normalizeLineFeeds = Regex.replace @"(\r\n|\r|\n)" "\n"

  let private removeSuccessiveLineFeeds = Regex.replace @"[\n]{2,}" "\n"

  let private removeSuccessiveWhiteSpace = Regex.replace @"[ ]{2,}" " "

  let private removeWhitespace = Regex.replace @"\s", ""

  let private trim (v: string) = v.Trim()

  let private clean =
    normalizeLineFeeds
    >> removeSuccessiveLineFeeds
    >> removeSuccessiveWhiteSpace
    >> trim

  let similar actual expected =
    "Should be equal" |> Expect.equal (actual |> clean) (expected |> clean)

  let equal expected actual =
    "Should be equal" |> Expect.equal (actual |> clean) (expected |> clean)

  let stringStart expectedStart actual =
    "Should start with"
    |> Expect.stringStarts (actual |> clean) (expectedStart |> clean)

  let stringContains (substring: string) (actual: string) =
    Expecto.Expect.stringContains (actual |> clean) (substring |> clean) (sprintf "Should contain '%s'" substring)

[<AutoOpen>]
module Serialization =
  open System.Text.Json.Serialization

  let serializeWithOptions<'t> (options: JsonFSharpOptions) (v: 't) =
    let o = options.ToJsonSerializerOptions()
    o.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    JsonSerializer.Serialize<'t>(v, o)

  let serializeWithTypicalOptions (v: 't) =
    let serializationOptions = JsonSerializerOptions()
    let jsonFsharpConverter = JsonFSharpConverter(defaultJsonUnionEncoding)
    serializationOptions.Converters.Add jsonFsharpConverter
    serializationOptions.Converters.Add(JsonStringEnumConverter())
    serializationOptions.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    serializationOptions.ReferenceHandler <- ReferenceHandler.IgnoreCycles
    serializationOptions.MaxDepth <- 512
    serializationOptions.Converters.Add(jsonFsharpConverter)
    // serializeWithOptions DefaultSerialize.JsonFSharpOptions v
    JsonSerializer.Serialize<'t>(v, serializationOptions)

  let deserializeWithOptions<'t> (options: JsonFSharpOptions) (v: string) =
    let o = options.ToJsonSerializerOptions()
    o.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
    JsonSerializer.Deserialize<'t>(v, o)

  let serializeWithEncoding<'t> (encoding: JsonUnionEncoding) (v: 't) =
    serializeWithOptions (JsonFSharpOptions(encoding)) v

  let serializeWithCustomEncoding<'t> (v: 't) =
    serializeWithEncoding defaultJsonUnionEncoding v

  let deserializeWithEncoding<'t> (encoding: JsonUnionEncoding) (v: string) =
    deserializeWithOptions<'t> (JsonFSharpOptions(encoding)) v

[<AutoOpen>]
module Helpers =

  let configureFor unionEncoding (t: System.Type) =
    let builder = GeneratorBuilder()
    builder.AddTypes [| t |] |> ignore
    builder.WithEncoding unionEncoding |> ignore
    let generator = builder.Build()
    generator

  let renderModuleAsArtifact t =
    let builder = GeneratorBuilder()
    builder.AddTypes [| t |] |> ignore
    let generator = builder.Build()
    let moduleName = getModuleName t

    let m = generator.GetModules() |> List.find (fun m -> m.Name = moduleName)

    generator.RenderModuleToString

  let renderModule t =
    let builder = GeneratorBuilder()
    builder.AddTypes [| t |] |> ignore
    let generator = builder.Build()
    let moduleName = getModuleName t

    let m = generator.GetModules() |> List.find (fun m -> m.Name = moduleName)

    generator.RenderModuleToString(m, [])

  let getModuleWithEncoding unionEncoding t =
    let config = configureFor unionEncoding t
    let modules = config.GetModules()

    modules |> List.find (fun m -> m.Name = t.Namespace)

  let getModule t =
    getModuleWithEncoding defaultJsonUnionEncoding t

  // Use
  let renderCustomTypeDef jsonEncoding (t: System.Type) =
    let config = configureFor jsonEncoding t
    config.RenderTypeToString t

  let definition t =
    renderCustomTypeDef defaultJsonUnionEncoding t

  let definitionWithCasing casing (t: System.Type) =
    Generator.RenderType(t, { Config.withDefaults () with PropertyCasing = casing })
