module Test.FSharpMap

open Expecto
open Xunit

open System.Text.Json
open System.Text.Json.Serialization

let typedef = typedefof<Map<string, string>>
let typeof = typeof<Map<string, string>>

let options =
  JsonFSharpOptions
    .Default()
    .ToJsonSerializerOptions()

let serialize v = JsonSerializer.Serialize(v, options)

let deserialize<'t> (v: string) =
  JsonSerializer.Deserialize<'t>(v, options)

// https://github.com/Tarmil/FSharp.SystemTextJson/blob/master/docs/Format.md#maps
[<Fact>]
let ``Serialize FSharpMap`` () =
  // string key is serialized as dictionary with string indexer
  let mapStringKey = serialize ([ ("k1", "v1"); ("k2", "v2") ] |> Map.ofSeq)

  Expect.similar mapStringKey """{"k1":"v1","k2":"v2"}"""

  // other key types are serialized as array of arrays
  let mapIntKey = serialize ([ (0, "v1"); (1, "v2") ] |> Map.ofSeq)

  Expect.similar mapIntKey """[[0,"v1"],[1,"v2"]]"""

  let mapComplexKey =
    serialize (
      [ ((0, 0), {| Id = "FOo" |})
        ((0, 1), {| Id = "FOo111" |}) ]
      |> Map.ofSeq
    )

  Expect.similar mapComplexKey """[[[0,0],{"Id":"FOo"}],[[0,1],{"Id":"FOo111"}]]"""

[<Fact>]
let ``FSharpMap<string,'t>`` () =
  let definition, value = renderTypeAndValue typeof

  Expect.similar
    definition
    """
export type FSharpStringMap<TValue> = { [key: string ]: TValue }
"""

//   Expect.similar
//     value
//     """
// export var defaultFSharpStringMap: <TValue>(t:string,tValue:TValue) => FSharpStringMap<TValue> = <TValue>(t:string,tValue:TValue) => ({})
// """

[<Fact>]
let ``FSharpMap<'k,'t>`` () =
  let definition, value = renderTypeAndValue typedef

  Expect.similar
    definition
    """
export type FSharpMap<TKey, TValue> = [TKey,TValue][]
"""

//   Expect.similar
//     value
//     """
// export var defaultFSharpMap: <TKey, TValue>(tKey:TKey,tValue:TValue) => FSharpMap<TKey, TValue> = <TKey, TValue>(tKey:TKey,tValue:TValue) => []
// """

type Language =
  | De
  | En

type LocalizableValue<'T> =
  { Default: 'T
    Localized: Map<Language, 'T> }

[<Fact>]
let ``Generic record with FSharpMap property - definition`` () =
  let typedef0 = renderTypeDef typedefof<LocalizableValue<string>>
  Expect.similar
    typedef0
    """export type LocalizableValue<T> = {
  default: T
  localized: Microsoft_FSharp_Collections.FSharpMap<Language,T>
}
"""

// [<Fact>]
// let ``Generic record with FSharpMap property - value`` () =
//
//   Expect.similar
//     value
//     """
// export var defaultLocalizableValue: <T>(defaultT:T) => LocalizableValue<T> = <T>(defaultT:T) => ({
//   default: defaultT,
//   localized: []
// })
// """

type LocalizableString = LocalizableValue<string>
type Container = { Title: LocalizableString }

[<Fact>]
let ``Record with property of a generic record that has a map property`` () =
  let rendered, value = renderTypeAndValue typedefof<Container>

  Expect.similar
    rendered
    """
export type Container = {
  title: LocalizableValue<System.String>
}
"""

//   Expect.similar
//     value
//     """
// export var defaultContainer: Container = {
//   title: defaultLocalizableValue(System.defaultString)
// }"""
