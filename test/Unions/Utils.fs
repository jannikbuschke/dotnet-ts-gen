[<AutoOpen>]
module Test.Unions.Utils

open Expecto
open TsGen
open System.Text.Json.Serialization
open Test

let options = JsonFSharpOptions(Gen.defaultJsonUnionEncoding)

let serialize<'t> = serializeWithOptions<'t> options
let deserialize<'t> = deserializeWithOptions<'t> options

let toTestInputs inputs =
    inputs
    |> List.collect (fun (encodings, expected) ->
        encodings
        |> List.map (fun encoding -> [| (encoding :> obj); (expected :> obj) |]))

let toTestInputs2 inputs =
    inputs
    |> List.collect (fun (encodings, expected1, expected2) ->
        encodings
        |> List.map (fun encoding -> [| (encoding :> obj); (expected1 :> obj); (expected2 :> obj) |]))
