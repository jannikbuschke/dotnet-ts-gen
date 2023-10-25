module Test.TimeOnly

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
    "\"09:05:00\""
  ]
  |> toTestCases

let tests =
  testList
    "TimeOnly"
    [
      yield!
        testInputs
        |> List.map (fun (encoding, expected) ->
          testCase
            (sprintf "TimeOnly - serialized (%A)" encoding)
            (fun () ->
              serializeWithEncoding encoding (TimeOnly.Parse("09:05"))
              |> Expect.similar expected
            )
        )

      testCase
        "TimeOnly - definition"
        (fun () ->
          typedefof<TimeOnly>
          |> definition
          |> Expect.similar
            """
export type TimeOnly = `${number}:${number}:${number}`
"""
        )
    ]
