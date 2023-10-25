module Test.Unions.FSharpOption

open System.Text.Json.Serialization
open Expecto
open TinyTypeGen
open Test
open type JsonUnionEncoding

type Record = { Val: int }
let value: obj list = [ Some 0; None; Some {| Val = 0 |}; Some { Val = 0 } ]
let optionOfResultValue = [ Some(Ok ""); Some(Error ""); None ]
let optionOfRecordValue = [ Some({ Val = 0 }) ]
let resultOfOption = [ Ok(Some ""); Error(Some 1); Ok(None); Error(None) ]


type Referencing =
  {
    primitive: int option
    record: Record option
  }
let referencingValues: obj list =
  [
    {
      primitive = Some 5
      record = Some { Val = 10 }
    }
  ]

let check value typedef encoding expectedJson expectedDefinition =
  value |> serializeWithEncoding encoding |> Expect.equal expectedJson
  typedef |> renderCustomTypeDef encoding |> Expect.stringStart expectedDefinition

let test = check value typedefof<Option<_>>
let testReferencing = check referencingValues typedefof<Referencing>

let testOptionOfRecord = check optionOfRecordValue typedefof<Option<Record>>

let testOptionOfResult =
  check optionOfResultValue typedefof<Option<Result<string, string>>>

let testResultOfOption =
  check resultOfOption typedefof<Result<Option<string>, Option<int>>>

