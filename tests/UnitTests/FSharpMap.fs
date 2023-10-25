module Test.FSharpMap

open Expecto
open System.Text.Json
open System.Text.Json.Serialization

let typedefofMapStringString = typedefof<Map<string, string>>
let typeofMapStringString = typeof<Map<string, string>>

let options = JsonFSharpOptions.Default().ToJsonSerializerOptions()

let serialize v = JsonSerializer.Serialize(v, options)

let deserialize<'t> (v: string) =
  JsonSerializer.Deserialize<'t>(v, options)

type Language =
  | De
  | En

type LocalizableValue<'T> =
  {
    Default: 'T
    Localized: Map<Language, 'T>
  }

type LocalizableString = LocalizableValue<string>
type Container = { Title: LocalizableString }

// https://github.com/Tarmil/FSharp.SystemTextJson/blob/master/docs/Format.md#maps
let tests =
  testList
    "FSharpMap"
    [
      testCase
        "Serialize FSharpMap"
        (fun () ->
          // string key is serialized as dictionary with string indexer
          let mapStringKey = serialize ([ "k1", "v1"; "k2", "v2" ] |> Map.ofSeq)

          Expect.similar mapStringKey """{"k1":"v1","k2":"v2"}"""

          // other key types are serialized as array of arrays
          let mapIntKey = serialize ([ 0, "v1"; 1, "v2" ] |> Map.ofSeq)

          Expect.similar mapIntKey """[[0,"v1"],[1,"v2"]]"""

          let mapComplexKey =
            serialize ([ ((0, 0), {| Id = "FOo" |}); ((0, 1), {| Id = "FOo111" |}) ] |> Map.ofSeq)

          Expect.similar mapComplexKey """[[[0,0],{"Id":"FOo"}],[[0,1],{"Id":"FOo111"}]]"""
        )
      testCase
        "FSharpMap<string,'t>"
        (fun () ->
          typeofMapStringString
          |> definition
          |> Expect.similar
            """
export type FSharpMap<TKey, TValue> = TKey extends string
 ? string extends TKey
 ? { [key: string]: TValue }
 : [[TKey, TValue]]
 : [[TKey, TValue]]
"""
        )
      testCase
        "FSharpMap<'k,'t>"
        (fun () ->
          typedefofMapStringString
          |> definition
          |> Expect.similar
            """
export type FSharpMap<TKey, TValue> = TKey extends string
 ? string extends TKey
 ? { [key: string]: TValue }
 : [[TKey, TValue]]
 : [[TKey, TValue]]
"""
        )
      testCase
        "Generic record with FSharpMap property - definition"
        (fun () ->
          typedefof<LocalizableValue<string>>
          |> definition
          |> Expect.similar
            """export type LocalizableValue<T> = {
  default: T
  localized: Microsoft_FSharp_Collections.FSharpMap<Language,T>
}
"""
        )
      testCase
        "Record with property of a generic record that has a map property"
        (fun () ->
          typedefof<Container>
          |> definition
          |> Expect.similar
            """
export type Container = {
  title: LocalizableValue<System.String>
}
"""
        )
    ]
