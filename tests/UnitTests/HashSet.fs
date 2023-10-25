module Test.HashSet

open System.Collections.Generic
open Expecto
open Test

let tests =
  testList
    "HashSet"
    [
      testCase
        "Serialize HashSet<string>"
        (fun () ->
          seq {
            "item1"
            "item2"
          }
          |> HashSet
          |> serializeWithCustomEncoding
          |> Expect.equal """["item1","item2"]"""
        )
      testCase
        "HashSet type"
        (fun () ->
          typedefof<HashSet<_>>
          |> definition
          |> Expect.equal
            """
export type HashSet<T> = Array<T>
"""
        )
    ]
