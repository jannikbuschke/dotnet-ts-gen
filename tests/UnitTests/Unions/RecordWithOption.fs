module Test.Unions.RecordWithOption

open System
open Expecto
open Test

type Record = { Id: string }

type RecordWithOption = { NumberOption: Record option }

let tests =
  testList
    "Unions.RecordWithOption"
    [
      testCase
        "Render record with option"
        (fun () ->
          typedefof<RecordWithOption>
          |> definition
          |> Expect.equal
            """
export type RecordWithOption = {
  numberOption: Microsoft_FSharp_Core.FSharpOption<Record>
}
"""
        )
    ]
