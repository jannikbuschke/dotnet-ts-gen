module Test.Unions.SingleCaseMultipleFields

open System.Text.Json.Serialization
open Expecto
open Test

type SingleCaseMultipleFields = | SingleCaseMultipleFields of string * int

// TODO: not yet supported
// let definition, value = renderTypeAndValue typedefof<SingleCaseMultipleFields>

let tests = testList "Unions.SingleCaseMultipleFields" []