let tests =
  testList
    "Unions.FSharpOption"
    [
      testCase
        "ExternalTag test"
        (fun () ->
          test
            ExternalTag
            """[{"Some":[0]},null,{"Some":[{"val":0}]},{"Some":[{"val":0}]}]"""
            "export type FSharpOption<T> = {Some:T[] } | null"
        )

      testCase
        "ExternalTag UnwrapRecordCase"
        (fun () ->
          test
            (ExternalTag ||| UnwrapRecordCases)
            """[{"Some":{"value":0}},null,{"Some":{"val":0}},{"Some":{"val":0}}]"""
            """export type FSharpOption<T> = { Some: { value: T } } | null
export type FSharpOption_T<T> = { Some: T } | null
"""
        )

      testCase
        "ExternalTag NamedFields"
        (fun () ->
          test
            (ExternalTag ||| NamedFields)
            """[{"Some":{"value":0}},null,{"Some":{"value":{"val":0}}},{"Some":{"value":{"val":0}}}]"""
            "export type FSharpOption<T> = { Some: { value: T } } | null"
        )

      testCase
        "InternalTag NamedFields"
        (fun () ->
          test
            (InternalTag ||| NamedFields)
            """[{"Case":"Some","value":0},null,{"Case":"Some","value":{"val":0}},{"Case":"Some","value":{"val":0}}]"""
            """export type FSharpOption<T> = { Case: "Some", value: T } | null"""
        )

      testCase
        "AdjacentTag NamedFields"
        (fun () ->
          test
            (AdjacentTag ||| NamedFields)
            """[{"Case":"Some","Fields":{"value":0}},null,{"Case":"Some","Fields":{"value":{"val":0}}},{"Case":"Some","Fields":{"value":{"val":0}}}]"""
            """export type FSharpOption<T> = { Case: "Some", Fields: { value: T } } | null"""
        )

      testCase
        "InternalTag UnwrapRecordCases"
        (fun () ->
          test
            (InternalTag ||| UnwrapRecordCases)
            """[{"Case":"Some","value":0},null,{"Case":"Some","val":0},{"Case":"Some","val":0}]"""
            """export type FSharpOption<T> = { Case: "Some", value: T } | null
export type FSharpOption_T<T> = { Case: "Some" } & T | null
"""
        )

      testCase
        "AdjacentTag UnwrapRecordCases"
        (fun () ->
          test
            (AdjacentTag ||| UnwrapRecordCases)
            """[{"Case":"Some","Fields":{"value":0}},null,{"Case":"Some","Fields":{"val":0}},{"Case":"Some","Fields":{"val":0}}]"""
            """export type FSharpOption<T> = { Case: "Some", Fields: { value: T } } | null
export type FSharpOption_T<T> = { Case: "Some", Fields: T } | null
"""
        )

      testCase
        "AdjacentTag NamedFields UnwrapSingleCaseUnions UnwrapSingleFieldCases"
        (fun () ->
          test
            (AdjacentTag
             ||| NamedFields
             ||| UnwrapSingleCaseUnions
             ||| UnwrapSingleFieldCases)
            """[{"Case":"Some","Fields":0},null,{"Case":"Some","Fields":{"val":0}},{"Case":"Some","Fields":{"val":0}}]"""
            """export type FSharpOption<T> = { Case: "Some", Fields: T } | null"""
        )

      testCase
        "AdjacentTag NamedFields UnwrapFieldlessTags UnwrapSingleFieldCases"
        (fun () ->
          test
            (AdjacentTag ||| NamedFields ||| UnwrapFieldlessTags ||| UnwrapSingleFieldCases)
            """[{"Case":"Some","Fields":0},null,{"Case":"Some","Fields":{"val":0}},{"Case":"Some","Fields":{"val":0}}]"""
            """export type FSharpOption<T> = { Case: "Some", Fields: T } | null"""
        )

      testCase
        "AdjacentTag NamedFields UnwrapFieldlessTags UnwrapSingleCaseUnions UnwrapSingleFieldCases"
        (fun () ->
          test
            (AdjacentTag
             ||| NamedFields
             ||| UnwrapFieldlessTags
             ||| UnwrapSingleCaseUnions
             ||| UnwrapSingleFieldCases)
            """[{"Case":"Some","Fields":0},null,{"Case":"Some","Fields":{"val":0}},{"Case":"Some","Fields":{"val":0}}]"""
            """export type FSharpOption<T> = { Case: "Some", Fields: T } | null"""
        )

      testCase
        "AdjacentTag"
        (fun () ->
          test
            AdjacentTag
            """[{"Case":"Some","Fields":[0]},null,{"Case":"Some","Fields":[{"val":0}]},{"Case":"Some","Fields":[{"val":0}]}]"""
            """export type FSharpOption<T> = { Case: "Some", Fields: [T] } | null"""
        )

      testCase
        "Default"
        (fun () -> test Default """[0,null,{"val":0},{"val":0}]""" """export type FSharpOption<T> = T | null""")

      testCase
        "NewtonsoftLike"
        (fun () ->
          test
            NewtonsoftLike
            """[{"Case":"Some","Fields":[0]},null,{"Case":"Some","Fields":[{"val":0}]},{"Case":"Some","Fields":[{"val":0}]}]"""
            """export type FSharpOption<T> = { Case: "Some", Fields: [T] } | null"""
        )

      testCase
        "Thoth"
        (fun () ->
          test
            ThothLike
            """[["Some",0],null,["Some",{"val":0}],["Some",{"val":0}]]"""
            """export type FSharpOption<T> = ["Some", T] | null"""
        )

      testCase
        "InternalTag UnwrapOption"
        (fun () ->
          test
            (InternalTag ||| UnwrapOption)
            """[0,null,{"val":0},{"val":0}]"""
            """export type FSharpOption<T> = T | null"""
        )

      testCase
        "InternalTag NamedFields UnwrapSingleFieldCases"
        (fun () ->
          test
            (InternalTag ||| NamedFields ||| UnwrapSingleFieldCases)
            """[{"Case":"Some","value":0},null,{"Case":"Some","value":{"val":0}},{"Case":"Some","value":{"val":0}}]"""
            """export type FSharpOption<T> = { Case: "Some", value: T } | null"""
        )

      testCase
        "InternalTag NamedFields UnwrapSingleCaseUnions UnwrapSingleFieldCases"
        (fun () ->
          test
            (InternalTag
             ||| NamedFields
             ||| UnwrapSingleCaseUnions
             ||| UnwrapSingleFieldCases)
            """[{"Case":"Some","value":0},null,{"Case":"Some","value":{"val":0}},{"Case":"Some","value":{"val":0}}]"""
            """export type FSharpOption<T> = { Case: "Some", value: T } | null"""
        )

      testCase
        "InternalTag UnwrapSingleFieldCase"
        (fun () ->
          test
            (InternalTag ||| UnwrapSingleFieldCases)
            """[["Some",0],null,["Some",{"val":0}],["Some",{"val":0}]]"""
            """export type FSharpOption<T> = ["Some", T] | null"""
        )

      testCase
        "ExternalTag UnwrapOption"
        (fun () ->
          test
            (ExternalTag ||| UnwrapOption)
            """[0,null,{"val":0},{"val":0}]"""
            """export type FSharpOption<T> = T | null"""
        )

      testCase
        "AdjacentTag UnwrapSingleFieldCases"
        (fun () ->
          test
            (AdjacentTag ||| UnwrapSingleFieldCases)
            """[{"Case":"Some","Fields":0},null,{"Case":"Some","Fields":{"val":0}},{"Case":"Some","Fields":{"val":0}}]"""
            """export type FSharpOption<T> = { Case: "Some", Fields: T } | null"""
        )

      testCase
        "InternalTag UnwrapSingleFieldCases"
        (fun () ->
          test
            (InternalTag ||| UnwrapSingleFieldCases)
            """[["Some",0],null,["Some",{"val":0}],["Some",{"val":0}]]"""
            """export type FSharpOption<T> = ["Some", T] | null"""
        )

      testCase
        "ExternalTag NamedFields UnwrapFieldlessTags UnwrapSingleCaseUnions UnwrapSingleFieldCases"
        (fun () ->
          test
            (ExternalTag
             ||| NamedFields
             ||| UnwrapFieldlessTags
             ||| UnwrapSingleCaseUnions
             ||| UnwrapSingleFieldCases)
            """[{"Some":0},null,{"Some":{"val":0}},{"Some":{"val":0}}]"""
            """export type FSharpOption<T> = { Some: T } | null"""
        )

      testCase
        "AdjacentTag UnwrapRecordCases OptionOfResult"
        (fun () ->
          testOptionOfResult
            (AdjacentTag ||| UnwrapRecordCases)
            """[{"Case":"Some","Fields":{"value":{"Case":"Ok","Fields":{"resultValue":""}}}},{"Case":"Some","Fields":{"value":{"Case":"Error","Fields":{"errorValue":""}}}},null]"""
            """export type FSharpOption<T> = { Case: "Some", Fields: { value: T } } | null"""
        )

      testCase
        "AdjacentTag UnwrapRecordCases ResultOfOption"
        (fun () ->
          testResultOfOption
            (AdjacentTag ||| UnwrapRecordCases)
            """[{"Case":"Ok","Fields":{"resultValue":{"Case":"Some","Fields":{"value":""}}}},{"Case":"Error","Fields":{"errorValue":{"Case":"Some","Fields":{"value":1}}}},{"Case":"Ok","Fields":{"resultValue":null}},{"Case":"Error","Fields":{"errorValue":null}}]"""
            """export type FSharpResult_Case_Ok<T,TError> = { Case: "Ok", Fields: { resultValue: T } }
export type FSharpResult_Case_Error<T,TError> = { Case: "Error", Fields: { errorValue: TError } }"""
        )

      testCase
        "Referencing - AdjacentTag UnwrapRecordCases"
        (fun () ->
          testReferencing
            (AdjacentTag ||| UnwrapRecordCases)
            """[{"primitive":{"Case":"Some","Fields":{"value":5}},"record":{"Case":"Some","Fields":{"val":10}}}]"""
            """export type Referencing = {
 primitive: Microsoft_FSharp_Core.FSharpOption<System.Int32>
 record: Microsoft_FSharp_Core.FSharpOption<Record>
}"""
        )

      testCase
        "Referencing - InternalTag UnwrapRecordCases"
        (fun () ->
          testReferencing
            (InternalTag ||| UnwrapRecordCases)
            """[{"primitive":{"Case":"Some","value":5},"record":{"Case":"Some","val":10}}]"""
            """export type Referencing = {
 primitive: Microsoft_FSharp_Core.FSharpOption<System.Int32>
 record: Microsoft_FSharp_Core.FSharpOption<Record>
}"""
        )

      testCase
        "ExternalTag UnwrapRecordCases"
        (fun () ->
          test
            (ExternalTag ||| UnwrapRecordCases)
            """[{"Some":{"value":0}},null,{"Some":{"val":0}},{"Some":{"val":0}}]"""
            """export type FSharpOption<T> = { Some: { value: T } } | null
export type FSharpOption_T<T> = { Some: T } | null"""
        )

      testCase
        "Referencing - ExternalTag UnwrapRecordCases"
        (fun () ->
          testReferencing
            (ExternalTag ||| UnwrapRecordCases)
            """[{"primitive":{"Some":{"value":5}},"record":{"Some":{"val":10}}}]"""
            """export type Referencing = {
 primitive: Microsoft_FSharp_Core.FSharpOption<System.Int32>
 record: Microsoft_FSharp_Core.FSharpOption<Record>
}"""
        )

      testCase
        "ExternalTag UnwrapOption UnwrapRecordCases"
        (fun () ->
          test
            (ExternalTag ||| UnwrapOption ||| UnwrapRecordCases)
            """[0,null,{"val":0},{"val":0}]"""
            """export type FSharpOption<T> = T | null
export type FSharpOption_T<T> = T | null"""
        )

      testCase
        "Referencing - ExternalTag UnwrapOption UnwrapRecordCases"
        (fun () ->
          testReferencing
            (ExternalTag ||| UnwrapOption ||| UnwrapRecordCases)
            """[{"primitive":5,"record":{"val":10}}]"""
            """export type Referencing = {
 primitive: Microsoft_FSharp_Core.FSharpOption<System.Int32>
 record: Microsoft_FSharp_Core.FSharpOption<Record>
}"""
        )

      testCase
        "Custom"
        (fun () ->
          test
            Config.defaultJsonUnionEncoding
            """[0,null,{"val":0},{"val":0}]"""
            """export type FSharpOption<T> = T | null
export type FSharpOption_T<T> = T | null"""
        )
    ]
