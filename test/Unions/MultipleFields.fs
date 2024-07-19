module Test.Unions.MultipleFields

open System.Text.Json.Serialization
open Expecto
open Test
open TsGen
open Xunit

type SimpleRecord0 = { Name: string }

type Du =
  | Case1
  | Case2 of int
  | Case3 of MyValue: int
  | Case4 of int * string
  | Case5 of int * MyValue: string * SimpleRecord0

// Serialization
let inputsCase1 =
  [ ([ JsonUnionEncoding.Default
       JsonUnionEncoding.UnwrapOption
       JsonUnionEncoding.AdjacentTag
       JsonUnionEncoding.AllowUnorderedTag
       JsonUnionEncoding.UnwrapSingleCaseUnions
       JsonUnionEncoding.Inherit
       JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions
       JsonUnionEncoding.NamedFields
       JsonUnionEncoding.NewtonsoftLike
       JsonUnionEncoding.UnwrapSingleFieldCases
       JsonUnionEncoding.UnionFieldNamesFromTypes ],
     "{\"Case\":\"Case1\"}")
    ([ JsonUnionEncoding.UnwrapFieldlessTags
       JsonUnionEncoding.FSharpLuLike
       JsonUnionEncoding.ThothLike ],
     "\"Case1\"")
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    // Fields as array
    ([], "{\"Case\":\"Some\",\"Fields\":[\"test\"]}")
    // External Tag, Fields as Array
    ([ JsonUnionEncoding.ExternalTag ], "{\"Case1\":[]}")
    // Array of tuples
    ([ JsonUnionEncoding.InternalTag ], "[\"Case1\"]")
    // Fields as record
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    // Untagged
    ([ JsonUnionEncoding.Untagged ], "{}")
    // Single fields are unwrapped
    ([], "{\"Case\":\"Some\",\"Fields\":\"test\"}") ]
  |> toTestInputs

[<Theory>]
[<MemberData(nameof inputsCase1)>]
let ``Record - Case1: field-less Case - serialized`` (encoding: JsonUnionEncoding) (expected: string) =
  let some = serializeWithEncoding encoding (Du.Case1)
  Expect.similar some expected

// Serialization
let inputsCase2 =
  [ ([ JsonUnionEncoding.Default
       JsonUnionEncoding.UnwrapOption
       JsonUnionEncoding.AdjacentTag
       JsonUnionEncoding.AllowUnorderedTag
       JsonUnionEncoding.UnwrapSingleCaseUnions
       JsonUnionEncoding.Inherit
       JsonUnionEncoding.NewtonsoftLike
       JsonUnionEncoding.UnionFieldNamesFromTypes ],
     "{\"Case\":\"Case2\",\"Fields\":[0]}")
    ([ JsonUnionEncoding.FSharpLuLike ], "{\"Case2\":0}")
    ([ JsonUnionEncoding.ThothLike ], "[\"Case2\",0]")
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    // Fields as array
    ([ JsonUnionEncoding.UnwrapFieldlessTags ], "{\"Case\":\"Case2\",\"Fields\":[0]}")
    // External Tag, Fields as Array
    ([ JsonUnionEncoding.ExternalTag ], "{\"Case2\":[0]}")
    // Array of tuples
    ([ JsonUnionEncoding.InternalTag ], "[\"Case2\",0]")
    // Fields as record
    ([ JsonUnionEncoding.NamedFields
       JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions ],
     "{\"Case\":\"Case2\",\"Fields\":{\"item\":0}}")
    // Untagged
    ([ JsonUnionEncoding.Untagged ], "{\"item\":0}")
    // Single fields are unwrapped
    ([ JsonUnionEncoding.UnwrapSingleFieldCases ], "{\"Case\":\"Case2\",\"Fields\":0}") ]
  |> toTestInputs

[<Theory>]
[<MemberData(nameof inputsCase2)>]
let ``Record - Case2: unnamed field - serialized`` (encoding: JsonUnionEncoding) (expected: string) =
  let some = serializeWithEncoding encoding (Du.Case2 0)
  Expect.similar some expected

