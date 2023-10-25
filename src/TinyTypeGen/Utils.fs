namespace TinyTypeGen

open System
open System.Text.RegularExpressions

type Serialize = obj -> string

module Regex =

  let replace (pattern: string) (replacement: string) (input: string) =
    Regex.Replace(input, pattern, replacement)

module Utils =
  open System.Collections.Generic

  let toLower (s: string) =
    (s.[0] |> Char.ToLower |> Char.ToString) + s.Substring(1)

  let normalizeLineFeeds = Regex.replace @"(\r\n|\r|\n)" "\n"

  let removeSuccessiveLineFeeds = Regex.replace @"[\n]{2,}" "\n"

  let removeSuccessiveWhiteSpace = Regex.replace @"[ ]{3,}" "  "

  let cleanTs =
    removeSuccessiveWhiteSpace
    >> normalizeLineFeeds
    >> removeSuccessiveLineFeeds
    >> removeSuccessiveWhiteSpace

  let camelize (arg: string) : string =
    Text.Json.JsonNamingPolicy.CamelCase.ConvertName arg

  let toSnakeCase (arg: string) : string =
    Regex.Replace(arg, @"(?<!^)([A-Z])", "_$1").ToLowerInvariant()

  let applyPropertyCasing (casing: PropertyCasing) (name: string) : string =
    match casing with
    | PropertyCasing.CamelCase -> camelize name
    | PropertyCasing.PascalCase -> name
    | PropertyCasing.SnakeCase -> toSnakeCase name

  let memoize f =
    let dict = Dictionary<_, _>()

    fun c ->
      let exist, value = dict.TryGetValue c

      match exist with
      | true -> value
      | _ ->
        let value = f c
        dict.Add(c, value)
        value
