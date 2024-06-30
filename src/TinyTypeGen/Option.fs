module TsGen.Option

open System.Text.Json.Serialization

let render (encoding: JsonUnionEncoding) (strategy: RenderStrategy) =
  let is v = encoding &&& v = v

  let definition, value =
    if is JsonUnionEncoding.Untagged then
      "{\"value\":T} | null", "null"
    elif (is JsonUnionEncoding.UnwrapOption) || (is JsonUnionEncoding.FSharpLuLike) then
      "T | null", "null"
    elif (is JsonUnionEncoding.NamedFields) then
      "{\"Case\":\"Some\",\"Fields\":{\"value\":T}} | null", "null"
    elif is JsonUnionEncoding.ThothLike then
      "[\"Some\",T] | null", "null"
    elif is (JsonUnionEncoding.ExternalTag ||| JsonUnionEncoding.UnwrapSingleFieldCases) then
      "{Some: T} | null", "null"
    elif is JsonUnionEncoding.InternalTag || is JsonUnionEncoding.ThothLike then
      "[\"Some\",T] | null", "null"
    elif is JsonUnionEncoding.Untagged then
      "{\"value\":\"test\"} | null", "null"
    elif is JsonUnionEncoding.UnwrapSingleFieldCases then
      "{\"Case\":\"Some\",\"Fields\":T} | null", "null"
    elif ([ JsonUnionEncoding.AdjacentTag
            JsonUnionEncoding.UnwrapSingleCaseUnions
            JsonUnionEncoding.Inherit
            JsonUnionEncoding.NewtonsoftLike
            JsonUnionEncoding.AllowUnorderedTag
            JsonUnionEncoding.UnwrapFieldlessTags
            JsonUnionEncoding.UnionFieldNamesFromTypes
            JsonUnionEncoding.ExternalTag
            JsonUnionEncoding.FSharpLuLike
            JsonUnionEncoding.ThothLike ]
          |> List.exists is) then
      "{\"Case\":\"Some\"; Fields: [T]} | null", "null"
    else
      "T | null // other", "null"

  let definition = "export type FSharpOption<T> = " + definition

  let value =
    "export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => "
    + value

  renderDefinitionAndOrValue definition //value strategy
