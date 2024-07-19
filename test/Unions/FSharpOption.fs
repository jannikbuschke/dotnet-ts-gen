module Test.Unions.FSharpOption

open System.Text.Json.Serialization
open Expecto
open Xunit
open Test


[<Fact>]
let ``FSharpOption - default - definition`` () =
  let definition, value = renderTypeAndValue typedefof<Option<string>>

  Expect.similar
    definition
    """
export type FSharpOption<T> = T | null
"""

// [<Fact>]
// let ``FSharpOption - default - value`` () =
//   Expect.similar
//     value
//     """
// export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null
// """

// Serialization
let inputsSome =
  [
    // Unwrapped
    ([ JsonUnionEncoding.Default
       JsonUnionEncoding.UnwrapOption
       JsonUnionEncoding.FSharpLuLike ],
     "\"test\"")
    ([ JsonUnionEncoding.UnwrapSingleCaseUnions ], "{\"Case\":\"Some\",\"Fields\":[\"test\"]}")
    ([ JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions ], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    // Fields as array
    ([ JsonUnionEncoding.AdjacentTag
       JsonUnionEncoding.Inherit
       JsonUnionEncoding.NewtonsoftLike
       JsonUnionEncoding.AllowUnorderedTag
       JsonUnionEncoding.UnwrapFieldlessTags

       JsonUnionEncoding.UnionFieldNamesFromTypes ],
     "{\"Case\":\"Some\",\"Fields\":[\"test\"]}")
    // External Tag, Fields as Array
    ([ JsonUnionEncoding.ExternalTag ], "{\"Some\":[\"test\"]}")
    ([ JsonUnionEncoding.ExternalTag ||| JsonUnionEncoding.UnwrapSingleFieldCases ], "{\"Some\":\"test\"}")
    // Array of tuples
    ([ JsonUnionEncoding.InternalTag; JsonUnionEncoding.ThothLike ], "[\"Some\",\"test\"]")
    // Fields as record
    ([ JsonUnionEncoding.NamedFields ], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    // Untagged
    ([ JsonUnionEncoding.Untagged ], "{\"value\":\"test\"}")
    // Single fields are unwrapped
    ([ JsonUnionEncoding.UnwrapSingleFieldCases ], "{\"Case\":\"Some\",\"Fields\":\"test\"}") ]
  |> toTestInputs

[<Theory>]
[<MemberData(nameof inputsSome)>]
let ``FSharpOption Some - serialized`` (encoding: JsonUnionEncoding) (expectedSome: string) =
  let some = serializeWithEncoding encoding (Some "test")
  Expect.similar some expectedSome

let testInputs =
  [
    // Unwrapped
    ([ JsonUnionEncoding.Default
       JsonUnionEncoding.UnwrapSingleCaseUnions
       JsonUnionEncoding.AdjacentTag
       JsonUnionEncoding.Inherit
       JsonUnionEncoding.NewtonsoftLike
       JsonUnionEncoding.UnwrapOption
       JsonUnionEncoding.AllowUnorderedTag
       JsonUnionEncoding.UnwrapFieldlessTags
       JsonUnionEncoding.UnionFieldNamesFromTypes
       JsonUnionEncoding.ExternalTag
       JsonUnionEncoding.FSharpLuLike
       JsonUnionEncoding.AdjacentTag ||| JsonUnionEncoding.NamedFields
       JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions ],
     "null")
    // Fields as array
    ([ JsonUnionEncoding.AdjacentTag
       JsonUnionEncoding.Inherit
       JsonUnionEncoding.NewtonsoftLike
       JsonUnionEncoding.UnwrapOption
       JsonUnionEncoding.AllowUnorderedTag
       JsonUnionEncoding.UnwrapFieldlessTags
       JsonUnionEncoding.UnionFieldNamesFromTypes ],
     "null")
    // External Tag, Fields as Array
    ([ JsonUnionEncoding.ExternalTag ], "null")
    ([ JsonUnionEncoding.ExternalTag ||| JsonUnionEncoding.UnwrapSingleFieldCases ], "null")
    // Array of tuples
    ([ JsonUnionEncoding.InternalTag; JsonUnionEncoding.ThothLike ], "null")
    // Fields as record
    ([ JsonUnionEncoding.NamedFields ], "null")
    // Untagged
    ([ JsonUnionEncoding.Untagged ], "null")
    // Single fields are unwrapped
    ([ JsonUnionEncoding.UnwrapSingleFieldCases ], "null") ]
  |> toTestInputs

[<Theory>]
[<MemberData(nameof testInputs)>]
let ``FSharpOption None - serialized`` (encoding: JsonUnionEncoding) (expectedNone: string) =
  let none = serializeWithEncoding encoding None
  Expect.similar none expectedNone

// Type tests
let typeTestInputs =
  [
    // Unwrapped
    ([ JsonUnionEncoding.Default
       JsonUnionEncoding.UnwrapOption
       JsonUnionEncoding.FSharpLuLike ],
     "export type FSharpOption<T> = T | null",
     "export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null")
    // Fields as array
    ([ JsonUnionEncoding.AdjacentTag
       JsonUnionEncoding.UnwrapSingleCaseUnions
       JsonUnionEncoding.Inherit
       JsonUnionEncoding.NewtonsoftLike
       JsonUnionEncoding.AllowUnorderedTag
       JsonUnionEncoding.UnwrapFieldlessTags
       JsonUnionEncoding.UnionFieldNamesFromTypes
       JsonUnionEncoding.ExternalTag ],
     "export type FSharpOption<T> = {\"Case\":\"Some\"; Fields: [T]} | null",
     "export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null")
    // External Tag, Fields as Array
    ([ JsonUnionEncoding.ExternalTag ||| JsonUnionEncoding.UnwrapSingleFieldCases ],
     "export type FSharpOption<T> = {Some: T} | null",
     "export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null")
    // JsonUnionEncoding.FSharpLuLike
    // JsonUnionEncoding.ThothLike
    // Array of tuples
    ([ JsonUnionEncoding.InternalTag; JsonUnionEncoding.ThothLike ],
     "export type FSharpOption<T> = [\"Some\",T] | null",
     "export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null")
    // Fields as record
    ([ JsonUnionEncoding.NamedFields ],
     "export type FSharpOption<T> = {\"Case\":\"Some\",\"Fields\":{\"value\":T}} | null",
     "export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null")
    // Untagged
    ([ JsonUnionEncoding.Untagged ],
     "export type FSharpOption<T> = {\"value\":T} | null",
     "export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null")
    // Single fields are unwrapped
    ([ JsonUnionEncoding.UnwrapSingleFieldCases ],
     "export type FSharpOption<T> = {\"Case\":\"Some\",\"Fields\":T} | null",
     "export var defaultFSharpOption: <T>(defaultT:T) => FSharpOption<T> = <T>(defaultT:T) => null") ]
  |> toTestInputs2

[<Theory>]
[<MemberData(nameof typeTestInputs)>]
let ``FSharpOption - other encodings`` (encoding: JsonUnionEncoding) (expectedDefinition: string) (expectedValue: string) =
  let definition, value = renderCustomTypeAndValue encoding typedefof<Option<_>>
  Expect.similar definition expectedDefinition
  // Expect.similar value expectedValue
