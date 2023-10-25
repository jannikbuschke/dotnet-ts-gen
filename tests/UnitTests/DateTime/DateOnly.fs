module Test.DateOnly

open System
open Expecto
open System.Text.Json.Serialization
open Test
open Test.Unions.Utils

let testInputs =
  [
    [
      JsonUnionEncoding.Default
      JsonUnionEncoding.UnwrapSingleCaseUnions
      JsonUnionEncoding.NamedFields ||| JsonUnionEncoding.UnwrapSingleCaseUnions
      JsonUnionEncoding.AdjacentTag
      JsonUnionEncoding.Inherit
      JsonUnionEncoding.NewtonsoftLike
      JsonUnionEncoding.UnwrapOption
      JsonUnionEncoding.AllowUnorderedTag
      JsonUnionEncoding.UnwrapFieldlessTags
      JsonUnionEncoding.UnionFieldNamesFromTypes

      JsonUnionEncoding.ExternalTag

      JsonUnionEncoding.ExternalTag ||| JsonUnionEncoding.UnwrapSingleFieldCases
      JsonUnionEncoding.InternalTag
      JsonUnionEncoding.ThothLike
      JsonUnionEncoding.NamedFields
      JsonUnionEncoding.Untagged
      JsonUnionEncoding.UnwrapSingleFieldCases
    ],
    "\"2023-05-05\""
  ]
  |> toTestCases

let tests =
  testList
    "DateOnly"
    [
      yield!
        testInputs
        |> List.map (fun (encoding, expected) ->
          testCase
            (sprintf "DateOnly - serialized (%A)" encoding)
            (fun () ->
              serializeWithEncoding encoding (DateOnly.Parse("2023-05-05"))
              |> Expect.similar expected
            )
        )

      testCase
        "DateOnly - definition"
        (fun () ->
          typedefof<DateOnly>
          |> definition
          |> Expect.similar
            """
export type DateOnly = `${number}-${number}-${number}`
"""
        )
    ]
