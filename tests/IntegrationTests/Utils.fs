[<AutoOpen>]
module IntegrationTests.Serialization

open System.Text.Json

open System.Text.Json.Serialization
open TinyTypeGen

let serializeWithOptions<'t> (options: JsonFSharpOptions) (v: 't) =
  let o = options.ToJsonSerializerOptions()
  o.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
  JsonSerializer.Serialize<'t>(v, o)

let serializeWithTypicalOptions (v: 't) =
  let serializationOptions = JsonSerializerOptions()
  let jsonFsharpConverter = JsonFSharpConverter(Config.defaultJsonUnionEncoding)
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

let serializeWithDefaultEncoding<'t> (v: 't) =
  serializeWithEncoding Config.defaultJsonUnionEncoding v

let deserializeWithEncoding<'t> (encoding: JsonUnionEncoding) (v: string) =
  deserializeWithOptions<'t> (JsonFSharpOptions(encoding)) v
