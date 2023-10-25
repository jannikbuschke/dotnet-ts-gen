module Test.Unions.MultipleCases

open System.Text.Json.Serialization
open Expecto
open Test
open TinyTypeGen.Config
open type JsonUnionEncoding

type SimpleRecord0 = { Name: string }

[<RequireQualifiedAccess>]
type Du =
  | Case1
  | Case2 of int
  | Case3 of MyValue: int
  | Case4 of int * string
  | Case5 of int * MyValue: string * SimpleRecord0

let inputsCase1 =
  [
    ([
      JsonUnionEncoding.Default
      JsonUnionEncoding.UnwrapOption
      JsonUnionEncoding.AdjacentTag
      JsonUnionEncoding.AllowUnorderedTag
      JsonUnionEncoding.UnwrapSingleCaseUnions
      JsonUnionEncoding.Inherit
      JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions
      JsonUnionEncoding.NamedFields
      JsonUnionEncoding.NewtonsoftLike
      JsonUnionEncoding.UnwrapSingleFieldCases
      JsonUnionEncoding.UnionFieldNamesFromTypes
     ],
     "{\"Case\":\"Case1\"}")
    ([
      JsonUnionEncoding.UnwrapFieldlessTags
      JsonUnionEncoding.FSharpLuLike
      JsonUnionEncoding.ThothLike
     ],
     "\"Case1\"")
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    ([], "{\"Case\":\"Some\",\"Fields\":[\"test\"]}")
    ([ JsonUnionEncoding.ExternalTag ], "{\"Case1\":[]}")
    ([ JsonUnionEncoding.InternalTag ], "[\"Case1\"]")
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    ([ JsonUnionEncoding.Untagged ], "{}")
    ([], "{\"Case\":\"Some\",\"Fields\":\"test\"}")
  ]
  |> toTestCases

let inputsCase2 =
  [
    ([
      JsonUnionEncoding.Default
      JsonUnionEncoding.UnwrapOption
      JsonUnionEncoding.AdjacentTag
      JsonUnionEncoding.AllowUnorderedTag
      JsonUnionEncoding.UnwrapSingleCaseUnions
      JsonUnionEncoding.Inherit
      JsonUnionEncoding.NewtonsoftLike
      JsonUnionEncoding.UnionFieldNamesFromTypes
     ],
     "{\"Case\":\"Case2\",\"Fields\":[0]}")
    ([ JsonUnionEncoding.FSharpLuLike ], "{\"Case2\":0}")
    ([ JsonUnionEncoding.ThothLike ], "[\"Case2\",0]")
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    ([ JsonUnionEncoding.UnwrapFieldlessTags ], "{\"Case\":\"Case2\",\"Fields\":[0]}")
    ([ JsonUnionEncoding.ExternalTag ], "{\"Case2\":[0]}")
    ([ JsonUnionEncoding.InternalTag ], "[\"Case2\",0]")
    ([
      JsonUnionEncoding.NamedFields
      JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions
     ],
     "{\"Case\":\"Case2\",\"Fields\":{\"item\":0}}")
    ([ JsonUnionEncoding.Untagged ], "{\"item\":0}")
    ([ JsonUnionEncoding.UnwrapSingleFieldCases ], "{\"Case\":\"Case2\",\"Fields\":0}")
  ]
  |> toTestCases

let inputsCase3 =
  [
    ([
      JsonUnionEncoding.Default
      JsonUnionEncoding.UnwrapOption
      JsonUnionEncoding.AdjacentTag
      JsonUnionEncoding.AllowUnorderedTag
      JsonUnionEncoding.UnwrapSingleCaseUnions
      JsonUnionEncoding.Inherit
      JsonUnionEncoding.NewtonsoftLike
      JsonUnionEncoding.UnionFieldNamesFromTypes
     ],
     "{\"Case\":\"Case3\",\"Fields\":[0]}")
    ([ JsonUnionEncoding.FSharpLuLike ], "{\"Case3\":0}")
    ([ JsonUnionEncoding.ThothLike ], "[\"Case3\",0]")
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    ([ JsonUnionEncoding.UnwrapFieldlessTags ], "{\"Case\":\"Case3\",\"Fields\":[0]}")
    ([ JsonUnionEncoding.ExternalTag ], "{\"Case3\":[0]}")
    ([ JsonUnionEncoding.InternalTag ], "[\"Case3\",0]")
    ([
      JsonUnionEncoding.NamedFields
      JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions
     ],
     "{\"Case\":\"Case3\",\"Fields\":{\"myValue\":0}}")
    ([ JsonUnionEncoding.Untagged ], "{\"myValue\":0}")
    ([ JsonUnionEncoding.UnwrapSingleFieldCases ], "{\"Case\":\"Case3\",\"Fields\":0}")
  ]
  |> toTestCases

let inputsCase4 =
  [
    ([
      JsonUnionEncoding.Default
      JsonUnionEncoding.UnwrapOption
      JsonUnionEncoding.AdjacentTag
      JsonUnionEncoding.AllowUnorderedTag
      JsonUnionEncoding.UnwrapSingleCaseUnions
      JsonUnionEncoding.Inherit
      JsonUnionEncoding.NewtonsoftLike
      JsonUnionEncoding.UnionFieldNamesFromTypes
     ],
     "{\"Case\":\"Case4\",\"Fields\":[0,\"test\"]}")
    ([ JsonUnionEncoding.FSharpLuLike ], "{\"Case4\":[0,\"test\"]}")
    ([ JsonUnionEncoding.ThothLike ], "[\"Case4\",0,\"test\"]")
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    ([ JsonUnionEncoding.UnwrapFieldlessTags ], "{\"Case\":\"Case4\",\"Fields\":[0,\"test\"]}")
    ([ JsonUnionEncoding.ExternalTag ], "{\"Case4\":[0,\"test\"]}")
    ([ JsonUnionEncoding.InternalTag ], "[\"Case4\",0,\"test\"]")
    ([
      JsonUnionEncoding.NamedFields
      JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions
     ],
     "{\"Case\":\"Case4\",\"Fields\":{\"item1\":0,\"item2\":\"test\"}}")
    ([ JsonUnionEncoding.Untagged ], "{\"item1\":0,\"item2\":\"test\"}")
    ([ JsonUnionEncoding.UnwrapSingleFieldCases ], "{\"Case\":\"Case4\",\"Fields\":[0,\"test\"]}")
  ]
  |> toTestCases

