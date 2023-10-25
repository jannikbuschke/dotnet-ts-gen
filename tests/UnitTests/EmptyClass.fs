module Test.EmptyClass

open Expecto
open Test

type EmptyClass() =
  let _ = "foo"

let tests =
  testList
    "EmptyClass"
    [
      testCase
        "Empty class"
        (fun () ->
          typeof<EmptyClass>
          |> definition
          |> Expect.equal
            """export type EmptyClass = {
  
}
"""
        )
    ]