// Serialization
let inputsCase3 =
  [ ([ JsonUnionEncoding.Default
       JsonUnionEncoding.UnwrapOption
       JsonUnionEncoding.AdjacentTag
       JsonUnionEncoding.AllowUnorderedTag
       JsonUnionEncoding.UnwrapSingleCaseUnions
       JsonUnionEncoding.Inherit
       JsonUnionEncoding.NewtonsoftLike
       JsonUnionEncoding.UnionFieldNamesFromTypes ],
     "{\"Case\":\"Case3\",\"Fields\":[0]}")
    ([ JsonUnionEncoding.FSharpLuLike ], "{\"Case3\":0}")
    ([ JsonUnionEncoding.ThothLike ], "[\"Case3\",0]")
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    // Fields as array
    ([ JsonUnionEncoding.UnwrapFieldlessTags ], "{\"Case\":\"Case3\",\"Fields\":[0]}")
    // External Tag, Fields as Array
    ([ JsonUnionEncoding.ExternalTag ], "{\"Case3\":[0]}")
    // Array of tuples
    ([ JsonUnionEncoding.InternalTag ], "[\"Case3\",0]")
    // Fields as record
    ([ JsonUnionEncoding.NamedFields
       JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions ],
     "{\"Case\":\"Case3\",\"Fields\":{\"myValue\":0}}")
    // Untagged
    ([ JsonUnionEncoding.Untagged ], "{\"myValue\":0}")
    // Single fields are unwrapped
    ([ JsonUnionEncoding.UnwrapSingleFieldCases ], "{\"Case\":\"Case3\",\"Fields\":0}") ]
  |> toTestInputs

[<Theory>]
[<MemberData(nameof inputsCase3)>]
let ``Record - Case 3 named field - serialized`` (encoding: JsonUnionEncoding) (expected: string) =
  let some = serializeWithEncoding encoding (Du.Case3 0)
  Expect.similar some expected

// Serialization
let inputsCase4 =
  [ ([ JsonUnionEncoding.Default
       JsonUnionEncoding.UnwrapOption
       JsonUnionEncoding.AdjacentTag
       JsonUnionEncoding.AllowUnorderedTag
       JsonUnionEncoding.UnwrapSingleCaseUnions
       JsonUnionEncoding.Inherit
       JsonUnionEncoding.NewtonsoftLike
       JsonUnionEncoding.UnionFieldNamesFromTypes ],
     "{\"Case\":\"Case4\",\"Fields\":[0,\"test\"]}")
    ([ JsonUnionEncoding.FSharpLuLike ], "{\"Case4\":[0,\"test\"]}")
    ([ JsonUnionEncoding.ThothLike ], "[\"Case4\",0,\"test\"]")
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    // Fields as array
    ([ JsonUnionEncoding.UnwrapFieldlessTags ], "{\"Case\":\"Case4\",\"Fields\":[0,\"test\"]}")
    // External Tag, Fields as Array
    ([ JsonUnionEncoding.ExternalTag ], "{\"Case4\":[0,\"test\"]}")
    // Array of tuples
    ([ JsonUnionEncoding.InternalTag ], "[\"Case4\",0,\"test\"]")
    // Fields as record
    ([ JsonUnionEncoding.NamedFields
       JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions ],
     "{\"Case\":\"Case4\",\"Fields\":{\"item1\":0,\"item2\":\"test\"}}")
    // Untagged
    ([ JsonUnionEncoding.Untagged ], "{\"item1\":0,\"item2\":\"test\"}")
    // Single fields are unwrapped
    ([ JsonUnionEncoding.UnwrapSingleFieldCases ], "{\"Case\":\"Case4\",\"Fields\":[0,\"test\"]}") ]
  |> toTestInputs

[<Theory>]
[<MemberData(nameof inputsCase4)>]
let ``Record - Case 4 named field - serialized`` (encoding: JsonUnionEncoding) (expected: string) =
  let some = serializeWithEncoding encoding (Du.Case4(0, "test"))
  Expect.similar some expected

// Serialization
let inputsCase5 =
  [ ([ JsonUnionEncoding.Default
       JsonUnionEncoding.UnwrapOption
       JsonUnionEncoding.AdjacentTag
       JsonUnionEncoding.AllowUnorderedTag
       JsonUnionEncoding.UnwrapSingleCaseUnions
       JsonUnionEncoding.Inherit
       JsonUnionEncoding.NewtonsoftLike
       JsonUnionEncoding.UnionFieldNamesFromTypes ],
     "{\"Case\":\"Case5\",\"Fields\":[0,\"test\",{\"name\":\"rec0\"}]}")
    ([ JsonUnionEncoding.FSharpLuLike ], "{\"Case5\":[0,\"test\",{\"name\":\"rec0\"}]}")
    ([ JsonUnionEncoding.ThothLike ], "[\"Case5\",0,\"test\",{\"name\":\"rec0\"}]")
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    // Fields as array
    ([ JsonUnionEncoding.UnwrapFieldlessTags ], "{\"Case\":\"Case5\",\"Fields\":[0,\"test\",{\"name\":\"rec0\"}]}")
    // External Tag, Fields as Array
    ([ JsonUnionEncoding.ExternalTag ], "{\"Case5\":[0,\"test\",{\"name\":\"rec0\"}]}")
    // Array of tuples
    ([ JsonUnionEncoding.InternalTag ], "[\"Case5\",0,\"test\",{\"name\":\"rec0\"}]")
    // Fields as record
    ([ JsonUnionEncoding.NamedFields
       JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions ],
     "{\"Case\":\"Case5\",\"Fields\":{\"item1\":0,\"myValue\":\"test\",\"item3\":{\"name\":\"rec0\"}}}")
    // Untagged
    ([ JsonUnionEncoding.Untagged ], "{\"item1\":0,\"myValue\":\"test\",\"item3\":{\"name\":\"rec0\"}}")
    // Single fields are unwrapped
    ([ JsonUnionEncoding.UnwrapSingleFieldCases ], "{\"Case\":\"Case5\",\"Fields\":[0,\"test\",{\"name\":\"rec0\"}]}") ]
  |> toTestInputs

