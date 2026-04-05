module Test.Unions.SingleCaseMultiField

open System
open Expecto
open System.Text.Json.Serialization
open Test
open type JsonUnionEncoding

[<RequireQualifiedAccess>]
type Du = | Case1 of Id: Guid * int * Result<string, string>

let value = Du.Case1(Guid.Empty, 42, Ok "Success")

let all =
  [
    AdjacentTag
    UnwrapOption
    AllowUnorderedTag
    UnwrapSingleCaseUnions
    Inherit
    NamedFields
    UnwrapSingleFieldCases
    UnionFieldNamesFromTypes

    ExternalTag
    InternalTag
    Untagged
    Inherit
    UnwrapFieldlessTags
    UnwrapOption
    UnwrapRecordCases
    AllowUnorderedTag
  ]

let fieldsArray2 =
  [
    InternalTag ||| UnwrapSingleFieldCases
  // ExternalTag ||| UnwrapSingleFieldCases
  ]

let fieldsArray3 =
  [
    // InternalTag ||| UnwrapSingleFieldCases
    ExternalTag ||| UnwrapSingleFieldCases
  ]

let fieldsArray = [ Default; AdjacentTag; AdjacentTag ||| UnwrapSingleFieldCases ]

let tests =
  testList
    "Unions.SingleCaseMultiField"
    [
      testCase
        "all serialized"
        (fun () ->
          let _ = all |> List.map (fun x -> x, serializeWithEncoding x value)
          ()
        )
      yield!
        fieldsArray2
        |> List.map (fun encoding ->
          testCase
            (sprintf "serialize: fields array2 (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.stringStart """ ["Case1","00000000-0000-0000-0000-000000000000",42,["Ok","Success"]]"""
            )
        )
      //TODO: fix this one, does not match serialization test above
      yield!
        fieldsArray2
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "typedef: fields array2 (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = { "Case1": [ System.Guid, System.Int32, Microsoft_FSharp_Core.FSharpResult<System.String,System.String> ] }
"""
            )
        )
      yield!
        fieldsArray3
        |> List.map (fun encoding ->
          testCase
            (sprintf "serialize: fields array3 (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.stringStart """ {"Case1":["00000000-0000-0000-0000-000000000000",42,{"Ok":"Success"}]}"""
            )
        )
      yield!
        fieldsArray3
        |> List.map (fun encoding ->
          ptestCase
            (sprintf "typedef: fields array3 (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = { "Case1": [ System.Guid, System.Int32, Microsoft_FSharp_Core.FSharpResult<System.String,System.String> ] }
"""
            )
        )
      yield!
        fieldsArray
        |> List.map (fun encoding ->
          testCase
            (sprintf "serialize: fields array (%A)" encoding)
            (fun () ->
              value
              |> serializeWithEncoding encoding
              |> Expect.stringStart
                """ {"Case":"Case1","Fields":["00000000-0000-0000-0000-000000000000",42,{"Case":"Ok","Fields": """
            )
        )
      yield!
        fieldsArray
        |> List.map (fun encoding ->
          testCase
            (sprintf "typedef: fields array (%A)" encoding)
            (fun () ->
              typedefof<Du>
              |> renderCustomTypeDef encoding
              |> Expect.stringStart
                """
export type Du_Case_Case1 = { Case: "Case1", Fields: [ System.Guid, System.Int32, Microsoft_FSharp_Core.FSharpResult<System.String,System.String> ] }
"""
            )
        )
      testCase
        "AdjacentTag UnwrapRecordCases serialize"
        (fun () ->
          value
          |> serializeWithEncoding (AdjacentTag ||| UnwrapRecordCases)
          |> Expect.stringStart
            """{"Case":"Case1","Fields":{"id":"00000000-0000-0000-0000-000000000000","item2":42,"item3":{"Case":"Ok","Fields":{"resultValue":"Success"}}}}"""
        )
      testCase
        "AdjacentTag UnwrapRecordCases typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef (AdjacentTag ||| UnwrapRecordCases)
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case: "Case1", Fields: { id: System.Guid, item2: System.Int32, item3: Microsoft_FSharp_Core.FSharpResult<System.String,System.String> } }
"""
        )
      testCase
        "serialize: external tag"
        (fun () ->
          value
          |> serializeWithEncoding ExternalTag
          |> Expect.equal """ {"Case1":["00000000-0000-0000-0000-000000000000",42,{"Ok":["Success"]}]} """
        )
      testCase
        "ExternalTag - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef ExternalTag
          |> Expect.stringStart
            """
export type Du_Case_Case1 = { Case1: [ System.Guid, System.Int32, Microsoft_FSharp_Core.FSharpResult<System.String,System.String> ] }
"""
        )
      testCase
        "InternalTag - serialize"
        (fun () ->
          value
          |> serializeWithEncoding InternalTag
          |> Expect.equal """ ["Case1","00000000-0000-0000-0000-000000000000",42,["Ok","Success"]] """
        )
      testCase
        "InternalTag - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef InternalTag
          |> Expect.stringStart
            """
export type Du_Case_Case1 = [ "Case1", System.Guid, System.Int32, Microsoft_FSharp_Core.FSharpResult<System.String,System.String> ]
"""
        )
      testCase
        "untagged - serialize"
        (fun () ->
          value
          |> serializeWithEncoding Untagged
          |> Expect.equal
            """ {"id":"00000000-0000-0000-0000-000000000000","item2":42,"item3":{"resultValue":"Success"}} """
        )
      ptestCase
        "untagged - typedef"
        (fun () ->
          typedefof<Du>
          |> renderCustomTypeDef Untagged
          |> Expect.equal """Untagged is not supported"""
        )
    ]
