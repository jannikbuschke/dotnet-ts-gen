module Test.Collections

open Expecto

type OtherRecord = { Id: string }

type MyRecord =
  {
    SimpleList: int list
    ComplexList: OtherRecord list
  }

let tests =
  testList
    "Collections"
    [
      testCase
        "Record with FSharpList property definition"
        (fun () ->
          typedefof<MyRecord>
          |> definition
          |> Expect.similar
            """
export type MyRecord = {
  simpleList: Microsoft_FSharp_Collections.FSharpList<System.Int32>
  complexList: Microsoft_FSharp_Collections.FSharpList<OtherRecord>
}
"""
        )
      testCase
        "FSharpList definition definition"
        (fun () ->
          typedefof<List<string>>
          |> definition
          |> Expect.similar """export type FSharpList<T> = Array<T>"""
        )
    ]
