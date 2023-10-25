module Test.Json

open System.Text.Json.Nodes
open Expecto
open Test

let tests =
  testList
    "JsonNode"
    [
      testCase
        "Render JsonNode"
        (fun () ->
          typedefof<JsonNode>
          |> definition
          |> Expect.similar
            """
export type JsonNode = | string
 | number
 | boolean
 | null
 | { [key: string]: JsonElement }
 | JsonElement[];
"""
        )
      testCase
        "Render JsonNode2"
        (fun () ->
          let v = JsonNode.Parse("""{"data": {"a": 1, "b": [1,2,3]}, "x": "hello"}""")
          ()
        )
    ]
