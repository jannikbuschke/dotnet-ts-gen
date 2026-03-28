module TinyTypeGen.Config

open System
open System.Text.Json.Serialization

let defaultJsonUnionEncoding =
  JsonUnionEncoding.AdjacentTag
  ||| JsonUnionEncoding.UnwrapSingleFieldCases
  ||| JsonUnionEncoding.UnwrapRecordCases
  ||| JsonUnionEncoding.UnwrapOption
  ||| JsonUnionEncoding.UnwrapSingleCaseUnions
  ||| JsonUnionEncoding.AllowUnorderedTag

[<RequireQualifiedAccess>]
type EmbeddedTemplate =
  | SimpleApiTemplate
  | ApiTemplateWithTanstackQuery

[<RequireQualifiedAccess>]
type Template =
  | Default
  | EmbeddedTemplate of EmbeddedTemplate
  | OfString of string
  | OfFile of string

type Api =
  {
    ApiTemplate: Template
    EndpointTemplate: Template
    TargetFile: string
    Endpoints: ApiEndpoint list
  }

type Configuration =
  {
    Types: Type list
    PredefinedTypes: PreDefinedTypes
    Apis: Api list
    IgnoredNamespaces: string list
    JsonUnionEncoding: JsonUnionEncoding
    PropertyCasing: PropertyCasing
    MaxRecursionDepth: int
  }

let withDefaults () =
  {
    Types =
      [
        typedefof<System.Collections.Generic.List<_>>
        typedefof<Guid>
        typedefof<Boolean>
        typedefof<Int16>
        typedefof<Int32>
        typedefof<Int64>
        typedefof<Int128>
        typedefof<Byte>
        typedefof<Char>
        typedefof<String>
        typedefof<UInt16>
        typedefof<UInt32>
        typedefof<UInt64>
        typedefof<Decimal>
        typedefof<DateTime>
        typedefof<DateTimeOffset>
        typedefof<TimeSpan>
        typedefof<DateOnly>
        typedefof<TimeOnly>
        typedefof<Object>
      ]
    PredefinedTypes = PredefinedTypes.defaultTypes
    IgnoredNamespaces =
      [
        "Microsoft.Net.Http.Headers"
        "System.IO"
        "System.Reflection"
        "System.Runtime.InteropServices"
        "System.Threading"
        "System.Threading.Tasks"
      ]
      |> List.map _.Replace(".", "_")
    Apis = []
    JsonUnionEncoding = defaultJsonUnionEncoding
    PropertyCasing = PropertyCasing.CamelCase
    MaxRecursionDepth = 100
  }

let forTypes types (config: Configuration) = { config with Types = types }

let withDefaultUnionEncoding (config: Configuration) =
  { config with JsonUnionEncoding = JsonUnionEncoding.Default }

let withThothLikeUnionEncoding (config: Configuration) =
  { config with JsonUnionEncoding = JsonUnionEncoding.ThothLike }

let withFSharpLuLikUnionEncoding (config: Configuration) =
  { config with JsonUnionEncoding = JsonUnionEncoding.FSharpLuLike }

let withNewtonsoftUnionEncoding (config: Configuration) =
  { config with JsonUnionEncoding = JsonUnionEncoding.NewtonsoftLike }

let withJsonUnionEncoding encoding (config: Configuration) =
  { config with JsonUnionEncoding = encoding }

let withIgnoredNamespaces namespaces (config: Configuration) =
  { config with IgnoredNamespaces = config.IgnoredNamespaces @ namespaces }

let clearNamespaces (config: Configuration) = { config with IgnoredNamespaces = [] }

let withEndpoints endpoints (config: Configuration) =
  let allTypes =
    config.Types
    @ (endpoints |> Seq.collect (fun v -> [ v.Request; v.Response ]) |> Seq.toList)

  { config with
      Types = allTypes
      Apis =
        {
          ApiTemplate = Template.Default
          Endpoints = endpoints
          TargetFile = $"api-{config.Apis.Length}.ts"
          EndpointTemplate = Template.Default
        }
        :: config.Apis
  }
