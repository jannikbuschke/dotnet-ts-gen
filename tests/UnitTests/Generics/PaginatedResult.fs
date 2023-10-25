module Test.Generics.PaginatedResult

open System.Collections.Generic
open Expecto
open Test

type PaginatedResult<'response> =
  {
    Items: IEnumerable<'response>
    Total: int
  }

let tests =
  testList
    "Generics.PaginatedResult"
    [
      testCase
        "x"
        (fun () ->
          typedefof<PaginatedResult<string>>
          |> definition
          |> Expect.equal
            """
export type PaginatedResult<response> = {
 items: System_Collections_Generic.IEnumerable<response>
 total: System.Int32
}
"""
        )
    ]
