module Test.Object

open Expecto
open Test

type A =
  {
    Data: System.Object
    X: obj
  }

let tests =
  testList
    "Object"
    [
      testCase
        "Render record with obj"
        (fun () ->
          typedefof<A>
          |> definition
          |> Expect.equal
            """
export type A = {
 data: System.Object
 x: System.Object
}
"""
        )
    ]
