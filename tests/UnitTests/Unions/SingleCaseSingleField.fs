module Test.Unions.SingleCaseSingleField

open System
open Expecto
open System.Text.Json.Serialization
open Test
open type JsonUnionEncoding

[<RequireQualifiedAccess>]
type Du = | Case1 of Guid

let value = Du.Case1 Guid.Empty

let check value typedef encoding expectedJson expectedDefinition =
  value |> serializeWithEncoding encoding |> Expect.equal expectedJson
  typedef |> renderCustomTypeDef encoding |> Expect.stringStart expectedDefinition

let test = check value typedefof<Du>

let tests =
  testList
    "Unions.SingleCaseSingleField"
    [
      testCase
        "ExternalTag test"
        (fun () ->
          test
            ExternalTag
            """{"Case1":["00000000-0000-0000-0000-000000000000"]}"""
            "export type Du_Case_Case1 = { Case1: [ System.Guid ] }"
        )
      testCase
        "ExternalTag NamedFields"
        (fun () ->
          test
            (ExternalTag ||| NamedFields)
            """{"Case1":{"item":"00000000-0000-0000-0000-000000000000"}}"""
            "export type Du_Case_Case1 = { Case1: { item: System.Guid } }"
        )
      testCase
        "InternalTag NamedFields"
        (fun () ->
          test
            (InternalTag ||| NamedFields)
            """{"Case":"Case1","item":"00000000-0000-0000-0000-000000000000"}"""
            """export type Du_Case_Case1 = { Case: "Case1", item: System.Guid }"""
        )
      testCase
        "AdjacentTag NamedFields"
        (fun () ->
          test
            (AdjacentTag ||| NamedFields)
            """{"Case":"Case1","Fields":{"item":"00000000-0000-0000-0000-000000000000"}}"""
            """export type Du_Case_Case1 = { Case: "Case1", Fields: { item: System.Guid } }"""
        )
      testCase
        "InternalTag UnwrapSingleCaseUnions"
        (fun () ->
          test
            (InternalTag ||| UnwrapSingleCaseUnions)
            "\"00000000-0000-0000-0000-000000000000\""
            """export type Du_Case_Case1 = System.Guid"""
        )
      testCase
        "AdjacentTag UnwrapSingleCaseUnions"
        (fun () ->
          test
            (AdjacentTag ||| UnwrapSingleCaseUnions)
            "\"00000000-0000-0000-0000-000000000000\""
            """export type Du_Case_Case1 = System.Guid"""
        )
      testCase
        "External UnwrapSingleCaseUnions"
        (fun () ->
          test
            (ExternalTag ||| UnwrapSingleCaseUnions)
            "\"00000000-0000-0000-0000-000000000000\""
            """export type Du_Case_Case1 = System.Guid"""
        )
      // --- Skipped (old/broken) ---
      yield!
        [
          Default
          UnwrapSingleCaseUnions
          NamedFields ||| UnwrapSingleCaseUnions
        ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "serialize: unwrapped (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.equal """ "00000000-0000-0000-0000-000000000000" """
            )
        )
      yield!
        [
          Default
          UnwrapSingleCaseUnions
          NamedFields ||| UnwrapSingleCaseUnions
        ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "typedef: unwrapped (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = System.Guid
"""
            )
        )
      yield!
        [
          AdjacentTag
          Inherit
          NewtonsoftLike
          UnwrapOption
          AllowUnorderedTag
          UnwrapFieldlessTags
          UnionFieldNamesFromTypes
        ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "serialize: fields as array (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.equal """{"Case":"Case1","Fields":["00000000-0000-0000-0000-000000000000"]}"""
            )
        )
      yield!
        [
          AdjacentTag
          Inherit
          NewtonsoftLike
          UnwrapOption
          AllowUnorderedTag
          UnwrapFieldlessTags
          UnionFieldNamesFromTypes
        ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "typedef: fields as array (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = { Case: "Case1"; Fields: [System.Guid] }
"""
            )
        )
      yield!
        [ ExternalTag ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "serialize: external tag, fields as array (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.equal """{"Case1":["00000000-0000-0000-0000-000000000000"]}"""
            )
        )
      yield!
        [ ExternalTag ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "definition: external tag, fields as array (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = { "Case1": [System.Guid] }
"""
            )
        )
      yield!
        [ ExternalTag ||| UnwrapSingleFieldCases ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "serialize: external tag, unwrapped (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.equal """{"Case1":"00000000-0000-0000-0000-000000000000"}"""
            )
        )
      yield!
        [ ExternalTag ||| UnwrapSingleFieldCases ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "definition: external tag, unwrapped (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = { "Case1": System.Guid }
"""
            )
        )
      yield!
        [ InternalTag; ThothLike ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "serialize: array of tuples (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.equal """["Case1","00000000-0000-0000-0000-000000000000"]"""
            )
        )
      yield!
        [ InternalTag; ThothLike ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "definition: array of tuples (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = ["Case1", System.Guid]
"""
            )
        )
      yield!
        [ NamedFields ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "serialize: fields as record (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.equal """{"Case":"Case1","Fields":{"item":"00000000-0000-0000-0000-000000000000"}}"""
            )
        )
      yield!
        [ NamedFields ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "definition: fields as record (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = {"Case":"Case1",Fields:{"item": System.Guid } }
"""
            )
        )
      yield!
        [ Untagged ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "serialize: untagged (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.equal """{"item":"00000000-0000-0000-0000-000000000000"}"""
            )
        )
      yield!
        [ Untagged ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "definition: untagged (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = { "item": System.Guid }
"""
            )
        )
      yield!
        [ JsonUnionEncoding.UnwrapSingleFieldCases ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "serialize: unwrap single field cases (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.equal """{"Case":"Case1","Fields":"00000000-0000-0000-0000-000000000000"}"""
            )
        )
      yield!
        [ JsonUnionEncoding.UnwrapSingleFieldCases ]
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "definition: unwrap single field cases (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = { Case: "Case1", Fields: System.Guid }
"""
            )
        )
    ]
