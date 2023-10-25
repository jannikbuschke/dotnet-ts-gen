module Test.Unit

open Expecto
open Test

let tests =
  testList
    "Unit"
    [
      testCase
        "Unit"
        (fun () ->
          typeof<unit>
          |> definition
          |> Expect.equal
            """
export type Unit = null
"""
        )
    ]
