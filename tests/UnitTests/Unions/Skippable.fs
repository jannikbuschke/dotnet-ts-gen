module Test.Unions.Skippable

open System.Text.Json.Serialization
open Expecto
open TinyTypeGen
open Test
open type JsonUnionEncoding

type Record = { Val: int }

let tags = [ AdjacentTag; InternalTag; ExternalTag ]
let options =
  [
    NamedFields
    UnwrapFieldlessTags
    UnwrapOption
    UnwrapSingleCaseUnions
    UnwrapSingleFieldCases
    UnwrapRecordCases
  ]

let powerSet (xs: 'a list) =
  List.fold (fun acc x -> acc @ (acc |> List.map (fun subset -> subset @ [ x ]))) [ [] ] xs
let allOptionCombis = powerSet options
let allOptionCombis2 =
  tags
  |> List.collect (fun x ->
    allOptionCombis
    |> List.map (fun y -> x ||| (List.fold (|||) JsonUnionEncoding.Default y))
  )

let value: obj list =
  [
    Include 0
    Skippable<string>.Skip
    Skippable<Record>.Skip
    Include { Val = 0 }
    {| Val = Skippable<string>.Skip |}
    {| Val = Skippable<string>.Include "" |}
    {| Val = Include {| X = 5 |} |}
  ]

let check value typedef encoding expectedJson expectedDefinition =
  value |> serializeWithEncoding encoding |> Expect.equal expectedJson
  typedef |> renderCustomTypeDef encoding |> Expect.stringStart expectedDefinition

let test = check value typedefof<Skippable<_>>

let tests =
  testList
    "Unions.Skippable"
    [
      testCase
        "ExternalTag test"
        (fun () ->
          test
            ExternalTag
            """[0,null,null,{"val":0},{},{"val":""},{"val":{"x":5}}]"""
            "export type Skippable<T> = T | undefined"
        )

      testCase
        "ExternalTag UnwrapRecordCase"
        (fun () ->
          test
            (ExternalTag ||| UnwrapRecordCases)
            """[0,null,null,{"val":0},{},{"val":""},{"val":{"x":5}}]"""
            """export type Skippable<T> = T | undefined
"""
        )

      testCase
        "ExternalTag NamedFields"
        (fun () ->
          test
            (ExternalTag ||| NamedFields)
            """[0,null,null,{"val":0},{},{"val":""},{"val":{"x":5}}]"""
            "export type Skippable<T> = T | undefined"
        )

      testCase
        "InternalTag NamedFields"
        (fun () ->
          test
            (InternalTag ||| NamedFields)
            """[0,null,null,{"val":0},{},{"val":""},{"val":{"x":5}}]"""
            """export type Skippable<T> = T | undefined"""
        )

      testCase
        "AdjacentTag NamedFields"
        (fun () ->
          test
            (AdjacentTag ||| NamedFields)
            """[0,null,null,{"val":0},{},{"val":""},{"val":{"x":5}}]"""
            """export type Skippable<T> = T | undefined"""
        )

      testCase
        "InternalTag UnwrapRecordCases"
        (fun () ->
          test
            (InternalTag ||| UnwrapRecordCases)
            """[0,null,null,{"val":0},{},{"val":""},{"val":{"x":5}}]"""
            """export type Skippable<T> = T | undefined
"""
        )

      testCase
        "AdjacentTag UnwrapRecordCases"
        (fun () ->
          test
            (AdjacentTag ||| UnwrapRecordCases)
            """[0,null,null,{"val":0},{},{"val":""},{"val":{"x":5}}]"""
            """export type Skippable<T> = T | undefined
"""
        )

      testCase
        "Custom"
        (fun () ->
          test
            Config.defaultJsonUnionEncoding
            """[0,null,null,{"val":0},{},{"val":""},{"val":{"x":5}}]"""
            """export type Skippable<T> = T | undefined"""
        )
    ]
