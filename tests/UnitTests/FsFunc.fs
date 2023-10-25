module Test.FsFunc

open Expecto
open Test

type A =
  {
    Title: string
    IsAllowed: unit -> bool
  }

let tests =
  testList
    "FsFunc"
    [
      testCase
        "Func is ignored"
        (fun () ->
          typeof<A>
          |> definition
          |> Expect.equal
            "export type A = {
  title: System.String
}"
        )
    ]
