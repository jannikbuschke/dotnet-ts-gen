﻿namespace TsGen

open System
open System.Text.Json.Serialization
open System.Text.RegularExpressions

type Serialize = obj -> string

module Regex =

  let replace (pattern: string) (replacement: string) (input: string) =
    Regex.Replace(input, pattern, replacement)

module Utils =
  let toLower (s: string) =
    (s.[0] |> Char.ToLower |> Char.ToString)
    + s.Substring(1)

  let normalizeLineFeeds = Regex.replace @"(\r\n|\r|\n)" "\n"

  let removeSuccessiveLineFeeds = Regex.replace @"[\n]{2,}" "\n"

  let removeSuccessiveWhiteSpace = Regex.replace @"[ ]{3,}" "  "

  let cleanTs =
    removeSuccessiveWhiteSpace
    >> normalizeLineFeeds
    >> removeSuccessiveLineFeeds
    >> removeSuccessiveWhiteSpace

  let camelize (arg: string) : string =
    System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName arg

// module DefaultSerialize =
//
//   let JsonDefaultUnionEncoding =
//     JsonUnionEncoding.AdjacentTag
//     ||| JsonUnionEncoding.UnwrapSingleFieldCases
//     ||| JsonUnionEncoding.UnwrapRecordCases
//     ||| JsonUnionEncoding.UnwrapOption
//     ||| JsonUnionEncoding.UnwrapSingleCaseUnions
//     ||| JsonUnionEncoding.AllowUnorderedTag
//
//   let JsonFSharpOptions = JsonFSharpOptions(JsonDefaultUnionEncoding)
//
//   let getOptions () =
//     let options = System.Text.Json.JsonSerializerOptions()
//
//     options.Converters.Add(JsonStringEnumConverter())
//
//     options.Converters.Add(JsonFSharpConverter(JsonDefaultUnionEncoding))
//     options
//
//   let serialize0 (options: System.Text.Json.JsonSerializerOptions) value =
//     System.Text.Json.JsonSerializer.Serialize(value, options)
//
//   let deserialize0<'a> (options: System.Text.Json.JsonSerializerOptions) (value: string) =
//     System.Text.Json.JsonSerializer.Deserialize<'a>(value, options)
//
//   let options = getOptions ()