[<Theory>]
[<MemberData(nameof inputsCase5)>]
let ``Record - Case 5 named field - serialized`` (encoding: JsonUnionEncoding) (expected: string) =
  let some = serializeWithEncoding encoding (Du.Case5(0,"test",{SimpleRecord0.Name="rec0"}))
  Expect.similar some expected

// let typedef, value = renderTypeAndValue typedefof<Du>

// [<Fact>]
// let ``Union with single record fields`` () =
//
//   Expect.similar
//     typedef
//     """
// export type DuWithRecordFields_Case_Case1 = { Case: "Case1" }
// export type DuWithRecordFields_Case_Case2 = { Case: "Case2", Fields: System.Int32 }
// export type DuWithRecordFields = DuWithRecordFields_Case_Case1 | DuWithRecordFields_Case_Case2
// export type DuWithRecordFields_Case = "Case1" | "Case2"
// """

// [<Fact>]
// let ``Union with single record fields value`` () =
//   Expect.similar
//     value
//     """
// export var DuWithRecordFields_AllCases = [ "Case1", "Case2" ] as const
// export var defaultDuWithRecordFields_Case_Case1 = { Case: "Case1", Fields: defaultSimpleRecord0 }
// export var defaultDuWithRecordFields_Case_Case2 = { Case: "Case2", Fields: defaultSimpleRecord0 }
// export var defaultDuWithRecordFields = defaultDuWithRecordFields_Case_Case1 as DuWithRecordFields
// """

type SimpleRecord = { Name: string }

type DuWithMultipleFields =
  | Case1 of System.Guid * string
  | Case2 of Foo: string * SimpleRecord * X: int32
  | Case3 of Id: System.Guid * Comment: string option

let options = JsonFSharpOptions(Gen.defaultJsonUnionEncoding)
let newtonsoftLike = JsonFSharpOptions.NewtonsoftLike()

let serialize<'t> = serializeWithOptions<'t> options
let deserialize<'t> = deserializeWithOptions<'t> options

// TODO: add generic version
[<Fact>]
let ``Union with multiple fields - definition`` () =
  let typedef2, value2 = renderTypeAndValue typedefof<DuWithMultipleFields>
  // let serialized0 =
  //   serialize (DuWithMultipleFields.Case1(System.Guid.NewGuid(), "Hello world"))
  //
  // let serialized1 =
  //   serialize (DuWithMultipleFields.Case2("FIII", { Name = "string" }, 5))

  Expect.similar
    typedef2
    """
export type DuWithMultipleFields_Case_Case1 = { Case: "Case1", Fields: { item1: System.Guid, item2: System.String } }
export type DuWithMultipleFields_Case_Case2 = { Case: "Case2", Fields: { foo: System.String, item2: SimpleRecord, x: System.Int32 } }
export type DuWithMultipleFields_Case_Case3 = { Case: "Case3", Fields: { id: System.Guid, comment: Microsoft_FSharp_Core.FSharpOption<System.String> } }
export type DuWithMultipleFields = DuWithMultipleFields_Case_Case1 | DuWithMultipleFields_Case_Case2 | DuWithMultipleFields_Case_Case3
export type DuWithMultipleFields_Case = "Case1" | "Case2" | "Case3"
"""

// [<Fact>]
// let ``Union with multiple fields - value`` () =
//   Expect.similar
//     value2
//     """
// export var DuWithMultipleFields_AllCases = [ "Case1", "Case2", "Case3" ] as const
// export var defaultDuWithMultipleFields_Case_Case1 = { Case: "Case1", Fields: { Item1: '00000000-0000-0000-0000-000000000000', Item2: '' } }
// export var defaultDuWithMultipleFields_Case_Case2 = { Case: "Case2", Fields: { Foo: '', Item2: defaultSimpleRecord, X: 0 } }
// export var defaultDuWithMultipleFields_Case_Case3 = { Case: "Case3", Fields: { Id: '00000000-0000-0000-0000-000000000000', Comment: Microsoft_FSharp_Core.defaultFSharpOption(System.defaultString) } }
// export var defaultDuWithMultipleFields = defaultDuWithMultipleFields_Case_Case1 as DuWithMultipleFields
// """