let inputsCase5 =
  [
    ([
      JsonUnionEncoding.Default
      JsonUnionEncoding.UnwrapOption
      JsonUnionEncoding.AdjacentTag
      JsonUnionEncoding.AllowUnorderedTag
      JsonUnionEncoding.UnwrapSingleCaseUnions
      JsonUnionEncoding.Inherit
      JsonUnionEncoding.NewtonsoftLike
      JsonUnionEncoding.UnionFieldNamesFromTypes
     ],
     "{\"Case\":\"Case5\",\"Fields\":[0,\"test\",{\"name\":\"rec0\"}]}")
    ([ JsonUnionEncoding.FSharpLuLike ], "{\"Case5\":[0,\"test\",{\"name\":\"rec0\"}]}")
    ([ JsonUnionEncoding.ThothLike ], "[\"Case5\",0,\"test\",{\"name\":\"rec0\"}]")
    ([], "{\"Case\":\"Some\",\"Fields\":{\"value\":\"test\"}}")
    ([ JsonUnionEncoding.UnwrapFieldlessTags ], "{\"Case\":\"Case5\",\"Fields\":[0,\"test\",{\"name\":\"rec0\"}]}")
    ([ JsonUnionEncoding.ExternalTag ], "{\"Case5\":[0,\"test\",{\"name\":\"rec0\"}]}")
    ([ JsonUnionEncoding.InternalTag ], "[\"Case5\",0,\"test\",{\"name\":\"rec0\"}]")
    ([
      JsonUnionEncoding.NamedFields
      JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions
     ],
     "{\"Case\":\"Case5\",\"Fields\":{\"item1\":0,\"myValue\":\"test\",\"item3\":{\"name\":\"rec0\"}}}")
    ([ JsonUnionEncoding.Untagged ], "{\"item1\":0,\"myValue\":\"test\",\"item3\":{\"name\":\"rec0\"}}")
    ([ JsonUnionEncoding.UnwrapSingleFieldCases ], "{\"Case\":\"Case5\",\"Fields\":[0,\"test\",{\"name\":\"rec0\"}]}")
  ]
  |> toTestCases

type SimpleRecord = { Name: string }

type DuWithMultipleFields =
  | Case1 of System.Guid * string
  | Case2 of Foo: string * SimpleRecord * X: int32
  | Case3 of Id: System.Guid * Comment: string option

let options = JsonFSharpOptions(defaultJsonUnionEncoding)
let newtonsoftLike = JsonFSharpOptions.NewtonsoftLike()

let serialize<'t> = serializeWithOptions<'t> options
let deserialize<'t> = deserializeWithOptions<'t> options

let tests =
  testList
    "Unions.MultipleCases"
    [
      yield!
        inputsCase1
        |> List.map (fun (encoding, expected) ->
          testCase
            (sprintf "Record - Case1: field-less Case - serialized (%A)" encoding)
            (fun () ->
              let some = serializeWithEncoding encoding Du.Case1
              Expect.similar some expected
            )
        )
      yield!
        inputsCase2
        |> List.map (fun (encoding, expected) ->
          testCase
            (sprintf "Record - Case2: unnamed field - serialized (%A)" encoding)
            (fun () ->
              let some = serializeWithEncoding encoding (Du.Case2 0)
              Expect.similar some expected
            )
        )
      yield!
        inputsCase3
        |> List.map (fun (encoding, expected) ->
          testCase
            (sprintf "Record - Case 3 named field - serialized (%A)" encoding)
            (fun () ->
              let some = serializeWithEncoding encoding (Du.Case3 0)
              Expect.similar some expected
            )
        )
      yield!
        inputsCase4
        |> List.map (fun (encoding, expected) ->
          testCase
            (sprintf "Record - Case 4 named field - serialized (%A)" encoding)
            (fun () ->
              let some = serializeWithEncoding encoding (Du.Case4(0, "test"))
              Expect.similar some expected
            )
        )
      yield!
        inputsCase5
        |> List.map (fun (encoding, expected) ->
          testCase
            (sprintf "Record - Case 5 named field - serialized (%A)" encoding)
            (fun () ->
              let some =
                serializeWithEncoding encoding (Du.Case5(0, "test", { SimpleRecord0.Name = "rec0" }))

              Expect.similar some expected
            )
        )
      testCase
        "Union with multiple fields - definition"
        (fun () ->
          typedefof<DuWithMultipleFields>
          |> renderCustomTypeDef defaultJsonUnionEncoding
          |> Expect.stringStart
            """
export type DuWithMultipleFields_Case_Case1 = { Case: "Case1", Fields: { item1: System.Guid, item2: System.String } }
export type DuWithMultipleFields_Case_Case2 = { Case: "Case2", Fields: { foo: System.String, item2: SimpleRecord, x: System.Int32 } }
export type DuWithMultipleFields_Case_Case3 = { Case: "Case3", Fields: { id: System.Guid, comment: Microsoft_FSharp_Core.FSharpOption<System.String> } }
"""
        )
    ]
