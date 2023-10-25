module Test.Unions.RecordWithResult

open Expecto
open Test

type RecordWithResult = { Result: Result<int, string> }

let tests =
  testList
    "Unions.RecordWithResult"
    [
      testCase
        "Result property"
        (fun () ->
          typedefof<RecordWithResult>
          |> definition
          |> Expect.equal
            """
export type RecordWithResult = {
  result: Microsoft_FSharp_Core.FSharpResult<System.Int32,System.String>
}
"""
        )
    ]
