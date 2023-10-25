[<AutoOpen>]
module Test.Unions.Utils

open Expecto
open System.Text.Json.Serialization
open Test
open TinyTypeGen.Config

let options = JsonFSharpOptions(defaultJsonUnionEncoding)

let serialize<'t> = serializeWithOptions<'t> options
let deserialize<'t> = deserializeWithOptions<'t> options

/// Flatten a list of (encodings, expected) pairs into (encoding, expected) pairs for parameterised tests.
let toTestCases inputs =
  inputs
  |> List.collect (fun (encodings, expected) -> encodings |> List.map (fun encoding -> encoding, expected))

/// Flatten a list of (encodings, expected1, expected2) pairs into (encoding, expected1, expected2) triples.
let toTestCases2 inputs =
  inputs
  |> List.collect (fun (encodings, expected1, expected2) ->
    encodings |> List.map (fun encoding -> encoding, expected1, expected2)
  )

/// Wrap a list of encodings into a list of single-element encoding tuples (for tests that only parameterise on encoding).
let wrapAsSingletonCases (encodings: JsonUnionEncoding list) : JsonUnionEncoding list = encodings
