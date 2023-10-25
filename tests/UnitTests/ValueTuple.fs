module Test.ValueTuple

open Expecto
open Test

type X = { Y: int * string * System.DateTime }

type XX = { Y: List<System.Tuple<int, string, System.DateTime>> }

let tests =
  testList
    "ValueTuple"
    [
      testCase
        "Render tuple 2"
        (fun () ->
          typedefof<int * string>
          |> definition
          |> Expect.equal
            """
export type Tuple<T1,T2> = [T1,T2]
"""
        )
      testCase
        "Render tuple 3"
        (fun () ->
          typedefof<int * string * System.DateTime>
          |> definition
          |> Expect.equal
            """
export type Tuple3<T1,T2,T3> = [T1,T2,T3]
"""
        )
      testCase
        "Render tuple 3 property"
        (fun () ->
          typedefof<X>
          |> definition
          |> Expect.equal
            """
export type X = {
 y: System.Tuple3<System.Int32,System.String,System.DateTime>
}
"""
        )
      testCase
        "Render System.Tuple property"
        (fun () ->
          typedefof<XX>
          |> definition
          |> Expect.equal
            """
export type XX = {
 y: Microsoft_FSharp_Collections.FSharpList<System.Tuple3<System.Int32,System.String,System.DateTime>>
}
"""
        )
    ]
