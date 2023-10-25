module Test.Generics.PropertyWithGenericArray

open System.Collections.Generic
open Expecto
open Test

type Record = { Items: IEnumerable<Result<string, string> array> }

let tests =
  testList
    "Generics.PropertyWithGenericArray"
    [
      testCase
        "Property with array of result"
        (fun () ->
          typedefof<Record>
          |> definition
          |> Expect.equal
            """
export type Record = {
    items: System_Collections_Generic.IEnumerable<Array<Microsoft_FSharp_Core.FSharpResult<System.String,System.String>>>
}
"""
        )
    ]
