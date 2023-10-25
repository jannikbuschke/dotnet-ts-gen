module Test.Dictionaries

open System.Collections.Generic
open Expecto
open Test

type OtherRecord = { Id: string }

type MyRecord =
  {
    SimpleList: int list
    ComplexList: OtherRecord list
  }

[<CLIMutable>]
type X = { X: int }

let tests =
  testList
    "Dictionary"
    [
      testCase
        "Serialize String dictionary"
        (fun () ->
          dict [ "key1", 5; "key2", -5 ]
          |> Dictionary
          |> serializeWithCustomEncoding
          |> Expect.equal """{"key1":5,"key2":-5}"""
        )
      testCase
        "Serialize Int dictionary"
        (fun () ->
          dict [ 1, 5; 2, -5 ]
          |> Dictionary
          |> serializeWithCustomEncoding
          |> Expect.equal """{"1":5,"2":-5}"""
        )
      testCase
        "Serialize complex dictionary"
        (fun () ->
          Expect.throws
            (fun () ->
              // complex types are not serializable with default config
              let v = serializeWithTypicalOptions (Dictionary<X, int>(dict [ ({ X = 1 }, 5) ]))
              Expect.similar v """{"1":5,"2":-5}"""
            )
            "Should throw"
        )
      testCase
        "String dictionary"
        (fun () ->
          typedefof<Dictionary<string, int>>
          |> definition
          |> Expect.equal
            """
export type Dictionary<TKey,TValue> = { [key: string]: TValue }
"""
        )
      testCase
        "Int dictionary"
        (fun () ->
          typedefof<Dictionary<int, int>>
          |> definition
          |> Expect.equal
            """
export type Dictionary<TKey,TValue> = { [key: string]: TValue }
"""
        )
    ]
