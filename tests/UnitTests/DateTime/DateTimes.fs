module Test.DateTime

open System
open Expecto
open Test

type SimpleRecord =
  {
    Id: Guid
    Name: string
    Number: int
    Obj: obj
  }

let tests =
  testList
    "DateTime"
    [
      testCase
        "DateTime definition"
        (fun () ->
          typedefof<DateTime>
          |> definition
          |> Expect.similar """export type DateTime = `${number}-${number}-${number}T${number}:${number}:${number}`"""
        )
    ]
