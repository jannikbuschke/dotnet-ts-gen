module TsGen.Config

open System.Text.Json.Serialization

type Configuration =
  { Types: System.Type list
    PredefinedTypes: PredefinedTypes.PreDefinedTypes
    ApiEndPoints: ApiEndpoint list
    JsonUnionEncoding: JsonUnionEncoding
     }

let withDefaults () =
  { Types =
      [ typedefof<System.Collections.Generic.List<_>>
        typedefof<System.Guid>
        typedefof<System.Boolean>
        typedefof<System.Int16>
        typedefof<System.Int32>
        typedefof<System.Int64>
        typedefof<System.Int128>
        typedefof<System.Byte>
        typedefof<System.Char>
        typedefof<System.String>
        typedefof<System.UInt16>
        typedefof<System.UInt32>
        typedefof<System.UInt64>
        typedefof<System.Decimal>
        typedefof<System.DateTime>
        typedefof<System.DateTimeOffset>
        typedefof<System.TimeSpan>
        typedefof<System.TimeSpan>
        typedefof<System.Object> ]
    PredefinedTypes = PredefinedTypes.defaultTypes
    ApiEndPoints = []
    JsonUnionEncoding = Gen.defaultJsonUnionEncoding
     }

let forTypes types (generator: Configuration) = { generator with Types = types }

let withJsonUnionEncoding encoding (generator:Configuration)=
  { generator with JsonUnionEncoding = encoding }

let withEndpoints endpoints (generator: Configuration) =
  let allTypes =
    generator.Types
    @ (endpoints |> Seq.collect (fun v -> [ v.Request; v.Response ]) |> Seq.toList)

  { generator with
      Types = allTypes
      ApiEndPoints = endpoints }

let build (generator: Configuration) =
  Run.init generator.PredefinedTypes generator.Types generator.ApiEndPoints generator.JsonUnionEncoding
